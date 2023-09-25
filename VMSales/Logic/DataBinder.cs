using Caliburn.Micro;
using System.Collections.Generic;
using System.Linq;

namespace VMSales.Logic
{
    public static class DataBinder
    {
        public static BindableCollection<T> ToBindableCollection<T>(this IEnumerable<T> source)
        {
            return new BindableCollection<T>(source);
        }
    }
}
