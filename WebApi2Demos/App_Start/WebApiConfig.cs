using System;
using System.Collections.Generic;
using System.Linq;
using System.Web.Http;
using WebApi2Demos.Common;

namespace WebApi2Demos
{
    public static class WebApiConfig
    {
        public static void Register(HttpConfiguration config)
        {
            // Web API configuration and services

            // Web API routes
            config.MapHttpAttributeRoutes();

            config.MessageHandlers.Add(new CustomRequestResponseHandler());

            config.Routes.MapHttpRoute(
                name: "DefaultApi",
                routeTemplate: "api/{controller}/{id}",
                defaults: new { id = RouteParameter.Optional }
            );

            config.Routes.MapHttpRoute(
                name: "DefaultApiFallBack",
                routeTemplate: "{controller}/{action}/{id}",
                defaults: new { id = RouteParameter.Optional, controller= "Home", action = "Get" }
            );
        }
    }
}
