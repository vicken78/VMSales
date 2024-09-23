using Caliburn.Micro;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media.Imaging;
using VMSales.Logic;
using VMSales.Models;

namespace VMSales.ViewModels
{
    public class ProductViewModel : BaseViewModel
    {
        IDatabaseProvider dataBaseProvider;
        private ObservableCollection<ProductModel> _ObservableCollectionProductModelDirty;

        public ObservableCollection<ProductModel> ObservableCollectionProductModelDirty
        {
            get => _ObservableCollectionProductModelDirty;
            set
            {
                _ObservableCollectionProductModelDirty = value;
                NotifyOfPropertyChange(() => ObservableCollectionProductModelDirty);
            }
        }
        public List<string> category_list { get; set; }
        public List<string> product_filter { get; set; }
        
        public ObservableCollection<ProductModel> ObservableCollectionProductModelClean { get; protected set; }
        public ObservableCollection<PurchaseOrderModel> ObservableCollectionPurchaseOrderModel { get; set; }
        public ObservableCollection<ProductModel> ObservableCollectionProductModel { get; set; }
        public ObservableCollection<CategoryModel> ObservableCollectionCategoryModel { get; set; }
        public ObservableCollection<SupplierModel> ObservableCollectionSupplierModel { get; set; }

        private ProductModel _SelectedProduct;
        public ProductModel SelectedProduct
        {
            get => _SelectedProduct; 
            set
            {
                if (_SelectedProduct != value)
                _SelectedProduct = value;
                NotifyOfPropertyChange(() => SelectedProduct);
                if (SelectedProduct?.IsSelected != null)
                {
                    LoadSupplier();
                    //Boolean productSelected = true;
                    //LoadFileList();
                }
            }
         }

        #region Selected Product Supplier
        private string _selected_supplier_name;
        public string selected_supplier_name
        {
            get => _selected_supplier_name; 
            set
            {
                if (_selected_supplier_name != value)
                _selected_supplier_name = value;
                NotifyOfPropertyChange(() => selected_supplier_name);
            }
        }
        private string _selected_lot_number;
        public string selected_lot_number
        {
            get => _selected_lot_number;
            set
            {
                if (_selected_lot_number != value)
                    _selected_lot_number = value;
                NotifyOfPropertyChange(() => selected_lot_number);
            }
        }
        private string _selected_product_filter;
        public string selected_product_filter
        {
            get => _selected_product_filter;
            set
            {
                if (_selected_product_filter != value)
                    _selected_product_filter = value;
                NotifyOfPropertyChange(() => selected_product_filter);
            }
        }



        #endregion


        #region Filters
        private bool _canRemoveSupplierFilter;
        public bool canRemoveSupplierFilter
        {
            get => _canRemoveSupplierFilter;
            set
            {
                if (_canRemoveSupplierFilter != value)
                    _canRemoveSupplierFilter = value;
                NotifyOfPropertyChange(() => canRemoveSupplierFilter);
            }
        }
        private bool _canRemoveCategoryFilter;
        public bool canRemoveCategoryFilter
        {
            get => _canRemoveCategoryFilter;
            set
            {
                if (_canRemoveCategoryFilter != value)
                    _canRemoveCategoryFilter = value;
                NotifyOfPropertyChange(() => canRemoveCategoryFilter);
            }
        }

        private enum FilterField
        {
            Category,
            Supplier,
            None
        }
        private SupplierModel _selected_supplier_filter;
        public SupplierModel selected_supplier_filter
        {
            get => _selected_supplier_filter;
            set
            {
       
                  if (_selected_supplier_filter != value)
                  {
                      _selected_supplier_filter = value; 
                      NotifyOfPropertyChange(() => selected_supplier_filter);
                      ApplyFilter(!string.IsNullOrEmpty(selected_supplier_filter?.supplier_name) ? FilterField.Supplier : FilterField.None);
                  }
            }
        }

        private CategoryModel _selected_category_filter;
        public CategoryModel selected_category_filter
        {
            get => _selected_category_filter;
            set
            {

                if (_selected_category_filter != value)
                {
                    _selected_category_filter = value;
                    NotifyOfPropertyChange(() => selected_category_filter);
                    ApplyFilter(!string.IsNullOrEmpty(_selected_category_filter?.category_name) ? FilterField.Category : FilterField.None);
                }
            }
        }

        private string _searchtext;
        public string searchtext
        {
            get => _searchtext; 
            set
            {
                if (_searchtext != value)
                {
                    _searchtext = value;
                    NotifyOfPropertyChange(() => searchtext);
                }
            }
        }

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
            }
        }

        public void AddCategoryFilter()
        {
            if (!canRemoveCategoryFilter)
                canRemoveCategoryFilter = true;
            NotifyOfPropertyChange(() => canRemoveCategoryFilter);
            UpdateProduct();
        }

        public void AddSupplierFilter()
        {
            if (!canRemoveSupplierFilter)
                canRemoveSupplierFilter = true;
            NotifyOfPropertyChange(() => canRemoveSupplierFilter);
            UpdateProduct();
        }

        public void RemoveCategoryFilterCommand()
        {
            canRemoveCategoryFilter = false;
            selected_category_filter.category_pk = 0;
            NotifyOfPropertyChange(() => canRemoveCategoryFilter);
            NotifyOfPropertyChange(() => selected_category_filter);
            UpdateProduct();
        }
        public void RemoveSupplierFilterCommand()
        {
            canRemoveSupplierFilter = false;
            selected_supplier_filter.supplier_pk = 0;
            NotifyOfPropertyChange(() => canRemoveSupplierFilter);
            NotifyOfPropertyChange(() => selected_supplier_filter);
            UpdateProduct();
        }

        public void UpdateProduct()
        {
            try
            {
                if (selected_category_filter.category_pk > 0 || selected_supplier_filter.supplier_pk > 0)
                {
                    dataBaseProvider = getprovider();
                    ObservableCollectionProductModelDirty.Clear();
                    ProductRepository ProductRepo = new ProductRepository(dataBaseProvider);
                    ObservableCollectionProductModelDirty = ProductRepo.GetAllWithID(
                        selected_supplier_filter.supplier_pk,
                        selected_category_filter.category_pk,
                        selected_product_filter,
                        searchtext
                        )
                        .Result.ToObservable();
                    ProductRepo.Dispose();
                }

                else
                {
                    ObservableCollectionProductModelDirty = new ObservableCollection<ProductModel>();
                    initial_load();
                }
            }
            catch (Exception e)
            {
                MessageBox.Show("An error has occured" + e);
            }
             
        }

        #endregion

        #region Commands
        //public async Task SaveCommand()
        public void SaveCommand()
        {
            // Create an instance of DataProcessor with ProductModel type
            var dataProcessor = new DataProcessor<ProductModel>();

            // Call the Compare method
            ObservableCollection<ProductModel> differences = dataProcessor.Compare(ObservableCollectionProductModelClean, ObservableCollectionProductModelDirty);

            foreach (var item in differences)
            {
                

                //ProductRepository ProductRepo = new ProductRepository(dataBaseProvider);
                // need to handle product supplier and product category as well.
                // not fully implemented


                try
                {
                    // implement check for foreign keys, if foreign key exists, warn user.
                    switch (item.Action)
                    {
                        case "Update":
                            //bool Update_Category = ProductRepo.Update(item).Result;
                            //if (Update_Category == false)
                            //{ throw new Exception("Update Failed"); }
                            //else
                                //ProductRepo.Commit();
                            break;
                        case "Insert":
                            //int category_pk = await ProductRepo.Insert(item);
                            //if (product_pk == 0)
                            //{ throw new Exception("Insert Failed"); }
                            //else
                                //ProductRepo.Commit();
                            break;
                        case "Delete":
                            //bool Delete_Product = ProductRepo.Delete(item).Result;
                            //if (Product_Category == false)
                            //{ MessageBox.Show("Foreign Key Exists, You must delete the associated foreign key first Category FK=" + item.category_pk); }
                            //else
                                //ProductRepo.Commit();
                            break;
                        //default:
                           // throw new InvalidOperationException("Unexpected Action read, expected Update Insert or Delete.");
                    }
                    //ProductRepo.Dispose();
                    //initial_load();
                    NotifyOfPropertyChange(() => ObservableCollectionProductModelDirty);

                }
                catch (Exception e)
                {
                    MessageBox.Show("An unexpected error has occured." + e);
                    //ProductRepo.Revert();
                    //ProductRepo.Dispose();
                }
            }
        }

        public void ResetCommand()
        {
         
            ObservableCollectionProductModelDirty.Clear();
            ObservableCollectionProductModelClean.Clear();
            RemoveCategoryFilterCommand();
            RemoveSupplierFilterCommand();
            initial_load();
        }

        public void SearchCommand()
        {
            if (selected_product_filter != null && searchtext != null)
            {
                dataBaseProvider = getprovider();
                ObservableCollectionProductModelDirty.Clear();
                ProductRepository ProductRepo = new ProductRepository(dataBaseProvider);
                ObservableCollectionProductModelDirty = ProductRepo.GetAllWithID(
                selected_supplier_filter.supplier_pk,
                selected_category_filter.category_pk,
                selected_product_filter,
                searchtext
                )
                .Result.ToObservable();
                ProductRepo.Dispose();

            }
        }

        public void ClearCommand()
        {
            searchtext = null;
        }


        #endregion


        public void initial_load()
        {
            _selected_supplier_filter = new SupplierModel { supplier_pk = 0, supplier_name = null };
            _selected_category_filter = new CategoryModel { category_pk = 0, category_name = null };

            ObservableCollectionProductModelDirty = new ObservableCollection<ProductModel>();
            ObservableCollectionProductModelClean = new ObservableCollection<ProductModel>();
            ObservableCollectionPurchaseOrderModel = new ObservableCollection<PurchaseOrderModel>();
            ObservableCollectionCategoryModel = new ObservableCollection<CategoryModel>();
            ObservableCollectionSupplierModel = new ObservableCollection<SupplierModel>();


            dataBaseProvider = getprovider();
            ProductRepository ProductRepo = new ProductRepository(dataBaseProvider);
            
            ObservableCollectionProductModelDirty = ProductRepo.GetAll().Result.ToObservable();
            ProductRepo.Dispose();

            DataBaseLayer.PurchaseOrderRepository PurchaseOrderRepo = new DataBaseLayer.PurchaseOrderRepository(dataBaseProvider);
            ObservableCollectionPurchaseOrderModel = PurchaseOrderRepo.GetAll().Result.ToObservable();
            PurchaseOrderRepo.Dispose();

            SupplierRepository SupplierRepo = new SupplierRepository(dataBaseProvider);
            ObservableCollectionSupplierModel = SupplierRepo.GetAll().Result.ToObservable();
            SupplierRepo.Dispose();


            CategoryRepository CategoryRepo = new CategoryRepository(dataBaseProvider);
            ObservableCollectionCategoryModel = CategoryRepo.GetAll().Result.ToObservable();
            CategoryRepo.Dispose();
            category_list = new List<string>();
            foreach (var item in ObservableCollectionCategoryModel)
            {
                category_list.Add(item.category_name);
            }
            product_filter = new List<string> { "Product Name", "Description", "SKU", "Brand Name", "Quantity", "Cost","Listed Price", "Listing Number", "Listing URL","Listing Date" };
        }
        public void LoadSupplier()
        {
            if (SelectedProduct != null && SelectedProduct.product_pk != 0)
            {
                try
                {
                    var ProductPurchaseRepo = new DataBaseLayer.PurchaseOrderRepository(dataBaseProvider);
                    ObservableCollectionPurchaseOrderModel = ProductPurchaseRepo.GetProductPurchase_Order(SelectedProduct.product_pk).Result.ToObservable();
                    ProductPurchaseRepo.Commit();
                    ProductPurchaseRepo.Dispose();
                }
                catch (Exception e)
                {
                    MessageBox.Show("An Error has occured:" + e);
                }
                finally
                {
                    foreach (var item in ObservableCollectionPurchaseOrderModel)
                    {
                        selected_supplier_name = item.supplier_name;
                        selected_lot_number = item.lot_number;
                    }
                }
            }
        }
            public ProductViewModel()
        {
            initial_load();
        }
    }

}


/* OLD CODE */

/*



        private BitmapImage _imageSource;
        public BitmapImage ImageSource
        {
            get { return _imageSource; }
            set
            {
                if (_imageSource != value)
                {
                    _imageSource = value;
                    //RaisePropertyChanged("ImageSource");
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
                //RaisePropertyChanged("filelist");
            }
        }

        private string _selectedfilelist;

        public string Selectedfilelist
        {
            get { return _selectedfilelist; }
            set
            {
                _selectedfilelist = value;
                //RaisePropertyChanged("Selectedfilelist");
                LoadImage(Selectedfilelist);
            }
        }



        
        private bool _canEnableSearchFilter;
        public bool canEnableSearchFilter
        {
            get { return _canEnableSearchFilter; }
            set
            {
                _canEnableSearchFilter = value;
                //RaisePropertyChanged("canEnableSearchFilter");
            }
        }
        #endregion
        #region Associate


        private bool _canEnableProductSupplier;
        public bool canEnableProductSupplier
        {
            get { return _canEnableProductSupplier; }
            set
            {
                _canEnableProductSupplier = value;
                //RaisePropertyChanged("canEnableProductSupplier");
            }
        }
        private bool _canEnableProductPurchase;
        public bool canEnableProductPurchase
        {
            get { return _canEnableProductPurchase; }
            set
            {
                _canEnableProductPurchase = value;
                //RaisePropertyChanged("canEnableProductPurchase");
            }
        }

        #endregion

        #region collections   


        #endregion
        #region Members

   
        private int _supplier_pk;
        public int supplier_pk
        {
            get { return _supplier_pk; }
            set
            {
                if (_supplier_pk == value) return;
                _supplier_pk = value;
                //RaisePropertyChanged("supplier_pk");
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
                //RaisePropertyChanged("supplier_fk");
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
                //RaisePropertyChanged("purchase_order_detail_pk");
                if (purchase_order_detail_pk != 0)
                {
                    canEnableProductPurchase = true;
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
                //RaisePropertyChanged("filelist");
            }
            else
            {
                filelist = new ObservableCollection<string>();
                //RaisePropertyChanged("filelist");
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
                SupplierRepository SupplierRepo = new SupplierRepository(dataBaseProvider);
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
        #region Selected Product
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
                   if (purchase_order_detail_pk > 0)
            {
                dataBaseProvider = getprovider();
                DataBaseLayer.PurchaseOrderRepository PurchaseOrderRepo = new DataBaseLayer.PurchaseOrderRepository(dataBaseProvider);
                selected_lot_number = PurchaseOrderRepo.Get_PurchaseOrderDetail_by_pk(purchase_order_detail_pk).Result;
                PurchaseOrderRepo.Commit();
                PurchaseOrderRepo.Dispose();
         

                }
            }
        }

        #endregion
        IDatabaseProvider dataBaseProvider;


        // expand


        public void RemoveCategoryFilterCommand()
        {
            selected_category_fk_filter = 0;
            canRemoveCategoryFilter = false;
            ApplyFilter(FilterField.None);
        }
        public void RemoveSupplierFilterCommand()
        {
            selected_supplier_fk_filter = 0;
            canRemoveSupplierFilter = false;
            ApplyFilter(FilterField.None);
        }
        public void AddCategoryFilter()
        {
            if (!canRemoveCategoryFilter)
                canRemoveCategoryFilter = true;
     
            // Load Supplier
            try
            {
                dataBaseProvider = getprovider();
                CategoryRepository CategoryRepo = new CategoryRepository(dataBaseProvider);
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
            //RaisePropertyChanged("SelectedItem");
            //RaisePropertyChanged("BindableCollectionProductModel");
        }
        public void AddSupplierFilter()
        {
            if (!canRemoveSupplierFilter)
                canRemoveSupplierFilter = true;

            // Load Supplier
            try
            {
                dataBaseProvider = getprovider();
                SupplierRepository SupplierRepo = new SupplierRepository(dataBaseProvider);
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
            //RaisePropertyChanged("SelectedItem");
            //RaisePropertyChanged("BindableCollectionProductModel");
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
            SupplierRepository SupplierRepo = new SupplierRepository(dataBaseProvider);
            // convert supplier_name to fk
            SelectedItem.supplier_fk = SupplierRepo.Get_by_supplier_name(selected_supplier_name).Result;
            SelectedItem.purchase_order_detail_fk = selected_lot_number;
            SupplierRepo.Commit();
            SupplierRepo.Dispose();

            CategoryRepository CategoryRepo = new CategoryRepository(dataBaseProvider);
            // convert category_name to fk
            SelectedItem.category_fk = CategoryRepo.Get_by_category_name(SelectedItem.category_name).Result;
            CategoryRepo.Commit();
            CategoryRepo.Dispose();


            DataBaseLayer.ProductRepository SelectProductRepo = new DataBaseLayer.ProductRepository(dataBaseProvider);
            bool get_product_category = SelectProductRepo.Get_Product_Category(SelectedItem).Result;
            SelectProductRepo.Commit();
            SelectProductRepo.Dispose();

            DataBaseLayer.ProductRepository ProductRepo = new DataBaseLayer.ProductRepository(dataBaseProvider);
       
            // Insert and Update Logic Needs to be redone.

            // Case Insert
            try
            {

                bool update_product_category = false;
                bool insert_product_category = false;

                if (SelectedItem.product_pk == 0)
                {
                    // new product_fk
                    int new_product_fk = ProductRepo.Insert(SelectedItem).Result;
                    SelectedItem.product_fk = new_product_fk;
                    ProductRepo.Commit();

                    // insert the Product Category
                    insert_product_category = ProductRepo.InsertProductCategory(SelectedItem).Result;
                    ProductRepo.Commit();
                    if (insert_product_category)
                    {
                        bool Insert_Product_Purchase_Order = ProductRepo.Insert_Product_Purchase_Order(SelectedItem).Result;
                        if (Insert_Product_Purchase_Order)
                        {
                            MessageBox.Show("1 Row Inserted");
                            ProductRepo.Commit();
                            ProductRepo.Dispose();
                        }
                    }
                }

                // Product Table
                bool Update_Product = ProductRepo.Update(SelectedItem).Result;

                MessageBox.Show("Update Product" + Update_Product.ToString());     
                if (Update_Product)
                {
                    SelectedItem.product_fk = SelectedItem.product_pk;

                    // does product_category exist?
                    // Product Category
                    // this is not working properly. 
                    if (get_product_category)
                    {
                        update_product_category = ProductRepo.Update_Product_Category(SelectedItem).Result;
                    }
                    // does not exist, insert product category
                    else
                    {
                        insert_product_category = ProductRepo.InsertProductCategory(SelectedItem).Result;
                    }
                  

                    // continue
                    if (update_product_category == true || insert_product_category == true)
                    {
                        // Product Supplier
                        bool Update_Product_Supplier = ProductRepo.Update_Product_Supplier(SelectedItem).Result;
                        if (Update_Product_Supplier)
                        {
                            // Product Purchase Order
                            bool Update_Product_Purchase_Order = ProductRepo.Update_Product_Purchase_Order(SelectedItem).Result;
                            if (Update_Product_Purchase_Order)
                            {
                                ProductRepo.Commit();
                                ProductRepo.Dispose();
                            }
                            else
                            {
                                MessageBox.Show("update_product_purchase_order failed");
                                ProductRepo.Revert();
                            }
                        }
                        else
                        {
                            MessageBox.Show("update product supplier failed");
                            ProductRepo.Revert();
                        }

                    }
                    else
                    {
                        MessageBox.Show("update product category failed");
                        ProductRepo.Revert();
                    }
                }
                else
                {
                    MessageBox.Show("update product failed");
                    ProductRepo.Revert();
                }
            }
            catch (Exception e)
            {
                MessageBox.Show("An error has occured." + e);
                ProductRepo.Revert();
                ProductRepo.Dispose();
            }
        }    
            
            public void AddCommand()
            {
            // clear filelist
            if (filelist?.Count > 0)
            {
                filelist.Clear();
            }
            filelist = new ObservableCollection<string>();
            //RaisePropertyChanged("filelist");
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
                listing_date = DateTime.MinValue
            };

            BindableCollectionProductModel.Insert(0, product);
            SelectedItem = null;
            //RaisePropertyChanged("BindableCollectionProductModel");
            //RaisePropertyChanged("SelectedItem");
        }
        public void DeleteCommand()
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
            // are we deleting an unsaved row?
            if (SelectedItem.product_pk == 0)
            {
                BindableCollectionProductModel.Remove(SelectedItem);
            }
            else 
            {
                // warn user
                bool Result = MessageBox.Show("Are you sure you want to delete " + SelectedItem.product_name + "\nThis change cannot be undone.", "Confirmation", MessageBoxButton.YesNo) == MessageBoxResult.Yes;

                if (Result == true)
                {


                    DataBaseLayer.ProductRepository ProductRepo = new DataBaseLayer.ProductRepository(dataBaseProvider);

                    // convert category_name to fk
                    string order_number = ProductRepo.Check_Product_Customer(SelectedItem).Result;
                    ProductRepo.Commit();
                    // if product is in customer order we cannot delete.
                    if (order_number != null)
                    {
                        ProductRepo.Dispose();
                        MessageBox.Show("This product is sold and associated with order number " + order_number + " Please delete customer order number first");
                        return;
                    }
                    else
                    {
                        // ok to delete

                        // Photo and Product Photo

                        bool Delete_Product = ProductRepo.Delete(SelectedItem).Result;
                        if (Delete_Product == true)
                        {
                            // Product Category
                            
                            bool Delete_Product_Category = ProductRepo.Delete_Product_Category(SelectedItem).Result;
                            if (Delete_Product_Category == true)
                            {
                                // Product Supplier 
                                bool Delete_Product_Supplier = ProductRepo.Delete_Product_Supplier(SelectedItem).Result;
                                if (Delete_Product_Supplier == true)
                                {
                                    // Product Purchase Order

                                    bool Delete_Product_Purchase_Order = ProductRepo.Delete_Product_Purchase_Order(SelectedItem).Result;
                                    if (Delete_Product_Purchase_Order == true)
                                    {
                                        ProductRepo.Commit();
                                        ProductRepo.Dispose();
                                        MessageBox.Show("1 Row Deleted");
                                    }
                                }
                            }
                        }
                    }
                }
            }
        }
        public void ResetCommand()
        {
            initial_load();
        }

        // databaselayer line 597
        public void SearchCommand()
        {
            canEnableSearchFilter = true;

            if (searchterm == "Condition" && !string.IsNullOrWhiteSpace(searchdropselect))
            
            {
                PropertyInfo property = typeof(ProductModel).GetProperty(searchterm);
                BindableCollectionProductModel = new BindableCollection<ProductModel>(
                BindableCollectionProductModel.Where(item => item.condition == searchdropselect));
                //RaisePropertyChanged("BindableCollectionProductModel");
            }
            if (!string.IsNullOrWhiteSpace(searchbox) && !string.IsNullOrWhiteSpace(selected_search))
            {
                  switch (selected_search)
                  {
                      case "Product Name":
                          searchterm = "product_name";
                          break;
                      case "Description":
                          searchterm = "description";
                          break;
                      case "SKU":
                          searchterm = "sku";
                          break;
                      case "Brand Name":
                          searchterm = "brand_name";
                          break;
                      default:
                          searchterm = "product_name";
                          break;
                  }
                    PropertyInfo property = typeof(ProductModel).GetProperty(searchterm);
                    BindableCollectionProductModel = new BindableCollection<ProductModel>(
                    BindableCollectionProductModel.Where(item => property.GetValue(item)?.ToString().IndexOf(searchbox, StringComparison.OrdinalIgnoreCase) >= 0));
                    //RaisePropertyChanged("BindableCollectionProductModel");
            }
        }

        public void ClearCommand()
        {
            canEnableSearchFilter = false;
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
            searchterm = null;
            showsearchdrop = Visibility.Hidden;
            showsearchtext = Visibility.Visible;
            searchbox = null;
            selected_search = null;
            _productSelected = false;
            productSelected = false;
            canEnableProductSupplier = false;
            canEnableProductPurchase = false;
            selected_supplier_name = null;
            selected_lot_number = 0;
            purchase_order_detail_pk = 0;

            searchdrop = new ObservableCollection<string>
            {
                "New", "Used"
            };


            searchdropdown = new ObservableCollection<string>
            {
                "Condition", "Product Name", "Description", "SKU", "Brand Name"
            };

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
            SupplierRepository SupplierRepo = new SupplierRepository(dataBaseProvider);

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
            ////RaisePropertyChanged("SelectedItem");
            ////RaisePropertyChanged("BindableCollectionProductModel");

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

*/