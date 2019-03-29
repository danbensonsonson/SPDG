using System;
using System.IO;

namespace Acceleratio.SPDG.Generator 
{
    public class Log
    {
        public static void Write(string message)
        {
            try
            {
                using (StreamWriter writer = new StreamWriter(DataGenerator.SessionID + ".log", true))
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
