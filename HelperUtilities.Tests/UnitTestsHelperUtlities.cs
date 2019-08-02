using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Diagnostics;

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
            Trace.WriteLine($"The input is '{input}' and the output is '" + Text.StaticTextUtils.RemoveSpecialCharactersFromString(input) + "'");
        }
        #endregion
    }
}
