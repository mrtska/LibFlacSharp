using System.Collections.Generic;
using System.IO;
using System.Text;

namespace LibFlacSharp.Metadata {

    public static class VorbisCommentType {

        public const string Artist = "ARTIST";

        public const string Album = "ALBUM";

        public const string Copyright = "COPYRIGHT";

        public const string DiscNumber = "DISCNUMBER";

        public const string DiscTotal = "DISCTOTAL";

        public const string Lyrics = "LYRICS";

        public const string Title = "TITLE";

        public const string TrackNumber = "TRACKNUMBER";

        public const string TrackTotal = "TRACKTOTAL";

        public const string Year = "YEAR";

        public const string Composer = "COMPOSER";

        public const string Genre = "GENRE";

    }


    public class VorbisCommentEntry {

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

    public class VorbisComment {

        private byte[] _VendorString;
        /// <summary>
        /// Vendor string.
        /// </summary>
        public string VendorString {
            get { return new string(Encoding.UTF8.GetChars(_VendorString)); }
            set { _VendorString = Encoding.UTF8.GetBytes(value); }
        }
        /// <summary>
        /// Comment Sets.
        /// </summary>
        public Dictionary<string, string> CommentList { get; private set; }

        public static VorbisComment FromByteArray(byte[] array) {

            using (var reader = new BinaryReader(new MemoryStream(array))) {

                var vorbis = new VorbisComment();
                var vendorLength = reader.ReadUInt32();
                vorbis._VendorString = reader.ReadBytes((int)vendorLength);
                var length = reader.ReadUInt32();

                vorbis.CommentList = new Dictionary<string, string>();
                for(int i = 0; i < length; i++) {

                    var entry = new VorbisCommentEntry();
                    entry.Length = reader.ReadUInt32();
                    entry._Comment = reader.ReadBytes((int)entry.Length);

                    var str = entry.Comment.Split('=');

                    vorbis.CommentList[str[0]] = str[1];
                }
                return vorbis;
            }
        }

        public byte[] ToByteArray() {

            var array = new MemoryStream();
            using (var writer = new BinaryWriter(array)) {

                VendorString = "LibFlacSharp 1.0.0 20181113";

                writer.Write(_VendorString.Length);
                writer.Write(_VendorString);
                writer.Write(CommentList.Count);
                foreach (var set in CommentList) {

                    var entry = new VorbisCommentEntry();
                    entry.Comment = set.Key + "=" + set.Value;

                    writer.Write(entry._Comment.Length);
                    writer.Write(entry._Comment);
                }
                return array.ToArray();
            }
        }
    }
}
