using System.Runtime.InteropServices;

namespace LibFlacSharp.Metadata {
    [StructLayout(LayoutKind.Sequential, Pack = 1)]
    public struct StreamInfo {

        private ushort _MinimumBlockSize;
        /// <summary>
        /// The minimum block size (in samples) used in the stream.
        /// </summary>
        public ushort MinimumBlockSize {
            get { return BitConverterExtension.ConvertEndian(_MinimumBlockSize); }
            set { _MinimumBlockSize = BitConverterExtension.ConvertEndian(value); }
        }

        public ushort _MaximumBlockSize;
        /// <summary>
        /// The maximum block size (in samples) used in the stream.
        /// (Minimum blocksize == maximum blocksize) implies a fixed-blocksize stream.
        /// </summary>
        public ushort MaximumBlockSize {
            get { return BitConverterExtension.ConvertEndian(_MaximumBlockSize); }
            set { _MaximumBlockSize = BitConverterExtension.ConvertEndian(value); }
        }

        private Int24 _MinimumFrameSize;
        /// <summary>
        /// The minimum frame size (in bytes) used in the stream.
        /// May be 0 to imply the value is not known.
        /// </summary>
        public int MinimumFrameSize {
            get { return BitConverterExtension.ConvertEndian24(_MinimumFrameSize.Value); }
            set { _MinimumFrameSize.Value = BitConverterExtension.ConvertEndian24(value); }
        }

        private Int24 _MaximumFrameSize;
        /// <summary>
        /// The maximum frame size (in bytes) used in the stream.
        /// May be 0 to imply the value is not known.
        /// </summary>
        public int MaximumFrameSize {
            get { return BitConverterExtension.ConvertEndian24(_MaximumFrameSize.Value); }
            set { _MaximumFrameSize.Value = BitConverterExtension.ConvertEndian24(value); }
        }

        private ulong _SampleRateChannelCountBitsPerSample;
        private ulong SampleRateChannelCountBitsPerSample {
            get { return BitConverterExtension.ConvertEndian(_SampleRateChannelCountBitsPerSample); }
            set { _SampleRateChannelCountBitsPerSample = BitConverterExtension.ConvertEndian(value); }
        }

        public int SampleRate {
            get {
                return (int) (SampleRateChannelCountBitsPerSample >> 44 & 0x000FFFFF);
            }
            set {
                SampleRateChannelCountBitsPerSample |= ((ulong)value & 0x000FFFFF) << 44;
            }
        }

        public byte ChannelCount {
            get {
                return (byte)(((SampleRateChannelCountBitsPerSample >> 41) & 0x07) + 1);
            }
            set => SampleRateChannelCountBitsPerSample |= ((ulong)(value & 0x07) << 41) - 1;
        }

        public byte BitsPerSample {
            get {
                return (byte)(((SampleRateChannelCountBitsPerSample >> 36) & 0x1F) + 1);
            }
            set {
                SampleRateChannelCountBitsPerSample |= ((ulong)(value & 0x1F) << 36) - 1;
            }
        }

        public long TotalSamples {
            get {
                return (long)(SampleRateChannelCountBitsPerSample & 0x0000000FFFFFFFFF);
            }
            set {
                SampleRateChannelCountBitsPerSample |= (ulong)(value & 0x0000000FFFFFFFFF);
            }
        }

        [MarshalAs(UnmanagedType.ByValArray, SizeConst = 16)]
        public byte[] Md5Signature;


        public static StreamInfo FromByteArray(byte[] array) {

            var size = Marshal.SizeOf<StreamInfo>();
            var ptr = Marshal.AllocHGlobal(size);

            Marshal.Copy(array, 0, ptr, size);

            var result = Marshal.PtrToStructure<StreamInfo>(ptr);
            Marshal.FreeHGlobal(ptr);

            return result;
        }

        public byte[] ToByteArray() {

            var size = Marshal.SizeOf(this);
            var bytearray = new byte[size];

            var ptr = Marshal.AllocHGlobal(size);
            Marshal.StructureToPtr(this, ptr, true);
            Marshal.Copy(ptr, bytearray, 0, size);
            Marshal.FreeHGlobal(ptr);

            return bytearray;
        }
    }
}
