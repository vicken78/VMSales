using Caliburn.Micro;
using Microsoft.Win32;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Drawing;
using System.Linq;
using System.Windows;
using System.Windows.Media.Imaging;
using VMSales.Logic;
using VMSales.Models;

namespace VMSales.ViewModels
{
    public class ProductViewModel : BaseViewModel
    {

        private BitmapImage _imageSource;
        public BitmapImage ImageSource
        {
            get { return _imageSource; }
            set
            {
                if (_imageSource != value)
                {
                    _imageSource = value;
                    RaisePropertyChanged("ImageSource");
                }
            }
        }

        private ObservableCollection<string> _filelist { get; set; }
        public ObservableCollection<string> filelist
        {
            get { return _filelist; }
            set
            {
                if (_filelist == value) return;
                _filelist = value;
                RaisePropertyChanged("filelist");
            }
        }

        private string _selectedfilelist;

        public string Selectedfilelist
        {
            get { return _selectedfilelist; }
            set
            {
                _selectedfilelist = value;
                RaisePropertyChanged("Selectedfilelist");
                LoadImage(Selectedfilelist);
            }
        }

        private bool _productSelected;
        public bool productSelected
        {
            get { return _productSelected; }
            set
            {
                _productSelected = value;
                RaisePropertyChanged("productSelected");
            }
        }


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
                RaisePropertyChanged("canRemoveCategoryFilter");
            }
        }

        private enum FilterField
        {
            Category,
            Supplier,
            None
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
        #endregion
        #region Associate

        private string _selected_supplier_name { get; set; }
        public string selected_supplier_name
        {
            get { return _selected_supplier_name; }
            set
            {
                if (_selected_supplier_name == value) return;
                _selected_supplier_name = value;
                RaisePropertyChanged("selected_supplier_name");
                if (selected_supplier_name != null)
                canEnableProductSupplier = true;
            }
        }
        private int _selected_lot_number { get; set; }
        public int selected_lot_number
        {
            get { return _selected_lot_number; }
            set
            {
                if (_selected_lot_number == value) return;
                _selected_lot_number = value;
                RaisePropertyChanged("selected_lot_number");
            }
        }
     
        private bool _canEnableProductSupplier;
        public bool canEnableProductSupplier
        {
            get { return _canEnableProductSupplier; }
            set
            {
                _canEnableProductSupplier = value;
                RaisePropertyChanged("canEnableProductSupplier");
            }
        }
        private bool _canEnableProductPurchase;
        public bool canEnableProductPurchase
        {
            get { return _canEnableProductPurchase; }
            set
            {
                _canEnableProductPurchase = value;
                RaisePropertyChanged("canEnableProductPurchase");
            }
        }

        #endregion

        #region collections   

        public BindableCollection<SupplierModel> BindableCollectionSupplierModel { get; set; }
        public BindableCollection<ProductModel> BindableCollectionProductModel { get; set; }
        public BindableCollection<CategoryModel> BindableCollectionCategoryModel { get; set; }
        public BindableCollection<PurchaseOrderModel> BindableCollectionPurchaseOrderModel { get; set; }

        #endregion
        #region Members
 
        public List<string> category_list { get; set; }

        private int _supplier_pk;
        public int supplier_pk
        {
            get { return _supplier_pk; }
            set
            {
                if (_supplier_pk == value) return;
                _supplier_pk = value;
                RaisePropertyChanged("supplier_pk");
            }
        }


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
                if (purchase_order_detail_pk != 0)
                {
                    canEnableProductPurchase = true;
                }
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
                if (SelectedItem?.IsSelected != null)
                 {
                    productSelected = true;
                    LoadFileList();
                    GetSupplierByProduct();
                 }
            }
        }


        #endregion

        #region filelistload

        private void LoadImage(string Selectedfilelist)
            {
                if (!string.IsNullOrEmpty(Selectedfilelist))
                {
                    try
                    {
                    BitmapImage image = new BitmapImage();
                        image.BeginInit();
                        image.CacheOption = BitmapCacheOption.OnLoad;
                        image.UriSource = new Uri(Selectedfilelist);
                        image.DecodePixelWidth = 250;
                        image.DecodePixelHeight = 250;
                        image.EndInit();
                        ImageSource = image;
                    }
                    catch (Exception ex)
                    {
                        // Handle any exceptions that may occur during image loading.
                       MessageBox.Show(ex.Message);
                    }
                }
                else
                {
                    // Clear the image source if the file path is null or empty
                    ImageSource = null;
                }
            }
        
        public void LoadFileList()
        {
            if (filelist?.Count > 0)
                filelist.Clear();

            if (SelectedItem.product_pk > 0)
            {
                DataBaseLayer.PhotoRepository PhotoRepo = new DataBaseLayer.PhotoRepository(dataBaseProvider);
                filelist = PhotoRepo.GetFileList(SelectedItem.product_pk).Result.ToObservable();
                PhotoRepo.Commit();
                PhotoRepo.Dispose();
                RaisePropertyChanged("filelist");
            }
            else
            {
                filelist = new ObservableCollection<string>();
                RaisePropertyChanged("filelist");
            }
        }
        #endregion

        #region OpenImageCommand

        public void OpenImageCommand()
        {
            if (Selectedfilelist != null)
            {
                // Handle opening the selected files
                IWindowManager _windowManager = new WindowManager();
                var popupwindow = new ProductPhotoViewModel(SelectedItem, Selectedfilelist);
                _windowManager.ShowWindowAsync(popupwindow);
                LoadFileList();
            }
        }

        #endregion

       #region SupplierConvertor

        public string GetSupplierByProduct()
        {
            try
            {
                // Load Supplier
                DataBaseLayer.SupplierRepository SupplierRepo = new DataBaseLayer.SupplierRepository(dataBaseProvider);
                if (SelectedItem.product_pk > 0)
                selected_supplier_name = SupplierRepo.Selected_Supplier(SelectedItem.product_pk).Result.First().ToString();
                SupplierRepo.Commit();
                SupplierRepo.Dispose();
               // return selected_supplier_name;
            }
            catch (Exception e)
            {
                MessageBox.Show("an expected error has occured." + e);
            }
            return selected_supplier_name; 
        }

        #endregion
        #region SupplierChange
        public void LoadSupplier()
        {
            int get_product_purchase_order_detail_fk = 0;
            if (SelectedItem != null && SelectedItem.product_pk != 0)
            {
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
                    LoadSelectedPurchaseOrder(get_product_purchase_order_detail_fk);
                }
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
            ApplyFilter(FilterField.None);
        }
        public void RemoveSupplierFilterCommand()
        {
            canRemoveSupplierFilter = false;
            ApplyFilter(FilterField.None);
        }
        public void AddCategoryFilter()
        {
            if (!canRemoveCategoryFilter)
                canRemoveCategoryFilter = true;
            int selected_supplier_fk_filter = 0;
            int selected_category_fk_filter = 0;

            // Load Supplier
            try
            {
                dataBaseProvider = getprovider();
                DataBaseLayer.CategoryRepository CategoryRepo = new DataBaseLayer.CategoryRepository(dataBaseProvider);
                selected_category_fk_filter = CategoryRepo.Get_by_category_name(selected_category_name_filter.category_name).Result;
                CategoryRepo.Commit();
                CategoryRepo.Dispose();
            }
            catch (Exception e)
            { MessageBox.Show("An Error has occured:" + e); }

            dataBaseProvider = getprovider();
            DataBaseLayer.ProductRepository ProductRepo = new DataBaseLayer.ProductRepository(dataBaseProvider);
            BindableCollectionProductModel.Clear();
            try
            {
                BindableCollectionProductModel = DataConversion.ToBindableCollection(ProductRepo.GetAllWithID(selected_supplier_fk_filter, selected_category_fk_filter).Result.ToObservable());
            }
            catch (Exception e)
            {
                MessageBox.Show("An error has occured" + e);
            }
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
        }
        public void AddSupplierFilter()
        {
            if (!canRemoveSupplierFilter)
                canRemoveSupplierFilter = true;

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
            try
            {
                BindableCollectionProductModel = DataConversion.ToBindableCollection(ProductRepo.GetAllWithID(selected_supplier_fk_filter, selected_category_fk_filter).Result.ToObservable());
            }
            catch (Exception e)
            {
                MessageBox.Show("An error has occured" + e);
            }
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
        }
       
        #endregion

        public void SaveCommand()
        {
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
                SelectedItem.listed_price = item.listed_price;
                SelectedItem.instock = item.instock;
                SelectedItem.condition = item.condition;
                SelectedItem.listing_url = item.listing_url;
                SelectedItem.listing_number = item.listing_number;
                SelectedItem.listing_date = item.listing_date;
            }
            // first check for null values
            if (SelectedItem.category_name == null)
                {
                    MessageBox.Show("Please select a category name.");
                    return;
                }
                if (SelectedItem.product_name == null)
                {
                    MessageBox.Show("Please enter a product name.");
                    return;
                }

                if (SelectedItem.sku == null)
                {
                    MessageBox.Show("Please enter a sku.");
                    return;
                }
                if (selected_supplier_name == null)
                {
                MessageBox.Show("Please select a supplier.");
                return;
                }
                if (selected_lot_number == 0)
                {
                    MessageBox.Show("Please select a lot number.");
                    return;
                }
            DataBaseLayer.SupplierRepository SupplierRepo = new DataBaseLayer.SupplierRepository(dataBaseProvider);
            //DataBaseLayer.ProductRepository ProductRepo = new DataBaseLayer.ProductRepository(dataBaseProvider);
            try
            {
                // convert supplier_name to pk
                int supplier_pk = SupplierRepo.Get_by_supplier_name(selected_supplier_name).Result;
                SupplierRepo.Commit();
                SupplierRepo.Dispose();
                // Insert
                if (SelectedItem.product_pk == 0)
                {
                    // Product
                    //int new_product_fk = ProductRepo.Insert(SelectedItem).Result;
                    //SelectedItem.product_fk = new_product_fk;

                    // Product Category
                    //bool insert_product_category = ProductRepo.InsertProductCategory(SelectedItem).Result;
                    /*if (insert_product_category == true)
                    {
                    

                        //FIX

                        // Product Supplier
                        // product purchase order 
                    }
                    else 
                    {
                        ProductRepo.Revert();
                        ProductRepo.Dispose();
                    }

                    ProductRepo.Revert();
                    ProductRepo.Dispose();
                    */
                }

                // Update
                else
                {
                    // Product
                    // Product Category
                    // Product Supplier
                    // product purchase order
     
                    //ProductRepo.Commit();
                    //ProductRepo.Dispose();
                }
            }
            catch (Exception e) 
            {
                MessageBox.Show("An error has occured." + e);
                //ProductRepo.Revert();
                //ProductRepo.Dispose();

            }

            //product key present?
            
            
            //MessageBox.Show("supplier_name" + selected_supplier_name); // name only not pk
            
            
            // purchase_order_detail_pk to use
            //MessageBox.Show("purchase_order_detail_pk " + selected_lot_number.ToString()); 
            

            // category_pk and product_pk to use

            //MessageBox.Show("cat_pk" + SelectedItem.category_pk.ToString()); outputs 2 on first row YES
            //MessageBox.Show("product_pk" + SelectedItem.product_pk.ToString()); outputs 1 on first row YES


            //MessageBox.Show("brandname" + SelectedItem.brand_name);
            //MessageBox.Show("prodname" + SelectedItem.product_name);
            //MessageBox.Show("desc" + SelectedItem.description);
            //MessageBox.Show("qty" + SelectedItem.quantity.ToString());
            //MessageBox.Show("sku" + SelectedItem.sku);
            //MessageBox.Show("listed_price" + SelectedItem.listed_price.ToString());
            //MessageBox.Show("instock" + SelectedItem.instock.ToString());
            //MessageBox.Show("listdate" + SelectedItem.listing_date.ToString());
            //MessageBox.Show("listurl" + SelectedItem.listing_url);
            //MessageBox.Show("listnum" + SelectedItem.listing_number);
            //MessageBox.Show("cost" + SelectedItem.cost.ToString());
            //MessageBox.Show("condition" + SelectedItem.condition);
            //MessageBox.Show("catname" + SelectedItem.category_name);



            //MessageBox.Show("product_fk" + SelectedItem.product_fk.ToString()); // NO
            //MessageBox.Show("supplier_fk" + supplier_fk.ToString()); NO
            //MessageBox.Show("purchase_order_detail_pk" + purchase_order_detail_pk.ToString()); NO
            //MessageBox.Show("supplier_fk"+SelectedItem.supplier_fk.ToString()); NO
            //MessageBox.Show("prod cat_pk" + SelectedItem.product_category_pk.ToString()); NO


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
            // clear filelist
            if (filelist?.Count > 0)
            {
                filelist.Clear();
            }
            filelist = new ObservableCollection<string>();
            RaisePropertyChanged("filelist");
            if (BindableCollectionProductModel.Count == 0)
            {
                BindableCollectionProductModel = new BindableCollection<ProductModel>();
            }

            // add the product line

            var product = new ProductModel()
            {
                product_pk = 0,
                product_fk = 0,
                brand_name = null,
                product_name = null,
                description = null,
                quantity = 0,
                cost = 0,
                sku = "",
                listed_price = 0,
                condition = "New",
                instock = 1,
                listing_url = null,
                listing_number = null,
                listing_date = DateTime.Today
            };

            BindableCollectionProductModel.Insert(0, product);
            SelectedItem = null;
            RaisePropertyChanged("BindableCollectionProductModel");
            RaisePropertyChanged("SelectedItem");
        }
        public void DeleteCommand()
        {
        }
        public void ResetCommand()
        {
            initial_load();
        }

        public void UploadImageCommand()
        {
        
        OpenFileDialog openFileDialog = new OpenFileDialog();
            openFileDialog.Filter = "Image Files (*.bmp, *.jpg, *.png)|*.bmp;*.jpg;*.png";
            if (openFileDialog.ShowDialog() == true)
            {
                string filePath = openFileDialog.FileName;
                IWindowManager _windowManager = new WindowManager();
                var popupwindow = new ProductPhotoViewModel(SelectedItem, filePath);
                _ = _windowManager.ShowWindowAsync(popupwindow);
            }
        }

        public void LoadSelectedPurchaseOrder(int purchase_order_detail_pk)
        {

            if (purchase_order_detail_pk > 0)
            {
                dataBaseProvider = getprovider();
                DataBaseLayer.PurchaseOrderRepository PurchaseOrderRepo = new DataBaseLayer.PurchaseOrderRepository(dataBaseProvider);
                selected_lot_number = PurchaseOrderRepo.Get_PurchaseOrderDetail_by_pk(purchase_order_detail_pk).Result;
                PurchaseOrderRepo.Commit();
                PurchaseOrderRepo.Dispose();
            }
        }

        public void initial_load()
        {
            // reset buttons
            _productSelected = false;
            productSelected = false;
            canEnableProductSupplier = false;
            canEnableProductPurchase = false;
            selected_supplier_name = null;
            selected_lot_number = 0;
            purchase_order_detail_pk = 0;
            // reset filters
            if (selected_supplier_name_filter != null && selected_supplier_name_filter.supplier_name != null)
            {
                selected_supplier_name_filter = null;
            }
            
            if (selected_category_name_filter != null && selected_category_name_filter.category_name != null)
            {
                selected_category_name_filter = null;
            }

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