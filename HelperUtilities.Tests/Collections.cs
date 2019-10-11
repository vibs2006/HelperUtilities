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
       

        [TestMethod]
        public void TestGenericPagingList()
        {
            List<testObj> objList = Common.GenerateSampleList(23);

            Trace.WriteLine($"List Generation is Completed for sample {objList.Count} Records");
            foreach (var item in objList)
            {
                Trace.WriteLine(item.Id + " - " + item.Name);
            }

            CollectionMethods.ProcessListViaPaging(objList, 10, (currentPageList) =>
            {
                Trace.WriteLine("\nNow Looping through currentPageList");
                foreach (var item in currentPageList)
                {
                    Trace.WriteLine($"{item.Id} - {item.Name}");
                }
            });

            //Alternative Method
            //CollectionMethods.ProcessListViaPaging(objList, 10, processEachList );
        }

        private void processEachList(IList<testObj> currentPageList)
        {
            Trace.WriteLine("\nNow Looping through currentPageList");
            foreach (var item in currentPageList)
            {
                Trace.WriteLine($"{item.Id} - {item.Name}");
            }
        }

        [TestMethod]
        public void TestPagingList()
        {
            List<testObj> objList = Common.GenerateSampleList(23);

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
