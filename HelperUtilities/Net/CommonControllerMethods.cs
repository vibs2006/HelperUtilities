using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace HelperUtilities.Net
{
    public class CommonControllerMethods
    {
        public enum HtmlFontColor { Black, Red, DarkRed, Green, DarkGreen }

        public enum HtmlFontWeight { Normal, Bold }

        public enum HtmlFontSize
        {
            medium,
            xx_small,
            x_small,
            small,
            large,
            x_large,
            xx_large,
            smaller,
            larger,
            inherit
        }

        public static string HtmlGenerator(string htmlTextWithoutHtmlBodyTags, HtmlFontColor fontColorValue = HtmlFontColor.Black, HtmlFontWeight fontWeightValue = HtmlFontWeight.Normal, HtmlFontSize fontSizeEnum = HtmlFontSize.inherit)
        {
            string _fontSize = fontSizeEnum.ToString().Replace('_', '-');
            StringBuilder sb = new StringBuilder();
            sb.Append("<html><body style='text-align:center'>");
            sb.Append($"<div style='color:{fontColorValue.ToString().ToLower()};font-weight:{fontWeightValue.ToString().ToLower()};font-size:{_fontSize};padding: 20px 20px 20px 20px; margin: 20px 20px 20px 20px'>");
            sb.Append(htmlTextWithoutHtmlBodyTags);
            sb.Append("</div></body></html>");
            return sb.ToString();
        }

        public static HttpResponseMessage InitializeController(string applicationName, string logDirectoryName = "log", bool checkFileWriteStatus = true)
        {
            if (string.IsNullOrWhiteSpace(logDirectoryName)) logDirectoryName = "log";

            if (checkFileWriteStatus)
            {
                var path = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, logDirectoryName);
                if (!Directory.Exists(path))
                {
                    try
                    {
                        //throw new Exception("test");
                        Directory.CreateDirectory(path);
                    }
                    catch
                    {
                        var message = new HttpResponseMessage(HttpStatusCode.InternalServerError)
                        {
                            Content = new StringContent(HtmlGenerator("Unable to Write / Access Directory \'" + logDirectoryName + "\'", HtmlFontColor.DarkRed, HtmlFontWeight.Bold), Encoding.Default, "text/html")
                        };
                        return message;
                    }
                }

                try
                {
                    //throw new Exception("test");
                    var fileName = Guid.NewGuid().ToString() + ".txt";
                    File.Create(path + "/" + fileName).Close();
                    File.Delete(path + "/" + fileName);
                }
                catch
                {
                    var message = new HttpResponseMessage(HttpStatusCode.InternalServerError)
                    {
                        Content = new StringContent(HtmlGenerator("Unable to Write/Delete Files in Directory <span style=\'font-size:larger\'>\'" + logDirectoryName + "\'</span>.", HtmlFontColor.DarkRed, HtmlFontWeight.Bold), Encoding.Default, "text/html")
                    };
                    return message;
                }
            }

            Assembly assembly = Assembly.GetExecutingAssembly();
            FileInfo fileInfo = new FileInfo(assembly.Location);
            DateTime lastModified = fileInfo.LastWriteTime;
            var response = new HttpResponseMessage
            {
                Content = new StringContent(HtmlGenerator($"{applicationName}.{lastModified.ToString("yyyy.MM.dd.HH.mm")}", HtmlFontColor.DarkGreen, HtmlFontWeight.Bold, HtmlFontSize.larger), Encoding.Default, "text/html")
            };
            return response;
        }
    }
}
