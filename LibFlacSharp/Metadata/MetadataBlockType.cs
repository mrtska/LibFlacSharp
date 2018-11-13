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
