using System;
using System.Collections.ObjectModel;
using System.Data;
using System.Runtime.CompilerServices;
using System.Windows.Data;
using VMSales.Database;
using VMSales.Models;
using System.Linq;
using System.Collections.Generic;
using System.ComponentModel;
using VMSales.ChangeTrack;
using System.Windows;
using VMSales.Models;
using VMSales.Logic;
using System.Threading.Tasks;
using System.Collections.ObjectModel;
using System.Windows;
using System;

namespace VMSales.ViewModels
{
    public class PurchaseOrderViewModel : BaseViewModel
    {
        #region Selected
        private string _selectedinvoicenumber;
        private string _selectedpurchasedate;

        public string SelectedInvoiceNumber
        {
            get { return _selectedinvoicenumber; }
            set
            {
                if (_selectedinvoicenumber == value)
                    return;
                _selectedinvoicenumber = value;
                RaisePropertyChanged("SelectedInvoiceNumber");
                //ApplyFilter(!string.IsNullOrEmpty(_selectedinvoicenumber) ? FilterField.InvoiceNumber : FilterField.None);
            }
        }
        public string SelectedPurchaseDate
        {
            get { return _selectedpurchasedate; }
            set
            {
                if (_selectedpurchasedate == value)
                    return;
                _selectedpurchasedate = value;
                RaisePropertyChanged("SelectedPurchaseDate");
                //ApplyFilter(!string.IsNullOrEmpty(_selectedpurchasedate) ? FilterField.PurchaseDate : FilterField.None);
            }
        }

        #endregion

        #region private Members
        private int _supplier_fk { get; set; }
        private ObservableCollection<string> _invoicenumber;
        private ObservableCollection<DateTime> _purchasedate;

        #endregion
        public int supplier_fk 
        {
        get { return _supplier_fk; }
        set 
            { 
                if (_supplier_fk == value) return;
                _supplier_fk = value;
                RaisePropertyChanged("supplier_fk");
                LoadPurchaseOrder(supplier_fk);
    }
}

        #region Collections
        public ObservableCollection<string> InvoiceNumber
        {
            get { return _invoicenumber; }
            set
            {
                if (_invoicenumber == value)
                    return;
                _invoicenumber = value;
                RaisePropertyChanged("InvoiceNumber");
            }
        }
        public ObservableCollection<DateTime> PurchaseDate
        {
            get { return _purchasedate; }
            set
            {
                if (_purchasedate == value)
                    return;
                _purchasedate = value;
                RaisePropertyChanged("PurchaseDate");
            }
        }

        public ObservableCollection<SupplierModel> ObservableCollectionSupplierModel { get; set; }
        private ObservableCollection<PurchaseOrderModel> ObservableCollectionPurchaseOrderModelclean { get; set; }
        public ObservableCollection<PurchaseOrderModel> ObservableCollectionPurchaseOrderModel
        {
            get { return ObservableCollectionPurchaseOrderModelclean; }
            set
            {
                if (ObservableCollectionPurchaseOrderModelclean == value) return;
                ObservableCollectionPurchaseOrderModelclean = value;
                RaisePropertyChanged("ObservableCollectionPurchaseOrderModel");
            }
        }
        #endregion
        public List<string> InvoiceNumberList { get; set; }
        public List<DateTime> PurchaseDateList { get; set; }

        IDatabaseProvider dataBaseProvider;

        public void LoadPurchaseOrder(int supplier_fk)
        {
            InvoiceNumberList = new List<string>();
            PurchaseDateList = new List<DateTime>();
            dataBaseProvider = BaseViewModel.getprovider();
            DataBaseLayer.PurchaseOrderRepository PurchaseOrderRepo = new DataBaseLayer.PurchaseOrderRepository(dataBaseProvider);
            var Result = PurchaseOrderRepo.GetAll(supplier_fk).Result;
            PurchaseOrderRepo.Commit();
            PurchaseOrderRepo.Dispose();

            foreach (var item in Result)
            {
                InvoiceNumberList.Add(item.invoice_number);
                PurchaseDateList.Add(item.purchase_date);
            }
            if (InvoiceNumberList.Count > 0 || PurchaseDateList.Count > 0)
            {
                InvoiceNumber = new ObservableCollection<string>(InvoiceNumberList.Cast<String>());
                PurchaseDate = new ObservableCollection<DateTime>(PurchaseDateList.Cast<DateTime>());
            }

        }

        #region pageload
        public PurchaseOrderViewModel()
        {
            // Get Suppliers
            ObservableCollectionSupplierModel = new ObservableCollection<SupplierModel>();
            dataBaseProvider = BaseViewModel.getprovider();
            DataBaseLayer.SupplierRepository SupplierRepo = new DataBaseLayer.SupplierRepository(dataBaseProvider);
            ObservableCollectionSupplierModel = SupplierRepo.GetAll().Result.ToObservable();
            SupplierRepo.Commit();
            SupplierRepo.Dispose();
        }

    }
    #endregion
}