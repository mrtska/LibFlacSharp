using LibFlacSharp.Metadata;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;

namespace LibFlacSharp {
    public class FlacFile : IDisposable {

        private readonly MemoryStream InMemoryByteArray = new MemoryStream();
        private BinaryReader Reader;

        private readonly Stream OriginalSteram;


        /// <summary>
        /// Stream related infomation.
        /// sampling hz. etc...
        /// </summary>
        public StreamInfo StreamInfo { get; private set; }

        public Dictionary<PictureType, Picture> Pictures { get; private set; }


        public VorbisComment VorbisComment { get; private set; }


        public byte[] Frame { get; private set; }

        /// <summary>
        /// Open flac file by file path.
        /// Should not be used in UWP.
        /// It maybe causes UnauthorizedAccessException.
        /// </summary>
        /// <param name="filePath"></param>
        public FlacFile(string filePath) {

            var stream = new FileStream(filePath, FileMode.Open);
            stream.CopyTo(InMemoryByteArray);

            OriginalSteram = stream;
            Initialize();
        }

        public FlacFile(Stream stream) {

            OriginalSteram = stream;
            stream.CopyTo(InMemoryByteArray);
            Initialize();
        }

        private void Initialize() {

            Reader = new BinaryReader(InMemoryByteArray);
            Pictures = new Dictionary<PictureType, Picture>();
            VerifyFileHeader();
            Parse();
        }


        private void VerifyFileHeader() {

            InMemoryByteArray.Position = 0;

            // Check the file marker.
            if (!Reader.ReadBytes(4).SequenceEqual(Encoding.ASCII.GetBytes("fLaC"))) {

                throw new FormatException("Invalid flac stream marker. Check input file.");
            }
        }

        private void Parse() {

            while(true) {

                var type = Reader.ReadByte();

                var last = (type & 0x80) == 0x80;

                if (last) {

                    break;
                }

                var length = BitConverterExtension.ToInt24(Reader.ReadBytes(3), 0);

                switch((MetadataBlockType)type) {

                    case MetadataBlockType.STREAMINFO: {

                            StreamInfo = StreamInfo.FromByteArray(Reader.ReadBytes(length));
                            break;
                        }
                    case MetadataBlockType.SEEKTABLE: {

                            Reader.ReadBytes(length);
                            break;
                        }
                    case MetadataBlockType.PICTURE: {

                            var picture = Picture.FromByteArray(Reader.ReadBytes(length));

                            Pictures[picture.PictureType] = picture;
                            break;
                        }
                    case MetadataBlockType.VORBIS_COMMENT: {

                            VorbisComment = VorbisComment.FromByteArray(Reader.ReadBytes(length));
                            break;
                        }
                    default: {
                            // Discard
                            Reader.ReadBytes(length);
                            break;
                        }
                }
            }

            if (VorbisComment == null) VorbisComment = new VorbisComment();
            Frame = Reader.ReadBytes((int)(InMemoryByteArray.Length - InMemoryByteArray.Position));
        }

        private string DetectMIMEType(byte[] array) {

            // jpeg
            if(array[0] == 0xFF && array[1] == 0xD8) {

                return "image/jpeg";
            }

            var png = new byte[] { 0x89, 0x50, 0x4E, 0x47, 0x0D, 0x0A, 0x1A, 0x0A };
            var flag = true;
            for (int i = 0; i < png.Length; i++) {

                if(png[i] != array[i]) {

                    flag = false;
                }
            }
            if(flag) {

                return "image/png";
            }

            flag = true;

            var gif = Encoding.ASCII.GetBytes("GIF");
            for (int i = 0; i < gif.Length; i++) {

                if (gif[i] != array[i]) {

                    flag = false;
                }
            }
            if (flag) {

                return "image/gif";
            }
            throw new NotSupportedException("Only png, jpeg and gif picture supported.");
        }


        /// <summary>
        /// Insert picture into the flac file.
        /// If picture already exists, overwrite them.
        /// </summary>
        /// <param name="type"></param>
        /// <param name="stream"></param>
        public void AddPicture(PictureType type, Stream stream) {

            using (var ms = new MemoryStream()) {

                stream.CopyTo(ms);
                var array = ms.ToArray();

                var picture = new Picture();

                picture.MIMEType = DetectMIMEType(array);

                if(type == PictureType.FileIconStandard && !picture.MIMEType.Contains("png")) {

                    throw new ArgumentException("FileIconStandard is only png determined by specification.");
                }

                picture.Description = "";
                picture.DescriptionLength = 0;

                picture.MIMETypeLength = (uint) picture.MIMEType.Length;

                picture.PictureDataLength = (uint) array.Length;
                picture.PictureData = array;

                Pictures[type] = picture;
            }
        }
        /// <summary>
        /// Set picture as PictureType.CoverFront.
        /// </summary>
        /// <param name="stream"></param>
        public void AddPicture(Stream stream) {

            AddPicture(PictureType.CoverFront, stream);
        }

        public async Task SaveAsync(Stream destination) {

            var writeTo = new MemoryStream();
            using (var writer = new BinaryWriter(writeTo)) {

                writer.Write(Encoding.ASCII.GetBytes("fLaC"));
                writer.Write((byte)MetadataBlockType.STREAMINFO);
                var stream = StreamInfo.ToByteArray();

                writer.Write(BitConverter.GetBytes(BitConverterExtension.ConvertEndian24(stream.Length)), 0, 3);
                writer.Write(stream);

                foreach(var picture in Pictures) {

                    stream = picture.Value.ToByteArray();
                    writer.Write((byte)MetadataBlockType.PICTURE);
                    writer.Write(BitConverter.GetBytes(BitConverterExtension.ConvertEndian24(stream.Length)), 0, 3);

                    writer.Write(stream);
                }

                stream = VorbisComment.ToByteArray();
                writer.Write((byte)MetadataBlockType.VORBIS_COMMENT);
                writer.Write(BitConverter.GetBytes(BitConverterExtension.ConvertEndian24(stream.Length)), 0, 3);

                writer.Write(stream);

                writer.Write((byte)0x81);

                writer.Write(Frame);

                await writeTo.FlushAsync();
                writeTo.Position = 0;
                await writeTo.CopyToAsync(destination);
            }
        }

        public void Dispose() {

            InMemoryByteArray?.Dispose();
            OriginalSteram?.Dispose();
        }
    }
}
