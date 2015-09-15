using System;
using System.Configuration;
using IntelliTect.Training.Mongo.Entities;
using MongoDB.Bson;
using MongoDB.Bson.Serialization;
using MongoDB.Driver;

namespace IntelliTect.Training.Mongo
{
    public static class Mongo
    {
        static Mongo()
        {
            BsonClassMap.RegisterClassMap<Restaurant>();
            BsonClassMap.RegisterClassMap<CuisineFindExplanation>();

            Client = new MongoClient( ConfigurationManager.ConnectionStrings["Mongo"].ConnectionString );
            TrainingDatabase = Client.GetDatabase( "mongo-training" );
            RawCollection = TrainingDatabase.GetCollection<BsonDocument>( "example" );
            ExampleCollection = TrainingDatabase.GetCollection<Restaurant>( "example" );
        }

        public static IMongoCollection<Restaurant> ExampleCollection { get; set; }
        public static IMongoClient Client { get; private set; }
        public static IMongoDatabase TrainingDatabase { get; private set; }
        public static IMongoCollection<BsonDocument> RawCollection { get; private set; }

        public static Restaurant GetDumpsterPalace()
        {
            return new Restaurant
                   {
                           address = new Address
                                     {
                                             building = "1234",
                                             street = "2nd Avenue",
                                             zipcode = "10075",
                                             coord = new[] { 73.9557413, 40.7720266 }
                                     },
                           borough = "Manhattan",
                           cuisine = "Freegan",
                           grades = new[]
                                    {
                                            new Grade
                                            {
                                                    date = DateTime.UtcNow.AddDays( -7 ),
                                                    grade = "F",
                                                    score = 3
                                            },
                                            new Grade
                                            {
                                                    date = DateTime.UtcNow.AddMonths( -1 ),
                                                    grade = "A",
                                                    score = 97
                                            }
                                    },
                           name = "Kelly's Dumpster Palace",
                           restaurant_id = "9988776655"
                   };
        }
    }
}