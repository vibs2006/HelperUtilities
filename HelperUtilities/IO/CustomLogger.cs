using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;

namespace HelperUtilities.IO
{
    public enum FileExtension
    {
        txt, log, csv, json, none
    }

    public class CustomLogger
    {
        private static readonly object _syncObject = new object();
        private static readonly object _syncRandomFileWriteObject = new object();
        private static readonly object _syncListCustomLoggerClass = new object();
        private static string _baseDirectory;
        private static string _defaultFolderName = "log";
        private static IDictionary<string, CustomLogger> _listInstances;

        private readonly string _referenceId;
        private static readonly string _guidKeyName = "GuidValue";
        private string _filePathForNormalLogs, _filePathForErrorLogs;
        StringBuilder _sbLog = new StringBuilder("\n*****************************************************************************" + Environment.NewLine);


        #region Properties (Setters / Getters)
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

        public string GuidKeyName
        {
            get
            {
                return _guidKeyName;
            }
        }
        #endregion

        static CustomLogger()
        {
            _baseDirectory = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, _defaultFolderName);
            if (!Directory.Exists(_baseDirectory))
            {
                Directory.CreateDirectory(_baseDirectory);
            }
            _listInstances = new Dictionary<string, CustomLogger>();

            //Following method will delete any file older than 30 days (of .txt type)
            DeleteFilesOlderMoreThanNdays(60);
        }

        public static string GetDefaultFileExtension(string fileName, FileExtension extension = FileExtension.txt)
        {
            if (string.IsNullOrWhiteSpace(fileName)) return string.Empty;

            fileName = fileName.Trim().Trim('.');

            if (extension == FileExtension.none)
            {
                if (fileName.Contains('.'))
                {
                    return fileName;
                }
                else
                {
                    return string.Concat(fileName, "." + extension.ToString());
                }
            }
            else
            {                
                return string.Concat(fileName.Replace(".",""), ".", extension.ToString());
            }
        }

        public CustomLogger(string referenceId = null, FileExtension extension = FileExtension.txt)
        {
            _referenceId = string.IsNullOrWhiteSpace(referenceId) ? Guid.NewGuid().ToString() : referenceId;            
            _filePathForNormalLogs = Path.Combine(_baseDirectory, GetDefaultFileExtension(DateTime.Now.ToString("yyyy-MM-dd") + "-logs", extension));
            _filePathForErrorLogs = Path.Combine(_baseDirectory, GetDefaultFileExtension(DateTime.Now.ToString("yyyy-MM-dd") + "-errors" , extension));
            AddReferenceToStaticLists();
        }

        public CustomLogger(HttpRequestMessage request, string referenceId = null, FileExtension extension = FileExtension.txt)
        {
            _referenceId = string.IsNullOrWhiteSpace(referenceId) ? Guid.NewGuid().ToString() : referenceId;
            if (request.Headers.Contains(_guidKeyName))
            {
                request.Headers.Remove(_guidKeyName);
            }
            request.Headers.Add(_guidKeyName, _referenceId);            
            _filePathForNormalLogs = Path.Combine(_baseDirectory, GetDefaultFileExtension(DateTime.Now.ToString("yyyy-MM-dd") + "-logs", extension));
            _filePathForErrorLogs = Path.Combine(_baseDirectory, GetDefaultFileExtension(DateTime.Now.ToString("yyyy-MM-dd") + "-errors", extension));
            AddReferenceToStaticLists();
        }

        /// <summary>
        ///  Alternative Class Constructor
        /// </summary>
        /// <param name="folderName">Only Folder name without any path</param>
        /// <param name="fileNameForLogs">Logs - Only File Name without any path. If you provide extension e.g filename.txt then default file extension is ignored else default file extension is added</param>
        /// <param name="fileNameForErrors">Errors - Only File Name without any path. If you provide extension e.g filename.txt then default file extension is ignored else default file extension is added</param>
        /// <param name="baseDirectoryWithAbsolutePath">This is optional if you want to provide abosolute directory path e.g something like c:\\yourfolder\\ etc. Leave this blank if not sure</param>
        /// <param name="referenceId">Its is supposed to be a unique value. Leave it blank if not sure. Windows will generate a unique Guid value automatically</param>   
        ///<param name="fileExtension">If No extension is specified in fileNameForLogs or fileNameForErrors then this default file extension is added in the end.</param>
        public CustomLogger(string folderName, string fileNameForLogs = null, string fileNameForErrors = null, 
            string baseDirectoryWithAbsolutePath = null,string referenceId = null, FileExtension fileExtension = FileExtension.txt)
        {
            if (string.IsNullOrWhiteSpace(folderName))
            {
                folderName = _defaultFolderName;
            }           

            if (string.IsNullOrWhiteSpace(baseDirectoryWithAbsolutePath))
            {
                _baseDirectory = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, folderName.Trim());
            }
            else
            {
                _baseDirectory = Path.Combine(baseDirectoryWithAbsolutePath.Trim(),folderName.Trim());
            }

            if (string.IsNullOrWhiteSpace(fileNameForLogs))
            {
                fileNameForLogs = Path.Combine(_baseDirectory, GetDefaultFileExtension(DateTime.Now.ToString("yyyy-MM-dd") + "-logs", fileExtension));
            }
            else 
            {
                fileNameForLogs = Path.Combine(_baseDirectory, GetDefaultFileExtension(fileNameForLogs.Trim(), fileExtension));
            }

            if (string.IsNullOrWhiteSpace(fileNameForErrors))
            {
                fileNameForErrors = Path.Combine(_baseDirectory, GetDefaultFileExtension(DateTime.Now.ToString("yyyy-MM-dd") + "-errors", fileExtension));
            } 
            else
            {
                fileNameForErrors = Path.Combine(_baseDirectory,GetDefaultFileExtension( fileNameForErrors.Trim(), fileExtension));
            }
            
            try
            {
                if (!Directory.Exists(_baseDirectory))
                {
                    Directory.CreateDirectory(_baseDirectory);                    
                }
            }
            catch (Exception Ex)
            {               
               throw new Exception("Unable to Create Directory with the following path - " + _baseDirectory, Ex);               
            }
            _referenceId = string.IsNullOrWhiteSpace(referenceId) ? Guid.NewGuid().ToString() : referenceId;            
            _filePathForNormalLogs = Path.Combine(_baseDirectory, fileNameForLogs);
            _filePathForErrorLogs = Path.Combine(_baseDirectory, fileNameForErrors);
        }

        private void AddReferenceToStaticLists()
        {
            if (_listInstances.ContainsKey(_referenceId) == false)
            {
                lock (_syncListCustomLoggerClass)
                {
                    _listInstances.Add(_referenceId, this);
                }
            }
        }

        private void DeleteReferenceToStaticLists()
        {
            if (_listInstances.ContainsKey(_referenceId) == true)
            {
                lock (_syncListCustomLoggerClass)
                {
                    _listInstances.Remove(_referenceId);
                }
            }
        }

        public void AppendLog(string logText, object obj = null)
        {
            if (_sbLog.Length == 0)
            {
                AddReferenceToStaticLists();
                _sbLog.AppendLine($"{_referenceId} - {DateTime.Now.ToString("dd/MM/yyyy HH:mm:ss.ffffff", CultureInfo.InvariantCulture)}");
            }
            _sbLog.AppendLine($"\t{logText} ({DateTime.Now.ToString("dd/MM/yyyy HH:mm:ss.ffffff", CultureInfo.InvariantCulture)})");
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

        public void CommitLog(string lastMessageBeforeCommitIfAny = null)
        {
            if (!string.IsNullOrWhiteSpace(lastMessageBeforeCommitIfAny))
            {
                _sbLog.AppendLine("\t" + lastMessageBeforeCommitIfAny);
            }
            if (_sbLog.Length > 0)
            {
                _sbLog.AppendLine($"ReferenceId '{_referenceId}' commited (Manual)");
                _sbLog.AppendLine("*****************************************************************************");
                Log(_sbLog.ToString(), _filePathForNormalLogs);
                _sbLog.Clear();
                DeleteReferenceToStaticLists();
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

        #region Static Methods

        public static CustomLogger GetLoggerInstance(HttpRequestMessage request)
        {
            string referenceId = string.Empty;
            request.Headers.TryGetValues(_guidKeyName, out IEnumerable<string> str);
            referenceId = str.FirstOrDefault();
            return GetLoggerInstance(referenceId);
        }

        public static CustomLogger GetLoggerInstance(string referenceId)
        {
            CustomLogger customLogger2;
            lock (_syncListCustomLoggerClass)
            {
                _listInstances.TryGetValue(referenceId, out customLogger2);
            }
            return customLogger2;
        }

        public static void DeleteFilesOlderMoreThanNdays(int noOfDays, string absoluteFolderPath = null)
        {
            var endDate = DateTime.Now.AddDays(0 - Math.Abs(noOfDays));
            var startDate = DateTime.Now.AddDays(-365); //1 year
            if (string.IsNullOrWhiteSpace(absoluteFolderPath)) absoluteFolderPath = _baseDirectory;
            var files = ListallFiles(absoluteFolderPath, startDate, endDate);
            foreach (var file in files)
            {
                File.Delete(Path.Combine(absoluteFolderPath, file.Key));
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

        public static Dictionary<string, DateTime> ListallFiles(string absoluteFolderPath = null, DateTime? StartingDate = null, DateTime? EndDate = null)
        {
            Dictionary<string, DateTime> _dict = new Dictionary<string, DateTime>();
            if (string.IsNullOrEmpty(absoluteFolderPath)) absoluteFolderPath = _baseDirectory;
            if (Directory.Exists(absoluteFolderPath))
            {
                DirectoryInfo dirInfo = new DirectoryInfo(absoluteFolderPath);
                var listFiles = dirInfo.GetFiles().ToList();
                if (listFiles != null && listFiles.Count > 0)
                {
                    if (listFiles.GetType().IsClass)
                    {
                        Trace.WriteLine(JsonConvert.SerializeObject(listFiles));
                    }
                    DateTime minDate = StartingDate.HasValue ? Convert.ToDateTime(StartingDate) : listFiles.Min(x => x.LastWriteTime);
                    DateTime maxDate = EndDate.HasValue ? Convert.ToDateTime(EndDate) : listFiles.Max(x => x.LastWriteTime);
                    var finalList = listFiles.Where(x => x.LastWriteTime >= minDate && x.LastWriteTime <= maxDate && x.Exists == true).OrderByDescending(x => x.LastWriteTime).ToList();
                    foreach (var file in finalList)
                    {
                        _dict.Add(file.Name, file.LastWriteTime);
                    } 
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

        /// <summary>
        /// 
        /// </summary>
        /// <param name="fileTextContent"></param>
        /// <param name="folderName"></param>
        /// <param name="fileNameWithoutExtension"></param>
        /// <param name="encoding">By Default Supports UTF-8 for maximum compatiblity</param>
        /// <param name="baseDirectory">By Default selects the root application folder.If provided then it should be an absolute url if provided e.g `c:\\yourfolder\\yourfoldersub` etc</param>
        /// <returns></returns>
        public static string LogAndReturnFileNameWithPath(string fileTextContent, string folderName = null,  string fileNameWithoutExtension = null, FileExtension fileExtension = FileExtension.txt, Encoding encoding = null, string baseDirectory = null)
        {
            if (string.IsNullOrWhiteSpace(baseDirectory))
            {
                baseDirectory = AppDomain.CurrentDomain.BaseDirectory;
            }

            if (string.IsNullOrWhiteSpace(folderName))
            {
                folderName = "log";
            }

            encoding = encoding ?? Encoding.UTF8;
            string completeFilePathWithExtension = string.Empty;
            lock (_syncRandomFileWriteObject)
            {
                if (string.IsNullOrWhiteSpace(fileNameWithoutExtension))
                {
                    fileNameWithoutExtension = Guid.NewGuid().ToString();
                }
                completeFilePathWithExtension = Path.Combine(baseDirectory, folderName, GetDefaultFileExtension(fileNameWithoutExtension, fileExtension));
                using (StreamWriter sw = new StreamWriter(completeFilePathWithExtension, false, encoding))
                {
                    sw.Write(fileTextContent);
                }
            }
            return completeFilePathWithExtension;
        }

        private static string GetDateTimeAndReference(string referenceId)
        {
            return DateTime.Now.ToString("dd/MM/yyyy HH:mm:ss", CultureInfo.InvariantCulture)
                + (string.IsNullOrEmpty(referenceId) ? "" : $"({referenceId})");
        }
        #endregion

        ~CustomLogger()
        {
            if (_sbLog.Length > 0)
            {
                _sbLog.AppendLine($"ReferenceId '{_referenceId}' commited (Auto)");
                _sbLog.AppendLine("*****************************************************************************");
                Log(_sbLog.ToString(), _filePathForNormalLogs);
                _sbLog.Clear();
                DeleteReferenceToStaticLists();
            }
        }
    }
}
