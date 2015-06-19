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
            IMongoClient result = Mongo.Client;
            IMongoClient result2 = Mongo.Client;

            Assert.AreSame( result, result2 );
        }

        [TestMethod]
        public void WhenDatabaseIsRequestedTwice_ItGivesDifferentInstance()
        {
            IMongoDatabase db = Mongo.Client.GetDatabase( "mongo-training" );
            IMongoDatabase db2 = Mongo.Client.GetDatabase( "mongo-training" );
            IMongoDatabase staticdb = Mongo.TrainingDatabase;

            Assert.AreNotSame( db, db2 );
            Assert.AreNotSame( db, staticdb );
        }

        [TestMethod]
        public async Task WhenGetCollectionWithDifferentTypes_ItGivesDifferentInstances()
        {
            IMongoCollection<BsonDocument> bsonCollection =
                    Mongo.TrainingDatabase.GetCollection<BsonDocument>( "example" );
            IMongoCollection<Restaurant> typedCollection = Mongo.TrainingDatabase.GetCollection<Restaurant>( "example" );

            Assert.AreNotSame( bsonCollection, typedCollection );

            long count1 = await bsonCollection.CountAsync( new BsonDocument() );
            long count2 = await typedCollection.CountAsync( new BsonDocument() );

            Assert.AreEqual( count1, count2 );
        }
    }
}