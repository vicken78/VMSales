using System;
using System.Collections.ObjectModel;
using System.Data;
using System.Windows.Data;
using VMSales.Models;
using System.Linq;
using System.Collections.Generic;
using System.ComponentModel;
using VMSales.Logic;
using System.Windows;
using System.Threading.Tasks;
using System.Text.RegularExpressions;
using System.Collections.Specialized;

namespace VMSales.ViewModels
{
    public class PurchaseOrderViewModel : BaseViewModel
    {

        private string invoicetemp;
        private DateTime purchase_datetemp;

        private PurchaseOrderModel _SelectedItem { get; set; }
        public PurchaseOrderModel SelectedItem
        {
            get { return _SelectedItem; }
            set
            {
                if (_SelectedItem == value)
                    return;
                _SelectedItem = value;
                RaisePropertyChanged("SelectedItem");
            }
        }

        #region Collections
        public ObservableCollection<string> FilterInvoiceNumber
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
        public ObservableCollection<DateTime> FilterPurchaseDate
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

        private bool _keep_last;
        public bool keep_last
        {
            get { return _keep_last; }
            set
            {
                if (_keep_last == value)
                    return;
                _keep_last = value;
                RaisePropertyChanged("keep_last");
            }
        }




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

        
        public CollectionView PurchaseOrderView
        {
            get

            {
                cvs.View.CurrentChanged += (sender, e) => SelectedItem = cvs.View.CurrentItem as PurchaseOrderModel;
                return (CollectionView)CollectionViewSource.GetDefaultView(ObservableCollectionPurchaseOrderModel);
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
   
            if (supplier_fk == 0)
            {
                MessageBox.Show("Please select a supplier.");
                return;
            }

            var selectedRows = ObservableCollectionPurchaseOrderModel.Where(i => i.IsSelected);
            foreach (var item in selectedRows)
            {
                invoicetemp = item.invoice_number;
                purchase_datetemp = item.purchase_date;
            }

            if (keep_last == false)
            {
                SelectedItem = new PurchaseOrderModel()
                {
                    invoice_number = "0",
                    purchase_date = DateTime.MinValue,
                    purchase_order_pk = 0,
                    purchase_order_fk = 0,
                    purchase_order_detail_pk = 0,
                    supplier_fk = this.supplier_fk,
                    lot_cost = 0,
                    lot_quantity = 0,
                    lot_number = "0",
                    lot_name = "Name",
                    lot_description = "",
                    sales_tax = 0,
                    shipping_cost = 0
                };
            }
            else
            {
                SelectedItem = new PurchaseOrderModel()
                {
                    invoice_number = invoicetemp,
                    purchase_date = purchase_datetemp,
                    purchase_order_pk = 0,
                    purchase_order_fk = 0,
                    purchase_order_detail_pk = 0,
                    supplier_fk = this.supplier_fk,
                    lot_cost = 0,
                    lot_quantity = 0,
                    lot_number = "0",
                    lot_name = "Name",
                    lot_description = "",
                    sales_tax = 0,
                    shipping_cost = 0
                };
            }
            ObservableCollectionPurchaseOrderModel.Add(SelectedItem);
        }
        public void DeleteCommand()
        {

            if (MessageBox.Show("Please Confirm Deletion.", "Question", MessageBoxButton.YesNo, MessageBoxImage.Warning) == MessageBoxResult.No)
            {
                return;
            }
            else
            {
                if (SelectedItem.purchase_order_detail_pk == 0 || SelectedItem.purchase_order_fk == 0)
                {

                    // delete from screen only.

                    ObservableCollectionPurchaseOrderModel.Remove(ObservableCollectionPurchaseOrderModel.Where(i => i.purchase_order_detail_pk == SelectedItem.purchase_order_detail_pk).Single());
                    RaisePropertyChanged("ObservableCollectionPurchaseModel");
                    return;
                }
                if (SelectedItem.purchase_order_pk != 0 && SelectedItem.purchase_order_fk != 0 && SelectedItem.purchase_order_detail_pk != 0)
                {
                    dataBaseProvider = getprovider();
                    DataBaseLayer.PurchaseOrderRepository PurchaseOrderRepo = new DataBaseLayer.PurchaseOrderRepository(dataBaseProvider);
                    try
                    {

                        Task<bool> deletePurchase_Order = PurchaseOrderRepo.Delete(SelectedItem);
                        if (deletePurchase_Order.Result == true)
                        {
                            PurchaseOrderRepo.Commit();
                            PurchaseOrderRepo.Dispose();
                        }
                        else
                        {
                            PurchaseOrderRepo.Revert();
                            PurchaseOrderRepo.Dispose();
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
        public void SaveCommand()
        {
            SelectedItem = new PurchaseOrderModel();

            var selectedRows = ObservableCollectionPurchaseOrderModel.Where(i => i.IsSelected);
            
            foreach (var item in selectedRows)
            {
                SelectedItem.supplier_fk = supplier_fk;
                SelectedItem.invoice_number = item.invoice_number;
                SelectedItem.purchase_date = item.purchase_date;
                SelectedItem.purchase_order_pk = item.purchase_order_pk;
                SelectedItem.purchase_order_fk = item.purchase_order_fk;
                SelectedItem.purchase_order_detail_pk = item.purchase_order_detail_pk;
                SelectedItem.sales_tax = item.sales_tax;
                SelectedItem.shipping_cost = item.shipping_cost;
                SelectedItem.lot_cost = item.lot_cost;
                SelectedItem.lot_name = item.lot_name.Trim();
                SelectedItem.lot_description = item.lot_description.Trim();
                SelectedItem.lot_quantity = item.lot_quantity;
                SelectedItem.lot_number = item.lot_number;
            }

            //  we need to check for default values here. better checks later.
                       if (SelectedItem.lot_name == "Name")
                       {
                           MessageBox.Show("Default Values must not be used.");
                        SelectedItem = null;
                        return;
                       }


            if (supplier_fk.ToString() == null || supplier_fk == 0)
            {
                dataBaseProvider = getprovider();
                DataBaseLayer.PurchaseOrderRepository PurchaseOrderRepo = new DataBaseLayer.PurchaseOrderRepository(dataBaseProvider);

                // attempt to get supplier.
                try
                {

                    Task<PurchaseOrderModel> get_supplier = PurchaseOrderRepo.get_supplier_fk(SelectedItem.purchase_order_pk);

                    if (get_supplier.Result.supplier_fk !=  0)
                    {
                        PurchaseOrderRepo.Commit();
                        PurchaseOrderRepo.Dispose();
                        SelectedItem.supplier_fk = get_supplier.Result.supplier_fk;
                        RaisePropertyChanged("supplier_fk");
                    }
                    else
                    {
                        PurchaseOrderRepo.Revert();
                        PurchaseOrderRepo.Dispose();
                        // we could not get a supplier from db.
                        MessageBox.Show("Please select a supplier.");
                        return;
                    }
                }
                catch (Exception err)
                {
                    MessageBox.Show("An error has occured: " + err);
                    PurchaseOrderRepo.Revert();
                    PurchaseOrderRepo.Dispose();
                }
            }

            // scenerio 4 not programmed
            // INSERT new invoice number, UPDATE purchase_order_details to that invoice number.


            // scenerio 1
            // same invoice number, UPDATE purchase_order_detail.

              dataBaseProvider = getprovider();
              DataBaseLayer.PurchaseOrderRepository SavePurchaseOrderRepo = new DataBaseLayer.PurchaseOrderRepository(dataBaseProvider);
            try
            {
                if (SelectedItem.purchase_order_detail_pk != 0)
                {
                    // verify it exists in db
                    Task<bool> get_purchase_order_detail_pk = SavePurchaseOrderRepo.Get_purchase_order_detail_pk(SelectedItem);
                    if (get_purchase_order_detail_pk.Result.Equals(true))
                    {
                        Task<bool> updatePurchase_Order = SavePurchaseOrderRepo.Update(SelectedItem);
                        if (updatePurchase_Order.Result.Equals(true))
                        {
                            SavePurchaseOrderRepo.Commit();
                            SavePurchaseOrderRepo.Dispose();
                            MessageBox.Show("Updated");
                            SelectedItem = new PurchaseOrderModel();
                            return;
                        }
                        else
                        {
                            SavePurchaseOrderRepo.Revert();
                            SavePurchaseOrderRepo.Dispose();
                            MessageBox.Show("Update failed to commit.");
                            return;
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("An Error has occured" + ex.ToString());
                SavePurchaseOrderRepo.Revert();
                SavePurchaseOrderRepo.Dispose();
                return;
            }

            //         SelectedItem.supplier_fk = this.supplier_fk;

            // scenerio 3
            // INSERT new invoice number, INSERT new purchase_order detail --- pod_pk = 0
            try
            {
                if (SelectedItem.purchase_order_detail_pk == 0)
                {
                    //dataBaseProvider = getprovider();
                    //DataBaseLayer.PurchaseOrderRepository PurchaseOrderRepo = new DataBaseLayer.PurchaseOrderRepository(dataBaseProvider);
                    Task<bool> insertPurchase_Order = SavePurchaseOrderRepo.Insert(SelectedItem);
                    
                    // new purchase order_detail_pk must be assigned 
                    if (insertPurchase_Order.Result.Equals(true))
                    {
                        MessageBox.Show("1 Row Inserted");
                        RaisePropertyChanged("ObservableCollectionPurchaseOrderModel");
                        var purchase_order_detail_pk_result = SavePurchaseOrderRepo.Get_last_insert();
                        SelectedItem.purchase_order_detail_pk = purchase_order_detail_pk_result.Result;
                        MessageBox.Show(SelectedItem.purchase_order_detail_pk.ToString());
                        SavePurchaseOrderRepo.Commit();
                        SavePurchaseOrderRepo.Dispose();
                        SelectedItem = new PurchaseOrderModel();
                        return;
                    }
                    else
                    {
                        SavePurchaseOrderRepo.Revert();
                        SavePurchaseOrderRepo.Dispose();
                        MessageBox.Show("An error has occured.  No changes were made");
                    }
                }
            }
            catch (Exception e)
            {
                MessageBox.Show("An Error has occured, no changes were made. Error:" + e);
                return;
            }

            #endregion
        }

        public void LoadPurchaseOrder(int supplier_fk)
        {

            ObservableCollectionPurchaseOrderModel = new ObservableCollection<PurchaseOrderModel>();

            //clear and reload invoicedate and purchasedate based on supplier
            /*     if (InvoiceNumber.Count > 0 && PurchaseDate.Count > 0)
                 {
                     InvoiceNumber.Clear();
                     PurchaseDate.Clear();
                 }
            */
            InvoiceNumberList = new List<string>();
            PurchaseDateList = new List<DateTime>();
            dataBaseProvider = getprovider();
            DataBaseLayer.PurchaseOrderRepository PurchaseOrderRepo = new DataBaseLayer.PurchaseOrderRepository(dataBaseProvider);
            var Result = PurchaseOrderRepo.GetAllWithID(supplier_fk).Result;
            if (Result.Count() > 0)
            {
                ObservableCollectionPurchaseOrderModel = Result.ToObservable();
            }

            RaisePropertyChanged("ObservableCollectionPurchaseOrderModel");
            PurchaseOrderRepo.Commit();
            PurchaseOrderRepo.Dispose();

            foreach (var item in ObservableCollectionPurchaseOrderModel)
            {
                SelectedItem = item;

                if (item.invoice_number != null || item.purchase_date != null)
                    if (!InvoiceNumberList.Contains(item.invoice_number))
                        InvoiceNumberList.Add(item.invoice_number);
                if (!PurchaseDateList.Contains(item.purchase_date))
                    PurchaseDateList.Add(item.purchase_date);
            }

            if (InvoiceNumberList.Count > 0 || PurchaseDateList.Count > 0)
            {
                FilterInvoiceNumber = new ObservableCollection<string>(InvoiceNumberList.Cast<String>());
                FilterPurchaseDate = new ObservableCollection<DateTime>(PurchaseDateList.Cast<DateTime>());
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

            dataBaseProvider = getprovider();
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
                SelectedItem = item;
                if (item.invoice_number != null || item.purchase_date != null)
                    if (!InvoiceNumberList.Contains(item.invoice_number))
                        InvoiceNumberList.Add(item.invoice_number);
                if (!PurchaseDateList.Contains(item.purchase_date))
                    PurchaseDateList.Add(item.purchase_date);
            }

            InvoiceNumberList.Sort();
            PurchaseDateList.Sort();

            if (InvoiceNumberList.Count > 0 || PurchaseDateList.Count > 0)
            {
                FilterInvoiceNumber = new ObservableCollection<string>(InvoiceNumberList.Cast<String>());
                FilterPurchaseDate = new ObservableCollection<DateTime>(PurchaseDateList.Cast<DateTime>());
            }

            // Get Suppliers
            ObservableCollectionSupplierModel = new ObservableCollection<SupplierModel>();
            dataBaseProvider = getprovider();
            DataBaseLayer.SupplierRepository SupplierRepo = new DataBaseLayer.SupplierRepository(dataBaseProvider);
            ObservableCollectionSupplierModel = SupplierRepo.GetAll().Result.ToObservable();
            SupplierRepo.Commit();
            SupplierRepo.Dispose();
        }

    }
    #endregion
}