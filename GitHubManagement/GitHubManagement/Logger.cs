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
        private static string pathLogData = null;

        public static void LogMessage(string message)
        {
            pathLogData = Environment.CurrentDirectory + "Log.txt";

            StreamWriter writer = new StreamWriter(pathLogData, append: true);
            writer.Write(DateTime.Now.ToString("dd:MM:yyyy") + "    " + message + "\r\n");

            writer.Close();
        }
    }
}
