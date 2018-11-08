using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace LibFlacSharp.Metadata {
    public static class BitConverterExtension {


        public static int ToInt24(byte[] array, int startIndex) {

            return (array[2 + startIndex] << 0) | (array[1 + startIndex] << 8) | (array[0 + startIndex] << 16);
        }
        public static ushort ConvertEndian(ushort bigendian) {

            byte lower = (byte)(bigendian & 0xFF);
            byte upper = (byte)((bigendian >> 8) & 0xFF);

            return (ushort)((lower << 8) | upper);
        }

        public static ulong ConvertEndian(ulong endian) {

            var bytes = BitConverter.GetBytes(endian);
            return BitConverter.ToUInt64(bytes.Reverse().ToArray(), 0);
        }

        public static int ConvertEndian20(int endian) {

            endian &= 0x000FFFFF;

            byte lower = (byte)(endian & 0xFF);
            byte middle = (byte)((endian >> 8) & 0xFF);
            byte upper = (byte)((endian >> 16) & 0x0F);


            return ((lower << 16) | (middle << 8) | upper) & 0x000FFFFF;
        }

        public static int ConvertEndian24(int endian) {

            endian &= 0x00FFFFFF;

            byte lower = (byte)(endian & 0xFF);
            byte middle = (byte)((endian >> 8) & 0xFF);
            byte upper = (byte)((endian >> 16) & 0xFF);

            return ((lower << 16) | (middle << 8) | upper) & 0x00FFFFFF;
        }
    }
}
