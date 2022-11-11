using System.Collections.Generic;
using System.Linq;

namespace VMSales.Logic
{
    public static class DataConversion
    {
        // so far not needed.
        public static string listToString(List<string> myList)
        {
            string myString = string.Join(",", myList);
            return myString;
        }

        public static List<string> stringToList(string myString)
        {
            myString = string.Concat(myString.Where(c => !char.IsWhiteSpace(c)));
            List<string> myList = (myString).Split(',').ToList();
            return myList;
        }
    }
}
