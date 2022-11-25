using System;
using System.Collections.ObjectModel;
using System.Data;
using VMSales.Database;
using VMSales.Models;
using System.Collections.Generic;
using System.ComponentModel;
using VMSales.ChangeTrack;
using System.Windows.Forms;
using System.Windows.Data;

namespace VMSales.ViewModels
{
    public class ProductViewModel : BaseViewModel
    {
        #region tracking
        public ChangeTracker<ProductModel> changetracker { get; set; }
        List<ProductModel> productListUpdate = new List<ProductModel>();
        List<ProductModel> productListCreate = new List<ProductModel>();
        List<ProductModel> productListDelete = new List<ProductModel>();
        #endregion

        // dropdown variables

        public List<PurchaseOrderModel> PurchaseOrder { get; set; }
        public List<string> supplierName { get; set; } = new List<string>();
        public List<string> lotNum { get; set; } = new List<string>();
        public List<string> supplierNameFilter { get; set; } = new List<string>();
        public List<string> categoryNameFilter { get; set; } = new List<string>();

        #region CollectionModels


        private ObservableCollection<ProductModel> ocproductview;

        public ObservableCollection<ProductModel> ocProductView
        {
            get { return ocproductview ?? (ocproductview = new ObservableCollection<ProductModel>()); }
        }

        private CollectionViewSource cvsProductView { get; set; } = new CollectionViewSource();
        public ICollectionView productView
        {
            get { return CollectionViewSource.GetDefaultView(ocProductView); }
        }
        #endregion

        #region Selected

        private string _selectedsuppliername;
        public string selectedSupplierName
        {
            get { return _selectedsuppliername; }
            set
            {
                _selectedsuppliername = value;
                RaisePropertyChanged("selectedSupplierName");
                MessageBox.Show(selectedSupplierName);

                // get FK
                //selectedSupplierName


           /*     List<PurchaseOrderModel> PurchaseOrder; 
                {


                };
           */


        // here is where we get lot and filter it.  

    }
}

        #endregion
        #region FilterMethods
        private bool _canremovecategoryfilter;
        private bool _canremovesupplierfilter;
        private enum FilterField
        {
            supplier,
            category,
            None
        }
        private void ApplyFilter(FilterField field)
        {
            switch (field)
            {
                case FilterField.category:
                    AddCategoryFilter();
                    //PurchaseOrderView.Filter = new Predicate<object>(x => ((PurchaseOrderModel)x).Purchasedate.ToString() == _selectedpurchasedate);
                    //RaisePropertyChanged("PurchaseOrderView");
                    break;
                case FilterField.supplier:
                    AddSupplierFilter();
                    //PurchaseOrderView.Filter = new Predicate<object>(x => ((PurchaseOrderModel)x).Invoicenumber.ToString() == _selectedinvoicenumber);
                    //RaisePropertyChanged("PurchaseOrderView");
                    break;
                default:
                    break;
            }
        }
        public bool canRemoveCategoryFilter
        {
            get { return _canremovecategoryfilter; }
            set
            {
                _canremovecategoryfilter = value;
                RaisePropertyChanged("canRemoveCategoryFilter");
            }
        }
        public bool canRemoveSupplierFilter
        {
            get { return _canremovesupplierfilter; }
            set
            {
                _canremovesupplierfilter = value;
                RaisePropertyChanged("canRemoveSupplierFilter");
            }
        }

        public void removeCategoryFilterCommand()
        {
            cvsProductView.Filter -= new FilterEventHandler(FilterByCategory);
            //SelectedInvoiceNumber = null;
            //ProductViewModel.Filter = null;
            canRemoveCategoryFilter = false;
            //RaisePropertyChanged("PurchaseOrderView");

        }
        public void removeSupplierFilterCommand()
        {
            cvsProductView.Filter -= new FilterEventHandler(FilterBySupplier);
            //SelectedPurchaseDate = null;
            //PurchaseOrderView.Filter = null;
            canRemoveSupplierFilter = false;
            //RaisePropertyChanged("PurchaseOrderView");
        }

        #endregion
        #region filterfunctions

        /* Notes on Adding Filters:
        *   Each filter is added by subscribing a filter method to the Filter event
        *   of the CVS.  Filters are applied in the order in which they were added. 
        *   To prevent adding filters mulitple times ( because we are using drop down lists
        *   in the view), the CanRemove***Filter flags are used to ensure each filter
        *   is added only once.  If a filter has been added, its corresponding CanRemove***Filter
        *   is set to true.       
        *   
        *   If a filter has been applied already and then they change their selection to another value 
        *   we need to undo the previous filter then apply the new one.
        *   This does not completey Reset the filter, just allows it to be changed to another filter value. 
        */

        public void AddCategoryFilter()
        {
            // see Notes on Adding Filters:
            if (canRemoveCategoryFilter)
            {
                cvsProductView.Filter -= new FilterEventHandler(FilterByCategory);
                cvsProductView.Filter += new FilterEventHandler(FilterByCategory);
                canRemoveCategoryFilter = false;
            }
            else
            {
                cvsProductView.Filter += new FilterEventHandler(FilterByCategory);
                canRemoveCategoryFilter = true;
            }
        }
        public void AddSupplierFilter()
        {
            // see Notes on Adding Filters:
            if (canRemoveSupplierFilter)
            {
                cvsProductView.Filter -= new FilterEventHandler(FilterBySupplier);
                cvsProductView.Filter += new FilterEventHandler(FilterBySupplier);
                canRemoveSupplierFilter = false;
            }
            else
            {
                cvsProductView.Filter += new FilterEventHandler(FilterBySupplier);
                canRemoveSupplierFilter = true;
            }
        }

        /* Notes on Filter Methods:
         * When using multiple filters, do not explicitly set anything to true.  Rather,
        * only hide things which do not match the filter criteria
        * by setting e.Accepted = false.  If you set e.Accept = true, if effectively
        * clears out any previous filters applied to it.  
        */
        // This is where purchase data is filtered.
        private void FilterByCategory(object sender, FilterEventArgs e)
        {
            var src = e.Item as CategoryModel;
            if (src == null)
                e.Accepted = false;
            //else if (string.Compare(SelectedInvoiceNumber, src.Invoicenumber.ToString()) != 0)
            //    e.Accepted = false;
        }
        private void FilterBySupplier(object sender, FilterEventArgs e)
        {
            var src = e.Item as SupplierModel;
            if (src == null)
                e.Accepted = false;
            //   else if (string.Compare(SelectedPurchaseDate, src.Purchasedate.ToString()) != 0)
            //       e.Accepted = false;
        }

        #endregion

        #region -ButtonCommands
        public void SaveCommand()
        {


            productListUpdate = changetracker.RowsUpdated;
            var kvp = new List<Tuple<string, string>>();

            foreach (ProductModel PO in productListUpdate)
            {
                if (PO.selectedCategory.ToString() != null || PO.selectedCondition.ToString() != null || PO.productName.ToString() != null || PO.productSKU.ToString() != null)
                {
                    kvp.Add(new Tuple<string, string>("category_name", PO.selectedCategory.ToString()));
                    kvp.Add(new Tuple<string, string>("brand_name", PO.productBrandName.ToString()));
                    kvp.Add(new Tuple<string, string>("product_name", PO.productName.ToString()));
                    kvp.Add(new Tuple<string, string>("description", PO.productDescription.ToString()));
                    kvp.Add(new Tuple<string, string>("qty", PO.productQuantity.ToString()));
                    kvp.Add(new Tuple<string, string>("cost", PO.productCost.ToString()));
                    kvp.Add(new Tuple<string, string>("sku", PO.productSKU.ToString()));
                    kvp.Add(new Tuple<string, string>("sold_price", PO.productSoldPrice.ToString()));
                    kvp.Add(new Tuple<string, string>("instock", PO.productStock.ToString()));
                    kvp.Add(new Tuple<string, string>("condition", PO.selectedCondition.ToString()));
                    kvp.Add(new Tuple<string, string>("listing_url", PO.productListingURL.ToString()));
                    kvp.Add(new Tuple<string, string>("listing_num", PO.productListingNumber.ToString()));
                    kvp.Add(new Tuple<string, string>("listing_date", PO.productListingDate.ToString()));
                    DataBaseOps.update("product", kvp, "", "");
                }

                MessageBox.Show(PO.selectedCategory.ToString());
                MessageBox.Show(PO.productName.ToString());
                MessageBox.Show(PO.productDescription.ToString());
                MessageBox.Show(PO.productSKU.ToString());
                MessageBox.Show(PO.productBrandName.ToString());
                MessageBox.Show(PO.selectedCondition.ToString());
                MessageBox.Show(PO.productQuantity.ToString());
                MessageBox.Show(PO.productCost.ToString());
                MessageBox.Show(PO.productSoldPrice.ToString());
                MessageBox.Show(PO.productStock.ToString());
                MessageBox.Show(PO.productListingNumber.ToString());
                MessageBox.Show(PO.productListingURL.ToString());
                MessageBox.Show(PO.productListingDate.ToString());
            
            }

        }
        public void AddCommand()
        {
        }
        public void DeleteCommand()
        {
        }
        public void ResetCommand()
        {
        }

        public void addRowCommand()
        {
            var obj = new ProductModel()
            {
                productCondition = new List<String> { "New", "Used" },
                categoryName = GetPurchaseOrders("category", "cname"),
                productBrandName = "",
                productName = "Name",
                productDescription = "Description",
                productQuantity = "0",
                productCost = 0,
                productSKU = "0",
                productSoldPrice = 0,
                productStock = 1,
                productListingURL = "",
                productListingNumber = "",
                productListingDate = DateTime.Now,
            };
            ocProductView.Add(obj);
        
    }
    #endregion

    public List<string> GetPurchaseOrders(string parameter, string stringtablename)
        {
            List<string> listName = new List<string>();

            // populate Supplier Filter DropDown
            DataTable myDataTable = DataBaseOps.SQLiteDataTableWithQuery(parameter);
            foreach (DataRow row in myDataTable.Rows)
            {
                //listName.Add(row[stringtablename].ToString());
            }

            return listName;
        }


        public ProductViewModel()
        {
            changetracker = new ChangeTracker<ProductModel>(ocProductView);

            // populate any existing products
            string productParameter = "product";
            DataTable productDataTable = DataBaseOps.SQLiteDataTableWithQuery(productParameter);

            if (productDataTable == null || productDataTable.Rows.Count < 1)
            {
                    var obj = new ProductModel()
                    {
                        productCondition = new List<String> { "New", "Used" },
                        selectedCondition = "New",
                        selectedCategory = "",
                        categoryName = GetPurchaseOrders("category", "cname"),
                        productBrandName = "",
                        productName = "Name",
                        productDescription = "Description",
                        productQuantity = "0",
                        productCost = 0,
                        productSKU = "0",
                        productSoldPrice = 0,
                        productStock = 1,
                        productListingURL = "",
                        productListingNumber = "",
                        productListingDate = DateTime.Now,
                    };
                    ocProductView.Add(obj);
            }


            foreach (DataRow row in productDataTable.Rows)
            {

                 // need to get category too
                var obj = new ProductModel()
                {
                    product_PK = (string)row["product_pk"].ToString(),
                    productBrandName = (string)row["bname"],
                    productName = (string)row["pname"],
                    productDescription = (string)row["description"],
                    productQuantity = (string)row["qty"],
                    productCost = (decimal)row["cost"],
                    productSKU = (string)row["sku"],
                    productSoldPrice = (decimal)row["soldprice"],
                    productStock = (int)row["instock"],
                    productCondition = new List<string> { (string)row["condition"] },
                    productListingURL = (string)row["listingurl"],
                    productListingNumber = (string)row["listingnum"],
                    productListingDate = (DateTime)row["listingdate"],
                };
                ocProductView.Add(obj);
            };

            /*
                     ocProductView.Add(new ProductModel()
                   {
                        productCondition = new List<String> { "New", "Used" },
                        categoryName = GetPurchaseOrders("category", "cname"),
                   }) ;
             */

            supplierName = GetPurchaseOrders("supplier", "sname");
            supplierNameFilter = GetPurchaseOrders("supplier", "sname");
            categoryNameFilter = GetPurchaseOrders("category", "cname");
            cvsProductView.Source = ocProductView;
            changetracker.StartTracking(ocProductView);
        }
    }
}