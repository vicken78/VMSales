using System;
using System.Globalization;
using System.Windows.Data;

namespace VMSales.Logic
{
    public class CollectionIsEnabled : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            // If ComboBox has a value selected, enable TextBox
            return value != null;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
    