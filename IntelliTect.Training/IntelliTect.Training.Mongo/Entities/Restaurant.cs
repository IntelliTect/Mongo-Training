using System;
using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;

// ReSharper disable InconsistentNaming

namespace IntelliTect.Training.Mongo.Entities
{
    public class Restaurant
    {
        [BsonId]
        public ObjectId Id { get; set; }

        public Address address { get; set; }
        public string borough { get; set; }
        public string cuisine { get; set; }
        public Grade[] grades { get; set; }
        public string name { get; set; }
        public string restaurant_id { get; set; }
    }

    public class Address
    {
        public string building { get; set; }
        public double[] coord { get; set; }
        public string street { get; set; }
        public string zipcode { get; set; }
    }

    public class Grade
    {
        public DateTime date { get; set; }
        public string grade { get; set; }
        public int? score { get; set; }
    }
}