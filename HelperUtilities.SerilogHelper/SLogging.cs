using Serilog;
using System;
using System.IO;
namespace HelperUtilities.SerilogHelper
{
    public static class SLogging
    {
        public static ILogger GetSerilogTextWriter(string absoluteFolderPath = null)
        {
            string filePath;
            if (string.IsNullOrEmpty(absoluteFolderPath) == false)
            {
                filePath = Path.Combine(absoluteFolderPath, "log.txt");
            }
            else
            {
                filePath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "logs", "log.txt");
            }

            return new LoggerConfiguration().WriteTo.File(filePath, rollingInterval: RollingInterval.Day).CreateLogger();
        }
    }
}
