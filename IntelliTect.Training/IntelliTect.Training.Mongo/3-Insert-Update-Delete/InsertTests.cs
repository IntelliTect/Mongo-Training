using System;
using System.Threading.Tasks;
using IntelliTect.Training.Mongo.Entities;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using MongoDB.Driver;

// ReSharper disable InconsistentNaming

namespace IntelliTect.Training.Mongo
{
    [TestClass]
    public class InsertTests
    {
        [TestMethod]
        public async Task WhenInsertingAMappedType_ItWorks()
        {
            // Arrange
            var myRestaurant = Mongo.GetDumpsterPalace();

            // Act
            await Mongo.ExampleCollection.InsertOneAsync( myRestaurant );

            // Assert
            Restaurant inserted = await
                    Mongo.ExampleCollection.Find( x => x.restaurant_id == "9988776655" ).SingleOrDefaultAsync();

            Assert.IsNotNull( inserted );
            Assert.IsNotNull( inserted.Id );

            DeleteResult result = await Mongo.ExampleCollection.DeleteManyAsync( x => x.restaurant_id == "9988776655" );
            Assert.IsTrue( result.DeletedCount > 0 );
        }
    }
}