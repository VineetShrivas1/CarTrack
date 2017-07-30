using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace CarTrackAPI.Utilities
{
    public class LoggingUtility
    {
        public static void Log(string error,bool isRequest)
        {
            string message = String.Empty;
            if (isRequest)
                message += "Request: " + error;
            else
                message += "Resposne: " + error;
            Elmah.ErrorLog.GetDefault(HttpContext.Current).Log(new Elmah.Error(new Exception(message)));
        }
        public static void Log<T>(T error, bool isRequest)
        {
            string message = String.Empty;
            if (isRequest)
                message += "Request: " + error;
            else
                message += "Resposne: " + error;
            Elmah.ErrorLog.GetDefault(HttpContext.Current).Log(new Elmah.Error(new Exception(message)));
        }
    }
}