using System;
using System.IO;
using Newtonsoft;
using Newtonsoft.Json;
using System.Diagnostics;
using HelperUtilities.Text;

namespace HelperUtilities.IO
{
    [Obsolete("This class is now being depreciated. Please use TextFileLogger for future purpose as its thread safe which is a must required in logging environment")]
    public class Writer_old
    {
        string _dirPathInitial; string filePath; string _defaultFileName;
        public string logDirectoryCompletePath { get; set; }

        /// <summary>
        /// Initial foldername is defaulted to 'logs' folder in app root location
        /// </summary>
        /// <param name="baseAbsolulateDirectoryPath">Should be a complete (absolute) file path e.g like 'c:\\yourpath' etc</param>
        /// <param name="defaultFileNameWithExtension">Should be a file name with extension. If no file extension e.g 'yourfile.txt' is provided then .txt is appended in filename</param>
        public Writer_old(string baseAbsolulateDirectoryPath = null, string defaultFileNameWithExtension = null)
        {
            if (string.IsNullOrEmpty(baseAbsolulateDirectoryPath))
            {
                _dirPathInitial = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "logs");
            }
            else
            {
                _dirPathInitial = baseAbsolulateDirectoryPath;
            }

            Directory.CreateDirectory(_dirPathInitial);

            if (string.IsNullOrEmpty(defaultFileNameWithExtension))
            {
                _defaultFileName = GetTodayLogFileName;
            }
            else
            {
                _defaultFileName = defaultFileNameWithExtension;
            }

            if (!_defaultFileName.Contains("."))
            {
                _defaultFileName = _defaultFileName + ".txt";
            }
        }

        public string GetTodayLogFileName
        {
            get
            {
                return $"Logs-{DateTime.Now.ToString("yyyy-MM-dd")}.txt";
            }
        }

        public void log(string logText, object objectData, string dirPath = null, string fileName = null, bool appendFile = true, bool requireTimeStamp = true, Formatting formatting = Formatting.None)
        {
            log(logText + ": " + Environment.NewLine + JsonConvert.SerializeObject(objectData, formatting).Replace(@"\", " "),
                dirPath,
                fileName, appendFile, requireTimeStamp);
        }

        public string InitialAbsoluteDirectoryPath
        {
            get
            {
                return _dirPathInitial;
            }
            set
            {
                _dirPathInitial = value;
            }
        }

        public string DefaultFileNameWithExtension
        {
            get
            {
                return _defaultFileName;
            }
            set
            {
                _defaultFileName = value;
            }
        }


        /// <summary>
        /// Provide Filename with Extension
        /// </summary>
        /// <param name="logText"></param>
        /// <param name="folderName">Should be a RELATIVE directory path e.g provide only directory name without any slash etc</param>
        /// <param name="filenameWithExtension">Provide Filename WITH Extension e.g output.csv</param>
        /// <param name="appendFile"></param>
        public string log(string logText, string folderName = null, string filenameWithExtension = null, bool appendFile = true, bool requireTimeStamp = true)
        {
            if (string.IsNullOrEmpty(folderName))
            {
                folderName = _dirPathInitial;
            }
            else
            {
                folderName = Path.Combine(_dirPathInitial, folderName.RemoveSpecialCharactersFromString());
            }

            if (string.IsNullOrEmpty(filenameWithExtension))
            {                
                filenameWithExtension = _defaultFileName;
            }

            if (!filenameWithExtension.Contains("."))
            {
                filenameWithExtension = filenameWithExtension + ".txt";
            }

            if (!Directory.Exists(folderName))
            {
                Directory.CreateDirectory(folderName);
            }

            filePath = Path.Combine(folderName, filenameWithExtension);
            using (StreamWriter sw = new StreamWriter(filePath, appendFile))
            {
                if (requireTimeStamp)
                {
                    sw.WriteLine(logText + $" ({DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss")})");
                }
                else
                {
                    sw.WriteLine(logText);
                }
            }
            return filePath;
        }

        /// <summary>
        /// Logs Exceptions
        /// </summary>
        /// <param name="exception"></param>
        /// <param name="folderName">Should be only Relative directory path e.g 'logs' etc</param>
        /// <param name="fileNameWithExtension">Provide file name WITH extension e.g error.txt. If Don't provide this parameter. By default it'll take Error-log-yyyy-mm-dd.txt as errorlog file name</param>
        public void log(Exception exception, string folderName = null, string fileNameWithExtension = null)
        {

            if (string.IsNullOrEmpty(fileNameWithExtension))
            {
                fileNameWithExtension = "Error-Log" + DateTime.Now.ToString("-yyyy-MM-dd") + ".txt";
            }

            if (string.IsNullOrEmpty(folderName))
            {
                folderName = "Errors";
            }

            do
            {
                log(exception.Source + " - " + exception.Message, folderName, fileNameWithExtension);
                log(exception.StackTrace, folderName, fileNameWithExtension);
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

        public bool DeleteAllRootFilesAndFolders(string absoluteFolderPath = null)
        {
            if (string.IsNullOrEmpty(absoluteFolderPath)) absoluteFolderPath = _dirPathInitial;

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

        public bool DeleteFolder(string folderName, bool isAbsolutePath = true)
        {
            string absoluteFolderPath;
            if (isAbsolutePath)
            {
                absoluteFolderPath = folderName;
            }
            else
            {
                absoluteFolderPath = Path.Combine(_dirPathInitial, folderName);
            }

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

        public bool DeleteFileWithAbsolutePath(string absoluteFileWithFullPath)
        {
            if (File.Exists(absoluteFileWithFullPath))
            {
                File.Delete(absoluteFileWithFullPath);
            }
            return true;
        }

        public void openExplorerDirectory(string dirAbsolutePath = null)
        {
            if (string.IsNullOrEmpty(dirAbsolutePath))
            {
                dirAbsolutePath = _dirPathInitial;
            }

            if (Directory.Exists(dirAbsolutePath))
            {
                ProcessStartInfo startInfo = new ProcessStartInfo
                {
                    Arguments = dirAbsolutePath,
                    FileName = "explorer.exe"
                };
                Process.Start(startInfo);
            }
        }
    }
}
