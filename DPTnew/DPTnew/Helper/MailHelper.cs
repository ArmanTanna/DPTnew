using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Mail;
using System.Web;

namespace DPTnew.Helper
{
    public class MailHelper
    {
        public static void SendMail(MailMessage mail)
        {
            SmtpClient client = new SmtpClient();
            client.Port = 25;
            client.DeliveryMethod = SmtpDeliveryMethod.Network;
            client.UseDefaultCredentials = false;
            client.Credentials = new NetworkCredential(System.Configuration.ConfigurationManager.AppSettings["hostusername"],
                System.Configuration.ConfigurationManager.AppSettings["hostpassword"]);
            client.Host = System.Configuration.ConfigurationManager.AppSettings["host"];
            client.Port = 587;
            client.EnableSsl = true;
            client.Send(mail);
        }
    }
}