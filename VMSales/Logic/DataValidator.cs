using System;
using System.Globalization;
using System.Windows;
using System.Windows.Data;

namespace VMSales.Logic
{
    public class DataValidator : IValueConverter
    {
        private DateTime? dateCreated = null;


        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value != null && !string.IsNullOrEmpty(value.ToString()))
            {
                    return value;
                }
                return string.IsNullOrEmpty((string)value) ? parameter : value;
        }

        public object ConvertBack(
              object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotSupportedException();
        }


        public static Boolean CheckLength(int length, string stringvalue)
        {
            if (stringvalue is null) return false;
            if (stringvalue.Length > length) return false;
            return true;
        }

        public static Boolean CheckNullWhiteSpace(string value)
        {
            // used for not null values - true value is not null - false - null
            if (String.IsNullOrWhiteSpace(value) == false)
               { return true; }
            return false;
        }
    }
}
