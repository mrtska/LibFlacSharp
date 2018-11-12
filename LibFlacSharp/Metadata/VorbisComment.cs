using System;
using System.Collections.Generic;
using System.IO;
using System.Runtime.InteropServices;
using System.Text;

namespace LibFlacSharp.Metadata {

    public struct VorbisCommentEntry {

        /// <summary>
        /// UTF8 comment string length.
        /// </summary>
        public uint Length;

        internal byte[] _Comment;
        /// <summary>
        /// Comment.
        /// </summary>
        public string Comment {
            get { return new string(Encoding.UTF8.GetChars(_Comment)); }
            set { _Comment = Encoding.UTF8.GetBytes(value); }
        }
    }

    public struct VorbisComment {

        /// <summary>
        /// The vendor string length.
        /// </summary>
        public uint VendorLength;


        private byte[] _VendorString;
        /// <summary>
        /// Vendor string.
        /// </summary>
        public string VendorString {
            get { return new string(Encoding.UTF8.GetChars(_VendorString)); }
            set { _VendorString = Encoding.UTF8.GetBytes(value); }
        }
        /// <summary>
        /// Comment list size.
        /// </summary>
        public uint UserCommentListLength;

        public VorbisCommentEntry[] CommentList;

        public static VorbisComment FromByteArray(byte[] array) {

            var ptr = Marshal.AllocHGlobal(array.Length);
            var basePtr = ptr;

            Marshal.Copy(array, 0, ptr, array.Length);

            var vorbis = new VorbisComment();

            vorbis.VendorLength = (uint)Marshal.ReadInt32(ptr);
            ptr += 4;

            var bytes = new byte[vorbis.VendorLength];
            Marshal.Copy(ptr, bytes, 0, bytes.Length);

            vorbis._VendorString = bytes;
            ptr += bytes.Length;

            vorbis.UserCommentListLength = (uint)Marshal.ReadInt32(ptr);
            ptr += 4;

            var list = new List<VorbisCommentEntry>();

            for(int i = 0; i < vorbis.UserCommentListLength; i++) {

                var entry = new VorbisCommentEntry();

                entry.Length = (uint)Marshal.ReadInt32(ptr);
                ptr += 4;
                bytes = new byte[entry.Length];
                Marshal.Copy(ptr, bytes, 0, bytes.Length);
                ptr += bytes.Length;

                entry._Comment = bytes;
                list.Add(entry);
            }
            vorbis.CommentList = list.ToArray();

            Marshal.FreeHGlobal(basePtr);
            return vorbis;
        }

        public byte[] ToByteArray() {

            var array = new MemoryStream();
            using (var writer = new BinaryWriter(array)) {

                writer.Write(VendorLength);
                writer.Write(_VendorString);
                writer.Write(UserCommentListLength);
                foreach (var entry in CommentList) {

                    writer.Write(entry.Length);
                    writer.Write(entry._Comment);
                }
                return array.ToArray();
            }
        }
    }
}
