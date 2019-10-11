using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HelperUtilities.Tests
{
    public class testObj
    {
        public int Id { get; set; }
        public string Name { get; set; }
    }

    public static class Common
    {
        static Random random = new Random();
        public static List<testObj> GenerateSampleList(int NumberOfRows)
        {
            List<testObj> objList = new List<testObj>();
            for (int i = 1; i <= NumberOfRows; i++)
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
            return objList;
        }
    }
}
