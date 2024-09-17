using Caliburn.Micro;
using Microsoft.Win32;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Media.Imaging;
using VMSales.Logic;
using VMSales.Models;

namespace VMSales.ViewModels
{ 
public class ProductViewModel : BaseViewModel
    {
        private ObservableCollection<ProductModel> _ObservableCollectionProductModelDirty { get; set; }

        public ObservableCollection<ProductModel> ObservableCollectionProductModelDirty
        {
            get => _ObservableCollectionProductModelDirty;
            set
            {
                _ObservableCollectionProductModelDirty = value;
                NotifyOfPropertyChange(() => ObservableCollectionProductModelDirty);
            }
        }
        public ObservableCollection<ProductModel> ObservableCollectionProductModelClean { get; protected set; }

        public ObservableCollection<SupplierModel> ObservableCollectionSupplierModel { get; set; }
        public ObservableCollection<ProductModel> ObservableCollectionProductModel { get; set; }
        public ObservableCollection<CategoryModel> ObservableCollectionCategoryModel { get; set; }
        public ObservableCollection<PurchaseOrderModel> ObservableCollectionPurchaseOrderModel { get; set; }



        IDatabaseProvider dataBaseProvider;


        //Commands
        public async Task SaveCommand()
        { 
        
        }

        public void ResetCommand()
        {
            ObservableCollectionProductModelDirty.Clear();
            ObservableCollectionProductModelClean.Clear();
            initial_load();
        }



        public void initial_load()
        {
            ObservableCollectionProductModelDirty = new ObservableCollection<ProductModel>();
            ObservableCollectionProductModelClean = new ObservableCollection<ProductModel>();
            ObservableCollectionPurchaseOrderModel = new ObservableCollection<PurchaseOrderModel>();
            ObservableCollectionCategoryModel = new ObservableCollection<CategoryModel>();
            ObservableCollectionSupplierModel = new ObservableCollection<SupplierModel>();

            dataBaseProvider = getprovider();
            DataBaseLayer.ProductRepository ProductRepo = new DataBaseLayer.ProductRepository(dataBaseProvider);
            ObservableCollectionProductModelDirty = ProductRepo.GetAll().Result.ToObservable();
            ProductRepo.Dispose();

            DataBaseLayer.PurchaseOrderRepository PurchaseOrderRepo = new DataBaseLayer.PurchaseOrderRepository(dataBaseProvider);
            ObservableCollectionPurchaseOrderModel = PurchaseOrderRepo.GetAll().Result.ToObservable();
            PurchaseOrderRepo.Dispose();

            CategoryRepository CategoryRepo = new CategoryRepository(dataBaseProvider);
            ObservableCollectionCategoryModel = CategoryRepo.GetAll().Result.ToObservable();
            CategoryRepo.Dispose();

            SupplierRepository SupplierRepo = new SupplierRepository(dataBaseProvider);
            ObservableCollectionSupplierModel = SupplierRepo.GetAll().Result.ToObservable();
            SupplierRepo.Dispose();
        }

        public ProductViewModel()
        {
            initial_load();
        }
    }

}


/* OLD CODE */

/*

namespace VMSales.ViewModels
{
    public class ProductViewModel : BaseViewModel
    {

        private string searchterm;
        private string _searchbox;
        public string searchbox
        {
            get { return _searchbox; }
            set
            {
                if (_searchbox != value)
                {
                    _searchbox = value;
                    //RaisePropertyChanged("searchbox");
                }
            }
        }
        public int selected_supplier_fk_filter;
        public int selected_category_fk_filter;

        private string _searchdropselect;
        public string searchdropselect
        {
            get { return _searchdropselect; }
            set
            {
                if (_searchdropselect != value)
                {
                    _searchdropselect = value;
                    //RaisePropertyChanged("searchdropselect");
                }
            }
        }

        private Visibility _showsearchtext;
        public Visibility showsearchtext
        {
            get { return _showsearchtext; }
            set
            {
                if (_showsearchtext != value)
                {
                    _showsearchtext = value;
                    //RaisePropertyChanged("showsearchtext");
                }
            }
        }

        private Visibility _showsearchdrop;
        public Visibility showsearchdrop
        {
            get { return _showsearchdrop; }
            set
            {
                if (_showsearchdrop != value)
                {
                    _showsearchdrop = value;
                    //RaisePropertyChanged("showsearchdrop");
                }
            }
        }


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

        private ObservableCollection<string> _searchdrop { get; set; }
        public ObservableCollection<string> searchdrop
        {
            get { return _searchdrop; }
            set
            {
                if (_searchdrop == value) return;
                _searchdrop = value;
                //RaisePropertyChanged("searchdrop");
            }
        }

        private ObservableCollection<string> _searchdropdown { get; set; }
        public ObservableCollection<string> searchdropdown
        {
            get { return _searchdropdown; }
            set
            {
                if (_searchdropdown == value) return;
                _searchdropdown = value;
                //RaisePropertyChanged("searchdropdown");
            }
        }

        private string _selected_search { get; set; }

        public string selected_search
        {
            get { return _selected_search; }
            set
            {
                _selected_search = value;
                //RaisePropertyChanged("selected_search");
                if (selected_search == "Condition")
                {
                    searchterm = "Condition";
                    showsearchdrop = Visibility.Visible;
                    showsearchtext = Visibility.Hidden;
                }
                else { showsearchdrop = Visibility.Hidden; showsearchtext = Visibility.Visible; }
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

        private bool _productSelected;
        public bool productSelected
        {
            get { return _productSelected; }
            set
            {
                _productSelected = value;
                //RaisePropertyChanged("productSelected");
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
                //RaisePropertyChanged("canRemoveSupplierFilter");
            }
        }
        private bool _canRemoveCategoryFilter;
        public bool canRemoveCategoryFilter
        {
            get { return _canRemoveCategoryFilter; }
            set
            {
                _canRemoveCategoryFilter = value;
                //RaisePropertyChanged("canRemoveCategoryFilter");
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
                //RaisePropertyChanged("selected_supplier_name_filter");
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
                //RaisePropertyChanged("selected_category_name_filter");
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
                //RaisePropertyChanged("selected_supplier_name");
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
                //RaisePropertyChanged("selected_lot_number");
            }
        }

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

        public List<string> category_list { get; set; }

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

        private ProductModel _SelectedItem { get; set; }
        public ProductModel SelectedItem
        {
            get { return _SelectedItem; }
            set
            {
                if (_SelectedItem == value) return;
                _SelectedItem = value;
                //RaisePropertyChanged("SelectedItem");
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


        // expand

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

            // Load Category

            try
            {
                category_list = new List<string>();
                BindableCollectionCategoryModel = new BindableCollection<CategoryModel>();
                CategoryRepository CategoryRepos = new CategoryRepository(dataBaseProvider);
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

*/