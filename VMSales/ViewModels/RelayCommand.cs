using System;

namespace VMSales.ViewModels
{
    internal class RelayCommand
    {
        private object removePurchaseDateFilter;
        private Func<bool> p;

        public RelayCommand(object removePurchaseDateFilter, Func<bool> p)
        {
            this.removePurchaseDateFilter = removePurchaseDateFilter;
            this.p = p;
        }
    }
}