using LibFlacSharp.Metadata;
using System;
using System.IO;
using System.Linq;
using System.Text;

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

        public Picture PictureInfo { get; private set; }

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
                    case MetadataBlockType.PICTURE: {

                            PictureInfo = Picture.FromByteArray(Reader.ReadBytes(length));
                            break;
                        }
                    default: {

                            ;

                            break;
                        }
                }
                






            }









        }



        public void Dispose() {

            InMemoryByteArray?.Dispose();
            OriginalSteram?.Dispose();
        }
    }
}
