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
using System.Windows.Data;
using VMSales.Logic;
using VMSales.Models;

namespace VMSales.ViewModels
{
    // read https://stackoverflow.com/questions/9608282/how-to-do-caliburn-micro-binding-of-view-model-to-combobox-selected-value

    public class ProductViewModel : BaseViewModel
    {
        public PurchaseOrderModel purchaseOrderModel { get; set; }
        #region collections

        private readonly ICollectionView countryEntries;
        public ICollectionView CountryEntries
        {
            get
            {
                return countryEntries;
            }
        }



        public ObservableCollection<SupplierModel> ObservableCollectionSupplierModel { get; set; }
        public BindableCollection<ProductModel> BindableCollectionProductModel { get; set; }
        public BindableCollection<CategoryModel> BindableCollectionCategoryModel { get; set; }

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

        private PurchaseOrderModel _selectedlotnumber;
        public PurchaseOrderModel selectedlotnumber
        {
            get { return _selectedlotnumber; }
            set
            {
                MessageBox.Show("HIT");
                RaisePropertyChanged("selectedlotnumber");
            
                    MessageBox.Show(selectedlotnumber.lot_number.ToString());
          

            }
        }
   
        private int _purchase_order_detail_pk;
        public int purchase_order_detail_pk
        {
            get { return _purchase_order_detail_pk; }
            set
            {
                MessageBox.Show("HIT");
            }
        }



        private ProductModel _selectedrow { get; set; }
        public ProductModel selectedrow 
        {
            get => this._selectedrow;
            set {
                if (this.selectedrow == value) return;
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

            if (BindableCollectionProductModel is null)
                {
                    BindableCollectionProductModel = new BindableCollection<ProductModel>();
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
                BindableCollectionProductModel.Add(selectedrow);
            
        }
        public void DeleteCommand()
        {
        }
        public void ResetCommand()
        {
        }

        public void LoadPurchaseOrder(int supplier_fk)
        {

            ObservableCollectionPurchaseOrderModel.Clear();
            ObservableCollectionPurchaseOrderModel = new ObservableCollection<PurchaseOrderModel>();
            dataBaseProvider = getprovider();
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
            dataBaseProvider = getprovider();

            // check for category

            DataBaseLayer.CategoryRepository CategoryRepo = new DataBaseLayer.CategoryRepository(dataBaseProvider);
            var catResult = CategoryRepo.GetCategory().Result;
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
                BindableCollectionCategoryModel = DataConversion.ToBindableCollection(catResult.ToObservable());
            }

            selectedrow = new ProductModel();
            selectedrow.category_list = new List<string>();
            selectedrow.category_dict = new Dictionary<int, string>();

           
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

                
                PurchaseOrderModel selectedlotnumber = new PurchaseOrderModel(); 
                foreach (var item in ObservableCollectionPurchaseOrderModel)
                {
                    selectedlotnumber.lot_number = item.lot_number;
                    selectedlotnumber.purchase_order_detail_pk = item.purchase_order_detail_pk;

                }

                // Load Products
                DataBaseLayer.ProductRepository ProductRepo = new DataBaseLayer.ProductRepository(dataBaseProvider);
                BindableCollectionProductModel = DataConversion.ToBindableCollection(ProductRepo.GetAll().Result.AsEnumerable());
                
                //MessageBox.Show(selectedrow.category_name);
                foreach (var item in BindableCollectionProductModel)
                {
                        selectedrow.category_name = item.category_name;
                      //  MessageBox.Show(item.category_name);
                }
                RaisePropertyChanged("category_name");
                ProductRepo.Commit();
                ProductRepo.Dispose();
            } 
        }

       

       
    }
}