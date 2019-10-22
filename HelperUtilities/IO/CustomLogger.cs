using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HelperUtilities.IO
{
    public class CustomLogger
    {        
        static readonly object _syncObject = new object();       
        static string _baseDirectory;


        string _referenceId, _filePathForNormalLogs, _filePathForErrorLogs;
        StringBuilder _sbLog;

        public string ReferenceId
        {
            get
            {
                return _referenceId;
            }
        }

        public string FilePathForNormalLogs
        {
            get
            {
                return _filePathForNormalLogs;
            }
        }

        public string FilePathForErrorLogs
        {
            get
            {
                return _filePathForErrorLogs;
            }
        }

        public string GenerateNewReferenceId()
        {
            CommitLog();
            _referenceId = Guid.NewGuid().ToString();
            return _referenceId;
        }

        static CustomLogger()
        {
            _baseDirectory = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "Log");
            if (!Directory.Exists(_baseDirectory))
            {
                Directory.CreateDirectory(_baseDirectory);
            }
        }

        public CustomLogger(string referenceId = null)
        {
            _referenceId = string.IsNullOrWhiteSpace(referenceId) ? Guid.NewGuid().ToString() : referenceId;
            _sbLog = new StringBuilder();
            _filePathForNormalLogs = Path.Combine(_baseDirectory, DateTime.Now.ToString("yyyy-MM-dd") + "-logs.txt");
            _filePathForErrorLogs = Path.Combine(_baseDirectory, DateTime.Now.ToString("yyyy-MM-dd") + "-errors.txt");
        }

        public CustomLogger(string baseDirectoryWithAbsolutePath, string fileNameWithExtensionForLog, string FileNameWithExtensionForErrors, string referenceId = null)
        {
            if (fileNameWithExtensionForLog.Length > 0 && fileNameWithExtensionForLog.Contains('.'))
            {
                fileNameWithExtensionForLog = fileNameWithExtensionForLog.Trim();
            }

            if (FileNameWithExtensionForErrors.Length > 0 && FileNameWithExtensionForErrors.Contains('.'))
            {
                FileNameWithExtensionForErrors = FileNameWithExtensionForErrors.Trim();
            }

            baseDirectoryWithAbsolutePath = baseDirectoryWithAbsolutePath.Trim();
            try
            {
                if (!Directory.Exists(baseDirectoryWithAbsolutePath))
                {
                    Directory.CreateDirectory(baseDirectoryWithAbsolutePath);
                    _baseDirectory = baseDirectoryWithAbsolutePath;
                }
            }
            catch (Exception Ex)
            {
                throw new Exception("Unable to Create Directory with the following path - " + baseDirectoryWithAbsolutePath, Ex);
            }

            _referenceId = string.IsNullOrWhiteSpace(referenceId) ? Guid.NewGuid().ToString() : referenceId;
            _sbLog = new StringBuilder();
            _filePathForNormalLogs = Path.Combine(_baseDirectory, fileNameWithExtensionForLog);
            _filePathForErrorLogs = Path.Combine(_baseDirectory, FileNameWithExtensionForErrors);
        }

        public void AppendLog(string logText, object obj = null)
        {
            if (_sbLog.Length == 0)
            {
                _sbLog.AppendLine($"{_referenceId} - {DateTime.Now.ToString("dd/MM/yyyy HH:mm:ss.ffffff")}");
            }
            _sbLog.AppendLine($"\t{logText} ({DateTime.Now.ToString("dd/MM/yyyy HH:mm:ss.ffffff")})");
            if (obj != null)
            {
                if (obj.GetType().IsClass)
                {
                    _sbLog.AppendLine($"\t{JsonConvert.SerializeObject(obj)}");
                }
                else
                {
                    _sbLog.AppendLine($"\t{obj.ToString()}");
                }
            }
        }

        public void CommitLog(string lastMessageBeforeCommitIfAny=null)
        {
            if (!string.IsNullOrWhiteSpace(lastMessageBeforeCommitIfAny))
            {
                _sbLog.AppendLine(lastMessageBeforeCommitIfAny);
            }
            if (_sbLog.Length > 0)
            {
                _sbLog.AppendLine($"ReferenceId '{_referenceId}' commited (Manual)");
                Log(_sbLog.ToString(), _filePathForNormalLogs);
                _sbLog.Clear();
            }
        }

        public void WriteException(Exception exception)
        {
            StringBuilder sb = new StringBuilder(Environment.NewLine + GetDateTimeAndReference(_referenceId) + Environment.NewLine);
            do
            {
                sb.AppendLine("\t" + exception.Source + " - " + exception.Message);
                sb.AppendLine("\t" + exception.StackTrace);
                exception = exception.InnerException;
            } while (exception != null);

            Log(sb.ToString(), _filePathForErrorLogs);
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

        public static Dictionary<string, DateTime> ListallFiles(string absoluteFolderPath = null, DateTime? StartingDate = null, DateTime? EndDate = null)
        {
            Dictionary<string, DateTime> _dict = new Dictionary<string, DateTime>();
            if (string.IsNullOrEmpty(absoluteFolderPath)) absoluteFolderPath = _baseDirectory;
            if (Directory.Exists(absoluteFolderPath))
            {
                DirectoryInfo dirInfo = new DirectoryInfo(absoluteFolderPath);
                var listFiles = dirInfo.GetFiles().ToList();
                Trace.WriteLine(JsonConvert.SerializeObject(listFiles));
                DateTime minDate = StartingDate.HasValue ? Convert.ToDateTime(StartingDate) : listFiles.Min(x => x.LastWriteTime);
                DateTime maxDate = EndDate.HasValue ? Convert.ToDateTime(EndDate) : listFiles.Max(x => x.LastWriteTime);
                var finalList = listFiles.Where(x => x.LastWriteTime >= minDate && x.LastWriteTime <= maxDate && x.Exists == true).OrderByDescending( x => x.LastWriteTime).ToList();
                foreach (var file in finalList)
                {
                    _dict.Add(file.Name, file.LastWriteTime);
                }
            }
            return _dict;
        }

        private static void Log(string logText, string absoluteFilePathWithExt)
        {
            lock (_syncObject)
            {                
                try
                {
                    using (StreamWriter sw = new StreamWriter(absoluteFilePathWithExt, true))
                    {
                        sw.WriteLine(logText);
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
            return DateTime.Now.ToString("dd/MM/yyyy HH:mm:ss",CultureInfo.InvariantCulture)
                + (string.IsNullOrEmpty(referenceId) ? "" : $"({referenceId})");
        }

        ~CustomLogger()
        {
            if (_sbLog.Length > 0)
            {
                _sbLog.AppendLine($"ReferenceId '{_referenceId}' commited (Auto)");
                Log(_sbLog.ToString(), _filePathForNormalLogs);
            }
        }


    }
}
