using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;

namespace HelperUtilities.Net
{
    public static class NetMethods
    {
        public static string GetIpAddress()
        {
            return System.Net.Dns.GetHostEntry(System.Net.Dns.GetHostName()).AddressList.GetValue(1).ToString();
        }

        public static string GetClientIpAddress(this HttpRequestMessage request)
        {
            // Web-hosting. Needs reference to System.Web.dll

            //if (request.Properties.ContainsKey(HttpContext))
            //{
            //    dynamic ctx = request.Properties[HttpContext];
            //    if (ctx != null)
            //    {
            //        return ctx.Request.UserHostAddress;
            //    }
            //}

            // Self-hosting. Needs reference to System.ServiceModel.dll. 
            //if (request.Properties.ContainsKey(RemoteEndpointMessage))
            //{
            //    dynamic remoteEndpoint = request.Properties[RemoteEndpointMessage];
            //    if (remoteEndpoint != null)
            //    {
            //        return remoteEndpoint.Address;
            //    }
            //}

            // Self-hosting using Owin. Needs reference to Microsoft.Owin.dll. 
            //if (request.Properties.ContainsKey(OwinContext))
            //{
            //    dynamic owinContext = request.Properties[OwinContext];
            //    if (owinContext != null)
            //    {
            //        return owinContext.Request.RemoteIpAddress;
            //    }
            //}

            return string.Empty;
        }
        //This Method Requires System.Web
        //public string GetIPAddress2()
        //{
        //    var context = HttpContext.Current;

        //    //HttpContext context = System.Web.HttpContext.Current;
        //    string ipAddress = context.Request.ServerVariables["HTTP_X_FORWARDED_FOR"];

        //    if (!string.IsNullOrEmpty(ipAddress))
        //    {
        //        string[] addresses = ipAddress.Split(',');
        //        if (addresses.Length != 0)
        //        {
        //            return addresses[0];
        //        }
        //    }
        //    return context.Request.ServerVariables["REMOTE_ADDR"];
        //}
    }
}
