using Caliburn.Micro;
using System;
using System.Linq;
using System.Threading.Tasks;
using System.Windows;
using VMSales.Logic;
using VMSales.Models;

namespace VMSales.ViewModels
{

    public class ProductViewModel : BaseViewModel
    {
        public PurchaseOrderModel purchaseOrderModel { get; set; }
        #region collections   

        public BindableCollection<SupplierModel> BindableCollectionSupplierModel { get; set; }
        public BindableCollection<ProductModel> BindableCollectionProductModel { get; set; }
        public BindableCollection<CategoryModel> BindableCollectionCategoryModel { get; set; }
        public BindableCollection<PurchaseOrderModel> BindableCollectionPurchaseOrderModel { get; set; }
    
        #endregion
        #region Members
        private int _supplier_fk;
        public int supplier_fk
        {
            get { return _supplier_fk; }
            set 
            {
                if (_supplier_fk == value) return;
                _supplier_fk = value;
                RaisePropertyChanged("supplier_fk");
                LoadPurchaseOrder(supplier_fk);
                LoadProducts(supplier_fk, 0);
            }
        }

        private int _purchase_order_detail_pk;
        public int purchase_order_detail_pk
        {
            get { return _purchase_order_detail_pk; }
            set
            {
                if (_purchase_order_detail_pk == value) return;
                _purchase_order_detail_pk = value;
                RaisePropertyChanged("purchase_order_detail_pk");
                LoadProducts(supplier_fk, purchase_order_detail_pk);
            }
        }

        private ProductModel _productmodel { get; set; }
        public ProductModel Productmodel 
        {
            get => this._productmodel;
            set {
                if (this._productmodel == value) return;
                this._productmodel = value;
                RaisePropertyChanged("Productmodel");
            }
        }    
  
        #endregion
        IDatabaseProvider dataBaseProvider;

        public void SaveCommand()
        {
            // check for null values
            if (Productmodel.category_fk == 0)
            {
                MessageBox.Show("Please select a Category.");
                return;
            }
            if (Productmodel.product_name is null)
            {
                MessageBox.Show("Please enter a product name.");
                return;
            }

            try
            {
                dataBaseProvider = getprovider();
                DataBaseLayer.ProductRepository ProductRepo = new DataBaseLayer.ProductRepository(dataBaseProvider);
                Task<bool> insertProduct = ProductRepo.Insert(Productmodel);
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

            if (BindableCollectionProductModel is null)
                {
                    BindableCollectionProductModel = new BindableCollection<ProductModel>();
                }
                   
                Productmodel.brand_name = null;
                Productmodel.product_name = null;
                Productmodel.description = null;
                Productmodel.quantity = 0;
                Productmodel.cost = 0;
                Productmodel.sku = "0";
                Productmodel.sold_price = -1;
                Productmodel.instock = 1;
                Productmodel.listing_url = null;
                Productmodel.listing_number = null;
                Productmodel.listing_date = DateTime.MinValue;
                BindableCollectionProductModel.Add(Productmodel);
            
        }
        public void DeleteCommand()
        {
        }
        public void ResetCommand()
        {
     
        }

        public void LoadPurchaseOrder(int supplier_fk)
        {  
            BindableCollectionPurchaseOrderModel.Clear();
            BindableCollectionPurchaseOrderModel = new BindableCollection<PurchaseOrderModel>();
            dataBaseProvider = getprovider();
            DataBaseLayer.PurchaseOrderRepository PurchaseOrderRepo = new DataBaseLayer.PurchaseOrderRepository(dataBaseProvider);
            BindableCollectionPurchaseOrderModel = DataConversion.ToBindableCollection(PurchaseOrderRepo.GetAllWithID(supplier_fk).Result.ToObservable());
            RaisePropertyChanged("BindableCollectionPurchaseOrderModel");
            PurchaseOrderRepo.Commit();
            PurchaseOrderRepo.Dispose();
            PurchaseOrderModel Purchaseordermodel = new PurchaseOrderModel(); 

            foreach (var item in BindableCollectionPurchaseOrderModel)
                {
                Purchaseordermodel.lot_number = item.lot_number;
                Purchaseordermodel.purchase_order_detail_pk = item.purchase_order_detail_pk;
                Productmodel.purchase_order_detail_fk = item.purchase_order_detail_pk;
            }
        }

        public void LoadProducts(int supplier_fk, int purchase_order_detail_pk)
        {
            try
            {
                if (supplier_fk == 0) { return; }

                if (supplier_fk > 0 && purchase_order_detail_pk == 0)
                {
                    Productmodel = new ProductModel();
                    BindableCollectionProductModel.Clear();
                    BindableCollectionProductModel = new BindableCollection<ProductModel>();
                    DataBaseLayer.ProductRepository ProductRepo = new DataBaseLayer.ProductRepository(dataBaseProvider);
                    BindableCollectionProductModel = DataConversion.ToBindableCollection(ProductRepo.GetAllWithID(supplier_fk).Result.ToObservable());
                    ProductRepo.Commit();
                    ProductRepo.Dispose();
                    if (BindableCollectionProductModel.Count > 0)
                    {
                        foreach (var item in BindableCollectionProductModel)
                        {
                            Productmodel.category_name = item.category_name;
                        }
                        RaisePropertyChanged("BindableCollectionProductModel");
                        return;
                    }
                }
                else
                {
                    Productmodel = new ProductModel();
                    BindableCollectionProductModel.Clear();
                    RaisePropertyChanged("BindableCollectionProductModel");
                    BindableCollectionProductModel = new BindableCollection<ProductModel>();
                    DataBaseLayer.ProductRepository ProductRepo = new DataBaseLayer.ProductRepository(dataBaseProvider);
                    BindableCollectionProductModel = DataConversion.ToBindableCollection(ProductRepo.GetAllWithAllID(supplier_fk, purchase_order_detail_pk).Result.ToObservable());
                    ProductRepo.Commit();
                    ProductRepo.Dispose();
                    if (BindableCollectionProductModel.Count > 0)
                    {
                          foreach (var item in BindableCollectionProductModel)
                          {
                              Productmodel.category_name = item.category_name;
                          }
                          RaisePropertyChanged("BindableCollectionProductModel");
                          return;
                          }
                       }
                }
            catch (Exception ex)
                {
                MessageBox.Show("A Database Error has occured. " + ex);
                }
        }


        public ProductViewModel()
        {
            dataBaseProvider = getprovider();

            // Get Categories
            DataBaseLayer.CategoryRepository CategoryRepo = new DataBaseLayer.CategoryRepository(dataBaseProvider);
            BindableCollectionCategoryModel = DataConversion.ToBindableCollection(CategoryRepo.GetAll().Result.ToObservable());
            if (BindableCollectionCategoryModel.Count == 0)
            {
                CategoryRepo.Revert();
                CategoryRepo.Dispose();
                MessageBox.Show("You must add categories.");
                return;
            }
            else
            {
                CategoryRepo.Commit();
                CategoryRepo.Dispose();
            }

            // Get Suppliers
            BindableCollectionSupplierModel = new BindableCollection<SupplierModel>();
            DataBaseLayer.SupplierRepository SupplierRepo = new DataBaseLayer.SupplierRepository(dataBaseProvider);
            BindableCollectionSupplierModel = DataConversion.ToBindableCollection(SupplierRepo.GetAll().Result.ToObservable());
            if (BindableCollectionSupplierModel.Count == 0)
            {
                CategoryRepo.Revert();
                CategoryRepo.Dispose();
                MessageBox.Show("Please add a supplier first");
                return;
            }
            else
            {
                SupplierRepo.Commit();
                SupplierRepo.Dispose();
            }


            // Check for Purchase Order
            BindableCollectionPurchaseOrderModel = new BindableCollection<PurchaseOrderModel>();
            DataBaseLayer.PurchaseOrderRepository PurchaseOrderRepo = new DataBaseLayer.PurchaseOrderRepository(dataBaseProvider);
            BindableCollectionPurchaseOrderModel = DataConversion.ToBindableCollection(PurchaseOrderRepo.GetAll().Result.ToObservable());
            if (BindableCollectionPurchaseOrderModel.Count() == 0)
            {
                PurchaseOrderRepo.Revert();
                PurchaseOrderRepo.Dispose();
                MessageBox.Show("You must add purchase orders.");
                return;
            }
            else
            {
                PurchaseOrderRepo.Commit();
                PurchaseOrderRepo.Dispose();
            }

                // Load Products
                DataBaseLayer.ProductRepository ProductRepo = new DataBaseLayer.ProductRepository(dataBaseProvider);
                BindableCollectionProductModel = DataConversion.ToBindableCollection(ProductRepo.GetAll().Result.ToObservable());
            
                Productmodel = new ProductModel();
                foreach (var item in BindableCollectionProductModel)
                    {
                            Productmodel.category_name = item.category_name;
                    }
                ProductRepo.Commit();
                ProductRepo.Dispose();
             
            } 
        }
    }