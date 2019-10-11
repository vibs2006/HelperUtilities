using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HelperUtilities.Collections
{
    public static class CollectionMethods
    {
        /// <summary>
        /// It Returns the Current List on given Page. Please note that param currentPage have its starting value as 0 not 1.
        /// </summary>
        /// <typeparam name="T">Your Class</typeparam>
        /// <param name="list"></param>
        /// <param name="currentPage">currentPage have its starting value as 0 not 1. Last Page should be total list count divided by pageSize</param>
        /// <param name="pageSize"></param>
        /// <returns></returns>
        public static IList<T> GetPage<T>(IList<T> list, int currentPage, int pageSize)
        {
            return list.Skip(currentPage * pageSize).Take(pageSize).ToList();
        }
    }
}
