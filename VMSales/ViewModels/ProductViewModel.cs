using System;
using System.Collections.ObjectModel;
using System.Data;
using System.Windows.Data;
using VMSales.Database;
using VMSales.Models;
using System.Linq;
using System.Collections.Generic;
using System.ComponentModel;
using VMSales.ChangeTrack;
using System.Windows;


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

        #region CollectionModels
        public List<string> supplierName { get; set; } = new List<string>();
        public List<string> categoryName { get; set; } = new List<string>();
        public ObservableCollection<ProductModel> ocProductView { get; set; } = new ObservableCollection<ProductModel>();
        private CollectionViewSource cvsProductView { get; set; } = new CollectionViewSource();
        public ICollectionView productView
        {
            get { return CollectionViewSource.GetDefaultView(ocProductView); }
        }
        #endregion

        #region Selected
        
        private string _selectedcategoryname;
        public string selectedCategoryName
        {
            get { return _selectedcategoryname; }
            set
            {
                  _selectedcategoryname = value;
                  RaisePropertyChanged("selectedCategoryName");
                  //MessageBox.Show(selectedCategoryName);
                  //LoadProduct(selectedCategoryName);
            }
        }


        private string _selectedsuppliername;
        //public SupplierModel selectedSupplierName
        public string selectedSupplierName
        {
            get { return _selectedsuppliername; }
            set
            {
                _selectedsuppliername = value;
                RaisePropertyChanged("selectedSupplierName");
                MessageBox.Show(selectedSupplierName);
                //SelectedSupplier_pk = _selectedsname.Supplier_pk.ToString();
                //LoadPurchaseOrder(SelectedSupplier_pk);
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

        #region ButtonCommands
        public void SaveCommand()
        {
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
        #endregion


        public ProductViewModel()
        {
            // populate Supplier Filter DropDown
            string supplierParameter = "supplier";
            DataTable supplierDataTable = DataBaseOps.SQLiteDataTableWithQuery(supplierParameter);
            foreach (DataRow row in supplierDataTable.Rows)
            {
                supplierName.Add((string)row["sname"]);
            }

            // populate Category Filter DropDown
            string categoryParameter = "category";
            DataTable categoryDataTable = DataBaseOps.SQLiteDataTableWithQuery(categoryParameter);
            foreach (DataRow row in categoryDataTable.Rows)
            {
                categoryName.Add((string)row["cname"]);
            }




            // populate any existing products
            string productParameter = "product";
            DataTable productDataTable = DataBaseOps.SQLiteDataTableWithQuery(productParameter);
                foreach (DataRow row in productDataTable.Rows)
                    {
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
                            productCondition = (string)row["condition"],
                            productListingURL = (string)row["listingurl"],
                            productListingNumber = (string)row["listingnum"],
                            productListingDate = (DateTime)row["listingdate"],
                         };
                    ocProductView.Add(obj);
                    };    
        }
    }
}