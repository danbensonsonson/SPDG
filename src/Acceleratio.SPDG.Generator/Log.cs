using System;
using System.IO;

namespace Acceleratio.SPDG.Generator 
{
    public class Log
    {
        private const string LOG_DIRECTORY = "Log\\";
        public static void Write(string message)
        {
            try
            {
                if (!Directory.Exists(LOG_DIRECTORY))
                    Directory.CreateDirectory(LOG_DIRECTORY);
                using (StreamWriter writer = new StreamWriter(LOG_DIRECTORY + DataGenerator.SessionID + ".log", true))
                {
                    writer.WriteLine(DateTime.Now.ToShortDateString() + " " + DateTime.Now.ToLongTimeString() + "\t" + message);
                }
            }
            catch (Exception ex)
            { 
                // do nothing, jsut missing a message
            }
        }
    }
}
