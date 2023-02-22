﻿using Caliburn.Micro;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Data;
using VMSales.Logic;
using VMSales.Models;

namespace VMSales.ViewModels
{
    public class ProductViewModel : BaseViewModel
    {
        #region Filters
        private bool _canRemoveSupplierFilter;
        public bool canRemoveSupplierFilter
        {
            get { return _canRemoveSupplierFilter; }
            set
            {
                _canRemoveSupplierFilter = value;
                RaisePropertyChanged("canRemoveSupplierFilter");
            }
        }
        private bool _canRemoveCategoryFilter;
        public bool canRemoveCategoryFilter
        {
            get { return _canRemoveCategoryFilter; }
            set
            {
                _canRemoveCategoryFilter = value;
                RaisePropertyChanged("canRemoveSupplierFilter");
            }
        }
        private enum FilterField
        {
            Category,
            Supplier,
            None
        }
        #endregion
        private string _selected_supplier_name { get; set; }
        public string selected_supplier_name
        {
            get { return _selected_supplier_name; }
            set
            {
                if (_selected_supplier_name == value) return;
                _selected_supplier_name = value;
                RaisePropertyChanged("selected_supplier_name");
            }
        }

        private SupplierModel _selected_supplier_name_filter { get; set; }
        public SupplierModel selected_supplier_name_filter
        {
            get { return _selected_supplier_name_filter; }
            set
            {
                if (_selected_supplier_name_filter == value) return;
                _selected_supplier_name_filter = value;
                RaisePropertyChanged("selected_supplier_name_filter");
                //filter product based on supplier name.
                ApplyFilter(!string.IsNullOrEmpty(selected_supplier_name_filter?.supplier_name) ? FilterField.Supplier : FilterField.None);
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

        private CategoryModel _selected_category_name_filter { get; set; }
        public CategoryModel selected_category_name_filter
        {
            get { return _selected_category_name_filter; }
            set
            {
                if (_selected_category_name_filter == value) return;
                _selected_category_name_filter = value;
                RaisePropertyChanged("selected_category_name_filter");
                //filter product based on category name.
                ApplyFilter(!string.IsNullOrEmpty(_selected_category_name_filter?.category_name) ? FilterField.Category : FilterField.None);
            }
        }

        #region collections   

        public BindableCollection<SupplierModel> BindableCollectionSupplierModel { get; set; }
        public BindableCollection<ProductModel> BindableCollectionProductModel { get; set; }
        public BindableCollection<CategoryModel> BindableCollectionCategoryModel { get; set; }
        public BindableCollection<PurchaseOrderModel> BindableCollectionPurchaseOrderModel { get; set; }

        #endregion
        #region Members
 
        public List<string> category_list { get; set; }

        private int _supplier_fk;
        public int supplier_fk
        {
            get { return _supplier_fk; }
            set
            {
                if (_supplier_fk == value) return;
                _supplier_fk = value;
                RaisePropertyChanged("supplier_fk");
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

        private ProductModel _SelectedItem { get; set; }
        public ProductModel SelectedItem
        {
            get { return _SelectedItem; }
            set
            {
                if (_SelectedItem == value) return;
                _SelectedItem = value;
                RaisePropertyChanged("SelectedItem");
                LoadSupplier();
            }
        }

        #endregion

     
        #region SupplierChange
        public void LoadSupplier()
        {
            int get_supplier_fk = 0;
            int get_product_purchase_order_detail_fk = 0;
            if (SelectedItem.product_pk != 0)
            {
                // Load Supplier
                DataBaseLayer.SupplierRepository  SupplierRepo = new DataBaseLayer.SupplierRepository(dataBaseProvider);
                selected_supplier_name = SupplierRepo.Selected_Supplier(SelectedItem.product_pk).Result.First().ToString();
                get_supplier_fk = SupplierRepo.Get_by_supplier_name(selected_supplier_name).Result;
                SupplierRepo.Commit();
                SupplierRepo.Dispose();
                // Check for Purchase_Product_Order
                try
                {
                    DataBaseLayer.ProductRepository ProductRepo = new DataBaseLayer.ProductRepository(dataBaseProvider);
                    get_product_purchase_order_detail_fk = ProductRepo.Get_product_purchase_order(SelectedItem.product_pk).Result;
                    ProductRepo.Commit();
                    ProductRepo.Dispose();
                }
                catch (Exception e)
                {
                    MessageBox.Show("An Error has occured:" + e);
                }
                finally
                {
                    LoadPurchaseOrder(get_supplier_fk, get_product_purchase_order_detail_fk);
                }
                // Load Purchase Order Lots
            }
        }

        #endregion
        IDatabaseProvider dataBaseProvider;

        #region FilterFunctions
        private void ApplyFilter(FilterField field)
        {
            switch (field)
            {
                case FilterField.Category:
                    AddCategoryFilter();
                    break;
                case FilterField.Supplier:
                    AddSupplierFilter();
                    break;
                case FilterField.None:
                    initial_load();
                    break;
                default:
                    MessageBox.Show("Error in FilterField");
                    break;
            }
        }

        public void RemoveCategoryFilterCommand()
        {
            canRemoveCategoryFilter = false;
        }
        public void RemoveSupplierFilterCommand()
        {
            canRemoveSupplierFilter = false;
            ApplyFilter(FilterField.None);
        }
        public void AddCategoryFilter()
        {
            if (canRemoveCategoryFilter)
            {
                  //cvs.Filter -= new FilterEventHandler(FilterByCategoryName);
                  //cvs.Filter += new FilterEventHandler(FilterByCategoryName);
            }
            else
            {
                //cvs.Filter += new FilterEventHandler(FilterByCategoryName);
                canRemoveCategoryFilter = true;
            }
        }
        public void AddSupplierFilter()
        {
            int selected_supplier_fk_filter = 0;
            int selected_category_fk_filter = 0;

            // Load Supplier
            try
            {
                dataBaseProvider = getprovider();
                DataBaseLayer.SupplierRepository SupplierRepo = new DataBaseLayer.SupplierRepository(dataBaseProvider);
                selected_supplier_fk_filter = SupplierRepo.Get_by_supplier_name(selected_supplier_name_filter.supplier_name).Result;
                SupplierRepo.Commit();
                SupplierRepo.Dispose();
            }
            catch (Exception e)
            { MessageBox.Show("An Error has occured:" + e); }

            dataBaseProvider = getprovider();
            DataBaseLayer.ProductRepository ProductRepo = new DataBaseLayer.ProductRepository(dataBaseProvider);
            BindableCollectionProductModel.Clear();
            BindableCollectionProductModel = DataConversion.ToBindableCollection(ProductRepo.GetAllWithID(selected_supplier_fk_filter, selected_category_fk_filter).Result.ToObservable());
            ProductRepo.Commit();
            ProductRepo.Dispose();
            SelectedItem = new ProductModel();
           
            // Load product_category, product purchase order, product supplier
            foreach (var item in BindableCollectionProductModel)
            {
                SelectedItem.category_name = item.category_name;
                // product_purchase_order
                SelectedItem.product_purchase_order_pk = item.product_purchase_order_pk;
                SelectedItem.product_fk = item.product_fk;
                SelectedItem.purchase_order_detail_fk = item.product_order_detail_fk;
                // product_supplier
                SelectedItem.product_supplier_pk = item.product_supplier_pk;
                SelectedItem.supplier_fk = item.supplier_fk;
            }
            RaisePropertyChanged("SelectedItem");
            RaisePropertyChanged("BindableCollectionProductModel");

            if (!canRemoveSupplierFilter)
                canRemoveSupplierFilter = true;
        }
       
        #endregion

        public void SaveCommand()
        {

            // need category_name
            //     SelectedItem = new ProductModel(); this should be run after save.
            var selectedRows = BindableCollectionProductModel.Where(i => i.IsSelected);

            foreach (var item in selectedRows)
            {
                SelectedItem.product_pk = item.product_pk;
                SelectedItem.brand_name = item.brand_name;
                SelectedItem.product_name = item.product_name;
                SelectedItem.category_name = item.category_name;
                SelectedItem.description = item.description;
                SelectedItem.quantity = item.quantity;
                SelectedItem.cost = item.cost;
                SelectedItem.sku = item.sku;
                SelectedItem.sold_price = item.sold_price;
                SelectedItem.instock = item.instock;
                SelectedItem.condition = item.condition;
                SelectedItem.listing_url = item.listing_url;
                SelectedItem.listing_number = item.listing_number;
                SelectedItem.listing_date = item.listing_date;
            }
        
            //check for product_purchase_order
            
            //check for product_supplier 

            //update or insert

            // check for null values
            if (supplier_fk != 0 && purchase_order_detail_pk != 0)
            {
                //          MessageBox.Show("Please select a Supplier.");
                //          return;
            }



            //   var selectedRows = BindableCollectionProductModel.Where(i => i.IsSelected);
            //   foreach (var item in selectedRows)
            //    {
            //           invoicetemp = item.invoice_number;
            //           purchase_datetemp = item.purchase_date;
            //    }

            // MessageBox.Show(Productmodel.product_pk.ToString());
            //Insert or Update

            // Check for Product Key
            /*            if (Productmodel.product_pk == 0)
                        {
                            // Insert
                            MessageBox.Show("Insert");
                        }
                        else
                        {
                            // Update
                            MessageBox.Show("Update");
                        }
            */





            //product key present?

            MessageBox.Show("supplier_fk" + supplier_fk.ToString());
            MessageBox.Show("purchase_order_detail_pk" + purchase_order_detail_pk.ToString());


            MessageBox.Show("prod cat_pk" + SelectedItem.product_category_pk.ToString());
            MessageBox.Show("cat_pk" + SelectedItem.category_pk.ToString());
            MessageBox.Show("product_pk" + SelectedItem.product_pk.ToString());
            MessageBox.Show("product_fk" + SelectedItem.product_fk.ToString());
            MessageBox.Show("brandname" + SelectedItem.brand_name);
            MessageBox.Show("prodname" + SelectedItem.product_name);
            MessageBox.Show("desc" + SelectedItem.description);
            MessageBox.Show("qty" + SelectedItem.quantity.ToString());
            MessageBox.Show("sku" + SelectedItem.sku);
            MessageBox.Show("soldprice" + SelectedItem.sold_price.ToString());
            MessageBox.Show("instock" + SelectedItem.instock.ToString());
            MessageBox.Show("listdate" + SelectedItem.listing_date.ToString());
            MessageBox.Show("listurl" + SelectedItem.listing_url);
            MessageBox.Show("listnum" + SelectedItem.listing_number);
            MessageBox.Show("cost" + SelectedItem.cost.ToString());
            MessageBox.Show("condition" + SelectedItem.condition);
            MessageBox.Show("catname" + SelectedItem.category_name);


        }



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
        //} 
        public void AddCommand()
        {

            if (BindableCollectionProductModel.Count == 0)
            {
                BindableCollectionProductModel = new BindableCollection<ProductModel>();
            }

            SelectedItem.product_pk = 0;
            SelectedItem.product_fk = 0;
            SelectedItem.brand_name = null;
            SelectedItem.product_name = null;
            SelectedItem.description = null;
            SelectedItem.quantity = 0;
            SelectedItem.cost = 0;
            SelectedItem.sku = "0";
            SelectedItem.sold_price = 0;
            SelectedItem.condition = "New";
            SelectedItem.instock = 1;
            SelectedItem.listing_url = null;
            SelectedItem.listing_number = null;
            SelectedItem.listing_date = DateTime.MinValue;
            BindableCollectionProductModel.Add(SelectedItem);
            RaisePropertyChanged("BindableCollectionProductModel");
        }
        public void DeleteCommand()
        {
        }
        public void ResetCommand()
        {

        }

        public void LoadPurchaseOrder(int supplier_fk,int purchase_order_detail_pk)
        {
            // load all lots in combobox, unless purchase_order_detail_pk is set.
            PurchaseOrderModel Purchaseordermodel = new PurchaseOrderModel();

            if (BindableCollectionPurchaseOrderModel  == null)
            {
                BindableCollectionPurchaseOrderModel = new BindableCollection<PurchaseOrderModel>();
            }

            dataBaseProvider = getprovider();
            DataBaseLayer.PurchaseOrderRepository PurchaseOrderRepo = new DataBaseLayer.PurchaseOrderRepository(dataBaseProvider);
        
            if (purchase_order_detail_pk == 0)
            {
                // load lot numbers based on the supplier_fk
                BindableCollectionPurchaseOrderModel = DataConversion.ToBindableCollection(PurchaseOrderRepo.GetAllWithID(supplier_fk).Result.ToObservable());
                RaisePropertyChanged("BindableCollectionPurchaseOrderModel");
               
                foreach (var item in BindableCollectionPurchaseOrderModel)
                {
                    Purchaseordermodel.lot_number = item.lot_number;
                    Purchaseordermodel.purchase_order_detail_pk = item.purchase_order_detail_pk;
                    SelectedItem.purchase_order_detail_fk = item.purchase_order_detail_pk;
                }
            }
            else
            {
                BindableCollectionPurchaseOrderModel = DataConversion.ToBindableCollection(PurchaseOrderRepo.Get_PurchaseOrderDetail_by_pk(purchase_order_detail_pk).Result.ToObservable());
                RaisePropertyChanged("BindableCollectionPurchaseOrderModel");
          
                foreach (var item in BindableCollectionPurchaseOrderModel)
                {
                    Purchaseordermodel.lot_number = item.lot_number;
                    Purchaseordermodel.purchase_order_detail_pk = item.purchase_order_detail_pk;
                    SelectedItem.purchase_order_detail_fk = item.purchase_order_detail_pk;
                }

            }
            PurchaseOrderRepo.Commit();
            PurchaseOrderRepo.Dispose();
            return;
        }

        public void LoadProducts(int supplier_fk, int purchase_order_detail_pk)
        {
            if (supplier_fk > 0 && purchase_order_detail_pk != 0)
            {
                try
                {
                    // check product_supplier and product_purchase_order, if not set don't filter and inform user not set.

                    //Productmodel = new ProductModel();
                    //BindableCollectionProductModel.Clear();
                    //RaisePropertyChanged("BindableCollectionProductModel");

                    //DataBaseLayer.ProductRepository ProductRepo = new DataBaseLayer.ProductRepository(dataBaseProvider);
                    //BindableCollectionProductModel = DataConversion.ToBindableCollection(ProductRepo.GetAllWithAllID(supplier_fk, purchase_order_detail_pk).Result.ToObservable());
                    //ProductRepo.Commit();
                    //ProductRepo.Dispose();
                    //RaisePropertyChanged("BindableCollectionProductModel";
                }
                catch (Exception ex)
                {
                    MessageBox.Show("A Database Error has occured. " + ex);
                }
            }
        }

        public void initial_load()
        {
            if (selected_supplier_name_filter != null && selected_supplier_name_filter.supplier_name != null)
            {
                selected_supplier_name_filter = null;
            }
            // begin

            dataBaseProvider = getprovider();
            // Get Suppliers
            BindableCollectionSupplierModel = new BindableCollection<SupplierModel>();
            DataBaseLayer.SupplierRepository SupplierRepo = new DataBaseLayer.SupplierRepository(dataBaseProvider);
            BindableCollectionSupplierModel = DataConversion.ToBindableCollection(SupplierRepo.GetAll().Result.ToBindableCollection());

            if (BindableCollectionSupplierModel.Count == 0)
            {
                SupplierRepo.Revert();
                SupplierRepo.Dispose();
                MessageBox.Show("Please add a supplier.");
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

            if (BindableCollectionProductModel != null)
            {
                BindableCollectionProductModel.Clear();
                BindableCollectionProductModel = new BindableCollection<ProductModel>();
            }
            BindableCollectionProductModel = DataConversion.ToBindableCollection(ProductRepo.GetAll().Result.ToObservable());
            ProductRepo.Commit();
            ProductRepo.Dispose();
            SelectedItem = new ProductModel();
            RaisePropertyChanged("SelectedItem");
            RaisePropertyChanged("BindableCollectionProductModel");

            // Load product_category, product purchase order, product supplier
            foreach (var item in BindableCollectionProductModel)
            {
                SelectedItem.category_name = item.category_name;
                // product_purchase_order
                SelectedItem.product_purchase_order_pk = item.product_purchase_order_pk;
                SelectedItem.product_fk = item.product_fk;
                SelectedItem.purchase_order_detail_fk = item.product_order_detail_fk;
                // product_supplier
                SelectedItem.product_supplier_pk = item.product_supplier_pk;
                SelectedItem.supplier_fk = item.supplier_fk;
            }

            // Load Category

            try
            {
                category_list = new List<string>();
                BindableCollectionCategoryModel = new BindableCollection<CategoryModel>();
                DataBaseLayer.CategoryRepository CategoryRepos = new DataBaseLayer.CategoryRepository(dataBaseProvider);
                BindableCollectionCategoryModel = DataConversion.ToBindableCollection(CategoryRepos.Get_all_category_name().Result.ToObservable());
                CategoryRepos.Commit();
                CategoryRepos.Dispose();
                foreach (var item in BindableCollectionCategoryModel)
                {
                    category_list.Add(item.category_name);
                }
            }
            catch (Exception ext)
            {
                MessageBox.Show(ext.ToString());
            }  
        }

        public ProductViewModel()
        {
            initial_load();
        }
    }
}