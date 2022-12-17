using Caliburn.Micro;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using VMSales.Logic;
using VMSales.Models;

namespace VMSales.ViewModels
{
    public class ProductViewModel : BaseViewModel
    {

    

        #region collections
        private ObservableCollection<SupplierModel> ObservableCollectionSupplierModelClean { get; set; }
        public ObservableCollection<SupplierModel> ObservableCollectionSupplierModel
        {
            get { return ObservableCollectionSupplierModelClean; }
            set { 
                ObservableCollectionSupplierModelClean = value;
                RaisePropertyChanged("ObservableCollectionSupplierModel");
                }
        }
        
        public ObservableCollection<PurchaseOrderModel> ObservableCollectionPurchaseOrderModel { get; set; }
        private ObservableCollection<ProductModel> ObservableCollectionProductModelclean { get; set; }
        public ObservableCollection<ProductModel> ObservableCollectionProductModel
        {
            get { return ObservableCollectionProductModelclean; }
            set
            {
                if (ObservableCollectionProductModelclean == value) return;
                ObservableCollectionProductModelclean = value;
                RaisePropertyChanged("ObservableCollectionProductModel");
            }
        }

        #endregion
        #region Members
        private int _supplier_fk { get; set; }
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

        private ProductModel _selectedrow = null;
        public ProductModel selectedrow { get => this._selectedrow; set { this._selectedrow = value; RaisePropertyChanged("selectedrow"); } }

        #endregion
        IDatabaseProvider dataBaseProvider;

        public void SaveCommand()
        {
            // check for null values
            if (selectedrow.category_fk == 0)
            {
                MessageBox.Show("Please select a Category.");
                return;
            }
            if (selectedrow.product_name is null)
            {
                MessageBox.Show("Please enter a product name.");
                return;
            }

            try
            {
                dataBaseProvider = BaseViewModel.getprovider();
                DataBaseLayer.ProductRepository ProductRepo = new DataBaseLayer.ProductRepository(dataBaseProvider);
                Task<bool> insertProduct = ProductRepo.Insert(selectedrow);
                if (insertProduct.Result == true)
                {
                    MessageBox.Show("1 Row Inserted.");
                    ProductRepo.Commit();
                    ProductRepo.Dispose();
                    return;
                }
                else
                {
                    ProductRepo.Revert();
                    ProductRepo.Dispose();
                    return;
                }
            }
            catch (Exception e)
            {
                MessageBox.Show("An Error has occured: " + e);
            }
        }
        public void AddCommand()
        {
            ObservableCollection<CategoryModel> ObservableCollectionCategoryModel = new ObservableCollection<CategoryModel>();         DataBaseLayer.CategoryRepository CategoryRepo = new DataBaseLayer.CategoryRepository(dataBaseProvider);
            var catResult = CategoryRepo.GetAll().Result;
            if (catResult.Count() == 0)
            {
                MessageBox.Show("You must add a category.");
                CategoryRepo.Revert();
                CategoryRepo.Dispose();
                return;
            }
            else
            {

                CategoryRepo.Commit();
                CategoryRepo.Dispose();
                ObservableCollectionCategoryModel = catResult.ToObservable();
                ObservableCollectionProductModel = new ObservableCollection<ProductModel>();

                selectedrow.category_dictionary = new Dictionary<int, String>();

                foreach (var item in ObservableCollectionCategoryModel)
                {
                    selectedrow.category_dictionary.Add(item.category_pk, item.category_name);
                }

                selectedrow.conditionlist = new List<String> { "New", "Used" };
                selectedrow.brand_name = null;
                selectedrow.product_name = null;
                selectedrow.description = null;
                selectedrow.quantity = 0;
                selectedrow.cost = 0;
                selectedrow.sku = "0";
                selectedrow.sold_price = -1;
                selectedrow.instock = 1;
                selectedrow.listing_url = null;
                selectedrow.listing_number = null;
                selectedrow.listing_date = DateTime.MinValue;

                ObservableCollectionProductModel.Add(selectedrow);
                RaisePropertyChanged("ObservableCollectionProductModel");
            }
        }
        public void DeleteCommand()
        {
        }
        public void ResetCommand()
        {
        }

        public void LoadPurchaseOrder(int supplier_fk)
        {
            dataBaseProvider = BaseViewModel.getprovider();

            // check for category

            DataBaseLayer.CategoryRepository CategoryRepo = new DataBaseLayer.CategoryRepository(dataBaseProvider);
            var catResult = CategoryRepo.GetAll().Result;
            if (catResult.Count() == 0)
            {
                MessageBox.Show("You must add categories.");
                CategoryRepo.Revert();
                CategoryRepo.Dispose();
                return;
            }
            else
            {
                CategoryRepo.Commit();
                CategoryRepo.Dispose();
            }

            ObservableCollectionPurchaseOrderModel = new ObservableCollection<PurchaseOrderModel>();
            DataBaseLayer.PurchaseOrderRepository PurchaseOrderRepo = new DataBaseLayer.PurchaseOrderRepository(dataBaseProvider);
            var purResult = PurchaseOrderRepo.GetAllWithID(supplier_fk).Result;

            if (purResult.Count() == 0)
            {
                MessageBox.Show("You must add purchase orders.");
                PurchaseOrderRepo.Revert();
                PurchaseOrderRepo.Dispose();
                return;
            }
            else
            {
                ObservableCollectionPurchaseOrderModel = purResult.ToObservable();
                foreach (var item in ObservableCollectionPurchaseOrderModel)
                {
                    selectedrow.purchase_order_detail_fk = item.purchase_order_detail_pk;
                    if (selectedrow.purchase_order_detail_fk == 0)
                    { MessageBox.Show("Warning: Purchase Order Key Not found."); }
                }
            }
            RaisePropertyChanged("ObservableCollectionPurchaseOrderModel");
            PurchaseOrderRepo.Commit();
            PurchaseOrderRepo.Dispose();
        }


        public ProductViewModel()
        {
            selectedrow = new ProductModel();
            // Get Suppliers
            ObservableCollectionSupplierModel = new ObservableCollection<SupplierModel>(); 
            dataBaseProvider = BaseViewModel.getprovider();
            DataBaseLayer.SupplierRepository SupplierRepo = new DataBaseLayer.SupplierRepository(dataBaseProvider);
            ObservableCollectionSupplierModel = SupplierRepo.GetAll().Result.ToObservable();
            SupplierRepo.Commit();
            SupplierRepo.Dispose();

            if (ObservableCollectionSupplierModel.Count == 0)
            {
                MessageBox.Show("Please add a supplier first");
                return;
            }
      
            // Load Products
            ObservableCollection <ProductModel> ObservableCollectionProductModel = new ObservableCollection<ProductModel>();
            //DataBaseLayer.ProductRepository PurchaseRepo = new DataBaseLayer.ProductRepository(dataBaseProvider);      
            //ObservableCollectionProductModel = PurchaseRepo.GetAll().Result.ToObservable();
            //PurchaseRepo.Commit();
            //PurchaseRepo.Dispose();
        }
    }
}

    