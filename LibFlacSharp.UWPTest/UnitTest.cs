using LibFlacSharp.Metadata;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.IO;
using System.Threading.Tasks;
using Windows.Storage;

namespace LibFlacSharp.UWPTest {

    [TestClass]
    public class UnitTest {

        [TestMethod]
        [ExpectedException(typeof(UnauthorizedAccessException))]
        [Ignore]
        public void TestBasicFlacFile() {

            new FlacFile(@"test1.flac");
        }

        [TestMethod]
        public async Task TestBasicFlacFileFromStream() {


            var stream = await KnownFolders.MusicLibrary.GetFileAsync("test1.flac");
            var png = await KnownFolders.MusicLibrary.GetFileAsync("300367.png");
            var stream2 = await KnownFolders.MusicLibrary.CreateFileAsync("test2.flac", CreationCollisionOption.ReplaceExisting);

            var file = new FlacFile(await stream.OpenStreamForWriteAsync());

            file.AddPicture(PictureType.CoverFront, await png.OpenStreamForReadAsync());

            file.VorbisComment.CommentList[VorbisCommentType.Album] = "test";

            await file.SaveAsync(await stream2.OpenStreamForWriteAsync());
        }
    
        [TestMethod]
        [ExpectedException(typeof(FileNotFoundException))]
        public void TestMissingFile() {

            new FlacFile("nothing.flac");
        }

    }
}
