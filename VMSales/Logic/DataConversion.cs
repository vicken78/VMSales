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

        // given category_name and dictionary 
        public static int Get_category_pk(string category_name, Dictionary<int, string> categorydict)
        {
            foreach (var value in categorydict.Values)
            {
                if (category_name == value)
                {
                    //      return categorydict.Keys
                    return 1;
                }
            }
            return 0;
        }

        // usage
        //int key = this.Get_category_pk(categorydict,category_name);
        public static K Get_category_pk<K, V>(Dictionary<K, V> categorydict, V category_name)
        {
            foreach (KeyValuePair<K, V> pair in categorydict)
            {
                if (EqualityComparer<V>.Default.Equals(pair.Value, category_name))
                {
                    return pair.Key;
                }
            }
            return default;
        }


    }
}
