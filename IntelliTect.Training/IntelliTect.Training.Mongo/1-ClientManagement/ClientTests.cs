using System.Threading.Tasks;
using IntelliTect.Training.Mongo.Entities;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using MongoDB.Bson;
using MongoDB.Driver;

// ReSharper disable InconsistentNaming

namespace IntelliTect.Training.Mongo
{
    [TestClass]
    public class ClientTests
    {
        [TestMethod]
        public void WhenClientIsRequestedTwice_ItGivesTheSameClient()
        {
            var result = Mongo.Client;
            var result2 = Mongo.Client;

            Assert.AreSame( result, result2 );
        }

        [TestMethod]
        public void WhenDatabaseIsRequestedTwice_ItGivesDifferentInstance()
        {
            var db = Mongo.Client.GetDatabase( "mongo-training" );
            var db2 = Mongo.Client.GetDatabase( "mongo-training" );
            var staticdb = Mongo.TrainingDatabase;

            Assert.AreNotSame( db, db2 );
            Assert.AreNotSame( db, staticdb );
        }

        [TestMethod]
        public async Task WhenGetCollectionWithDifferentTypes_ItGivesDifferentInstances()
        {
            var bsonCollection = Mongo.TrainingDatabase.GetCollection<BsonDocument>( "example" );
            var typedCollection = Mongo.TrainingDatabase.GetCollection<Restaurant>( "example" );

            Assert.AreNotSame( bsonCollection, typedCollection );

            var count1 = await bsonCollection.CountAsync( new BsonDocument() );
            var count2 = await typedCollection.CountAsync( new BsonDocument() );

            Assert.AreEqual( count1, count2 );
        }
    }
}