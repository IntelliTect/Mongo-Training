using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using IntelliTect.Training.Mongo.Entities;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using MongoDB.Driver;

// ReSharper disable InconsistentNaming

namespace IntelliTect.Training.Mongo
{
    [TestClass]
    public class FilterTests
    {
        [ClassInitialize]
        public static void SetupIndexes( TestContext context )
        {
            Mongo.ExampleCollection.Indexes.DropAllAsync().Wait();
            IndexKeysDefinitionBuilder<Restaurant> builder = Builders<Restaurant>.IndexKeys;

            IndexKeysDefinition<Restaurant> newIndex = builder.Ascending( x => x.address.zipcode );
            Mongo.ExampleCollection.Indexes.CreateOneAsync( newIndex ).Wait();

            IndexKeysDefinition<Restaurant> textIndex = builder.Text( x => x.name );
            Mongo.ExampleCollection.Indexes.CreateOneAsync( textIndex ).Wait();

            IndexKeysDefinition<Restaurant> geoIndex = builder.Geo2D( x => x.address.coord );
            Mongo.ExampleCollection.Indexes.CreateOneAsync( geoIndex ).Wait();
        }

        [TestMethod]
        public async Task WhenFindFirstWithMultipleHits_ItGetsTheFirstOne()
        {
            // Arrange
            FilterDefinition<Restaurant> filter = Builders<Restaurant>.Filter.Eq( x => x.address.zipcode, "11224" );

            // Act
            Restaurant restaurant = await Mongo.ExampleCollection.Find( filter ).FirstAsync();

            // Assert
            Assert.AreEqual( "40356018", restaurant.restaurant_id );
        }

        [TestMethod]
        public async Task WhenFindSingleWithMultipleHits_ItThrowsInvalidOperation()
        {
            // Arrange
            FilterDefinition<Restaurant> filter = Builders<Restaurant>.Filter.Eq( x => x.address.zipcode, "11224" );

            // Act
            try
            {
                await Mongo.ExampleCollection.Find( filter ).SingleAsync();
            }
            catch ( InvalidOperationException exception )
            {
                Assert.IsNotNull( exception );
                return;
            }

            Assert.Fail( "No exception caught" );
        }

        [TestMethod]
        public async Task WhenFindingCaseInsensitiveText_ItWorksWithText()
        {
            // Arrange
            FilterDefinition<Restaurant> filter = Builders<Restaurant>.Filter.Text( "castle" );
            // Note, no field is specified
            // .Text performs a text search on the content of ALL the fields indexed with a text index

            // Act
            List<Restaurant> results = await Mongo.ExampleCollection.Find( filter ).ToListAsync();

            // Assert
            Assert.IsTrue( results.Any( x => x.name == "White Castle" ) );
            Assert.IsTrue( results.Any( x => x.name == "Burp Castle" ) );
            Assert.IsTrue( results.Any( x => x.name == "Spa Castle/Juice Farm" ) );
            // It understands stop or break characters
        }

        [TestMethod]
        public async Task WhenFindingWithGeoSpatialQuery_ItFindsResults()
        {
            // Arrange
            FilterDefinition<Restaurant> filter = Builders<Restaurant>.Filter.GeoWithinCenter( x => x.address.coord,
                    -74,
                    40.74,
                    0.005 );

            // Act
            List<Restaurant> results = await Mongo.ExampleCollection.Find( filter ).ToListAsync();

            // Assert
            Assert.AreEqual( 244, results.Count );
            Assert.IsTrue( results.Any( x => x.name == "The Grey Dog" ) );
            // Verify here: https://www.google.com/maps/search/restaurants/@40.7399994,-74,18z/data=!4m7!2m6!3m5!1srestaurants!2s40.739999999999995,+-74!4m2!1d-74!2d40.74
        }

        [TestMethod]
        public async Task WhenFindingWithElemMatch_ItReturnsExpectedResults()
        {
            // Arrange
            FilterDefinition<Restaurant> filter = Builders<Restaurant>.Filter.ElemMatch( x => x.grades,
                    grade => grade.score >= 80 && grade.score <= 90 );

            // Act
            List<Restaurant> eightiethPercentile = await Mongo.ExampleCollection.Find( filter ).ToListAsync();

            // Assert
            Assert.AreEqual( 9, eightiethPercentile.Count );

            foreach ( Restaurant restaurant in eightiethPercentile )
            {
                Trace.WriteLine( restaurant.name );
                foreach ( Grade grade in restaurant.grades )
                {
                    Trace.WriteLine( "\t" + grade.score );
                }
            }
        }
    }
}