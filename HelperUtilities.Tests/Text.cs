﻿using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using HelperUtilities.Text;
using System.Diagnostics;
using System.IO;

namespace HelperUtilities.Tests
{
    [TestClass]
    public class Text
    {
        [TestMethod]
        public void TestTextFromStreamAndBack()
        {
            Stream stream = StaticTextUtils.GetStreamFromText("This is Test.\nThis is Test Line2\nTest is Test Line3");

            Trace.Write(StaticTextUtils.GetUTF8TextFromSteam(stream));
        }

        [TestMethod]
        public void GenerateRandomWords()
        {
            Trace.WriteLine("Generating 10 Random Numbers");
            for(int i = 1; i <= 10; i++)
            {
                Trace.WriteLine(StaticTextUtils.GenerateRandomWord());
            }
        }

        [TestMethod]
        public void TestBase32EncodeAndDecode()
        {

            TestInputOutputForBase32StringConversion("vaibhav@parcelhero.in");

            TestInputOutputForBase32StringConversion("123vaibhav@parcelhero.in");

            TestInputOutputForBase32StringConversion("123vaibhav@parcelhero.in1234");

            TestInputOutputForBase32StringConversion("12235656");

        }

        private void TestInputOutputForBase32StringConversion(string inputString)
        {
            Trace.WriteLine("*********************");
            Trace.WriteLine($"Original String is {inputString}");
            var outputString = Base32.ToBase32String(inputString);
            Trace.WriteLine("Output String is " + outputString);
            var convertedBackString = Base32.FromBase32String(outputString);
            Trace.WriteLine("Original String is " + convertedBackString);
            Trace.WriteLine("*********************");
        }
    }
}
