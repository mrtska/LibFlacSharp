using System.IO;
using System.Runtime.InteropServices;
using System.Text;

namespace LibFlacSharp.Metadata {

    [StructLayout(LayoutKind.Sequential, Pack = 1)]
    public struct Picture {

        private uint _PictureType;
        /// <summary>
        /// The picture type according to the ID3v2 APIC frame.
        /// </summary>
        public PictureType PictureType {
            get { return (PictureType) BitConverterExtension.ConvertEndian(_PictureType); }
            set { _PictureType = BitConverterExtension.ConvertEndian((uint) value); }
        }

        private uint _MIMETypeLength;
        /// <summary>
        /// The length of the MIME type string in bytes.
        /// </summary>
        public uint MIMETypeLength {
            get { return BitConverterExtension.ConvertEndian(_MIMETypeLength); }
            set { _MIMETypeLength = BitConverterExtension.ConvertEndian(value); }
        }

        [MarshalAs(UnmanagedType.ByValArray)]
        private byte[] _MIMEType;
        /// <summary>
        /// The MIME type string, in printable ASCII characters 0x20-0x7e.
        /// The MIME type may also be --> to signify that the data part is a URL of the picture instead of the picture data itself.
        /// </summary>
        public string MIMEType {
            get { return new string(Encoding.ASCII.GetChars(_MIMEType)); }
            set { _MIMEType = Encoding.ASCII.GetBytes(value); }
        }

        private uint _DescriptionLength;
        /// <summary>
        /// The length of the description string in bytes.
        /// </summary>
        public uint DescriptionLength {
            get { return BitConverterExtension.ConvertEndian(_DescriptionLength); }
            set { _DescriptionLength = BitConverterExtension.ConvertEndian(value); }
        }

        /// <summary>
        /// The description of the picture, in UTF-8.
        /// </summary>
        public byte[] _Description;
        public string Description {
            get { return new string(Encoding.UTF8.GetChars(_Description)); }
            set { _Description = Encoding.UTF8.GetBytes(value); }
        }


        private uint _PictureWidth;
        /// <summary>
        /// 	The width of the picture in pixels.
        /// </summary>
        public uint PictureWidth {
            get { return BitConverterExtension.ConvertEndian(_PictureWidth); }
            set { _PictureWidth = BitConverterExtension.ConvertEndian(value); }
        }

        private uint _PictureHeight;
        /// <summary>
        /// The height of the picture in pixels.
        /// </summary>
        public uint PictureHeight {
            get { return BitConverterExtension.ConvertEndian(_PictureHeight); }
            set { _PictureHeight = BitConverterExtension.ConvertEndian(value); }
        }

        private uint _PictureDepth;
        /// <summary>
        /// The color depth of the picture in bits-per-pixel.
        /// </summary>
        public uint PictureDepth {
            get { return BitConverterExtension.ConvertEndian(_PictureDepth); }
            set { _PictureDepth = BitConverterExtension.ConvertEndian(value); }
        }


        private uint _PictureIndexColorCount;
        /// <summary>
        /// For indexed-color pictures (e.g. GIF), the number of colors used, or 0 for non-indexed pictures.
        /// </summary>
        public uint PictureIndexColorCount {
            get { return BitConverterExtension.ConvertEndian(_PictureIndexColorCount); }
            set { _PictureIndexColorCount = BitConverterExtension.ConvertEndian(value); }
        }

        private uint _PictureDataLength;
        /// <summary>
        /// The length of the picture data in bytes.
        /// </summary>
        public uint PictureDataLength {
            get { return BitConverterExtension.ConvertEndian(_PictureDataLength); }
            set { _PictureDataLength = BitConverterExtension.ConvertEndian(value); }
        }

        /// <summary>
        /// The binary picture data.
        /// </summary>
        [MarshalAs(UnmanagedType.ByValArray)]
        public byte[] PictureData;

        /// <summary>
        /// Extract picture binary as Stream.
        /// </summary>
        /// <returns></returns>
        public Stream ExtractPicture() {

            if(PictureData == null) {

                return null;
            }
            return new MemoryStream(PictureData);
        }


        /// <summary>
        /// For internal use.
        /// </summary>
        /// <param name="array">Raw byte array</param>
        /// <returns></returns>
        internal static Picture FromByteArray(byte[] array) {

            var ptr = Marshal.AllocHGlobal(array.Length);
            var basePtr = ptr;

            Marshal.Copy(array, 0, ptr, array.Length);

            var picture = new Picture();

            picture._PictureType = (uint)Marshal.ReadInt32(ptr);
            picture._MIMETypeLength = (uint)Marshal.ReadInt32(ptr, 4);

            var bytes = new byte[picture.MIMETypeLength];
            ptr += 8;
            Marshal.Copy(ptr, bytes, 0, bytes.Length);

            picture._MIMEType = bytes;

            ptr += bytes.Length;

            picture._DescriptionLength = (uint) Marshal.ReadInt32(ptr);

            ptr += 4;

            bytes = new byte[picture.DescriptionLength];

            Marshal.Copy(ptr, bytes, 0, bytes.Length);

            picture._Description = bytes;

            ptr += bytes.Length;

            picture._PictureWidth = (uint) Marshal.ReadInt32(ptr);
            picture._PictureHeight = (uint) Marshal.ReadInt32(ptr, 4);
            picture._PictureDepth = (uint) Marshal.ReadInt32(ptr, 8);
            picture._PictureIndexColorCount = (uint) Marshal.ReadInt32(ptr, 12);
            picture._PictureDataLength = (uint) Marshal.ReadInt32(ptr, 16);

            ptr += 20;

            bytes = new byte[picture.PictureDataLength];

            Marshal.Copy(ptr, bytes, 0, bytes.Length);

            picture.PictureData = bytes;

            Marshal.FreeHGlobal(basePtr);

            return picture;
        }
        public byte[] ToByteArray() {

            var array = new MemoryStream();
            using (var writer = new BinaryWriter(array)) {

                writer.Write(_PictureType);
                writer.Write(_MIMETypeLength);
                writer.Write(_MIMEType);
                writer.Write(_DescriptionLength);
                writer.Write(_Description);
                writer.Write(_PictureWidth);
                writer.Write(_PictureHeight);
                writer.Write(_PictureDepth);
                writer.Write(_PictureIndexColorCount);
                writer.Write(_PictureDataLength);
                writer.Write(PictureData);
                return array.ToArray();
            }
        }
    }

    public enum PictureType {

        Other,
        FileIconStandard, // png only
        FileIcon,
        CoverFront,
        CoverBack,
        LeafletPage,
        Media,
        LeadArtist,
        Artist,
        Conductor,
        Lyricist,
        RecordingLocation,
        DuringRecording,
        DuringPerformance,
        VideoScreenCapture,
        BrightColoredFish,
        Illustration,
        Band,
        Publisher
    }
}
