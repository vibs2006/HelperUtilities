namespace WebApi2Demos.Common
{
    internal class ApiLog
    {
        public string Headers { get; set; }
        public string AbsoluteUri { get; set; }
        public string Host { get; set; }
        public string RequestBody { get; set; }
        public string UserHostAddress { get; set; }
        public string Useragent { get; set; }
        public string RequestedMethod { get; set; }
        public string StatusCode { get; set; }
        public object Guid { get; set; }
        public string RequestType { get; set; }
    }
}