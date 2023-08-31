using System;
using System.Collections.ObjectModel;
using System.Data;
using System.Windows.Data;
using VMSales.Models;
using System.Linq;
using System.Collections.Generic;
using VMSales.Logic;
using System.Windows;
using System.Threading.Tasks;

namespace VMSales.ViewModels
{
    public class PurchaseOrderViewModel : BaseViewModel
    {
        private int _cmbSupplier { get; set; }
        public int cmbSupplier
        {
            get { return _cmbSupplier; }
            set
            {
                if (_cmbSupplier == value)
                    return;
                _cmbSupplier = value;
                RaisePropertyChanged("cmbSupplier");
            }
        }

        private ObservableCollection<PurchaseOrderModel> POM = new ObservableCollection<PurchaseOrderModel>();
        private List<int> purchase_order_products;
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

        private ObservableCollection<PurchaseOrderModel> _ObservableCollectionTotalPurchaseOrderModel;
        public ObservableCollection<PurchaseOrderModel> ObservableCollectionTotalPurchaseOrderModel
        {
            get { return _ObservableCollectionTotalPurchaseOrderModel; }
            set
            {
                if (_ObservableCollectionTotalPurchaseOrderModel == value) return;
                _ObservableCollectionTotalPurchaseOrderModel = value;
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
        private bool _cancanremovesupplierfilter;
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
        public bool CanRemoveSupplierFilter
        {
            get { return _cancanremovesupplierfilter; }
            set
            {
                _cancanremovesupplierfilter = value;
                RaisePropertyChanged("CanRemoveSupplierFilter");
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
                CanRemoveSupplierFilter = !CanRemoveSupplierFilter;
            }
        }

        public List<string> InvoiceNumberList { get; set; }
        public List<DateTime> PurchaseDateList { get; set; }

        IDatabaseProvider dataBaseProvider;

        #region ButtonCommands
        public void RemoveSupplier()
        {
            cmbSupplier = -1;
            supplier_fk = 0;
            RaisePropertyChanged("supplier_fk");
            LoadPurchaseOrder(0);
            CanRemoveSupplierFilter = false;
        }



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
                    shipping_cost = 0,
                    quantity_check = 0
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
            initial_load();
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
                SelectedItem.quantity_check = item.quantity_check;
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

                    if (get_supplier.Result.supplier_fk != 0)
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

            // scenerio 3
            // INSERT new invoice number, INSERT new purchase_order detail --- pod_pk = 0
            try
            {
                if (SelectedItem.purchase_order_detail_pk == 0)
                {
                    Task<int> insertPurchase_Order = SavePurchaseOrderRepo.Insert(SelectedItem);

                    // new purchase order_detail_pk must be assigned 
                    if (insertPurchase_Order.Result > 0)
                    {
                        var purchase_order_detail_pk_result = SavePurchaseOrderRepo.Get_last_insert();
                        SelectedItem.purchase_order_detail_pk = purchase_order_detail_pk_result.Result;
                        SavePurchaseOrderRepo.Commit();
                        SavePurchaseOrderRepo.Dispose();
                        MessageBox.Show("1 Row Inserted, POD_PK " + SelectedItem.purchase_order_detail_pk.ToString());
                        RaisePropertyChanged("ObservableCollectionPurchaseOrderModel");
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

        public void GenerateCommand()
        {
            // select all without qty check and make sure product does not exist.

            dataBaseProvider = getprovider();
            DataBaseLayer.PurchaseOrderRepository ProductPurchaseOrderRepo = new DataBaseLayer.PurchaseOrderRepository(dataBaseProvider);
            try
            {
                var result = ProductPurchaseOrderRepo.Eligible_Products();
                purchase_order_products = new List<int>();
                purchase_order_products = result.Result.ToList();
                ProductPurchaseOrderRepo.Commit();
                ProductPurchaseOrderRepo.Dispose();
            }
            catch (Exception ex)
            {
                MessageBox.Show("An Error has occured" + ex.ToString());
                ProductPurchaseOrderRepo.Revert();
                ProductPurchaseOrderRepo.Dispose();
                return;
            }

            //for each purchase_order_pk, get info and insert into product


            if (purchase_order_products.Count == 0)
            {
                MessageBox.Show("No eligible purchase orders are available.");
            }
            else
            {
                ProductModel PM;
                foreach (var purchase_order_detail_pk in purchase_order_products)
                {
                    //select each purchase_order_detail_pk
                    try
                    {
                        DataBaseLayer.PurchaseOrderRepository PurchaseOrder = new DataBaseLayer.PurchaseOrderRepository(dataBaseProvider);
                        POM.Add(PurchaseOrder.GetAllWithPK(purchase_order_detail_pk).Result);
                        PurchaseOrder.Commit();
                        PurchaseOrder.Dispose();
                    }
                    catch (Exception er)
                    {
                        MessageBox.Show("An unexpected error has occured during selection." + er);
                        return;
                    }
                }
                int record_count = 0;
                foreach (var item in POM)
                {
                    int count = 0;
                    for (int x = 0; x < item.lot_quantity; x++)
                    {
                        record_count++;
                        count++;
                        PM = new ProductModel
                        {
                            product_name = item.lot_name + count.ToString(),
                            brand_name = null,
                            description = null,
                            quantity = 1,
                            cost = Math.Round((item.lot_cost + item.sales_tax + item.shipping_cost) / item.lot_quantity, 2),
                            sku = item.lot_number,
                            instock = 1,
                            condition = "Used",
                            listing_date = DateTime.MinValue,
                            listing_number = null,
                            listing_url = null,
                            listed_price = 0,
                            supplier_fk = item.supplier_fk
                        };

                        DataBaseLayer.ProductRepository ProductRepo = new DataBaseLayer.ProductRepository(dataBaseProvider);
                        try
                        {

                            //insert into product
                            Task<int> insert_product_pk = ProductRepo.InsertProduct(PM);

                            //insert into product_purchase_order
                            PM.product_fk = insert_product_pk.Result;
                            PM.supplier_fk = item.supplier_fk;
                            PM.purchase_order_detail_fk = item.purchase_order_detail_pk;
                            Task<int> insert_product_purchase_order = ProductRepo.Insert(PM);
                            if (insert_product_purchase_order.Result == 0) throw new Exception();
                            ProductRepo.Commit();
                            ProductRepo.Dispose();
                        }
                        catch (Exception err)
                        {
                            MessageBox.Show("An Unexpected Error has occured:" + err);
                            ProductRepo.Revert();
                            ProductRepo.Dispose();
                            return;
                        }
                    }
                }

                MessageBox.Show("Completed " + record_count.ToString() + " Records.  Check Product Page.");
            }
        }

        public void LoadPurchaseOrder(int supplier_fk)
        {

            ObservableCollectionPurchaseOrderModel = new ObservableCollection<PurchaseOrderModel>();

            //clear and reload invoicedate and purchasedate based on supplier
            if (InvoiceNumberList.Count > 0 && PurchaseDateList.Count > 0)
            {
                InvoiceNumberList.Clear();
                PurchaseDateList.Clear();
            }

            dataBaseProvider = getprovider();
            DataBaseLayer.PurchaseOrderRepository PurchaseOrderRepo = new DataBaseLayer.PurchaseOrderRepository(dataBaseProvider);


            if (supplier_fk == 0)
            {
                ObservableCollectionPurchaseOrderModel = PurchaseOrderRepo.GetAll().Result.ToObservable();
                LoadFilterLists(ObservableCollectionPurchaseOrderModel);
                ObservableCollectionTotalPurchaseOrderModel = PurchaseOrderRepo.GetAllTotal(supplier_fk).Result.ToObservable();
            }
            else
            {
                var Result = PurchaseOrderRepo.GetAllWithID(supplier_fk).Result;
                if (Result.Count() > 0)
                {
                    ObservableCollectionPurchaseOrderModel = Result.ToObservable();
                    LoadFilterLists(ObservableCollectionPurchaseOrderModel);
                    ObservableCollectionTotalPurchaseOrderModel.Clear();
                    ObservableCollectionTotalPurchaseOrderModel = PurchaseOrderRepo.GetAllTotal(supplier_fk).Result.ToObservable();
                }
            }
            RaisePropertyChanged("ObservableCollectionPurchaseOrderModel");
            RaisePropertyChanged("ObservableCollectionTotalPurchaseOrderModel");
            PurchaseOrderRepo.Commit();
            PurchaseOrderRepo.Dispose();




            /*
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
            */
        }

        public void LoadFilterLists(ObservableCollection<PurchaseOrderModel> ObservableCollectionPurchaseOrderModel)
        {
            if (FilterInvoiceNumber != null || FilterPurchaseDate != null)
            {
                FilterInvoiceNumber.Clear();
                FilterPurchaseDate.Clear();
            }


            foreach (var item in ObservableCollectionPurchaseOrderModel)
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
            RaisePropertyChanged("FilterInvoiceNumber");
            RaisePropertyChanged("FilterPurchaseDate");

            if (ObservableCollectionPurchaseOrderModel is null)
            {
                throw new ArgumentNullException(nameof(ObservableCollectionPurchaseOrderModel));
            }
        }

        public void initial_load()
        {
            //Initial Page Load
            InvoiceNumberList = new List<string>();
            PurchaseDateList = new List<DateTime>();

            // Get All Purchase Order
            ObservableCollectionPurchaseOrderModel = new ObservableCollection<PurchaseOrderModel>();
            ObservableCollectionTotalPurchaseOrderModel = new ObservableCollection<PurchaseOrderModel>();

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
                    shipping_cost = 0,
                    quantity_check = 0
                });
            }
            ObservableCollectionPurchaseOrderModel = PurchaseOrderRepo.GetAll().Result.ToObservable();
            ObservableCollectionTotalPurchaseOrderModel = PurchaseOrderRepo.GetAllTotal(supplier_fk).Result.ToObservable();
            cvs.Source = ObservableCollectionPurchaseOrderModel;
            PurchaseOrderRepo.Commit();
            PurchaseOrderRepo.Dispose();
            RaisePropertyChanged("ObservableCollectionPurchaseOrderModel");

            LoadFilterLists(ObservableCollectionPurchaseOrderModel);

            // Get Suppliers
            ObservableCollectionSupplierModel = new ObservableCollection<SupplierModel>();
            dataBaseProvider = getprovider();
            DataBaseLayer.SupplierRepository SupplierRepo = new DataBaseLayer.SupplierRepository(dataBaseProvider);
            ObservableCollectionSupplierModel = SupplierRepo.GetAll().Result.ToObservable();
            SupplierRepo.Commit();
            SupplierRepo.Dispose();
        }


        #region pageload
        public PurchaseOrderViewModel()
        {
            initial_load();
        }

    }
    #endregion
}