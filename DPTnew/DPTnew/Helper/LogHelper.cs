using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Mail;
using System.Web;

namespace DPTnew.Helper
{
    public class LogHelper
    {
        public static void WriteLog(string log)
        {
            StreamWriter sw = File.AppendText("E:\\Case\\log.txt");
            try
            {
                string logLine = System.String.Format(
                    "{0:G}: {1}.", System.DateTime.Now, log);
                sw.WriteLine(logLine);
            }
            finally
            {
                sw.Close();
            }
        }
    }
}