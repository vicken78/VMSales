using Caliburn.Micro;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Threading.Tasks;
using System.Windows;
using VMSales.Logic;
using VMSales.Models;

namespace VMSales.ViewModels
{
    public class ProductViewModel : BaseViewModel
    {
        public List<string> categorylist { get; set; }

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

               private string _selected_category { get; set; }
               public string selected_category
               {
                   get { return _selected_category; }
                   set
                   {
                       if (_selected_category == value) return;
                       _selected_category = value;
                       RaisePropertyChanged("selected_category");
                   }
               }
        




        private ProductModel _productmodel { get; set; }
        public ProductModel Productmodel
        {
            get => this._productmodel;
            set
            {
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
            //if (supplier_fk == 0)
            //{
            //    MessageBox.Show("Please select a Supplier.");
            //    return;
            //}
            //   if (Productmodel.category_name is null)
            //  {
            //      MessageBox.Show("Please select a Category.");
            //      return;
            //  }
            //  if (purchase_order_detail_pk == 0)
            //  {
            //      MessageBox.Show("Please select a Lot Number.");
            //      return;
            //  }

        /*    foreach (var item in BindableCollectionProductModel)
            {
                item.category_pk = Productmodel.category_pk;
                item.category_list = Productmodel.category_list;
                item.product_category_pk = Productmodel.product_category_pk;
                item.product_pk = Productmodel.product_pk;
                item.product_pk = Productmodel.product_fk;
                item.brand_name = Productmodel.brand_name;
                item.product_name = Productmodel.product_name;
                item.description = Productmodel.description;
                item.quantity = Productmodel.quantity;
                item.sku = Productmodel.sku;
                item.sold_price = Productmodel.sold_price;
                item.instock = Productmodel.instock;
                item.listing_date = Productmodel.listing_date;
                item.listing_url = Productmodel.listing_url;
                item.listing_number = Productmodel.listing_number;
                item.cost = Productmodel.cost;
                item.condition = Productmodel.condition;
                item.category_name = Productmodel.category_name;
            }
        */
            //Insert or Update
            MessageBox.Show(Productmodel.product_pk.ToString());


            //product key present?
            /*
            MessageBox.Show("supplier_fk" + supplier_fk.ToString());
            MessageBox.Show("purchase_order_detail_pk" + purchase_order_detail_pk.ToString());
       
            MessageBox.Show("prod cat_pk" + Productmodel.product_category_pk.ToString());
            MessageBox.Show("cat_pk" + Productmodel.category_pk.ToString());
            //MessageBox.Show("catlist"+Productmodel.category_list.ToString());
            MessageBox.Show("product_pk" + Productmodel.product_pk.ToString());
            MessageBox.Show("product_fk" + Productmodel.product_fk.ToString());
            MessageBox.Show("brandname" + Productmodel.brand_name);
            MessageBox.Show("prodname" + Productmodel.product_name);
            MessageBox.Show("desc" + Productmodel.description);
            MessageBox.Show("qty" + Productmodel.quantity.ToString());
            MessageBox.Show("sku" + Productmodel.sku);
            MessageBox.Show("soldprice" + Productmodel.sold_price.ToString());
            MessageBox.Show("instock" + Productmodel.instock.ToString());
            MessageBox.Show("listdate" + Productmodel.listing_date.ToString());
            MessageBox.Show("listurl" + Productmodel.listing_url);
            MessageBox.Show("listnum" + Productmodel.listing_number);
            MessageBox.Show("cost" + Productmodel.cost.ToString());
            MessageBox.Show("condition" + Productmodel.condition);
            MessageBox.Show("catname" + Productmodel.category_name);
            */
            




            /*      try
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
                  }*/
        }
        public void AddCommand()
        {
            
            if (BindableCollectionProductModel is null)
            {
                BindableCollectionProductModel = new BindableCollection<ProductModel>();
            }

          
            Productmodel.product_pk = 0;
            Productmodel.brand_name = null;
            Productmodel.product_name = null;
            Productmodel.description = null;
            Productmodel.quantity = 0;
            Productmodel.cost = 0;
            Productmodel.sku = "0";
            Productmodel.sold_price = 0;
            Productmodel.condition = "New";
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
                }
                RaisePropertyChanged("BindableCollectionProductModel");
            }
            catch (Exception ex)
            {
                MessageBox.Show("A Database Error has occured. " + ex);
            }
        }


        public ProductViewModel()
        {
            dataBaseProvider = getprovider();

            //  Get Categories count
                     BindableCollectionCategoryModel = new BindableCollection<CategoryModel>();
                     DataBaseLayer.CategoryRepository CategoryRepo = new DataBaseLayer.CategoryRepository(dataBaseProvider);
                     var catcount = DataConversion.ToBindableCollection(CategoryRepo.GetAll().Result.ToObservable());
                   if (catcount.Count == 0)
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
                SupplierRepo.Revert();
                SupplierRepo.Dispose();
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
            ProductRepo.Commit();
            ProductRepo.Dispose();
            Productmodel = new ProductModel();
            foreach (var item in BindableCollectionProductModel)
            {
                Productmodel.product_pk = item.product_pk;
                selected_category = item.category_name;
            }
            // Load Category
            {
                try
                {
                    categorylist = new List<string>();
                    BindableCollectionCategoryModel = new BindableCollection<CategoryModel>();
                    DataBaseLayer.CategoryRepository CategoryRepos = new DataBaseLayer.CategoryRepository(dataBaseProvider);
                    BindableCollectionCategoryModel = DataConversion.ToBindableCollection(CategoryRepos.Get_Product_Category(Productmodel.product_pk).Result.ToObservable());
        
                    CategoryRepos.Commit();
                    CategoryRepos.Dispose();
                    foreach (var item in BindableCollectionCategoryModel)
                    {
                        categorylist.Add(item.category_name);
                    }
                }
                catch (Exception ext)
                {
                    MessageBox.Show(ext.ToString());
                }
  
            }

        }
           
    }
}
    