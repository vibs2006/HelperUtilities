using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Diagnostics;
using System.Collections.Generic;
using HelperUtilities.Collections;

namespace HelperUtilities.Tests
{
    [TestClass]
    public class Collections
    {
        Random random = new Random();
        public class testObj
        {
            public int Id { get; set; }
            public string Name { get; set; }
        }

        [TestMethod]
        public void TestPagingList()
        {            
            List<testObj> objList = new List<testObj>();

           
            for (int i = 1; i <= 129; i++)
            {
                string s = string.Empty;
                // random lowercase letter
                for (int j = 1; j <= 10; j++)
                {
                    int a = random.Next(0, 26);
                    char ch = (char)('a' + a);
                    s = s + ch;
                }
                objList.Add(new testObj
                {
                    Id = i,
                    Name = HelperUtilities.Text.StaticTextUtils.GenerateRandomWord()
                });
            }
            Trace.WriteLine("List Generation is Completed for sample 23 Records");
            foreach (var item in objList)
            {
                Trace.WriteLine(item.Id + " - " + item.Name);
            }

            int pageSize = 11;
            int totalPages = objList.Count / pageSize;

            Trace.WriteLine($"Total pages are {totalPages} with total count of {objList.Count} and pageSize of {pageSize}");
            //var NewList = HelperUtilities.Collections.CollectionMethods.GetPage<testObj>(objList, totalPages, pageSize);
            //Trace.WriteLine("Now Paging through last list");
            //foreach (var item in NewList)
            //{
            //    Trace.WriteLine(item.Id + " -- " + item.Name);
            //}

            //NewList = CollectionMethods.GetPage(objList, 0, pageSize);
            //Trace.WriteLine("Now Page through FIRST page of list");
            //foreach (var item in NewList)
            //{
            //    Trace.WriteLine(item.Id + " -- " + item.Name);
            //} 
            Trace.WriteLine("Now Looping through all pages one by one");           
            for (int i=0;i <= totalPages; i++)
            {
                Trace.WriteLine($"Page No. {i}\n starts");
                var list = CollectionMethods.GetPage(objList, i, pageSize);
                foreach(var item in list)
                {
                    Trace.WriteLine($"{item.Id} - {item.Name}");
                }
                Trace.WriteLine($"Page No. {i} ends");
            }

        }
    }
}
