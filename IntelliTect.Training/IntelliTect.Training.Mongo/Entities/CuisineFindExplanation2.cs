using MongoDB.Bson.Serialization.Attributes;

namespace IntelliTect.Training.Mongo.Entities.Two
{

    public class CuisineFindExplanation2
    {
        public Queryplanner queryPlanner { get; set; }
        public Executionstats executionStats { get; set; }
        public Serverinfo serverInfo { get; set; }
    }

    public class Queryplanner
    {
        public int plannerVersion { get; set; }
        [BsonElement("namespace")]
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
        [BsonElement("$eq")]
        public string eq { get; set; }
    }

    public class Winningplan
    {
        public string stage { get; set; }
        public Inputstage inputStage { get; set; }
    }

    public class Inputstage
    {
        public string stage { get; set; }
        public Keypattern keyPattern { get; set; }
        public string indexName { get; set; }
        public bool isMultiKey { get; set; }
        public string direction { get; set; }
        public Indexbounds indexBounds { get; set; }
    }

    public class Keypattern
    {
        public int cuisine { get; set; }
    }

    public class Indexbounds
    {
        public string[] cuisine { get; set; }
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
        public int docsExamined { get; set; }
        public int alreadyHasObj { get; set; }
        public Inputstage1 inputStage { get; set; }
    }

    public class Inputstage1
    {
        public string stage { get; set; }
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
        public Keypattern1 keyPattern { get; set; }
        public string indexName { get; set; }
        public bool isMultiKey { get; set; }
        public string direction { get; set; }
        public Indexbounds1 indexBounds { get; set; }
        public int keysExamined { get; set; }
        public int dupsTested { get; set; }
        public int dupsDropped { get; set; }
        public int seenInvalidated { get; set; }
        public int matchTested { get; set; }
    }

    public class Keypattern1
    {
        public int cuisine { get; set; }
    }

    public class Indexbounds1
    {
        public string[] cuisine { get; set; }
    }

    public class Serverinfo
    {
        public string host { get; set; }
        public int port { get; set; }
        public string version { get; set; }
        public string gitVersion { get; set; }
    }

}