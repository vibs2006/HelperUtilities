using Newtonsoft.Json;
using System;
using System.Configuration;
using System.Net;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using HelperUtilties.IO;

namespace HelperUtilties.Rest
{
    public static class RestHttpClient
    {
        static HttpClient client;
        static HttpRequestMessage objHttpRequestMessage = null;
        static string jsonResult = string.Empty;
        static Writer wr;

        public static void initialize()
        {
            ServicePointManager.SecurityProtocol = SecurityProtocolType.Tls12;
            client = new HttpClient();
            wr = new Writer();
        }

        public static string RemoveLineEndings(this string value)
        {
            if (String.IsNullOrEmpty(value))
            {
                return value;
            }
            string lineSeparator = ((char)0x2028).ToString();
            string paragraphSeparator = ((char)0x2029).ToString();

            return value.Replace("\r\n", string.Empty)
                        .Replace("\n", string.Empty)
                        .Replace("\r", string.Empty)
                        .Replace(lineSeparator, string.Empty)
                        .Replace(paragraphSeparator, string.Empty);
        }

        public static OutputObjectType HttpJsonGet<OutputObjectType>(string url,bool logRestCalls = true, bool logExceptionMails = false)
        {
            if (client == null)
            {
                initialize();
                //var sp = ServicePointManager.FindServicePoint(new Uri(url));
                //sp.ConnectionLeaseTimeout = 60 * 1000; // 1 Minute
            }            

            string returnedJsonString = string.Empty;
            objHttpRequestMessage = new HttpRequestMessage();
            objHttpRequestMessage.Method = HttpMethod.Get;
            objHttpRequestMessage.Headers.Accept.Add(new System.Net.Http.Headers.MediaTypeWithQualityHeaderValue("application/json"));
            objHttpRequestMessage.Headers.Add("Authorization", ConfigurationManager.AppSettings.Get("AuthorizationHeader"));
            objHttpRequestMessage.RequestUri = new Uri(url);
            if (logRestCalls) wr.log($"*********Request to url starts at {url} ({DateTime.Now.ToString("yyyy MM dd HH:mm:ss")})", requireTimeStamp: false, fileName: $"RestAccessLog{DateTime.Now.ToString("yyyyMMdd")}.txt");
            HttpResponseMessage objHttpResponseMessage = null;
            try
            {
                Task<HttpResponseMessage> taskHttpResponsePost
                    = client.SendAsync(objHttpRequestMessage, HttpCompletionOption.ResponseContentRead);

                objHttpResponseMessage = taskHttpResponsePost.Result;
                returnedJsonString = objHttpResponseMessage.Content.ReadAsStringAsync().Result;
                jsonResult = returnedJsonString;

                if (logRestCalls)
                {
                    wr.log(returnedJsonString, requireTimeStamp: false, fileName: $"RestAccessLog{DateTime.Now.ToString("yyyyMMdd")}.txt");
                    wr.log($"*********Request to url ends at {url} ({DateTime.Now.ToString("yyyy MM dd HH:mm:ss")})", requireTimeStamp: false, fileName: $"RestAccessLog{DateTime.Now.ToString("yyyyMMdd")}.txt");
                }

                JsonSerializerSettings set = new JsonSerializerSettings
                {
                    TypeNameHandling = TypeNameHandling.Auto,
                    Formatting = Formatting.None
                };

                //if (url.Contains("3"))  //Similating Exception to Test Results
                //{
                //    throw new Exception("Test Exception");
                //}

                return JsonConvert.DeserializeObject<OutputObjectType>(returnedJsonString.RemoveLineEndings(), set);
            }
            catch (Exception ex)
            {
                //var sp = ServicePointManager.FindServicePoint(new Uri(url));
                //sp.ConnectionLeaseTimeout = 60 * 1000; // 1 Minute
                client.Dispose();
                client = new HttpClient();
                new Writer().log(ex);
                object obj = null;
                if (logExceptionMails) EmailLogger.SendMailException(ex, toAddr: System.Configuration.ConfigurationManager.AppSettings.Get("errorLogMail"));
                return (OutputObjectType)Convert.ChangeType(obj, typeof(OutputObjectType));
            }
        }

        public static OutputObjectType HttpJsonPost<InputObjectType,OutputObjectType>(string url, InputObjectType obj, bool logExceptionMails = false)
        {
            if (client == null)
            {
                initialize();
                var sp = ServicePointManager.FindServicePoint(new Uri(url));
                sp.ConnectionLeaseTimeout = 60 * 1000; // 1 Minute
            }

            string returnedJsonString = string.Empty;
            objHttpRequestMessage = new HttpRequestMessage();
            objHttpRequestMessage.Method = HttpMethod.Post;
            objHttpRequestMessage.Headers.Accept.Add(new System.Net.Http.Headers.MediaTypeWithQualityHeaderValue("application/json"));
            objHttpRequestMessage.Headers.Add("Authorization", ConfigurationManager.AppSettings.Get("AuthorizationHeader"));
            objHttpRequestMessage.RequestUri = new Uri(url);
            var stringContent = JsonConvert.SerializeObject(obj);
            objHttpRequestMessage.Content = new StringContent(stringContent,Encoding.UTF8,"application/json");
            wr.log($"*********Request to url starts at {url} ({DateTime.Now.ToString("yyyy MM dd HH:mm:ss")})",requireTimeStamp: false, fileName: $"RestAccessLog{DateTime.Now.ToString("yyyyMMdd")}.txt");
            wr.log(stringContent + Environment.NewLine, requireTimeStamp:false, fileName: $"RestAccessLog{DateTime.Now.ToString("yyyyMMdd")}.txt");
            HttpResponseMessage objHttpResponseMessage = null;
            try
            {
                Task<HttpResponseMessage> taskHttpResponsePost
                    = client.SendAsync(objHttpRequestMessage, HttpCompletionOption.ResponseContentRead);

                objHttpResponseMessage = taskHttpResponsePost.Result;
                returnedJsonString = objHttpResponseMessage.Content.ReadAsStringAsync().Result;
                jsonResult = returnedJsonString;

                JsonSerializerSettings set = new JsonSerializerSettings
                {
                    TypeNameHandling = TypeNameHandling.Auto,
                    Formatting = Formatting.None
                };

                //if (url.Contains("3"))  //Similating Exception to Test Results
                //{
                //    throw new Exception("Test Exception");
                //}
                wr.log(returnedJsonString, requireTimeStamp:false, fileName: $"RestAccessLog{DateTime.Now.ToString("yyyyMMdd")}.txt");
                wr.log($"*********Request to url ends at {url} ({DateTime.Now.ToString("yyyy MM dd HH:mm:ss")})", requireTimeStamp: false, fileName: $"RestAccessLog{DateTime.Now.ToString("yyyyMMdd")}.txt");
                return JsonConvert.DeserializeObject<OutputObjectType>(returnedJsonString.RemoveLineEndings(), set);
            }
            catch (Exception ex)
            {
                var sp = ServicePointManager.FindServicePoint(new Uri(url));
                sp.ConnectionLeaseTimeout = 60 * 1000; // 1 Minute
                client.Dispose();
                client = new HttpClient();
                wr.log(ex);
                object obj1 = null;
                if (logExceptionMails) EmailLogger.SendMailException(ex, toAddr: System.Configuration.ConfigurationManager.AppSettings.Get("errorLogMail"));
                return (OutputObjectType)Convert.ChangeType(obj1, typeof(OutputObjectType));
            }
        }
    }
}
