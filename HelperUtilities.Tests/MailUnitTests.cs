using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Diagnostics;
using HelperUtilities.IO;
using System.Globalization;
using HelperUtilities.MailKitNew;
using System.Collections.Generic;

namespace HelperUtilities.Tests
{
    [TestClass]
    public class MailUnitTests
    {
        [TestMethod]
        public void TestEmailSend()
        {
            MailHelper _mail = new MailHelper(mailUserName: "relio.mkt@gmail.com", mailPassword: "fhbqaa");
            Dictionary<string, string> email = new Dictionary<string, string>();
            email.Add("vibs2006@gmail.com", "Vaibhav Agarwal");
            _mail.SendMail(email, "relio.mkt@gmail.com", "Relio MKT", "Test Subject 1", "Test Mail Body.<br /><b>This is Bold</b><br />Regards");

            _mail = new MailHelper(mailUserName: "your_email@gmail.com", mailPassword: "fhbqaa",isHtml: false);
            _mail.SendMail(email, "relio.mkt@gmail.com", "Relio MKT", "Test Subject 1", "Test Mail Body.<br />This is text<br />Regards");
        }
    }
}
