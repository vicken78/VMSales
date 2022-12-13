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
        public ObservableCollection<SupplierModel> ObservableCollectionSupplierModel { get; set; }
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
            var obj = new ProductModel()
            {
                //productCondition = new List<String> { "New", "Used" },
                brand_name = "",
                product_name = "Name",
                description = "Description",
                quantity = "0",
                cost = 0,
                sku = "0",
                sold_price = 0,
                instock = 1,
                listing_url = "",
                listing_number = "",
                listing_date = DateTime.Now,
            };
        }
        public void DeleteCommand()
        {
        }
        public void ResetCommand()
        {
        }

        public void LoadPurchaseOrder(int supplier_fk)
        {
            ObservableCollectionPurchaseOrderModel = new ObservableCollection<PurchaseOrderModel>();
            dataBaseProvider = BaseViewModel.getprovider();
            DataBaseLayer.PurchaseOrderRepository PurchaseOrderRepo = new DataBaseLayer.PurchaseOrderRepository(dataBaseProvider);
            var Result = PurchaseOrderRepo.GetAllWithID(supplier_fk).Result;

            if (Result.Count() == 0)
            {
                MessageBox.Show("You must add purchase orders.");
                PurchaseOrderRepo.Revert();
                PurchaseOrderRepo.Dispose();
                return;
            }
            else
            {
                ObservableCollectionPurchaseOrderModel = Result.ToObservable();
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
        }
    }
}

    