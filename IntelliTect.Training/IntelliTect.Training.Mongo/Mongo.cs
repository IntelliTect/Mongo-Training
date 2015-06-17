using System.Configuration;
using MongoDB.Bson;
using MongoDB.Driver;

namespace IntelliTect.Training.Mongo
{
    public static class Mongo
    {
        static Mongo()
        {
            Client = new MongoClient(ConfigurationManager.ConnectionStrings["Mongo"].ConnectionString);
            TrainingDatabase = Client.GetDatabase( "mongo-training" );
            RawCollection = TrainingDatabase.GetCollection<BsonDocument>( "example" );
        }

        public static IMongoClient Client { get; private set; }

        public static IMongoDatabase TrainingDatabase { get; private set; }

        public static IMongoCollection<BsonDocument> RawCollection { get; private set; }

    }
}