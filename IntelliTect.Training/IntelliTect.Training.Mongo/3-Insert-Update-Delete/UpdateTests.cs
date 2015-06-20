using System.Threading.Tasks;
using IntelliTect.Training.Mongo.Entities;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using MongoDB.Driver;

// ReSharper disable InconsistentNaming

namespace IntelliTect.Training.Mongo
{
    [TestClass]
    public class UpdateTests
    {
        [TestMethod]
        public async Task WhenReplacingExistingSingleDocument_ItWorks()
        {
            // Arrange
            Restaurant bullys = await Mongo.ExampleCollection.Find( x => x.restaurant_id == "40361708" ).SingleAsync();

            // Act
            bullys.name = "Bully's Deli®";
            ReplaceOneResult result =
                    await
                            Mongo.ExampleCollection.ReplaceOneAsync( x => x.Id == bullys.Id,
                                    bullys,
                                    new UpdateOptions { IsUpsert = false } );

            // Assert
            Assert.AreEqual( 1, result.MatchedCount );
            Assert.AreEqual( 1, result.ModifiedCount );

            FilterDefinition<Restaurant> filter = Builders<Restaurant>.Filter.Eq( x => x.restaurant_id, "40361708" );
            UpdateDefinition<Restaurant> update = Builders<Restaurant>.Update.Set( x => x.name, "Bully'S Deli" );
            UpdateResult cleanup = await Mongo.ExampleCollection.UpdateOneAsync( filter, update );
            Assert.AreEqual( 1, cleanup.ModifiedCount );
            Assert.AreEqual( 1, cleanup.ModifiedCount );
        }

        [TestMethod]
        public async Task WhenUpsertingWithNewDocTwice_ItInsertsThenUpdates()
        {
            // Arrange
            Restaurant mine = Mongo.GetDumpsterPalace();
            FilterDefinition<Restaurant> filter = Builders<Restaurant>.Filter.Eq( x => x.restaurant_id, "9988776655" );

            // Act
            ReplaceOneResult result1 =
                    await
                            Mongo.ExampleCollection.ReplaceOneAsync( filter,
                                    mine,
                                    new UpdateOptions { IsUpsert = true } );

            // Assert
            Assert.AreEqual( 0, result1.MatchedCount );
            Assert.IsNotNull( result1.UpsertedId );

            // 2nd Act
            mine.cuisine = "Paleo-friendly";
            ReplaceOneResult result2 =
                    await
                            Mongo.ExampleCollection.ReplaceOneAsync( x => x.Id == result1.UpsertedId,
                                    mine,
                                    new UpdateOptions { IsUpsert = true } );

            // 2nd Assert
            Assert.AreEqual( 1, result2.MatchedCount );
            Assert.AreEqual( result1.UpsertedId, mine.Id );

            DeleteResult result = await Mongo.ExampleCollection.DeleteManyAsync( x => x.restaurant_id == "9988776655" );
            Assert.IsTrue( result.DeletedCount > 0 );
        }
    }
}