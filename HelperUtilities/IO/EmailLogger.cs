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
            string userName, string password, bool isSSL)
        {
            StringBuilder sb = new StringBuilder();
            sb.AppendLine(ex.Source + " - " + ex.Message + $" ({DateTime.Now.ToString("yyyy MM dd HH:mm:ss")})");
            sb.AppendLine(ex.StackTrace);
            do
            {
                ex = ex.InnerException;
                if (ex != null)
                {
                    sb.AppendLine(ex.Source + " - " + ex.Message);
                    sb.AppendLine(ex.StackTrace);
                }

            } while (ex.InnerException != null);

            return SendMail(fromAddr, toAddr, subject, $"{htmlBody}<br /> Exception from ZendeskUsersSync {DateTime.Now.ToString("yyyy MM dd")} {sb.ToString()}", smtpHost, smtpPort, userName, password, isSSL);
        }

        public static bool SendMail(string fromAddr, string toAddr, string subject, string htmlBody, string smtpHost, int smtpPort,
            string userName, string password, bool isSSL)
        {

            MailMessage msg = new MailMessage(fromAddr, toAddr, subject, htmlBody);
            msg.IsBodyHtml = true;
            SmtpClient client = new SmtpClient();
            client.Host = smtpHost;
            client.Port = smtpPort;
            client.UseDefaultCredentials = false;
            client.Credentials = new NetworkCredential(userName, password);
            client.EnableSsl = isSSL;
            client.Send(msg);
            return true;
        }
    }
}
