using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Web;

namespace ProductTracking.Store
{
    public class ShipmentTrace
    {
        public void WriteTrace(string BatchNo, string Message)
        {
            try
            {
                string traceFolder = "TRACE";
                string traceFileName = BatchNo + "_ShipmentTrace.txt";
                string applPath = Environment.CurrentDirectory;
                string traceFolderPath = System.Web.HttpContext.Current.Server.MapPath($"~/{traceFolder}/");
                if (!Directory.Exists(traceFolderPath))
                    Directory.CreateDirectory(traceFolderPath);
                System.IO.File.AppendAllText(traceFolderPath + traceFileName, DateTime.Now + "-------" + Message + Environment.NewLine);
            }
            catch (Exception ex)
            {
                throw ex;
            }

        }
    }

    public static class LogSteps
    {
        public static void Log(string message)
        {
            try
            {
                string traceFolder = "TRACE";
                string traceFileName = "_TanTrace.log";
                string applPath = Environment.CurrentDirectory;
                string traceFolderPath = System.Web.HttpContext.Current.Server.MapPath($"~/{traceFolder}/");
                if (!Directory.Exists(traceFolderPath))
                    Directory.CreateDirectory(traceFolderPath);
                System.IO.File.AppendAllText(traceFolderPath + traceFileName, DateTime.Now + "-------" + message + Environment.NewLine);
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
    }
}