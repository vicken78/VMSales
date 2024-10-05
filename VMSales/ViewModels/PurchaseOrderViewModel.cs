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
using System.Diagnostics;
using VMSales.Views;
using System.ComponentModel;

namespace VMSales.ViewModels
{
    public class PurchaseOrderViewModel : BaseViewModel
    {
        public ICollectionView PurchaseOrderView { get; set; }
        private PurchaseOrderModel _SelectedItem;
        public PurchaseOrderModel SelectedItem
        {
            get => _SelectedItem;
            set
            {
                if (_SelectedItem != value)
                {
                    _SelectedItem = value;
                    NotifyOfPropertyChange(() => SelectedItem);
                }
            }
         }


        public ObservableCollection<SupplierModel> ObservableCollectionSupplierModel { get; set; }

        private ObservableCollection<PurchaseOrderModel> _ObservableCollectionPurchaseOrderModelDirty { get; set; }
        public ObservableCollection<PurchaseOrderModel> ObservableCollectionPurchaseOrderModelDirty
        {
            get => _ObservableCollectionPurchaseOrderModelDirty;
            set
            {
                _ObservableCollectionPurchaseOrderModelDirty = value;
                NotifyOfPropertyChange(() => ObservableCollectionPurchaseOrderModelDirty);
            }
        }
        public ObservableCollection<PurchaseOrderModel> ObservableCollectionPurchaseOrderModelClean { get; protected set; }

        IDatabaseProvider dataBaseProvider;

        private bool _keep_last;
        public bool keep_last
        {
            get => _keep_last;
            set
            {
                if (_keep_last != value)
                {
                    _keep_last = value;
                    NotifyOfPropertyChange(() => keep_last);  
                }
            }
        }

        #region Filters

        private bool _cancanremoveinvoicenumberfilter;
        public bool CanRemoveInvoiceNumberFilter
        {
            get => _cancanremoveinvoicenumberfilter;
            set
            {
                if (_cancanremoveinvoicenumberfilter != value)
                {
                    _cancanremoveinvoicenumberfilter = value;
                    NotifyOfPropertyChange(() => CanRemoveInvoiceNumberFilter);
                }
            }
        }

        private bool _cancanremovepurchasedatefilter;
        public bool CanRemovePurchaseDateFilter
        {
            get => _cancanremovepurchasedatefilter; 
            set
            {
                if (_cancanremovepurchasedatefilter != value)
                {
                    _cancanremovepurchasedatefilter = value;
                    NotifyOfPropertyChange(() => CanRemovePurchaseDateFilter);
                }
            }
        }

        private bool _cancanremovesupplierfilter;
        public bool CanRemoveSupplierFilter
        {
            get => _cancanremovesupplierfilter; 
            set
            {
                if (_cancanremovesupplierfilter != value)
                {
                    _cancanremovesupplierfilter = value;
                    NotifyOfPropertyChange(() => CanRemoveSupplierFilter);
                }
            }

        }

        private enum FilterField
        {
            InvoiceNumber,
            PurchaseDate,
            Supplier,
            None
        }
       
        private ObservableCollection<string> _FilterInvoiceNumber;
        public ObservableCollection<string> FilterInvoiceNumber
        {
            get => _FilterInvoiceNumber;
            set
            {
                if (_FilterInvoiceNumber != value)
                {
                    _FilterInvoiceNumber = value;
                    NotifyOfPropertyChange(() => FilterInvoiceNumber);
                }
            }
        }

        private ObservableCollection<DateTime> _FilterPurchaseDate;
        public ObservableCollection<DateTime> FilterPurchaseDate
        {
            get =>  _FilterPurchaseDate; 
            set
            {
                if (_FilterPurchaseDate != value) 
                {
                    _FilterPurchaseDate = value;
                    NotifyOfPropertyChange(() => FilterPurchaseDate);
                }
            }
        }


        private ObservableCollection<String> _FilterSupplier;
        public ObservableCollection<String> FilterSupplier
        {
            get => _FilterSupplier;
            set
            {
                if (_FilterSupplier != value)
                {
                    _FilterSupplier = value;
                    NotifyOfPropertyChange(() => FilterSupplier);
                }
            }
        }

        private string _SelectedInvoiceNumber;
        public string  SelectedInvoiceNumber 
        { 
            get => _SelectedInvoiceNumber;
        set
            {
                if (_SelectedInvoiceNumber != value)
                {
                    _SelectedInvoiceNumber = value;
                    NotifyOfPropertyChange(() => SelectedInvoiceNumber);
                    ApplyFilter(!string.IsNullOrEmpty(SelectedInvoiceNumber) ? FilterField.InvoiceNumber : FilterField.None);
                    _cancanremoveinvoicenumberfilter = true;
                    NotifyOfPropertyChange(() => _cancanremoveinvoicenumberfilter);
                }
            }
        }

        private bool FilterByInvoiceNumber(object item)
        {
            if (item is PurchaseOrderModel purchaseOrder)
            {
                return string.IsNullOrEmpty(SelectedInvoiceNumber) ||
                purchaseOrder.invoice_number.Contains(SelectedInvoiceNumber);
            }
            return false;
        }

        private DateTime? _SelectedPurchaseDate;
        public DateTime? SelectedPurchaseDate
        { 
            get => _SelectedPurchaseDate;
            set
            {
                if (_SelectedPurchaseDate != value)
                {
                    _SelectedPurchaseDate = value;
                    NotifyOfPropertyChange(() => SelectedPurchaseDate);
                    ApplyFilter(!string.IsNullOrEmpty(SelectedPurchaseDate.ToString()) ? FilterField.PurchaseDate : FilterField.None);
                    _cancanremovepurchasedatefilter = true;
                    NotifyOfPropertyChange(() => _cancanremovepurchasedatefilter);

                }
            }
        }

        private string _SelectedSupplier;
        public string SelectedSupplier
        {
            get => _SelectedSupplier;
            set
            {
                if (_SelectedSupplier != value)
                {
                    _SelectedSupplier = value;
                    NotifyOfPropertyChange(() => SelectedSupplier);
                    ApplyFilter(!string.IsNullOrEmpty(SelectedSupplier) ? FilterField.Supplier : FilterField.None);
                }
            }
        }

        #endregion

        //Commands
        public async Task SaveCommand()
        {
            var dataProcessor = new DataProcessor<PurchaseOrderModel>();
        }

        public void RemoveInvoiceNumberFilterCommand()
        {
            SelectedInvoiceNumber = null;
            PurchaseOrderView.Filter = null;
            NotifyOfPropertyChange(() => SelectedInvoiceNumber);
            CanRemoveInvoiceNumberFilter = false;
            PurchaseOrderView.Refresh();
        }

        public void ResetCommand()
        {
            SelectedInvoiceNumber = null;
            SelectedPurchaseDate = null;
            NotifyOfPropertyChange(() => SelectedInvoiceNumber);
            PurchaseOrderView.Filter = null;
            CanRemoveInvoiceNumberFilter = false;
            CanRemovePurchaseDateFilter = false;
            CanRemoveSupplierFilter = false;
            PurchaseOrderView.Refresh();
            initial_load();
        }

        public void initial_load()
        {
            if (ObservableCollectionPurchaseOrderModelDirty is null)
            ObservableCollectionPurchaseOrderModelDirty = new ObservableCollection<PurchaseOrderModel>();
            ObservableCollectionPurchaseOrderModelClean = new ObservableCollection<PurchaseOrderModel>();

            dataBaseProvider = getprovider();
            PurchaseOrderRepository PurchaseOrderRepo = new PurchaseOrderRepository(dataBaseProvider);
            ObservableCollectionPurchaseOrderModelDirty = PurchaseOrderRepo.GetAll().Result.ToObservable();

            if (PurchaseOrderView == null)
            {
                var collectionViewSource = new CollectionViewSource { Source = ObservableCollectionPurchaseOrderModelDirty };
                PurchaseOrderView = collectionViewSource.View;
            }

            ObservableCollectionPurchaseOrderModelClean = new ObservableCollection<PurchaseOrderModel>(ObservableCollectionPurchaseOrderModelDirty.Select(item => new PurchaseOrderModel
            {
                // Copy properties from the item, or use a copy constructor if available
                invoice_number = item.invoice_number,
                purchase_date = item.purchase_date,
                purchase_order_detail_pk = item.purchase_order_detail_pk,
                purchase_order_fk = item.purchase_order_fk,
                purchase_order_pk = item.purchase_order_pk,
                sales_tax = item.sales_tax,
                shipping_cost = item.shipping_cost,
                supplier_name = item.supplier_name,
                supplier_fk = item.supplier_fk,
            }
            ));
            PurchaseOrderRepo.Commit();
            PurchaseOrderRepo.Dispose();

            
            if (ObservableCollectionPurchaseOrderModelDirty != null)
            {
                FilterPurchaseDate = new ObservableCollection<DateTime>(ObservableCollectionPurchaseOrderModelDirty.Select(p => p.purchase_date));

                FilterInvoiceNumber = new ObservableCollection<string>(ObservableCollectionPurchaseOrderModelDirty
            .Select(p => p.invoice_number)   // Select the invoice numbers
            .Distinct()                      // Remove duplicates
            .OrderBy(x => x));               // Sort 

                FilterPurchaseDate = new ObservableCollection<DateTime>(ObservableCollectionPurchaseOrderModelDirty
            .Select(p => p.purchase_date)
            .Distinct() 
            .OrderBy(x => x)); 
            }
            else
            {
                FilterInvoiceNumber = new ObservableCollection<string>();
                FilterPurchaseDate = new ObservableCollection<DateTime>();
                //FilterSupplierName = new ObservableCollection<string>();
            }
        }

        private void ApplyFilter(FilterField field)
        {
            switch (field)
            {
                case FilterField.InvoiceNumber:
                    AddInvoiceNumberFilter();
                    PurchaseOrderView.Filter = new Predicate<object>(x => ((PurchaseOrderModel)x).invoice_number.ToString() == SelectedInvoiceNumber);
                    break;
                case FilterField.PurchaseDate:
                    AddPurchaseDateFilter();
                    //FilterView.Filter = new Predicate<object>(x => ((PurchaseOrderModel)x).purchase_date.ToString() == SelectedPurchaseDate.ToString());
                    break;
                case FilterField.Supplier:
                    //AddSupplierFilter();
                    //PurchaseOrderView.Filter = new Predicate<object>(x => ((PurchaseOrderModel)x).supplier_name.ToString() == SelectedSupplier);
                    break;
                default:
                    break;
            }
        }

        #region filter methods
        public void AddInvoiceNumberFilter()
        {
            if (CanRemoveInvoiceNumberFilter)
            {
                PurchaseOrderView.Filter = null;
                //cvs.Filter -= new FilterEventHandler(FilterByInvoiceNumber);
                //cvs.Filter += new FilterEventHandler(FilterByInvoiceNumber);
            }
            else
            {
                //cvs.Filter += new FilterEventHandler(FilterByInvoiceNumber);
                CanRemoveInvoiceNumberFilter = true;
                NotifyOfPropertyChange(() => CanRemoveInvoiceNumberFilter);
            }
        }
        public void AddPurchaseDateFilter()
        {
            if (CanRemovePurchaseDateFilter)
            {
                //cvs.Filter -= new FilterEventHandler(FilterByPurchaseDate);
                //cvs.Filter += new FilterEventHandler(FilterByPurchaseDate);
            }
            else
            {
                //cvs.Filter += new FilterEventHandler(FilterByPurchaseDate);
                //CanRemovePurchaseDateFilter = true;
            }
        }
 /*
        public void AddSupplierFilter()
        {
            if (CanRemoveSupplierFilter)
            {
                cvs.Filter -= new FilterEventHandler(FilterBySupplier);
                cvs.Filter += new FilterEventHandler(FilterBySupplier);
            }
            else
            {
                cvs.Filter += new FilterEventHandler(FilterBySupplier);
                CanRemoveSupplierFilter = true;
            }
        }
 */
 

 /*       private void FilterByInvoiceNumber(object sender, FilterEventArgs e)
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
            else if (string.Compare(SelectedPurchaseDate.ToString(), src.purchase_date.ToString()) != 0)
                e.Accepted = false;
        }

        public void RemoveInvoiceNumberFilterCommand()
        {
            cvs.Filter -= new FilterEventHandler(FilterByInvoiceNumber);
            SelectedInvoiceNumber = null;
            //FilterView.Filter = null;
            CanRemoveInvoiceNumberFilter = false;
            ////RaisePropertyChanged("PurchaseOrderView");
 
        }
        public void RemovePurchaseDateFilterCommand()
        {
            cvs.Filter -= new FilterEventHandler(FilterByPurchaseDate);
            SelectedPurchaseDate = DateTime.MinValue;
            //FilterView.Filter = null;
            CanRemovePurchaseDateFilter = false;
            ////RaisePropertyChanged("PurchaseOrderView");
        }
 */
        #endregion



        public PurchaseOrderViewModel()
        {
            initial_load();
        }
    }
}

/*
 * OLD CODE
 * 
public class PurchaseOrderViewModel : BaseViewModel
{
    //private ObservableCollection<PurchaseOrderModel> POM = new ObservableCollection<PurchaseOrderModel>();
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
            ////RaisePropertyChanged("SelectedItem");
        }
    }

    #endregion


    #region Members
    private int _supplier_fk { get; set; }
    #endregion



    public int supplier_fk
    {
        get { return _supplier_fk; }
        set
        {
            if (_supplier_fk == value) return;
            _supplier_fk = value;
            ////RaisePropertyChanged("supplier_fk");
            LoadPurchaseOrder(supplier_fk);
            CanRemoveSupplierFilter = !CanRemoveSupplierFilter;
            // Remove filters for purchase date and invoice
            RemoveInvoiceNumberFilterCommand();
            RemovePurchaseDateFilterCommand();
            // ReLoad
            LoadFilterLists(ObservableCollectionPurchaseOrderModel);

        }
    }

    public List<string> InvoiceNumberList { get; set; }
    public List<DateTime> PurchaseDateList { get; set; }

    IDatabaseProvider dataBaseProvider;

    #region ButtonCommands
    // remove Supplier Filter
    public void RemoveSupplier()
    {
        supplier_fk = 0;
        ////RaisePropertyChanged("supplier_fk");
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
                ////RaisePropertyChanged("ObservableCollectionPurchaseModel");
                return;
            }
            if (SelectedItem.purchase_order_pk != 0 && SelectedItem.purchase_order_fk != 0 && SelectedItem.purchase_order_detail_pk != 0)
            {
                dataBaseProvider = getprovider();
                PurchaseOrderRepository PurchaseOrderRepo = new PurchaseOrderRepository(dataBaseProvider);
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

        //  Check for default values here. better checks later.
        if (SelectedItem.lot_name == "Name")
        {
            MessageBox.Show("Default Values must not be used.");
            SelectedItem = null;
            return;
        }

        if (supplier_fk.ToString() == null || supplier_fk == 0)
        {
            dataBaseProvider = getprovider();
            PurchaseOrderRepository PurchaseOrderRepo = new PurchaseOrderRepository(dataBaseProvider);

            // attempt to get supplier.
            try
            {

                Task<PurchaseOrderModel> get_supplier = PurchaseOrderRepo.get_supplier_fk(SelectedItem.purchase_order_pk);

                if (get_supplier.Result.supplier_fk != 0)
                {
                    PurchaseOrderRepo.Commit();
                    PurchaseOrderRepo.Dispose();
                    SelectedItem.supplier_fk = get_supplier.Result.supplier_fk;
                    ////RaisePropertyChanged("supplier_fk");
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

        // scenerio 1
        // same invoice number, UPDATE purchase_order_detail.

        dataBaseProvider = getprovider();
        PurchaseOrderRepository SavePurchaseOrderRepo = new PurchaseOrderRepository(dataBaseProvider);
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
                int insertPurchase_Order = SavePurchaseOrderRepo.Insert(SelectedItem).Result;


                // new purchase order_detail_pk must be assigned 


                if (insertPurchase_Order > 0)
                {
                    var purchase_order_detail_pk_result = SavePurchaseOrderRepo.Get_last_insert();
                    SelectedItem.purchase_order_detail_pk = purchase_order_detail_pk_result.Result;
                    SavePurchaseOrderRepo.Commit();
                    SavePurchaseOrderRepo.Dispose();
                    MessageBox.Show("1 Row Inserted: Lot Name " + SelectedItem.lot_name);
                    ////RaisePropertyChanged("ObservableCollectionPurchaseOrderModel");
                    SelectedItem = new PurchaseOrderModel();
                    return;
                }
            }
        }
        catch (Exception e)
        {
            MessageBox.Show("An Error has occured, no changes were made. Error:" + e);
            SavePurchaseOrderRepo.Revert();
            SavePurchaseOrderRepo.Dispose();
            return;
        }

        #endregion
    }


    // generate product from purchase order
    public void GenerateCommand()
    {
        // select all without qty check and make sure product does not exist.

        dataBaseProvider = getprovider();
        PurchaseOrderRepository ProductPurchaseOrderRepo = new PurchaseOrderRepository(dataBaseProvider);
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
                    PurchaseOrderRepository PurchaseOrder = new PurchaseOrderRepository(dataBaseProvider);
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

                    ProductRepository ProductRepo = new ProductRepository(dataBaseProvider);
                    try
                    {

                        //insert into product
                        Task<int> insert_product_pk = ProductRepo.Insert(PM);

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
        PurchaseOrderRepository PurchaseOrderRepo = new PurchaseOrderRepository(dataBaseProvider);


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
        ////RaisePropertyChanged("ObservableCollectionPurchaseOrderModel");
        ////RaisePropertyChanged("ObservableCollectionTotalPurchaseOrderModel");
        PurchaseOrderRepo.Commit();
        PurchaseOrderRepo.Dispose();

    }

    public void LoadFilterLists(ObservableCollection<PurchaseOrderModel> ObservableCollectionPurchaseOrderModel)
    {
        // first clear
        if (FilterInvoiceNumber != null || FilterPurchaseDate != null)
        {
            cvs.Filter -= new FilterEventHandler(FilterByInvoiceNumber);
            cvs.Filter -= new FilterEventHandler(FilterByPurchaseDate);

            FilterInvoiceNumber.Clear();
            FilterPurchaseDate.Clear();
        }


        //then load

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
        //////RaisePropertyChanged("FilterInvoiceNumber");
        //////RaisePropertyChanged("FilterPurchaseDate");

        if (ObservableCollectionPurchaseOrderModel is null)
        {
            throw new ArgumentNullException(nameof(ObservableCollectionPurchaseOrderModel));
        }
    }

    public void initial_load()
    {
        try
        {
            //Initial Page Load

            InvoiceNumberList = new List<string>();
            PurchaseDateList = new List<DateTime>();

            // Get All Purchase Order
            ObservableCollectionPurchaseOrderModel = new ObservableCollection<PurchaseOrderModel>();
            ObservableCollectionTotalPurchaseOrderModel = new ObservableCollection<PurchaseOrderModel>();

            dataBaseProvider = getprovider();
            PurchaseOrderRepository PurchaseOrderRepo = new PurchaseOrderRepository(dataBaseProvider);
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
                    quantity_check = 0,
                    isproductinventory = false
                });
            }


        ObservableCollectionPurchaseOrderModel = PurchaseOrderRepo.GetAll().Result.ToObservable();
        ObservableCollectionTotalPurchaseOrderModel = PurchaseOrderRepo.GetAllTotal(supplier_fk).Result.ToObservable();
        cvs.Source = ObservableCollectionPurchaseOrderModel;
        PurchaseOrderRepo.Commit();
        PurchaseOrderRepo.Dispose();
        ////RaisePropertyChanged("ObservableCollectionPurchaseOrderModel");

        LoadFilterLists(ObservableCollectionPurchaseOrderModel);

        // Get Suppliers
        ObservableCollectionSupplierModel = new ObservableCollection<SupplierModel>();
        dataBaseProvider = getprovider();
        SupplierRepository SupplierRepo = new SupplierRepository(dataBaseProvider);
        ObservableCollectionSupplierModel = SupplierRepo.GetAll().Result.ToObservable();
        SupplierRepo.Commit();
        SupplierRepo.Dispose();
        }
        catch (Exception e)
        {
            MessageBox.Show(e.ToString());
        }
    }


    #region pageload
    public PurchaseOrderViewModel()
    {
    initial_load();
    }

}
#endregion
}

*/