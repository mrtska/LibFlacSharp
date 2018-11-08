using System;
using System.IO;
using System.Threading.Tasks;
using LibFlacSharp.Metadata;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Windows.Storage;
using Windows.Storage.Pickers;

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

            var file = new FlacFile(await stream.OpenStreamForWriteAsync());


        }
    
        [TestMethod]
        [ExpectedException(typeof(FileNotFoundException))]
        public void TestMissingFile() {

            new FlacFile("nothing.flac");
        }

    }
}
