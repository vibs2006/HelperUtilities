using System;
using System.Collections.Generic;
using System.Linq;
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
