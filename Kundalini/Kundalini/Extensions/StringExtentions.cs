using System;
using System.Collections.Generic;
using System.Text;

namespace Kundalini.Extensions
{
    using System.Linq;

    public static class StringExtentions
    {
        public static string BytesToString(this byte[] array)
        {
            return Encoding.Default.GetString(array);
        }
    }
}
