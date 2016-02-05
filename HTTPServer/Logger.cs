using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;

namespace HTTPServer
{
    class Logger
    {
        public static void LogException(Exception ex)
        {
            // TODO: Create log file named log.txt to log exception details in it
            StreamWriter logger_sw = new StreamWriter("log.txt");
            // for each exception write its details associated with datetime 
            logger_sw.WriteLine("Date Time :" + DateTime.Now);
            logger_sw.WriteLine("Exception :" + ex.Message);
            logger_sw.Close();

        }
    }
}
