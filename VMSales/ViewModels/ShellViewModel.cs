﻿using Caliburn.Micro;

namespace VMSales.ViewModels
{
    public class ShellViewModel : Conductor<object>
    {
        public ShellViewModel()
        {

        }

        public void LoadCategory()
        {
              ActivateItemAsync(new CategoryViewModel());
        }


        public void LoadPurchaseOrder()
        {
            ActivateItemAsync(new PurchaseOrderViewModel());
        }
        public void LoadSupplier()
        {
            ActivateItemAsync(new SupplierViewModel());
        }
    }
}