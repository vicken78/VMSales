using System;
using System.Collections.ObjectModel;
using System.Data;
using System.Windows.Data;
using VMSales.Models;
using System.Linq;
using VMSales.Logic;
using System.Threading.Tasks;
using System.Windows;
using System.Diagnostics;

namespace VMSales.ViewModels
{
    public class PurchaseOrderViewModel : BaseViewModel
    {

        public string newinvoicenumber;
        public string newsuppliername;
        public DateTime? newpurchasedate;


        public CollectionViewSource PurchaseOrderView { get; set; }

        private int _RowCount;
        public int RowCount
        {
            get => _RowCount = PurchaseOrderView?.View?.Cast<object>().Count() ?? 0;

        }

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
                if (SelectedItem != null)
                {
                    var supplier_fk = SelectedItem.supplier_fk;
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
        private ObservableCollection<PurchaseOrderModel> _ObservableCollectionTotalPurchaseOrderModel;
        public ObservableCollection<PurchaseOrderModel> ObservableCollectionTotalPurchaseOrderModel 
        {
            get => _ObservableCollectionTotalPurchaseOrderModel;
            set
            {
                _ObservableCollectionTotalPurchaseOrderModel = value;
                NotifyOfPropertyChange(() => ObservableCollectionTotalPurchaseOrderModel);
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
            get => _FilterPurchaseDate;
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
        public string SelectedInvoiceNumber
        {
            get => _SelectedInvoiceNumber;
            set
            {
                if (_SelectedInvoiceNumber != value)
                {
                    _SelectedInvoiceNumber = value;
                    NotifyOfPropertyChange(() => SelectedInvoiceNumber);
                    ApplyFilter(!string.IsNullOrEmpty(SelectedInvoiceNumber) ? FilterField.InvoiceNumber : FilterField.None);
                    NotifyOfPropertyChange(() => RowCount);
                    _cancanremoveinvoicenumberfilter = true;
                    NotifyOfPropertyChange(() => _cancanremoveinvoicenumberfilter);
                }
            }
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
                    NotifyOfPropertyChange(() => RowCount);
                    _cancanremovepurchasedatefilter = true;
                    NotifyOfPropertyChange(() => _cancanremovepurchasedatefilter);
                }
            }
        }

        private int _SelectedSupplier;
        public int SelectedSupplier
        {
            get => _SelectedSupplier;
            set
            {
                if (_SelectedSupplier != value)
                {
                    _SelectedSupplier = value;
                    NotifyOfPropertyChange(() => SelectedSupplier);
                    ApplyFilter(!string.IsNullOrEmpty(SelectedSupplier.ToString()) ? FilterField.Supplier : FilterField.None);
                    NotifyOfPropertyChange(() => RowCount);
                    _cancanremovesupplierfilter = true;
                    NotifyOfPropertyChange(() => _cancanremovesupplierfilter);
                    UpdateTotal(SelectedSupplier);
                }
            }
        }

        #endregion

            public void UpdateTotal(int supplier_fk)
            {
            ObservableCollectionTotalPurchaseOrderModel.Clear();
            dataBaseProvider = getprovider();
            PurchaseOrderRepository PurchaseOrderRepo = new PurchaseOrderRepository(dataBaseProvider);
            ObservableCollectionTotalPurchaseOrderModel = PurchaseOrderRepo.GetAllTotal(supplier_fk).Result.ToObservable();
            PurchaseOrderRepo.Commit();
            PurchaseOrderRepo.Dispose();
            NotifyOfPropertyChange(() => ObservableCollectionTotalPurchaseOrderModel);
            }


        //Commands
        public async Task SaveCommand()
        {
            // Create an instance of DataProcessor with PurchaseOrderModel type
            var dataProcessor = new DataProcessor<PurchaseOrderModel>();
            // Call the Compare method
            ObservableCollection<PurchaseOrderModel> differences = dataProcessor.Compare(ObservableCollectionPurchaseOrderModelClean, ObservableCollectionPurchaseOrderModelDirty);
            foreach (var item in differences)
            {
                //PurchaseOrderRepository PurchaseOrderRepo = new PurchaseOrderRepository(dataBaseProvider);
                try
                {
                    switch (item.Action)
                    {
                        case "Update":
                            //temp 
                            Debug.WriteLine("Update");
                            break;
                        case "Insert":
                            break;
                        case "Delete":
                            break;
                        default:
                            throw new InvalidOperationException("Unexpected Action read, expected Update Insert or Delete.");
                    }
                }
                catch (Exception e)
                {
                    MessageBox.Show("An unexpected error has occured." + e);
                    //PurchaseOrderRepo.Revert();
                    //PurchaseOrderRepo.Dispose();
                }
                //PurchaseOrderRepo.Dispose();
                //initial_load();
            }
        }

            public void RemoveInvoiceNumberFilterCommand()
        {
            SelectedInvoiceNumber = null;
            NotifyOfPropertyChange(() => SelectedInvoiceNumber);
            CanRemoveInvoiceNumberFilter = false;
            PurchaseOrderView.Filter -= new FilterEventHandler(FilterByInvoiceNumber);
            NotifyOfPropertyChange(() => RowCount);
        }
        public void RemovePurchaseDateFilterCommand()
        {
            SelectedPurchaseDate = null;
            NotifyOfPropertyChange(() => SelectedPurchaseDate);
            CanRemovePurchaseDateFilter = false;
            PurchaseOrderView.Filter -= new FilterEventHandler(FilterByPurchaseDate);
            NotifyOfPropertyChange(() => RowCount);
        }
        public void RemoveSupplierFilterCommand()
        {
            SelectedSupplier = 0;
            NotifyOfPropertyChange(() => SelectedSupplier);
            CanRemoveSupplierFilter = false;
            PurchaseOrderView.Filter -= new FilterEventHandler(FilterBySupplier);
            NotifyOfPropertyChange(() => RowCount);
            UpdateTotal(0);
        }

        public void ResetCommand()
        {

            if (keep_last == false)
            {
            newinvoicenumber = null;
            newsuppliername = null;
            newpurchasedate = null;
            }

            PurchaseOrderView.Filter -= new FilterEventHandler(FilterByInvoiceNumber);
            PurchaseOrderView.Filter -= new FilterEventHandler(FilterByPurchaseDate);
            PurchaseOrderView.Filter -= new FilterEventHandler(FilterBySupplier);

            SelectedInvoiceNumber = null;
            SelectedPurchaseDate = null;
            SelectedSupplier = 0;
            
            NotifyOfPropertyChange(() => SelectedInvoiceNumber);
            NotifyOfPropertyChange(() => SelectedPurchaseDate);
            NotifyOfPropertyChange(() => SelectedSupplier);

            CanRemoveInvoiceNumberFilter = false;
            CanRemovePurchaseDateFilter = false;
            CanRemoveSupplierFilter = false;
            initial_load();
        }

        public void initial_load()
        {
            if (ObservableCollectionPurchaseOrderModelDirty is null)
            {
                ObservableCollectionPurchaseOrderModelDirty = new ObservableCollection<PurchaseOrderModel>();
                ObservableCollectionPurchaseOrderModelClean = new ObservableCollection<PurchaseOrderModel>();
                ObservableCollectionSupplierModel = new ObservableCollection<SupplierModel>();
                ObservableCollectionTotalPurchaseOrderModel = new ObservableCollection<PurchaseOrderModel>();

            }
            else
            {
                ObservableCollectionPurchaseOrderModelDirty.Clear();
                ObservableCollectionPurchaseOrderModelClean.Clear();
                ObservableCollectionSupplierModel.Clear();
            }

            dataBaseProvider = getprovider();
            PurchaseOrderRepository PurchaseOrderRepo = new PurchaseOrderRepository(dataBaseProvider);
            var result = PurchaseOrderRepo.GetAll().Result.ToObservable();

            ObservableCollectionTotalPurchaseOrderModel = PurchaseOrderRepo.GetAllTotal(0).Result.ToObservable();

            PurchaseOrderRepo.Commit();
            PurchaseOrderRepo.Dispose();

            foreach (var item in result)
            {
                ObservableCollectionPurchaseOrderModelDirty.Add(item);
            }

         

            // get supplier_name and supplier_fk
            dataBaseProvider = getprovider();
            SupplierRepository SupplierRepo = new SupplierRepository(dataBaseProvider);
            var suppliersresult = SupplierRepo.GetAll().Result.ToObservable();

            foreach (var item in suppliersresult) 
            { 
                ObservableCollectionSupplierModel.Add(item);
            }


            SupplierRepo.Commit();
            SupplierRepo.Dispose();


            // initialize the view

            if (PurchaseOrderView == null)
            {
                // Initialize PurchaseOrderView and set its source
                PurchaseOrderView = new CollectionViewSource { Source = ObservableCollectionPurchaseOrderModelDirty };

                // Subscribe to filter event after initializing
                PurchaseOrderView.Filter += (s, e) =>
                {
                    NotifyOfPropertyChange(() => RowCount);  // Update row count when filter changes
                };
            }
            else
            {
                // If PurchaseOrderView already exists, just reset the source
                PurchaseOrderView.Source = ObservableCollectionPurchaseOrderModelDirty;

                // If filter logic is already in place, you can ensure the Filter event is handled here too
                PurchaseOrderView.Filter += (s, e) =>
                {
                    NotifyOfPropertyChange(() => RowCount);  // Update row count when filter changes
                };

                // Subscribe to the CollectionChanged event to handle changes in the collection
                PurchaseOrderView.View.CollectionChanged += (s, e) =>
                {
                    NotifyOfPropertyChange(() => RowCount);
                };
       }

            PurchaseOrderView.View.Refresh();
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
                supplier_fk = item.supplier_fk,
                supplier_name = item.supplier_name,
                lot_number = item.lot_number,
                lot_description = item.lot_description,
                lot_cost = item.lot_cost,
                lot_name = item.lot_name,
                lot_quantity = item.lot_quantity,
                quantity_check = item.quantity_check
            }
            ));
        
            if (ObservableCollectionPurchaseOrderModelDirty != null)
            {
 
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
                FilterSupplier = new ObservableCollection<string>();
            }
        }

        private void ApplyFilter(FilterField field)
        {
            switch (field)
            {
                case FilterField.InvoiceNumber:
                    AddInvoiceNumberFilter();
                    break;
                case FilterField.PurchaseDate:
                    AddPurchaseDateFilter();
                    break;
                case FilterField.Supplier:
                    AddSupplierFilter();
                    break;
                default:
                    break;
            }
        }

        private void FilterByInvoiceNumber(object sender, FilterEventArgs e)
        {
            var src = e.Item as PurchaseOrderModel;
            if (src == null)
                e.Accepted = false;
            else if (string.Compare(SelectedInvoiceNumber, src.invoice_number) != 0)
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

        private void FilterBySupplier(object sender, FilterEventArgs e)
        {
            var src = e.Item as PurchaseOrderModel;
            if (src == null)
                e.Accepted = false;
            else if (string.Compare(SelectedSupplier.ToString(), src.supplier_fk.ToString()) != 0)
                e.Accepted = false;
        }

        #region filter methods
        public void AddInvoiceNumberFilter()
        {
            if (CanRemoveInvoiceNumberFilter)
            {
                PurchaseOrderView.Filter -= new FilterEventHandler(FilterByInvoiceNumber);
            }
            else
            {
                PurchaseOrderView.Filter += new FilterEventHandler(FilterByInvoiceNumber);
                CanRemoveInvoiceNumberFilter = true;
                NotifyOfPropertyChange(() => CanRemoveInvoiceNumberFilter);
            }
        }
        public void AddPurchaseDateFilter()
        {
            if (CanRemovePurchaseDateFilter)
            {
                PurchaseOrderView.Filter -= new FilterEventHandler(FilterByPurchaseDate);
            }
            else
            {
                PurchaseOrderView.Filter += new FilterEventHandler(FilterByPurchaseDate);
                CanRemovePurchaseDateFilter = true;
                NotifyOfPropertyChange(() => CanRemovePurchaseDateFilter);
            }
        }

        public void AddSupplierFilter()
        {
            if (CanRemoveSupplierFilter)
            {
                PurchaseOrderView.Filter -= new FilterEventHandler(FilterBySupplier);
            }
            else
            {
                PurchaseOrderView.Filter += new FilterEventHandler(FilterBySupplier);
                CanRemoveSupplierFilter = true;
                NotifyOfPropertyChange(() => CanRemoveSupplierFilter);
            }
        }
 
        #endregion



        public PurchaseOrderViewModel()
        {
            initial_load();
        }
    }
}

/*
 * OLD CODE deleting as we go
 * 
public class PurchaseOrderViewModel : BaseViewModel
{
    //private ObservableCollection<PurchaseOrderModel> POM = new ObservableCollection<PurchaseOrderModel>();
    private List<int> purchase_order_products;
    private string invoicetemp;
    private DateTime purchase_datetemp;

    public void AddCommand()
    {


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


//generate product from purchase order

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

// end generate


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

*/