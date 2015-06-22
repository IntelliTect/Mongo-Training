using System.IO;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using MongoDB.Driver.GridFS;

// ReSharper disable InconsistentNaming

namespace IntelliTect.Training.Mongo
{
    [TestClass]
    public class GridFsTests
    {
        // NB: Assumes the Word docs version1-6 are loaded into a GridFS collection called "fs" in the "mongo-training" database.

        [TestMethod]
        public async Task WhenReadingGridFS_ItReturnsBufferOfCorrectSize()
        {
            // Arrange
            MongoGridFSFileInfo file = MongoLegacy.GridFs.FindOne("version1.doc");
            var buffer = new byte[file.Length];

            // Act
            using ( MongoGridFSStream mongoStream = file.OpenRead() )
            {
                await mongoStream.ReadAsync( buffer, 0, (int) file.Length );
            }

            // Assert
            Assert.AreEqual(27136, buffer.Length);
            Assert.AreEqual(27136, file.Length);

            CompareMd5Hash(file.MD5, buffer);
        }

        [TestMethod]
        public async Task WhenSeekingToASpecificFileSegment_ItWorks()
        {
            // Arrange
            MongoGridFSFileInfo file = MongoLegacy.GridFs.FindOne("version2.doc");
            var buffer = new byte[32];

            // Act
            using ( MongoGridFSStream mongoStream = file.OpenRead() )
            {
                mongoStream.Seek(20000, SeekOrigin.Begin);
                await mongoStream.ReadAsync( buffer, 0, 32 );
            }

            // Assert
            Assert.AreEqual(73, buffer[0]);
            Assert.AreEqual(0, buffer[1]);
            Assert.AreEqual(110, buffer[2]);
            // ...
            Assert.AreEqual( 111, buffer[18] );
            Assert.AreEqual( 0, buffer[19] );
            Assert.AreEqual( 110, buffer[20] );

        }

        private void CompareMd5Hash( string sourceMd5, byte[] buffer )
        {
            using ( MD5 md5 = MD5.Create() )
            {
                byte[] hash = md5.ComputeHash( buffer, 0, buffer.Length );

                var result = new StringBuilder( hash.Length * 2 );

                foreach ( byte t in hash )
                {
                    result.Append( t.ToString( "x2" ) );
                }


                Assert.AreEqual( result.ToString(), sourceMd5 );
            }
        }
    }
}