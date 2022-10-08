using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace HelperUtilities.Text
{
    public static class StaticTextUtils
    {
        static Random random = new Random();
        public static string RemoveLineBreaks(this string lines)
        {
            return lines.Replace("\r", "").Replace("\n", "");
        }

        public static string Left(this string input, int maxLength)
        {
            return input.PadRight(maxLength).Substring(0, maxLength).TrimEnd();
        }

        public static string ReplaceLineBreaks(this string lines, string replacement)
        {
            return lines.Replace(Environment.NewLine, replacement)
                        .Replace("\r", replacement)
                        .Replace("\n", replacement);
        }

        public static string GetNumbersFromString(string input)
        {
            return string.Join("", input.ToCharArray().Where(Char.IsDigit));
        }

        public static string RemoveSpecialCharactersFromString(this string input)
        {
            return Regex.Replace(input.Trim(), @"[^0-9a-zA-Z]+", string.Empty);
        }

        public static string GenerateRandomWord(int wordLength = 10, bool AppendNumberInEnd = true, int maxNumberValue = 100, bool firstCharaterAsUpperCase = true)
        {

            string s = string.Empty;
            for (int j = 1; j <= wordLength; j++)
            {
                char ch;
                int a = random.Next(0, 26);
                if (firstCharaterAsUpperCase)
                {
                    if (j == 1)
                    {
                        ch = Char.ToUpper((char)('a' + a));

                    }
                    else
                    {
                        ch = (char)('a' + a);
                    }
                }
                else
                {
                    ch = (char)('a' + a);
                }
                s = s + ch;
            }

            if (AppendNumberInEnd)
            {
                s = s + random.Next(0, maxNumberValue).ToString();
            }

            return s;
        }

        public static string ConvertExceptionToString(Exception exception)
        {
            StringBuilder sb = new StringBuilder();
            sb.AppendLine("Exception Encountered!" + DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss"));
            do
            {
                sb.AppendLine("\t" + exception.Source + " - " + exception.Message);
                sb.AppendLine("\t" + exception.StackTrace);
                exception = exception.InnerException;
            } while (exception != null);

            return sb.ToString();
        }

        public static string RemoveLineEndings(this string value)
        {
            if (string.IsNullOrEmpty(value))
            {
                return value;
            }
            string lineSeparator = ((char)0x2028).ToString();
            string paragraphSeparator = ((char)0x2029).ToString();

            return value.Replace("\r\n", string.Empty)
                        .Replace("\n", string.Empty)
                        .Replace("\r", string.Empty)
                        .Replace(lineSeparator, string.Empty)
                        .Replace(paragraphSeparator, string.Empty);
        }

        public static string ConvertToBase64String(string inputString)
        {
            if (string.IsNullOrWhiteSpace(inputString)) return string.Empty;
            return Convert.ToBase64String(Encoding.Default.GetBytes(inputString));
        }

        public static string ConvertFromBase64StringToNormalString(string base64EncodedString)
        {
            if (string.IsNullOrWhiteSpace(base64EncodedString)) return string.Empty;
            try
            {
                return Encoding.Default.GetString(Convert.FromBase64String(base64EncodedString));
            }
            catch 
            {
                throw;
            }            
        }

        public static Stream GetStreamFromText(string text)
        {
            if (string.IsNullOrWhiteSpace(text)) return null;

            Stream _stream = new MemoryStream();
            byte[] bytes = Encoding.UTF8.GetBytes(text);
            _stream.Write(bytes, 0, bytes.Length);
            _stream.Position = 0;
            return _stream;
        }

        public static string GetUTF8TextFromSteam(Stream _stream)
        {
            if (_stream == null) return string.Empty;

            StringBuilder _sb = new StringBuilder();

            if (_stream.Position != 0)
            {
                _stream.Position = 0;
            }

            using (StreamReader _wr = new StreamReader(_stream))
            {

                while (!_wr.EndOfStream)
                {
                    _sb.AppendLine(_wr.ReadLine());
                }
            }
            return _sb.ToString();
        }

    }
}
