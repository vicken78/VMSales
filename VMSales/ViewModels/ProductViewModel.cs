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
        public string selected_category_pk { get; set; }
   
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
       


        }
        public void AddCommand()
        {
            ObservableCollection<CategoryModel> ObservableCollectionCategoryModel = new ObservableCollection<CategoryModel>();         DataBaseLayer.CategoryRepository CategoryRepo = new DataBaseLayer.CategoryRepository(dataBaseProvider);
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
                ObservableCollectionCategoryModel = catResult.ToObservable();
                ProductModel ProductModel = new ProductModel();

                ProductModel.category_pk = new List<int>();
                ProductModel.category_name = new List<string>();

                foreach (var item in ObservableCollectionCategoryModel)
                {
                    ProductModel.category_pk.Add(item.category_pk);
                    ProductModel.category_name.Add(item.category_name);
                }

                ObservableCollectionProductModel = new ObservableCollection<ProductModel>();

                ProductModel.condition = new List<String> { "New", "Used" };
                ProductModel.brand_name = null;
                ProductModel.product_name = null;
                ProductModel.description = null;
                ProductModel.quantity = 0;
                ProductModel.cost = 0;
                ProductModel.sku = "0";
                ProductModel.sold_price = -1;
                ProductModel.instock = 1;
                ProductModel.listing_url = null;
                ProductModel.listing_number = null;
                ProductModel.listing_date = DateTime.MinValue;

                ObservableCollectionProductModel.Add(ProductModel);
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
            }
            RaisePropertyChanged("ObservableCollectionPurchaseOrderModel");
            PurchaseOrderRepo.Commit();
            PurchaseOrderRepo.Dispose();
        }


        public ProductViewModel()
        {
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

    