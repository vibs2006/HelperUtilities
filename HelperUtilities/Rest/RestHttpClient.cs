using Newtonsoft.Json;
using System;
using System.Net;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using HelperUtilities.Text;

namespace HelperUtilities.Rest
{
    public enum MediaType { texthtml, json, csv }

    public static class RestHttpClient
    {
        static HttpClient client;

        public static void initialize(string url = null)
        {
            ServicePointManager.SecurityProtocol = SecurityProtocolType.Tls12;
            if (client == null)
            {
                client = new HttpClient();
            }

            if (!string.IsNullOrWhiteSpace(url))
            {
                System.Threading.Thread.Sleep(1000);
                ServicePointManager.FindServicePoint(new Uri(url)).ConnectionLeaseTimeout = (int)TimeSpan.FromMinutes(1).TotalMilliseconds;
            }
        }

        public static void HandleException(Exception ex, StringBuilder sb, string url)
        {
            try
            {
                sb = sb ?? new StringBuilder();
                initialize(url);
                sb.AppendLine(StaticTextUtils.ConvertExceptionToString(ex));
            }
            finally
            {
                System.Threading.Thread.Sleep(10000);
            }
        }

        private static string GetMediaType(MediaType mediaType)
        {
            string mediaQualityHeadervalue = string.Empty;
            switch (mediaType)
            {
                case MediaType.texthtml:
                    mediaQualityHeadervalue = "text/html";
                    break;
                case MediaType.json:
                    mediaQualityHeadervalue = "application/json";
                    break;

                case MediaType.csv:
                    mediaQualityHeadervalue = "text/csv";
                    break;

                default:
                    break;
            }
            return mediaQualityHeadervalue;
        }


        public static string HttpGetRequest<OutputObjectType>(string url, out StringBuilder sb, out bool isHttpRequestSuccessful,
            MediaType AcceptMediaType = MediaType.texthtml, string AuthorizationHeaderValue = null)
        {
            sb = new StringBuilder();
            isHttpRequestSuccessful = false;
            string mediaQualityHeadervalue = GetMediaType(AcceptMediaType);
            HttpRequestMessage objHttpRequestMessage = null;

            if (client == null)
            {
                initialize(url);
            }

            objHttpRequestMessage = new HttpRequestMessage
            {
                Method = HttpMethod.Get
            };

            objHttpRequestMessage.Headers.Accept.Add(new System.Net.Http.Headers.MediaTypeWithQualityHeaderValue(mediaQualityHeadervalue));
            if (!string.IsNullOrWhiteSpace(AuthorizationHeaderValue))
            {
                objHttpRequestMessage.Headers.Add("Authorization", AuthorizationHeaderValue);
                sb.AppendLine("Authorization Header Found with value " + AuthorizationHeaderValue);
            }
            objHttpRequestMessage.RequestUri = new Uri(url.Trim());
            sb.AppendLine($"*********Request to url starts at {url.Trim()} ({DateTime.Now.ToString("yyyy MM dd HH:mm:ss")})");
            HttpResponseMessage objHttpResponseMessage = null;
            try
            {
                Task<HttpResponseMessage> taskHttpResponsePost
                    = client.SendAsync(objHttpRequestMessage, HttpCompletionOption.ResponseContentRead);

                objHttpResponseMessage = taskHttpResponsePost.Result;
                string returnedJsonString = objHttpResponseMessage.Content.ReadAsStringAsync().Result;
                isHttpRequestSuccessful = objHttpResponseMessage.IsSuccessStatusCode;
                returnedJsonString = string.IsNullOrWhiteSpace(returnedJsonString) ? "" : returnedJsonString.RemoveLineEndings();
                sb.AppendLine($"Returned data is: \'{ returnedJsonString}\'");
                sb.AppendLine($"*********Request to url ends at {url.Trim()} at ({DateTime.Now.ToString("yyyy MM dd HH:mm:ss")})");

                JsonSerializerSettings set = new JsonSerializerSettings
                {
                    TypeNameHandling = TypeNameHandling.Auto,
                    Formatting = Formatting.None
                };

                if (string.IsNullOrWhiteSpace(returnedJsonString))
                {
                    return string.Empty;
                }

                return returnedJsonString;
            }
            catch (Exception ex)
            {
                HandleException(ex, sb, url);
                return string.Empty;
            }
        }

        public static OutputObjectType HttpGetRequestWithJsonReturnData<OutputObjectType>(string url, out StringBuilder sb, out bool isHttpRequestSuccessful,
            MediaType AcceptMediaType = MediaType.json, string AuthorizationHeaderValue = null)
        {
            sb = new StringBuilder();
            isHttpRequestSuccessful = false;
            string mediaQualityHeadervalue = GetMediaType(AcceptMediaType);
            HttpRequestMessage objHttpRequestMessage = null;
            if (client == null)
            {
                initialize(url);
            }

            if (AcceptMediaType != MediaType.json)
            {
                return default;
            }

            objHttpRequestMessage = new HttpRequestMessage
            {
                Method = HttpMethod.Get
            };
            objHttpRequestMessage.Headers.Accept.Add(new System.Net.Http.Headers.MediaTypeWithQualityHeaderValue(mediaQualityHeadervalue));
            if (!string.IsNullOrWhiteSpace(AuthorizationHeaderValue))
            {
                objHttpRequestMessage.Headers.Add("Authorization", AuthorizationHeaderValue);
                sb.AppendLine("Authorization Header Found with value " + AuthorizationHeaderValue);
            }
            objHttpRequestMessage.RequestUri = new Uri(url.Trim());
            sb.AppendLine($"*********Request to url starts at {url.Trim()} ({DateTime.Now.ToString("yyyy MM dd HH:mm:ss")})");
            HttpResponseMessage objHttpResponseMessage = null;
            try
            {
                Task<HttpResponseMessage> taskHttpResponsePost
                    = client.SendAsync(objHttpRequestMessage, HttpCompletionOption.ResponseContentRead);

                objHttpResponseMessage = taskHttpResponsePost.Result;
                string returnedJsonString = objHttpResponseMessage.Content.ReadAsStringAsync().Result;
                isHttpRequestSuccessful = objHttpResponseMessage.IsSuccessStatusCode;
                returnedJsonString = string.IsNullOrWhiteSpace(returnedJsonString) ? "" : returnedJsonString.RemoveLineEndings();
                sb.AppendLine($"Returned JSON string is: \'{ returnedJsonString}\'");
                sb.AppendLine($"*********Request to url ends at {url.Trim()} at ({DateTime.Now.ToString("yyyy MM dd HH:mm:ss")})");

                JsonSerializerSettings set = new JsonSerializerSettings
                {
                    TypeNameHandling = TypeNameHandling.Auto,
                    Formatting = Formatting.None
                };

                if (string.IsNullOrWhiteSpace(returnedJsonString))
                {
                    return default;
                }
                return JsonConvert.DeserializeObject<OutputObjectType>(returnedJsonString, set);
            }
            catch (Exception ex)
            {
                HandleException(ex, sb, url);
                return default;
            }
        }

        public static OutputObjectType HttpJsonPost<InputObjectType, OutputObjectType>(string url, InputObjectType inputJsonObject, out StringBuilder sb, out bool IsHttpRequestSuccessful,
            MediaType mediaTypeEnum = MediaType.json, string AuthorizationHeaderValue = null, Encoding encoding = null)
        {
            sb = new StringBuilder();
            IsHttpRequestSuccessful = false;
            HttpRequestMessage objHttpRequestMessage = null;
            encoding = encoding ?? Encoding.UTF8;
            string mediaQualityHeadervalue = GetMediaType(mediaTypeEnum);

            if (client == null)
            {
                initialize(url);             
            }

            if (mediaTypeEnum != MediaType.json)
            {
                return default;
            }

            string returnedJsonString = string.Empty;
            objHttpRequestMessage = new HttpRequestMessage();
            objHttpRequestMessage.Method = HttpMethod.Post;
            objHttpRequestMessage.Headers.Accept.Add(new System.Net.Http.Headers.MediaTypeWithQualityHeaderValue(mediaQualityHeadervalue));
            if (!string.IsNullOrWhiteSpace(AuthorizationHeaderValue))
            {
                objHttpRequestMessage.Headers.Add("Authorization", AuthorizationHeaderValue);
            }
            objHttpRequestMessage.RequestUri = new Uri(url);
            string stringContent = string.Empty;

            if (inputJsonObject != null && inputJsonObject.GetType().IsClass)
            {
                stringContent = JsonConvert.SerializeObject(inputJsonObject);
            }
            objHttpRequestMessage.Content = new StringContent(stringContent, encoding, mediaQualityHeadervalue);
            sb.AppendLine($"*********Request to url starts at {url} ({DateTime.Now.ToString("yyyy MM dd HH:mm:ss")})");
            sb.AppendLine("Content Generated for HTTP Post Request is given below:");
            sb.AppendLine(stringContent);
            sb.AppendLine("Content For HTTP Post Ends now");

            HttpResponseMessage objHttpResponseMessage = null;
            try
            {
                sb.AppendLine("Request Initiates");
                Task<HttpResponseMessage> taskHttpResponsePost
                    = client.SendAsync(objHttpRequestMessage, HttpCompletionOption.ResponseContentRead);

                objHttpResponseMessage = taskHttpResponsePost.Result;
                returnedJsonString = objHttpResponseMessage.Content.ReadAsStringAsync().Result;

                JsonSerializerSettings set = new JsonSerializerSettings
                {
                    TypeNameHandling = TypeNameHandling.Auto,
                    Formatting = Formatting.None
                };

                returnedJsonString = returnedJsonString != null ? returnedJsonString.RemoveLineEndings() : string.Empty;

                sb.AppendLine("Returned JSON String is given below:");
                sb.AppendLine(returnedJsonString);
                sb.AppendLine($"*********Request to url ends at {url} ({DateTime.Now.ToString("yyyy MM dd HH:mm:ss")})");
                if (string.IsNullOrWhiteSpace(returnedJsonString)) return default;
                return JsonConvert.DeserializeObject<OutputObjectType>(returnedJsonString, set);
            }
            catch (Exception ex)
            {
                HandleException(ex, sb, url);
                return default;
            }
        }
    }
}
