using System;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using System.Text;

namespace LibFlacSharp.Metadata {

    [StructLayout(LayoutKind.Sequential, Pack = 1, Size = 3)]
    public struct Int24 {

        public byte Low;

        public byte Middle;

        public byte High;

        public int Value {
            get {
                return BitConverterExtension.ToInt24(new[] { High, Middle, Low }, 0);
            }
            set {

                Low = (byte) (value & 0xFF);
                Middle = (byte)((value >> 8) & 0xFF);
                High = (byte)((value >> 16) & 0xFF);
            }
        }
    }
}
