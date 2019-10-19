using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HelperUtilities.IO
{
    public class CustomTextFileLogger
    {
        string _referenceId;
        StringBuilder _sbLog;
        public CustomTextFileLogger(string referenceId = null)
        {
            _referenceId = string.IsNullOrWhiteSpace(referenceId) ? Guid.NewGuid().ToString() : referenceId;
            _sbLog = new StringBuilder();
        }

        public void AppendLog(string logText, object obj = null)
        {
            if (_sbLog.Length == 0)
            {
                _sbLog.AppendLine($"{_referenceId} - {DateTime.Now.ToString("dd/MM/yyyy HH:mm:ss")} - {logText}");
            }
            else
            {
                _sbLog.AppendLine($"\t{logText} ({DateTime.Now.ToString("dd/MM/yyyy HH:mm:ss")})");
            }

            if (obj != null)
            {
                Type t = obj.GetType();
                if (t.IsClass == true) _sbLog.AppendLine(Newtonsoft.Json.JsonConvert.SerializeObject(obj));
            }
        }

        public void CommitLog()
        {
            if (_sbLog.Length > 0)
            {
                CustomFileWriter.Log(_sbLog.ToString());
                _sbLog.Clear();
            }
        }

        public void WriteException(Exception Ex)
        {
            CustomFileWriter.Log(Ex);
        }

        ~CustomTextFileLogger()
        {
            if (_sbLog.Length > 0)
            {
                CustomFileWriter.Log(_sbLog.ToString());               
            }
        }


    }
}
