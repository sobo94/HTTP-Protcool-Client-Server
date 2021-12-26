using System;
using System.Collections.Generic;
using System.IO;
using System.Reflection;
using System.Threading;

namespace myOwnWebServer
{
    public class MyOwnLogger
    {
        public static void Log(string logTitle,string logMessage)
        {
            Thread newThread = new Thread(MyOwnLogger.Writer);
            Dictionary<string, string> logDictionary = new Dictionary<string, string>
            {
                {"logTitle", logTitle},
                {"logMessage", logMessage}
            };
            newThread.Start(logDictionary);
        }



        private static void Writer(object logInfo)
        {
            Dictionary<string, string> logDictionary = (Dictionary<string, String>) logInfo;
            string logTitle= logDictionary["logTitle"];
            string logMessage = logDictionary["logMessage"];
            string directoryName = Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location);
           
            
            try
            {
                using (StreamWriter w = File.AppendText(directoryName + "\\" + "myOwnWebServer.log"))
                {
                    Logger(logTitle,logMessage, w);
                }
            }
            catch (Exception)
            {
                // ignored
            }
        }

        private static void Logger(string logTitle,string logMessage, TextWriter txtWriter)
        {
            try
            {
                txtWriter.Write(" {0} {1}:{2}:{3} ",
                    DateTime.Now.ToShortDateString(), DateTime.Now.Hour,DateTime.Now.Minute,DateTime.Now.Second);
                txtWriter.Write("[ {0} ] - ",logTitle);
                txtWriter.Write(logMessage);
                txtWriter.WriteLine();
            }
            catch (Exception)
            {
                // ignored
            }
        }
    }
}
