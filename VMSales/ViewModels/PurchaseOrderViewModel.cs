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
                ApplyFilter(!string.IsNullOrEmpty(_selectedinvoicenumber) ? FilterField.InvoiceNumber : FilterField.None);
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
                ApplyFilter(!string.IsNullOrEmpty(_selectedpurchasedate) ? FilterField.PurchaseDate : FilterField.None);
            }
        }

        #endregion

        #region Members
        private int _supplier_fk { get; set; }
        private ObservableCollection<string> _invoicenumber;
        private ObservableCollection<DateTime> _purchasedate; 
        private CollectionViewSource cvsPurchaseOrderView { get; set; } = new CollectionViewSource();
        //may have to add supplier_name to this.
        #endregion
  
        #region filter methods
        private bool _cancanremoveinvoicenumberfilter;
        private bool _cancanremovepurchasedatefilter;
        private enum FilterField
        {
            InvoiceNumber,
            PurchaseDate,
            None
        }
        private void ApplyFilter(FilterField field)
        {
            switch (field)
            {
                case FilterField.InvoiceNumber:
                    AddInvoiceNumberFilter();
                    PurchaseOrderView.Filter = new Predicate<object>(x => ((PurchaseOrderModel)x).invoice_number.ToString() == _selectedinvoicenumber);
                    RaisePropertyChanged("PurchaseOrderView");
                    break;
                case FilterField.PurchaseDate:
                    AddPurchaseDateFilter();
                    PurchaseOrderView.Filter = new Predicate<object>(x => ((PurchaseOrderModel)x).purchase_date.ToString() == _selectedpurchasedate);
                    RaisePropertyChanged("PurchaseOrderView");
                    break;
                default:
                    break;
            }
        }

        public void AddInvoiceNumberFilter()
        {
            // see Notes on Adding Filters:
            if (CanRemoveInvoiceNumberFilter)
            {
                cvsPurchaseOrderView.Filter -= new FilterEventHandler(FilterByInvoiceNumber);
                cvsPurchaseOrderView.Filter += new FilterEventHandler(FilterByInvoiceNumber);
            }
            else
            {
                cvsPurchaseOrderView.Filter += new FilterEventHandler(FilterByInvoiceNumber);
                CanRemoveInvoiceNumberFilter = true;
            }
        }
        public void AddPurchaseDateFilter()
        {
            if (CanRemovePurchaseDateFilter)
            {
                cvsPurchaseOrderView.Filter -= new FilterEventHandler(FilterByPurchaseDate);
                cvsPurchaseOrderView.Filter += new FilterEventHandler(FilterByPurchaseDate);
            }
            else
            {
                cvsPurchaseOrderView.Filter += new FilterEventHandler(FilterByPurchaseDate);
                CanRemovePurchaseDateFilter = true;
            }
        }
        private void FilterByInvoiceNumber(object sender, FilterEventArgs e)
        {
            var src = e.Item as PurchaseOrderModel;
            if (src == null)
                e.Accepted = false;
            else if (string.Compare(SelectedInvoiceNumber, src.invoice_number.ToString()) != 0)
                e.Accepted = false;
        }
        private void FilterByPurchaseDate(object sender, FilterEventArgs e)
        {
            var src = e.Item as PurchaseOrderModel;
            if (src == null)
                e.Accepted = false;
            else if (string.Compare(SelectedPurchaseDate, src.purchase_date.ToString()) != 0)
                e.Accepted = false;
        }

        public bool CanRemoveInvoiceNumberFilter
        {
            get { return _cancanremoveinvoicenumberfilter; }
            set
            {
                _cancanremoveinvoicenumberfilter = value;
                RaisePropertyChanged("CanRemoveInvoiceNumberFilter");
            }
        }
        public bool CanRemovePurchaseDateFilter
        {
            get { return _cancanremovepurchasedatefilter; }
            set
            {
                _cancanremovepurchasedatefilter = value;
                RaisePropertyChanged("CanRemovePurchaseDateFilter");
            }
        }
        public void RemoveInvoiceNumberFilterCommand()
        {
            cvsPurchaseOrderView.Filter -= new FilterEventHandler(FilterByInvoiceNumber);
            SelectedInvoiceNumber = null;
            PurchaseOrderView.Filter = null;
            CanRemoveInvoiceNumberFilter = false;
            RaisePropertyChanged("PurchaseOrderView");

        }
        public void RemovePurchaseDateFilterCommand()
        {
            cvsPurchaseOrderView.Filter -= new FilterEventHandler(FilterByPurchaseDate);
            SelectedPurchaseDate = null;
            PurchaseOrderView.Filter = null;
            CanRemovePurchaseDateFilter = false;
            RaisePropertyChanged("PurchaseOrderView");
        }

        #endregion 

        public ICollectionView PurchaseOrderView
        {
            get { return CollectionViewSource.GetDefaultView(ObservableCollectionPurchaseOrderModel); }
        }

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
            var Result = PurchaseOrderRepo.GetAllWithID(supplier_fk).Result;
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
            // Get All Purchase Order
            ObservableCollectionPurchaseOrderModel = new ObservableCollection<PurchaseOrderModel>();
            dataBaseProvider = BaseViewModel.getprovider();
            DataBaseLayer.PurchaseOrderRepository PurchaseRepo = new DataBaseLayer.PurchaseOrderRepository(dataBaseProvider);
            ObservableCollectionPurchaseOrderModel = PurchaseRepo.GetAll().Result.ToObservable();
            PurchaseRepo.Commit();
            PurchaseRepo.Dispose();

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