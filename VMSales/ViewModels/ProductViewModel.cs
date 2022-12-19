using Caliburn.Micro;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Runtime.CompilerServices;
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
        public ObservableCollection<SupplierModel> ObservableCollectionSupplierModel { get; set; }
        private ObservableCollection<ProductModel> _ObservableCollectionProductModel;
    
        public ObservableCollection<ProductModel> ObservableCollectionProductModel
        {
            get { return _ObservableCollectionProductModel = _ObservableCollectionProductModel ?? new ObservableCollection<ProductModel>(); }
            set 
            {
                _ObservableCollectionProductModel = value;
                RaisePropertyChanged("ObservableCollectionProductModel");
            }
        }
        private ObservableCollection<PurchaseOrderModel> _ObservableCollectionPurchaseOrderModel;
        public ObservableCollection<PurchaseOrderModel> ObservableCollectionPurchaseOrderModel
        { 
        get { return _ObservableCollectionPurchaseOrderModel = _ObservableCollectionPurchaseOrderModel ?? new ObservableCollection<PurchaseOrderModel>(); }
            set
            {
                _ObservableCollectionPurchaseOrderModel = value;
                RaisePropertyChanged("ObservableCollectionPurchaseOrderModel");
            }
        }



        #endregion
        #region Members
        private int _supplier_fk;
        public int supplier_fk
        {
            get { return _supplier_fk; }
            set 
            {
                _supplier_fk = value;
                RaisePropertyChanged("supplier_fk");
                LoadPurchaseOrder(supplier_fk);     
            }
        }

        private int _purchase_order_detail_pk;
        public int purchase_order_detail_pk
        {
            get { return _purchase_order_detail_pk; }
            set
            {
                _purchase_order_detail_pk = value;
                RaisePropertyChanged("purchase_order_detail_pk");
            }
        }




        private ProductModel _selectedrow;
        public ProductModel selectedrow
        {
            get => this._selectedrow;
            set {
                this._selectedrow = value;
                RaisePropertyChanged("selectedrow");
            }
        }    
  
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
                dataBaseProvider = getprovider();
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
            ObservableCollection<CategoryModel> ObservableCollectionCategoryModel = new ObservableCollection<CategoryModel>();         
            DataBaseLayer.CategoryRepository CategoryRepo = new DataBaseLayer.CategoryRepository(dataBaseProvider);
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
             
                // if category dictionary is empty add.
                if (selectedrow.category_dictionary.Count < 1)
                {
                    foreach (var item in ObservableCollectionCategoryModel)
                    {
                        selectedrow.category_dictionary.Add(item.category_pk, item.category_name);
                    }
                }
                if (ObservableCollectionProductModel is null)
                {
                    ObservableCollectionProductModel = new ObservableCollection<ProductModel>();
                }

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
            dataBaseProvider = getprovider();

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

            ObservableCollectionPurchaseOrderModel.Clear();
            ObservableCollectionPurchaseOrderModel = new ObservableCollection<PurchaseOrderModel>();

            DataBaseLayer.PurchaseOrderRepository PurchaseOrderRepo = new DataBaseLayer.PurchaseOrderRepository(dataBaseProvider);
            ObservableCollectionPurchaseOrderModel = PurchaseOrderRepo.GetAllWithID(supplier_fk).Result.ToObservable();
            PurchaseOrderRepo.Commit();
            PurchaseOrderRepo.Dispose();

            foreach (var item in ObservableCollectionPurchaseOrderModel)
                {
                    selectedrow.purchase_order_detail_fk = item.purchase_order_detail_pk;
                    if (selectedrow.purchase_order_detail_fk == 0)
                    { MessageBox.Show("Warning: Purchase Order Key Not found."); }
                }
            }
         
        public ProductViewModel()
        {
            selectedrow = new ProductModel();
            selectedrow.category_dictionary = new Dictionary<int, string>();
            // Get Suppliers
            ObservableCollectionSupplierModel = new ObservableCollection<SupplierModel>();
            dataBaseProvider = getprovider();
            DataBaseLayer.SupplierRepository SupplierRepo = new DataBaseLayer.SupplierRepository(dataBaseProvider);
            ObservableCollectionSupplierModel = SupplierRepo.GetAll().Result.ToObservable();
            SupplierRepo.Commit();
            SupplierRepo.Dispose();

            if (ObservableCollectionSupplierModel.Count == 0)
            {
                MessageBox.Show("Please add a supplier first");
                return;
            }

            // Load Purchase Order
            ObservableCollectionPurchaseOrderModel = new ObservableCollection<PurchaseOrderModel>();
            DataBaseLayer.PurchaseOrderRepository PurchaseOrderRepo = new DataBaseLayer.PurchaseOrderRepository(dataBaseProvider);
            ObservableCollectionPurchaseOrderModel = PurchaseOrderRepo.GetAll().Result.ToObservable();
            if (ObservableCollectionPurchaseOrderModel.Count() == 0)
            {
                MessageBox.Show("You must add purchase orders.");
                PurchaseOrderRepo.Revert();
                PurchaseOrderRepo.Dispose();
                return;
            }
            else
            {
                PurchaseOrderRepo.Commit();
                PurchaseOrderRepo.Dispose();

                // Load Products
                DataBaseLayer.ProductRepository ProductRepo = new DataBaseLayer.ProductRepository(dataBaseProvider);
                ObservableCollectionProductModel = ProductRepo.GetAll().Result.ToObservable();

                // set category and condition
                foreach (var item in ObservableCollectionProductModel)
                {
                    selectedrow.category_name = item.category_name;
                    selectedrow.condition = item.condition;
         

                    //selectedrow.category_dictionary.Add(item.category_pk, item.category_name);
                }


                ProductRepo.Commit();
                ProductRepo.Dispose();
            } 
        }
    }
}