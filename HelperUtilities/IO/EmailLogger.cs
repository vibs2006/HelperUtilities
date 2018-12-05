using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Mail;
using System.Text;
using System.Threading;

namespace HelperUtilties.IO
{
    public static class EmailLogger
    {
        public static bool SendMailWithException(Exception ex, string fromAddr, string toAddr, string subject, string htmlBody, string smtpHost, int smtpPort,
            string userName, string password, bool isSSL, bool UseDefaultCredentials = false)
        {
            StringBuilder sb = new StringBuilder();            
            do
            {
                sb.AppendLine(ex.Source + " - " + ex.Message + $" ({DateTime.Now.ToString("yyyy MM dd HH:mm:ss")}) <br />");
                sb.AppendLine(ex.StackTrace + " <br />");
                ex = ex.InnerException;
            } while (ex.InnerException != null);

            return SendMail(fromAddr, toAddr, subject, $"{htmlBody}<br /> Exception from ZendeskUsersSync {DateTime.Now.ToString("yyyy MM dd")} {sb.ToString()}", smtpHost, smtpPort, userName, password, isSSL, UseDefaultCredentials);
        }

        public static bool SendMail(string fromAddr, string toAddr, string subject, string htmlBody, string smtpHost, int smtpPort,
            string userName, string password, bool isSSL, bool UseDefaultCredentials = false)
        {

            MailMessage msg = new MailMessage(fromAddr, toAddr, subject, htmlBody);
            msg.IsBodyHtml = true;
            SmtpClient client = new SmtpClient();
            client.Host = smtpHost;
            client.Port = smtpPort;
            client.UseDefaultCredentials = UseDefaultCredentials;
            client.Credentials = new NetworkCredential(userName, password);
            client.EnableSsl = isSSL;
            client.Send(msg);
            return true;
        }
    }
}
