using System;
using System.IO;

namespace HelperUtilties.IO
{
    public class Writer
    {
        string dirPathInitial; string filePath; string defaultFileName;
        public Writer()
        {
            dirPathInitial = Path.Combine(AppDomain.CurrentDomain.BaseDirectory,"logs");
            defaultFileName = "log.txt";
        }

        /// <summary>
        /// Provide Filename with Extension
        /// </summary>
        /// <param name="logText"></param>
        /// <param name="dirPath"></param>
        /// <param name="fileName"> Provide Filename with Extension e.g output.csv</param>
        /// <param name="appendFile"></param>
        public void log(string logText, string dirPath = null, string fileName = null, bool appendFile = true, bool requireTimeStamp = true)
        {
            if (string.IsNullOrEmpty(dirPath))
            {
                dirPath = dirPathInitial;
            }

            if (string.IsNullOrEmpty(fileName))
            {
                fileName = defaultFileName;
            }

            if (!Directory.Exists(dirPath))
            {
                Directory.CreateDirectory(dirPath);
            }

            filePath = Path.Combine(dirPath, fileName);
            using (
            StreamWriter sw = new StreamWriter(filePath, appendFile))
            {
                if (requireTimeStamp)
                {
                    sw.WriteLine(logText + $" ({DateTime.Now.ToString("yyyy MM dd HH:mm:ss")})");
                }
                else
                {
                    sw.WriteLine(logText);
                }
            }
        }

        public void log(Exception exception, string dirPath = null, string fileName = null)
        {
            if (string.IsNullOrEmpty(dirPath))
            {
                dirPath = dirPathInitial;
            }

            if (string.IsNullOrEmpty(fileName))
            {
                fileName = "Error-Log" + DateTime.Now.ToString("-yyyy-MM-dd") + ".txt";
            }

            if (!Directory.Exists(dirPath))
            {
                Directory.CreateDirectory(dirPath);
            }

            do
            {
                log(exception.Source + " - " + exception.Message, dirPath, fileName);
                log(exception.StackTrace, dirPath, fileName);
                exception = exception.InnerException;
            } while (exception != null);

            //if (Directory.Exists(dirPath))
            //{
            //    ProcessStartInfo startInfo = new ProcessStartInfo
            //    {
            //        Arguments = dirPath,
            //        FileName = "explorer.exe"
            //    };
            //    Process.Start(startInfo);
            //}
        }
    }
}
