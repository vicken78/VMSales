using System;
using System.Collections.ObjectModel;
using System.Data;
using System.Windows.Data;
using VMSales.Models;
using System.Linq;
using System.Collections.Generic;
using System.ComponentModel;
using VMSales.Logic;
using VMSales.ChangeTrack;
using System.Windows;
using System.Runtime.CompilerServices;
using System.Threading.Tasks;
using System.Collections.Specialized;
using System.Collections;

namespace VMSales.ViewModels
{

    // The way I am doing filtering is read only. This is no good.  
    // changed binding of purchaseorder to the observable collection.

    public class PurchaseOrderViewModel : BaseViewModel
    {
        private BaseViewModel baseViewModel;

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
        private CollectionViewSource cvs = new CollectionViewSource();
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
            if (CanRemoveInvoiceNumberFilter)
            {
                cvs.Filter -= new FilterEventHandler(FilterByInvoiceNumber);
                cvs.Filter += new FilterEventHandler(FilterByInvoiceNumber);
            }
            else
            {
                cvs.Filter += new FilterEventHandler(FilterByInvoiceNumber);
                CanRemoveInvoiceNumberFilter = true;
            }
        }
        public void AddPurchaseDateFilter()
        {
            if (CanRemovePurchaseDateFilter)
            {
                cvs.Filter -= new FilterEventHandler(FilterByPurchaseDate);
                cvs.Filter += new FilterEventHandler(FilterByPurchaseDate);
            }
            else
            {
                cvs.Filter += new FilterEventHandler(FilterByPurchaseDate);
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
            cvs.Filter -= new FilterEventHandler(FilterByInvoiceNumber);
            SelectedInvoiceNumber = null;
            PurchaseOrderView.Filter = null;
            CanRemoveInvoiceNumberFilter = false;
            RaisePropertyChanged("PurchaseOrderView");

        }
        public void RemovePurchaseDateFilterCommand()
        {
            cvs.Filter -= new FilterEventHandler(FilterByPurchaseDate);
            SelectedPurchaseDate = null;
            PurchaseOrderView.Filter = null;
            CanRemovePurchaseDateFilter = false;
            RaisePropertyChanged("PurchaseOrderView");
        }

        #endregion 

        private PurchaseOrderModel _selectedrow = null;
        public PurchaseOrderModel selectedrow { get => this._selectedrow; set { this._selectedrow = value; RaisePropertyChanged("selectedrow"); } }

        public ICollectionView PurchaseOrderView
        {
            get

            {
                cvs.View.CurrentChanged += (sender, e) => selectedrow = cvs.View.CurrentItem as PurchaseOrderModel;
                //return CollectionViewSource.GetDefaultView(ObservableCollectionPurchaseOrderModel); 
                return cvs.View;
            }
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

        public List<string> InvoiceNumberList { get; set; }
        public List<DateTime> PurchaseDateList { get; set; }

        IDatabaseProvider dataBaseProvider;

        #region ButtonCommands
        public void AddCommand()
        {
            var obj = new PurchaseOrderModel()
            {
                purchase_order_pk = 0,
                purchase_order_fk = '0',
                purchase_order_detail_pk = 0,
                supplier_fk = this.supplier_fk,
                invoice_number = "0",
                purchase_date = DateTime.MinValue,
                lot_cost = 0,
                lot_quantity = 0,
                lot_number = "0",
                lot_name = "Name",
                lot_description = "",
                sales_tax = 0,
                shipping_cost = 0
            };

            ObservableCollectionPurchaseOrderModel.Add(obj);
            RaisePropertyChanged("ObservableCollectionPurchaseOrderModel");
        }
        public void DeleteCommand()
        {

            if (MessageBox.Show("Please Confirm Deletion.", "Question", MessageBoxButton.YesNo, MessageBoxImage.Warning) == MessageBoxResult.No)
            {
                return;
            }
            else
            {
                if (selectedrow.purchase_order_detail_pk == 0 || selectedrow.purchase_order_fk == 0)
                {
                    // delete from screen only.
                    return;
                }
                if (selectedrow.purchase_order_pk != 0 && selectedrow.purchase_order_fk != 0 && selectedrow.purchase_order_detail_pk != 0)
                {
                    dataBaseProvider = BaseViewModel.getprovider();
                    DataBaseLayer.PurchaseOrderRepository PurchaseOrderRepo = new DataBaseLayer.PurchaseOrderRepository(dataBaseProvider);
                    try
                    {

                        // fix
                        Task<bool> deletePurchase_Order = PurchaseOrderRepo.Delete(selectedrow);
                        if (deletePurchase_Order.Result == true)
                        {
                            PurchaseOrderRepo.Commit();
                            PurchaseOrderRepo.Dispose();
                        }
                        else
                        {
                            throw new Exception();
                        }
                    }
                    catch (Exception e)
                    {
                        PurchaseOrderRepo.Revert();
                        PurchaseOrderRepo.Dispose();

                        MessageBox.Show("An Error has occured, no changes were made. Error:" + e);
                        return;
                    }
                }
            }
        }
        public void ResetCommand()
        {

        }
        public void SaveCommand(object sender, RoutedEventArgs e)
        {
   
            if (supplier_fk.ToString() == null || supplier_fk == 0)
            { 
                MessageBox.Show("Please select a supplier.");
                return;
            }

            if (selectedrow.purchase_order_detail_pk == 0 || selectedrow.purchase_order_fk == 0 || selectedrow.purchase_order_detail_pk == 0)
            {
                if (selectedrow.lot_name == "Name")
                {
                    MessageBox.Show("Default Values must not be used.");
                    return;
                }

                // insert new row
                dataBaseProvider = BaseViewModel.getprovider();
                DataBaseLayer.PurchaseOrderRepository PurchaseOrderRepo = new DataBaseLayer.PurchaseOrderRepository(dataBaseProvider);
                try
                {
                    // we need to check for default values here. better checks later.
                    Task<bool> insertPurchase_Order = PurchaseOrderRepo.Insert(selectedrow);
                    if (insertPurchase_Order.Result == true)
                    {
                        
                        PurchaseOrderRepo.Commit();
                        PurchaseOrderRepo.Dispose();
                        MessageBox.Show("Data Inserted");
                        return;
                    }
                    else { MessageBox.Show("Insert failed to commit."); }
                }
                catch (Exception ex)
                {
                    MessageBox.Show("An Error has occured." + ex);
                    PurchaseOrderRepo.Revert();
                    PurchaseOrderRepo.Dispose();
                }
                return;
        }
            // update
              if (selectedrow.purchase_order_detail_pk != 0)
              {
                selectedrow.supplier_fk = this.supplier_fk;

                dataBaseProvider = BaseViewModel.getprovider();
                DataBaseLayer.PurchaseOrderRepository PurchaseOrderRepo = new DataBaseLayer.PurchaseOrderRepository(dataBaseProvider);
                try
                {
                      Task<bool> updatePurchase_Order = PurchaseOrderRepo.Update(selectedrow);
     
                        if (updatePurchase_Order.Result.Equals(true))
                        {
                          PurchaseOrderRepo.Commit();
                          PurchaseOrderRepo.Dispose();
                        }
                        else { MessageBox.Show("Update failed to commit."); }
                }
                catch (Exception ex)
                {
                    MessageBox.Show("An Error has occured." + ex);
                      PurchaseOrderRepo.Revert();
                      PurchaseOrderRepo.Dispose();
                }
                return;
            }
        }
        #endregion

     


        public void LoadPurchaseOrder(int supplier_fk)
        {

            ObservableCollectionPurchaseOrderModel = new ObservableCollection<PurchaseOrderModel>();

            //clear and reload invoicedate and purchasedate
            if (InvoiceNumber.Count > 0 && PurchaseDate.Count > 0)
            {
                InvoiceNumber.Clear();
                PurchaseDate.Clear();
            }
         
            InvoiceNumberList = new List<string>();
            PurchaseDateList = new List<DateTime>();
            dataBaseProvider = BaseViewModel.getprovider();
            DataBaseLayer.PurchaseOrderRepository PurchaseOrderRepo = new DataBaseLayer.PurchaseOrderRepository(dataBaseProvider);
            var Result = PurchaseOrderRepo.GetAllWithID(supplier_fk).Result;

            if (Result.Count() == 0)
            {
                selectedrow = null;
                ObservableCollectionPurchaseOrderModel.Add(new PurchaseOrderModel()
                {
                    purchase_order_pk = 0,
                    purchase_order_fk = 0,
                    purchase_order_detail_pk = 0,
                    supplier_fk = this.supplier_fk,
                    invoice_number = "0",
                    purchase_date = DateTime.MinValue,
                    lot_cost = 0,
                    lot_quantity = 0,
                    lot_number = "0",
                    lot_name = "Name",
                    lot_description = "",
                    sales_tax = 0,
                    shipping_cost = 0
                });
            }
            else
            {
                ObservableCollectionPurchaseOrderModel = Result.ToObservable();
            }
            RaisePropertyChanged("ObservableCollectionPurchaseOrderModel");
            PurchaseOrderRepo.Commit();
            PurchaseOrderRepo.Dispose();

            foreach (var item in ObservableCollectionPurchaseOrderModel)
            {
                selectedrow = item;
                    
                if (item.invoice_number != null || item.purchase_date != null)
                    if (!InvoiceNumberList.Contains(item.invoice_number))
                        InvoiceNumberList.Add(item.invoice_number);
                if (!PurchaseDateList.Contains(item.purchase_date))
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
            //Initial Page Load
            InvoiceNumberList = new List<string>();
            PurchaseDateList = new List<DateTime>();

            // Get All Purchase Order
            ObservableCollectionPurchaseOrderModel = new ObservableCollection<PurchaseOrderModel>();

            dataBaseProvider = BaseViewModel.getprovider();
            DataBaseLayer.PurchaseOrderRepository PurchaseOrderRepo = new DataBaseLayer.PurchaseOrderRepository(dataBaseProvider);
            var Result = PurchaseOrderRepo.GetAll().Result;
            if (Result.Count() == 0)
            {
                Result.DefaultIfEmpty(new PurchaseOrderModel()
                {
                    purchase_order_pk = 0,
                    purchase_order_fk = 0,
                    purchase_order_detail_pk = 0,
                    supplier_fk = this.supplier_fk,
                    invoice_number = "0",
                    purchase_date = DateTime.MinValue,
                    lot_cost = 0,
                    lot_quantity = 0,
                    lot_number = "0",
                    lot_name = "Name",
                    lot_description = "",
                    sales_tax = 0,
                    shipping_cost = 0
                });
            }
            ObservableCollectionPurchaseOrderModel = PurchaseOrderRepo.GetAll().Result.ToObservable();
            cvs.Source = ObservableCollectionPurchaseOrderModel;
            PurchaseOrderRepo.Commit();
            PurchaseOrderRepo.Dispose();
            RaisePropertyChanged("ObservableCollectionPurchaseOrderModel");

            foreach (var item in Result)
            {
                selectedrow = item;
                if (item.invoice_number != null || item.purchase_date != null)
                    if (!InvoiceNumberList.Contains(item.invoice_number)) 
                      InvoiceNumberList.Add(item.invoice_number);
                    if (!PurchaseDateList.Contains(item.purchase_date))
                      PurchaseDateList.Add(item.purchase_date);
                }
           if (InvoiceNumberList.Count > 0 || PurchaseDateList.Count > 0)
           {
               InvoiceNumber = new ObservableCollection<string>(InvoiceNumberList.Cast<String>());
               PurchaseDate = new ObservableCollection<DateTime>(PurchaseDateList.Cast<DateTime>());
           }

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