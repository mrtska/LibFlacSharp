# LibFlacSharp

LibFlacSharp is fullscratched flac metadata library.

Not contains encode/decode audio feature.

.NET Standard 2.0.

# Feature
- Extract album art.
- Edit Vorbis comments.
- Save album art.


# How to use

## Save album art
```cs
var flac = new FlacFile(fileStream);
flac.AddPicture(PictureType.CoverFront, pngStream);
await flac.SaveAsync(toSaveStream);
```

## Edit Vorbis comments
```cs
var flac = new FlacFile(fileStream);
flac.VorbisComment.CommentList[VorbisCommentType.Album] = "test";
await flac.SaveAsync(toSaveStream);
```