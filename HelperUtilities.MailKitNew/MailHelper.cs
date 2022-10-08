using MailKit.Net.Smtp;
using MimeKit;
using System;
using System.Collections.Generic;
using System.Linq;
namespace HelperUtilities.MailKitNew
{


    public class MailHelper
    {
        int _port = 587;
        string _smtpServer = "smtp.gmail.com";
        string _mailUserName = "";
        string _mailPassword = "";
        bool _requiresCredentials = true;
        bool _isHtml = true;
        string _fromEmailAddress = "";
        string _fromEmailName = "";
        public MailHelper(int? port = null, string smtpServer = null, string mailUserName = null, string mailPassword = null, bool? requireCredentials = null, bool? isHtml = null, string fromEmailAddress = null, string fromEmailName = null)
        {
            _port = port.HasValue ? port.Value : _port;
            _smtpServer = string.IsNullOrWhiteSpace(smtpServer) ? _smtpServer : smtpServer;
            _mailUserName = mailUserName ?? _mailPassword;
            _mailPassword = mailPassword ?? _mailPassword;
            _requiresCredentials = requireCredentials.HasValue ? requireCredentials.Value : _requiresCredentials;
            _isHtml = isHtml.HasValue ? isHtml.Value : _isHtml;
            _fromEmailAddress = string.IsNullOrWhiteSpace(fromEmailAddress) ? _fromEmailAddress : fromEmailAddress;
            _fromEmailName = string.IsNullOrWhiteSpace(fromEmailName) ? _fromEmailAddress : fromEmailName;
        }

        public static string ReplaceLineBreaksWithBr(string body, string replacement)
        {
            return body.Replace(Environment.NewLine, replacement)
                        .Replace("\r", replacement)
                        .Replace("\n", replacement);
        }

        public static string ReplaceHtmlFromLineBreaks(string body)
        {
            return body.Replace("<br />", Environment.NewLine)
                        .Replace("<br/>", Environment.NewLine)
                        .Replace("<br>", Environment.NewLine);
        }

        public bool SendMail(string toEmail, string toName, string FromEmail, string fromEmailName, string subject, string body)
        {
            return SendMail(new Dictionary<string, string>
            {
                { toEmail, toName }
            }, FromEmail, fromEmailName, subject, body);
        }

        public bool SendMail(List<string> toEmail, string FromEmail, string fromEmailName, string subject, string body)
        {
            if (toEmail is null || toEmail.Count == 0) return false;
            Dictionary<string, string> _dict = new Dictionary<string, string>();
            foreach (string k in toEmail)
            {
                _dict.Add(k, k);
            }
            return SendMail(_dict, FromEmail, fromEmailName, subject, body);
        }

        public bool SendMail(Dictionary<string, string> _dictTobeSent, string fromEmailAddress = null, string fromEmailName = null, string subject = null, string body = null)
        {
            fromEmailAddress = fromEmailAddress ?? _fromEmailAddress;
            fromEmailName = fromEmailName ?? _fromEmailName;
            if (string.IsNullOrWhiteSpace(fromEmailName)) fromEmailName = fromEmailAddress;
            if (string.IsNullOrWhiteSpace(fromEmailAddress)) return false;

            var message = new MimeMessage();
            message.From.Add(new MailboxAddress(fromEmailName, fromEmailAddress));
            if (_dictTobeSent is null || _dictTobeSent.Count == 0) return false;
            foreach (KeyValuePair<string, string> _dict in _dictTobeSent)
            {
                message.To.Add(new MailboxAddress(name: _dict.Value, address: _dict.Key));
            }

            message.To.Add(new MailboxAddress(
                _dictTobeSent.Values.FirstOrDefault(),
                _dictTobeSent.Keys.FirstOrDefault()
                ));

            message.Subject = subject;

            var textPart = new TextPart(_isHtml == true ? "html" : "plain");

            if (_isHtml)
            {
                textPart.Text = ReplaceLineBreaksWithBr(body, "<br />");
            }
            else
            {
                textPart.Text = ReplaceHtmlFromLineBreaks(body);
            }

            message.Body = textPart;

            using (var client = new SmtpClient())
            {
                client.Connect(_smtpServer, _port);
                //client.AuthenticationMechanisms.Remove("XOAUTH2");
                if (_requiresCredentials == true)
                {
                    client.Authenticate(_mailUserName, _mailPassword);
                }
                client.Send(message);
                client.Disconnect(true);
            }

            return true;
        }

        /*
         * Mime Content Holder
         * http://www.mimekit.net/docs/html/Creating-Messages.htm
            var message = new MimeMessage ();
            message.From.Add (new MailboxAddress ("Joey", "joey@friends.com"));
            message.To.Add (new MailboxAddress ("Alice", "alice@wonderland.com"));
            message.Subject = "How you doin?";

            var builder = new BodyBuilder ();

            // Set the plain-text version of the message text
            builder.TextBody = @"Hey Alice,

            What are you up to this weekend? Monica is throwing one of her parties on
            Saturday and I was hoping you could make it.

            Will you be my +1?

            -- Joey
            ";

            // In order to reference selfie.jpg from the html text, we'll need to add it
            // to builder.LinkedResources and then use its Content-Id value in the img src.
            var image = builder.LinkedResources.Add (@"C:\Users\Joey\Documents\Selfies\selfie.jpg");
            image.ContentId = MimeUtils.GenerateMessageId ();

            // Set the html version of the message text
            builder.HtmlBody = string.Format (@"<p>Hey Alice,<br>
            <p>What are you up to this weekend? Monica is throwing one of her parties on
            Saturday and I was hoping you could make it.<br>
            <p>Will you be my +1?<br>
            <p>-- Joey<br>
            <center><img src=""cid:{0}""></center>", image.ContentId);

            // We may also want to attach a calendar event for Monica's party...
            builder.Attachments.Add (@"C:\Users\Joey\Documents\party.ics");

            // Now we just need to set the message body and we're done
            message.Body = builder.ToMessageBody (); 

            Example 2
        var message = new MimeMessage();
    // add from, to, subject and other needed properties to your message

    var builder = new BodyBuilder();
    builder.HtmlBody = htmlContent;
    builder.TextBody = textContent;

    // you can either create MimeEntity object(s)
    // this might get handy in case you want to pass multiple attachments from somewhere else
    byte[] myFileAsByteArray = LoadMyFileAsByteArray();
    var attachments = new List<MimeEntity>
    {
    // from file
    MimeEntity.Load("myFile.pdf"),
    // file from stream
    MimeEntity.Load(new MemoryStream(myFileAsByteArray)),
    // from stream with a content type defined
    MimeEntity.Load(new ContentType("application", "pdf"), new MemoryStream(myFileAsByteArray))
    }

    // or add file directly - there are a few more overloads to this
    builder.Attachments.Add("myFile.pdf");
    builder.Attachments.Add("myFile.pdf", myFileAsByteArray);
    builder.Attachments.Add("myFile.pdf", myFileAsByteArray , new ContentType("application", "pdf"));

    // append previously created attachments
    foreach (var attachment in attachments)
    {
    builder.Attachments.Add(attachment);
    }

    message.Body = builder.ToMessageBody();
         */

    }

}