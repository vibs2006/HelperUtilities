using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Diagnostics;
using HelperUtilities.IO;
//using System.Threading;
using System.Threading.Tasks;
using System.Globalization;

namespace HelperUtilities.Tests
{
    [TestClass]
    public class UnitTestsHelperUtlities
    {
        #region Test All Strings and Text Based Methods
        [TestMethod]
        public void TestReplaceSpecialCharacters()
        {
            string input = "A&*&7/sfsdfs!~//sdfsfsf";
            string output = string.Empty;
            Trace.WriteLine($"The input is '{input}' and the output is '" + HelperUtilities.Text.StaticTextUtils.RemoveSpecialCharactersFromString(input) + "'");
        }
        #endregion

        #region TestMiscMethods
        [TestMethod]
        public void TestMultithreadingLogWriter()
        {
            CustomLogger.DeleteAllRootFilesAndFolders();
           // CustomFileWriter.DeleteAllRootFilesAndFolders();
            Stopwatch sw = new Stopwatch();
            sw.Start();
            Trace.WriteLine("Stop Watch Started");


            for (int i=0; i < 1000; i++){

            System.Threading.Thread t1 = new System.Threading.Thread(() => {
                CustomLogger _wr = new CustomLogger();
                _wr.AppendLog($"This is first line of log");
                //var a = rd.Next(0, 5);
                //var b = 5 / a;
                _wr.AppendLog($"This is second line of log for");
            });

            t1.Start();
            }


            //Random rd = new Random();
            //Parallel.For(0, 1000, (index) =>
            //{
            //    CustomLogger _wr = new CustomLogger();
            //    _wr.AppendLog($"This is first line of log for '{index}'");
            //    //var a = rd.Next(0, 5);
            //    //var b = 5 / a;
            //    _wr.AppendLog($"This is second line of log for '{index}'");
                
            //    //CustomFileWriter.Log($"Test Message '{index}'", Guid.NewGuid().ToString());
            //    //_wr.CommitLog();
            //});

            Trace.WriteLine("Cursor moves to Test 2");

            //Parallel.For(0, 1000, (index) =>
            //{
            //    CustomTextFileLogger _wr = new CustomTextFileLogger();
            //    _wr.AppendLog($"This is first line of log for '{index}'");
            //    _wr.AppendLog($"This is second line of log for '{index}'");
            //    _wr.AppendLog("Test 2");
            //    //CustomFileWriter.Log($"Test Message '{index}'", Guid.NewGuid().ToString());
            //    _wr.CommitLog();
            //});

            //Trace.WriteLine("Cursor moves to Test 3");

            //Parallel.For(0, 1000, (index) =>
            //{
            //    CustomTextFileLogger _wr = new CustomTextFileLogger();
            //    _wr.AppendLog($"This is first line of log for '{index}'");
            //    _wr.AppendLog($"This is second line of log for '{index}'");
            //    //CustomFileWriter.Log($"Test Message '{index}'", Guid.NewGuid().ToString());
            //    _wr.AppendLog("Test 1");
            //    _wr.CommitLog();
            //});

            sw.Stop();
            Trace.WriteLine($"Stop watch ended with elapsed time of total {sw.ElapsedMilliseconds} milliseconds");
        }

        [TestMethod]
        public void TestListFiles()
        {
            var obj = CustomLogger.ListallFiles();

            foreach (var key in obj)
            {
                Trace.WriteLine($"{key.Key} - {key.Value.ToString("dd/MM/yyyy HH:mm:ss", CultureInfo.InvariantCulture)}");
            }
        }

        #endregion
    }
}
