using System;
using System.Configuration;
using MongoDB.Driver;
using MongoDB.Driver.GridFS;

namespace IntelliTect.Training.Mongo
{
    public static class MongoLegacy
    {
        // NB: GridFS support in the new 2.0 driver is scheduled for version 2.1 https://jira.mongodb.org/browse/CSHARP-1191

        static MongoLegacy()
        {
            Server =
                    new MongoServer( new MongoServerSettings
                                     {
                                             Server =
                                                     new MongoServerAddress(
                                                     ConfigurationManager.AppSettings[
                                                             "MongoAddress"],
                                                     Convert.ToInt32(
                                                             ConfigurationManager.AppSettings[
                                                                     "MongoPort"] ) )
                                     } );
            GridFs = new MongoGridFS( Server, "mongo-training", new MongoGridFSSettings() );
        }

        public static MongoGridFS GridFs { get; set; }
        public static MongoServer Server { get; set; }
    }
}