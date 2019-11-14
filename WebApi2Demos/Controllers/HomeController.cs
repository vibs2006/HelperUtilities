using HelperUtilities.IO;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;
using WebApi2Demos;

namespace WebApi2Demos.Controllers
{
    public class HomeController : ApiController
    {
        public IHttpActionResult Get()
        {
            CustomLogger _logger = CustomLogger.GetLoggerInstance(Request);
            _logger.AppendLog("Writting something in middle or its related sub classes");
            return Ok("Welcome");
        }
    }
}
