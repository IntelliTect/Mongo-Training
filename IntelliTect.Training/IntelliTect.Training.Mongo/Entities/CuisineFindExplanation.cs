using MongoDB.Bson.Serialization.Attributes;

namespace IntelliTect.Training.Mongo.Entities
{
    public class CuisineFindExplanation
    {
        public Queryplanner queryPlanner { get; set; }
        public Executionstats executionStats { get; set; }
        public Serverinfo serverInfo { get; set; }
    }

    public class Queryplanner
    {
        public int plannerVersion { get; set; }

        [BsonElement( "namespace" )]
        public string _namespace { get; set; }

        public bool indexFilterSet { get; set; }
        public Parsedquery parsedQuery { get; set; }
        public Winningplan winningPlan { get; set; }
        public object[] rejectedPlans { get; set; }
    }

    public class Parsedquery
    {
        public Cuisine cuisine { get; set; }
    }

    public class Cuisine
    {
        [BsonElement( "$eq" )]
        public string eq { get; set; }
    }

    public class Winningplan
    {
        public string stage { get; set; }
        public Filter filter { get; set; }
        public string direction { get; set; }
    }

    public class Filter
    {
        public Cuisine cuisine { get; set; }
    }

    public class Executionstats
    {
        public bool executionSuccess { get; set; }
        public int nReturned { get; set; }
        public int executionTimeMillis { get; set; }
        public int totalKeysExamined { get; set; }
        public int totalDocsExamined { get; set; }
        public Executionstages executionStages { get; set; }
        public object[] allPlansExecution { get; set; }
    }

    public class Executionstages
    {
        public string stage { get; set; }
        public Filter filter { get; set; }
        public int nReturned { get; set; }
        public int executionTimeMillisEstimate { get; set; }
        public int works { get; set; }
        public int advanced { get; set; }
        public int needTime { get; set; }
        public int needFetch { get; set; }
        public int saveState { get; set; }
        public int restoreState { get; set; }
        public int isEOF { get; set; }
        public int invalidates { get; set; }
        public string direction { get; set; }
        public int docsExamined { get; set; }
    }


    public class Serverinfo
    {
        public string host { get; set; }
        public int port { get; set; }
        public string version { get; set; }
        public string gitVersion { get; set; }
    }
}