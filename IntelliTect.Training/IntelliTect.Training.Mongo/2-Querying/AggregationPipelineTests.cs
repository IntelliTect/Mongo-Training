using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using IntelliTect.Training.Mongo.Entities;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using MongoDB.Bson;
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
        public async Task WhenAggregateWithCount_ItReturnsExpectedResults()
        {
            // Arrange
            var pipeline =
                    Mongo.ExampleCollection.Aggregate( new AggregateOptions { AllowDiskUse = true } )
                            // Allow Mongod to use disk if memory is low
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

        [TestMethod]
        public async Task WhenAggregateAverageOnArray_ItReturnsExpectedResults()
        {
            // Arrange
            IAggregateFluent<BsonDocument> pipeline =
                    Mongo.ExampleCollection.Aggregate( new AggregateOptions { AllowDiskUse = true } )
                            // Allow Mongod to use disk if memory is low
                            .Match( restaurant => restaurant.cuisine == "Hamburgers" )
                            .Unwind( restaurant => restaurant.grades )
                            .Group( new BsonDocument
                                    {
                                            { "_id", new BsonDocument( "Name", "$name" ) },
                                            {
                                                    "Score",
                                                    new BsonDocument( "$avg", "$grades.score" )
                                            }
                                    } )
                            .Sort( new BsonDocument( "Score", -1 ) );


            // Act
            List<BsonDocument> results = await pipeline.ToListAsync();

            // Assert
            Assert.AreEqual( 84, results.Count );

            results.ForEach( x => Trace.WriteLine( x.ToJson() ) );
        }

        [TestMethod]
        public async Task WhenMultipleGroupingsWithAnonymousTypes_ItProjectsAsExpected()
        {
            // Arrange
            var pipeline = Mongo.ExampleCollection.Aggregate()
                    .Group( restaurant => new { Borough = restaurant.borough, Zip = restaurant.address.zipcode },
                            g =>
                                    new
                                    {
                                            BoroughAndZip = g.Key,
                                            NumberOfGrades = g.Sum( restaurant => restaurant.grades.Length )
                                    } )
                    .Group( x => x.BoroughAndZip.Borough,
                            g => new { Borough = g.Key, AverageNumberOfGradesPerZip = g.Average( x => x.NumberOfGrades ) } )
                    .SortBy( x => x.Borough );


            // Act
            var results = await pipeline.ToListAsync();

            // Assert
            Assert.AreEqual(6, results.Count);
            Assert.AreEqual("Bronx", results.First().Borough);
            results.ForEach(x => Trace.WriteLine(x.ToJson()));
        }
    }
}