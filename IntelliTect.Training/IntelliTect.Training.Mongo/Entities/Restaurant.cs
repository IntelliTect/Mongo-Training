using System;

namespace IntelliTect.Training.Mongo.Entities
{

    public class Restaurant
    {
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
        public float[] coord { get; set; }
        public string street { get; set; }
        public string zipcode { get; set; }
    }

    public class Grade
    {
        public DateTimeOffset date { get; set; }
        public string grade { get; set; }
        public int score { get; set; }
    }

}