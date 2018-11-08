using System;
using System.Collections.Generic;
using System.Text;

namespace LibFlacSharp.Metadata {
    public enum MetadataBlockType : byte {

        STREAMINFO = 0,
        PADDING,
        APPLICATION,
        SEEKTABLE,
        VORBIS_COMMENT,
        CUESHEET,
        PICTURE
    }
}
