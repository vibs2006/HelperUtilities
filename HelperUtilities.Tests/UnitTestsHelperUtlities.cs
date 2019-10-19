using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Diagnostics;
using HelperUtilities.IO;
//using System.Threading;
using System.Threading.Tasks;

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
            CustomFileWriter.DeleteAllRootFilesAndFolders();

            // Random rd = new Random();
            //Parallel.For(0, 10000, (index) => {               
            //    CustomFileWriter.Log($"Test Message '{index}'", Guid.NewGuid().ToString());
            //});      

        }

        #endregion
    }
}
