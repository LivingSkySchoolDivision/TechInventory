using System;
using System.Collections.Generic;
using System.Text;

namespace lskysd.techinventory.util
{
    public static class ListExtensions
    {
        public static string ToCommaSeparatedString<T>(this List<T> list)
        {
            StringBuilder returnMe = new StringBuilder();

            foreach(T listItem in list)
            {
                returnMe.Append(listItem);
                returnMe.Append(", ");
            }

            returnMe.Remove(returnMe.Length - 2, 2);

            return returnMe.ToString();
        }
    }
}
