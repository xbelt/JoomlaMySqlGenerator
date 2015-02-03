using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;

namespace Warenlager.API
{
    static class Extensions
    {
        public static string FirstWord(this string input)
        {
            var index = input.IndexOf(" ");
            if (index < 0)
            {
                return input;
            }
            return input.SubstringSafe(0, index);
        }

        public static string RemoveEnd(this string source, int number)
        {
            return source.SubstringSafe(0, source.Length - number);
        }

        public static string SubstringSafe(this string input, int start, int length)
        {
            if (input == null)
                return "";
            if (start < 0)
                start = 0;
            if (start >= input.Length)
                return "";
            if (start + length >= input.Length)
                return input.Substring(start);
            return input.Substring(start, length);
        }

        public static string RemoveCurrencyEnd(this string input)
        {
            if (input == null)
                return "";
            input = input.ToUpper();
            if (input.EndsWith("€"))
                return input.RemoveEnd(1);
            if (input.EndsWith("CHF"))
                return input.RemoveEnd(3);
            return input;
        }

        public static T GetAndRemoveFirst<T>(this List<T> input) where T : class 
        {
            if (input.Any())
            {
                var result = input[0];
                input.RemoveAt(0);
                return result;
            }
            return null;
        }
    }
}
