﻿using System.Collections.Generic;
using System.Diagnostics;
using System.Threading.Tasks;
using IntelliTect.Training.Mongo.Entities;
using IntelliTect.Training.Mongo.Entities.Two;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using MongoDB.Bson;
using MongoDB.Bson.Serialization;
using MongoDB.Driver;

// ReSharper disable InconsistentNaming

namespace IntelliTect.Training.Mongo
{
    [TestClass]
    public class QueryTests
    {
        [TestMethod]
        public async Task WhenUsingFilterBuilder_ItGivesResults()
        {
            // Arrange
            FilterDefinition<Restaurant> filter = Builders<Restaurant>.Filter.Eq( x => x.borough, "Brooklyn" );

            // Act
            List<Restaurant> restaurants = await Mongo.ExampleCollection.Find( filter ).ToListAsync();

            // Assert
            Assert.AreEqual( 6086, restaurants.Count );
        }


        [TestMethod]
        public async Task WhenQueryingWithIndexes_ItIsFaster()
        {
            // Arrange
            await Mongo.ExampleCollection.Indexes.DropAllAsync();

            // Act
            Stopwatch sw1 = Stopwatch.StartNew();
            List<Restaurant> results1 = await Mongo.ExampleCollection.Find( x => x.cuisine == "Irish" ).ToListAsync();
            sw1.Stop();

            IndexKeysDefinitionBuilder<Restaurant> indexer = Builders<Restaurant>.IndexKeys;
            IndexKeysDefinition<Restaurant> newIndex = indexer.Ascending( x => x.cuisine );
            await Mongo.ExampleCollection.Indexes.CreateOneAsync( newIndex );

            Stopwatch sw2 = Stopwatch.StartNew();
            List<Restaurant> results2 = await Mongo.ExampleCollection.Find( x => x.cuisine == "Irish" ).ToListAsync();
            sw2.Stop();

            // Assert
            Assert.AreEqual( results1.Count, results2.Count );
            Assert.IsTrue( sw1.ElapsedMilliseconds > sw2.ElapsedMilliseconds );

            Trace.WriteLine( string.Format( "Without index: {0}ms.  With index: {1}ms.",
                    sw1.ElapsedMilliseconds,
                    sw2.ElapsedMilliseconds ) );
        }

        [TestMethod]
        public async Task WhenUsingExplain_ItShowsIndexHits()
        {
            // Arrange
            await Mongo.ExampleCollection.Indexes.DropAllAsync();
            FilterDefinition<BsonDocument> filter = Builders<BsonDocument>.Filter.Eq( "cuisine", "Irish" );

            // Act
            BsonDocument results1 = await Mongo.RawCollection.Find(filter,
                    new FindOptions { Modifiers = new BsonDocument( "$explain", "1" ) } ).SingleOrDefaultAsync();


            IndexKeysDefinitionBuilder<Restaurant> indexer = Builders<Restaurant>.IndexKeys;
            IndexKeysDefinition<Restaurant> newIndex = indexer.Ascending( x => x.cuisine );
            await Mongo.ExampleCollection.Indexes.CreateOneAsync( newIndex );

            BsonDocument results2 = await Mongo.RawCollection.Find(filter,
                    new FindOptions { Modifiers = new BsonDocument("$explain", "1") }).SingleOrDefaultAsync();

            // Assert
            CuisineFindExplanation explain1 = BsonSerializer.Deserialize<CuisineFindExplanation>( results1 );
            CuisineFindExplanation2 explain2 = BsonSerializer.Deserialize<CuisineFindExplanation2>( results2 );

            Assert.AreEqual( "COLLSCAN", explain1.queryPlanner.winningPlan.stage); // Shows a full collection scan
            Assert.AreEqual( "IXSCAN", explain2.queryPlanner.winningPlan.inputStage.stage); // Shows an index scan

            Trace.WriteLine( results1.ToJson());
            Trace.WriteLine( results2.ToJson());
        }
    }
}