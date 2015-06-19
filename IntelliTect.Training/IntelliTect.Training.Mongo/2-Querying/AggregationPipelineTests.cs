using System.Linq;
using System.Threading.Tasks;
using IntelliTect.Training.Mongo.Entities;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using MongoDB.Driver;

// ReSharper disable InconsistentNaming

namespace IntelliTect.Training.Mongo
{
    [TestClass]
    public class AggregationPipelineTests
    {
        [ClassInitialize]
        public static void SetupIndexes( TestContext context )
        {
            Mongo.ExampleCollection.Indexes.DropAllAsync().Wait();
            IndexKeysDefinitionBuilder<Restaurant> builder = Builders<Restaurant>.IndexKeys;

            IndexKeysDefinition<Restaurant> newIndex = builder.Ascending( x => x.cuisine );
            Mongo.ExampleCollection.Indexes.CreateOneAsync( newIndex ).Wait();

            IndexKeysDefinition<Restaurant> zipIndex = builder.Ascending( x => x.address.zipcode );
            Mongo.ExampleCollection.Indexes.CreateOneAsync( zipIndex ).Wait();

            IndexKeysDefinition<Restaurant> geoIndex = builder.Geo2D( x => x.address.coord );
            Mongo.ExampleCollection.Indexes.CreateOneAsync( geoIndex ).Wait();
        }

        [TestMethod]
        public async Task WhenAggregateWithAverage_ItReturnsExpectedResults()
        {
            // Arrange
            var pipeline =
                    Mongo.ExampleCollection.Aggregate( new AggregateOptions { AllowDiskUse = true } )  // Allow Mongod to use disk if memory is low
                            .Match( restaurant => restaurant.cuisine == "Irish" && restaurant.borough == "Manhattan" )
                            .Group( restaurant => restaurant.address.zipcode,
                                    g =>
                                            new
                                            {
                                                    Zip = g.Key,
                                                    Count = g.Count()
                                            } )
                            .SortBy( x => x.Zip );

            // Act
            var results = await pipeline.ToListAsync();

            // Assert
            Assert.AreEqual( 28, results.Count );
        }
    }
}