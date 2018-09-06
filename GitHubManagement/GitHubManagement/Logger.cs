using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;

namespace GitHubManagement
{
    public class Logger
    {
        private static StreamWriter writer;
        private static string pathLogData = Environment.CurrentDirectory + "Log.txt";


        static Logger()
        {
            writer = new StreamWriter(pathLogData, append: true);
        }

        public static void LogMessage(string exceptionMessage, string methodenName, string message)
        {
            writer.Write(DateTime.Now.ToString("dd:MM:yyyy") + "    " + methodenName + "Message: "  + message + "\n" + "Exception :   " + exceptionMessage + "\r\n");
        }
        public static void CloseWriter()
        {
            writer.Close();
        }
    }
}
