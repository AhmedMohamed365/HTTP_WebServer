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
            StreamWriter logger_writer = new StreamWriter("log.txt",false);
            //Datetime:
            DateTime todayDate = new DateTime();
            //message:
            logger_writer.WriteLine(ex.Message + todayDate.ToString() );

            logger_writer.Close();
            // for each exception write its details associated with datetime 
        }
    }
}
