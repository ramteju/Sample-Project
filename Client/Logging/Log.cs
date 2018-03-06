using Client.Views;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using MongoDB.Driver;
using Client.Common;
using System.Diagnostics;

namespace Client.Logging
{
    public static class Log
    {
        private static readonly string MONGO_USER = ConfigurationManager.AppSettings["MONGO_USER"];
        private static readonly string MONGO_PASSWORD = ConfigurationManager.AppSettings["MONGO_PASSWORD"];
        private static readonly string MONGO_URL = ConfigurationManager.AppSettings["MONGO_URL"];
        private static readonly string LOG_ENTRIES = "LogEntries";
        private static readonly string DB = "Reactions";
        private static string version = S.VersionInfo;

        private static IMongoClient mongoClient;
        private static IMongoDatabase mongoDB;

        static private void Init()
        {
            mongoClient = new MongoClient(MONGO_URL);
            mongoDB = mongoClient.GetDatabase(DB);
        }

        public static void This(Exception ex)
        {
            if (ex != null)
            {
                try
                {
                    if (mongoDB == null)
                        Init();

                    List<string> sources = new List<string>();
                    StackTrace stackTrace = new StackTrace(ex, true);
                    if (stackTrace != null && stackTrace.GetFrames() != null)
                        sources = stackTrace.GetFrames().Select(f => f.GetFileName() + " - " + f.GetFileLineNumber()).ToList();
                    var logEntries = mongoDB.GetCollection<MongoLogEntry>(LOG_ENTRIES);
                    logEntries.InsertOne(new MongoLogEntry
                    {
                        StackTrace = ex.ToString(),
                        System = Environment.MachineName,
                        User = Environment.UserName,
                        Message = ex.Message,
                        Version = version,
                        TanId = T.TanId,
                        Source = String.Join(";", sources),
                        InnerStackTrace = ex.InnerException?.ToString(),
                        Timestamp = DateTime.Now,
                    });
                }
                catch (Exception e)
                {
                    AppErrorBox.ShowErrorMessage("Unable To Log Error, Please Report To IT Team", ex.ToString() + Environment.NewLine + e.ToString());
                }
            }
        }
    }
}
