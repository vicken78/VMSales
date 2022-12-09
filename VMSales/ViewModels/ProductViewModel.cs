using System;
using System.Collections.ObjectModel;
using System.Data;
using VMSales.Models;
using VMSales.Logic;
using System.Collections.Generic;
using System.ComponentModel;
using VMSales.ChangeTrack;
using System.Windows.Forms;
using System.Windows.Data;

namespace VMSales.ViewModels
{
    public class ProductViewModel : BaseViewModel
    {
        public IList<PurchaseOrderModel> PurchaseOrder = new List<PurchaseOrderModel>();
        private IDatabaseProvider dataBaseProvider;

        public void SaveCommand()
        {

        }
        public void AddCommand()
        {
            var obj = new ProductModel()
            {
                productCondition = new List<String> { "New", "Used" },
                productBrandName = "",
                productName = "Name",
                productDescription = "Description",
                productQuantity = "0",
                productCost = 0,
                productSKU = "0",
                productSoldPrice = 0,
                productStock = 1,
                productListingURL = "",
                productListingNumber = "",
                productListingDate = DateTime.Now,
            };

        }
        public void DeleteCommand()
        {
        }
        public void ResetCommand()
        {
        }

        public ProductViewModel()
        {
            dataBaseProvider = BaseViewModel.getprovider();
            DataBaseLayer.PurchaseOrderRepository PurchaseRepo = new DataBaseLayer.PurchaseOrderRepository(dataBaseProvider);
            PurchaseOrder = (IList<PurchaseOrderModel>)PurchaseRepo.GetAll().Result;
            PurchaseRepo.Commit();
            PurchaseRepo.Dispose();

            /*ObservableCollectionCategoryModel = new ObservableCollection<CategoryModel>();
            ObservableCollectionCategoryModel = CategoryRepo.GetAll().Result.ToObservable();
            ObservableCollectionCategoryModelclean = ObservableCollectionCategoryModel;
            CategoryRepo.Commit();
            CategoryRepo.Dispose();
            */
        }
    }
}