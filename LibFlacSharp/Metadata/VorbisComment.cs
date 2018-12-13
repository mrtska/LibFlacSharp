using System.Collections.Generic;
using System.IO;
using System.Text;

namespace LibFlacSharp.Metadata {

    public static class VorbisCommentType {

        public const string Title = "Title";

        public const string Artist = "Artist";

        public const string Lyricist = "Lyricist";

        public const string Composer = "Composer";

        public const string Album = "Album";

        public const string Genre = "Genre";

        public const string Lyrics = "Lyrics";

        public const string Year = "Year";

        public const string TrackNumber = "TrackNumber";

        public const string TrackTotal = "TrackTotal";

        public const string DiscNumber = "DiscNumber";

        public const string DiscTotal = "DiscTotal";

        public const string Copyright = "Copyright";
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
                    switch(str[0].ToLower()) {
                        case "title":
                            vorbis.CommentList[VorbisCommentType.Title] = str[1];
                            break;
                        case "artist":
                            vorbis.CommentList[VorbisCommentType.Artist] = str[1];
                            break;
                        case "lyricist":
                            vorbis.CommentList[VorbisCommentType.Lyricist] = str[1];
                            break;
                        case "composer":
                            vorbis.CommentList[VorbisCommentType.Composer] = str[1];
                            break;
                        case "album":
                            vorbis.CommentList[VorbisCommentType.Album] = str[1];
                            break;
                        case "genre":
                            vorbis.CommentList[VorbisCommentType.Genre] = str[1];
                            break;
                        case "lyrics":
                            vorbis.CommentList[VorbisCommentType.Lyrics] = str[1];
                            break;
                        case "year":
                            vorbis.CommentList[VorbisCommentType.Year] = str[1];
                            break;
                        case "tracknumber":
                            vorbis.CommentList[VorbisCommentType.TrackNumber] = str[1];
                            break;
                        case "tracktotal":
                            vorbis.CommentList[VorbisCommentType.TrackTotal] = str[1];
                            break;
                        case "discnumber":
                            vorbis.CommentList[VorbisCommentType.DiscNumber] = str[1];
                            break;
                        case "disctotal":
                            vorbis.CommentList[VorbisCommentType.DiscTotal] = str[1];
                            break;
                        default:
                            vorbis.CommentList[str[0]] = str[1];
                            break;
                    }
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
