using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace HelperUtilities.Text
{
    public static class StaticTextUtils
    {
        public static string RemoveSpecialCharactersFromString(this string input)
        {
            return Regex.Replace(input.Trim(), @"[^0-9a-zA-Z]+", string.Empty);
        }
    }
}
