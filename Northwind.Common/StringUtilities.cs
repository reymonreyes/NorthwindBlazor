using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Northwind.Common.Extensions
{
    public static class StringUtilities
    {
        public static bool IsEmptyOrLengthLessThan(this string str, int length)
        {
            return string.IsNullOrWhiteSpace(str) || (!string.IsNullOrWhiteSpace(str) && str.Length < length);
        }
    }
}
