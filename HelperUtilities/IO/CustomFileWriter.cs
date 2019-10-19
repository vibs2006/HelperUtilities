using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using Newtonsoft.Json;
using System.Diagnostics;

namespace HelperUtilities.IO
{
    public static class CustomFileWriter
    {
        private static string _finalFilePathForLogMessages;
        private static string _finalFilePathForErrorMessages;
        private static string _baseDirectory;
        

        private static readonly object _syncObject = new object();
        private static readonly object _syncObjectError = new object();

        public static bool AllowChangingDirectoryPath { get; set; }
       
        static CustomFileWriter()
        {
            _baseDirectory = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "logs");
            if (!Directory.Exists(_baseDirectory))
            {
                try
                {
                    Directory.CreateDirectory(_baseDirectory);
                }
                catch (Exception Ex)
                {
                    throw new Exception($"Unable to create directory", Ex);
                }
            }

            string errorDirectory = Path.Combine(_baseDirectory, "Errors");

            if (!Directory.Exists(errorDirectory))
            {
                try
                {
                    Directory.CreateDirectory(errorDirectory);
                }
                catch (Exception Ex)
                {
                    throw new Exception($"Unable to create directory", Ex);
                }
            }
            _finalFilePathForErrorMessages = Path.Combine(errorDirectory, "Errors-" + DateTime.Now.ToString("yyyy-MM-dd") + ".txt");
            _finalFilePathForLogMessages = Path.Combine(_baseDirectory, DateTime.Now.ToString("yyyy-MM-dd") + ".txt");
        }

        public static void Log(string logText, string referenceId = null,object obj = null)
        {
            lock (_syncObject)
            {
                try
                {
                    using (StreamWriter sw = new StreamWriter(_finalFilePathForLogMessages, true))
                    {
                        sw.WriteLine(logText);
                        if (obj != null) sw.WriteLine(JsonConvert.SerializeObject(obj));
                    }
                }
                catch (Exception Ex)
                {
                    Trace.TraceError(Ex.Message + Environment.NewLine + Ex.StackTrace);
                    
                }
            }
        }

        private static string GetDateTimeAndReference(string referenceId)
        {
            return  DateTime.Now.ToString("dd/MM/yyyy HH:mm:ss")
                + (string.IsNullOrEmpty(referenceId) ? "" : $"({referenceId})");
        }

        public static void Log(Exception exception, string referenceId = null)
        {
            StringBuilder sb = new StringBuilder();
            do
            {
                sb.AppendLine(exception.Source + " - " + exception.Message);
                sb.AppendLine(exception.StackTrace);
                exception = exception.InnerException;
            } while (exception != null);

            lock (_syncObjectError)
            {
                using (StreamWriter sw = new StreamWriter(_finalFilePathForErrorMessages, true))
                {
                    sw.WriteLine(GetDateTimeAndReference(referenceId) + Environment.NewLine + sb.ToString());
                }
            }            
        }

        
        public static bool DeleteAllRootFilesAndFolders(string absoluteFolderPath = null)
        {
            if (string.IsNullOrEmpty(absoluteFolderPath)) absoluteFolderPath = _baseDirectory;

            if (Directory.Exists(absoluteFolderPath))
            {
                DirectoryInfo di = new DirectoryInfo(absoluteFolderPath);

                foreach (FileInfo file in di.GetFiles())
                {
                    file.Delete();
                }
                foreach (DirectoryInfo dir in di.GetDirectories())
                {
                    dir.Delete(true);
                }
            }
            return true;
        }
    }
}