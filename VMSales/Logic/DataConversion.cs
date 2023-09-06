using Caliburn.Micro;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Globalization;
using System.Linq;

namespace VMSales.Logic
{
    public static class DataConversion
    {

        public static BindableCollection<T> ToBindableCollection<T>(this IEnumerable<T> source)
        {
            return new BindableCollection<T>(source);
        }

        public static bool IsEmptyOrAllSpaces(this string str)
        {
            return null != str && str.All(c => c.Equals(' '));
        }

        // so far not needed.
        public static ObservableCollection<T> Convert<T>(IEnumerable<T> original)
        {
            return new ObservableCollection<T>(original);
        }

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

        public static ObservableCollection<T> ToObservable<T>(this IEnumerable<T> source)
        {
            return new ObservableCollection<T>(source);
        }
    }
}
