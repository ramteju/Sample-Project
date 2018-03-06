using System;

namespace Client.Logging
{
    public class MongoLogEntry
    {
        public MongoDB.Bson.ObjectId _id { get; set; }
        public string System { get; set; }
        public string Message { get; set; }
        public string User { get; set; }
        public string Version { get; set; }
        public string StackTrace { get; set; }
        public string Source { get; set; }
        public string InnerStackTrace { get; set; }
        public DateTime Timestamp { get; set; }
        public int TanId { get; set; }
    }
}
