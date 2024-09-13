using Caliburn.Micro;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Globalization;
using System.Linq;
using System.Windows.Data;

namespace VMSales.Logic
{
    public static class DataConversion 
    {
        // used in product view model (remove it)
        public static BindableCollection<T> ToBindableCollection<T>(this IEnumerable<T> source)
        {
            return new BindableCollection<T>(source);
        }

        public static bool IsEmptyOrAllSpaces(this string str)
        {
            return null != str && str.All(c => c.Equals(' '));
        }
        
        public static ObservableCollection<T> ToObservable<T>(this IEnumerable<T> source)
        {
            return new ObservableCollection<T>(source);
        }
    }
}