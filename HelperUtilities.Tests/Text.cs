using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using HelperUtilities.Text;
using System.Diagnostics;

namespace HelperUtilities.Tests
{
    [TestClass]
    public class Text
    {
        [TestMethod]
        public void GenerateRandomWords()
        {
            Trace.WriteLine("Generating 10 Random Numbers");
            for(int i = 1; i <= 10; i++)
            {
                Trace.WriteLine(HelperUtilities.Text.StaticTextUtils.GenerateRandomWord());
            }
        }
    }
}
