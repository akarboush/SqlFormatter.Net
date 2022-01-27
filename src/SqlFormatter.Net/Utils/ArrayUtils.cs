using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SqlFormatter.Net.Utils
{
    public static class ArrayUtils
    {
        public static void SortByLengthDesc(this string[] arr)
        {
            Array.Sort(arr, (a, b) =>
            {
                var diffLen = b.Length - a.Length;
                return diffLen == 0 ? a.CompareTo(b) : diffLen;
            });
        }

        public static void ExcapeSpecialRegexCharacters(this string[] arr)
        {          
            for (var i = 0; i < arr.Length; i++)
            {
                arr[i] = RegexUtils.ExcapeSpecialCharacters(arr[i]);
            }
        }
        public static void ExcapeParen(this string[] arr)
        {
            for (var i = 0; i < arr.Length; i++)
            {
                arr[i] = RegexUtils.ExcapeParen(arr[i]);
            }
        }
    }
}
