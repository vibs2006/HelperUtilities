using HelperUtilities.IO;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Web;

namespace WebApi2Demos.Common
{
    public class CustomRequestResponseHandler : DelegatingHandler
    {
        protected override async Task<HttpResponseMessage> SendAsync(
            HttpRequestMessage request,
            CancellationToken cancellationToken)
        {            
            var requestedMethod = request.Method;
            var userHostAddress = HttpContext.Current != null ?
                HttpContext.Current.Request.UserHostAddress : "0:0:0:0";

            var userAgent = request.Headers.UserAgent.ToString();
            var requestMessage = await request.Content.ReadAsByteArrayAsync();
            var urlAccessed = request.RequestUri.AbsoluteUri;

            var requestHeadersString = new StringBuilder();
            foreach (var header in request.Headers)
            {
                requestHeadersString.Append($"({header.Key}:");
                foreach (var items in header.Value)
                {
                    requestHeadersString.Append($"[{items}]");
                }
                requestHeadersString.Append(") ");
            }
            var logger = new CustomLogger(request);

            var requestLog = new ApiLog
            {
                Headers = requestHeadersString.ToString(),
                AbsoluteUri = urlAccessed,
                Host = userHostAddress,
                RequestBody = Encoding.UTF8.GetString(requestMessage),
                UserHostAddress = userHostAddress,
                Useragent = userAgent,
                RequestedMethod = requestedMethod.ToString(),
                StatusCode = string.Empty,
                Guid = logger.ReferenceId,
                RequestType = "Request"
            };

            logger.AppendLog("Now Logging Request Object", requestLog);
            logger.AppendLog(Environment.NewLine);

            var response = await base.SendAsync(request, cancellationToken);
            response.Headers.Add("Access-Control-Allow-Origin", "*");

            byte[] responseMessage;

            if (response.IsSuccessStatusCode)
            {
                responseMessage = await response.Content.ReadAsByteArrayAsync();
            }
            else
            {
                responseMessage = Encoding.UTF8.GetBytes(response.ReasonPhrase);
            }

            var responseLog = new ApiLog
            {
                RequestType = "Response",
                AbsoluteUri = urlAccessed,
                Host = userHostAddress,
                RequestBody = Encoding.UTF8.GetString(responseMessage),
                UserHostAddress = userHostAddress,
                Useragent = userAgent,
                RequestedMethod = requestedMethod.ToString(),
                StatusCode = response.StatusCode.ToString(),
                Guid = logger.ReferenceId
            };

            logger.AppendLog("\nLogging Response Object", responseLog);
            logger.CommitLog();
            return response;
        }

    }
}