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
using System.Linq;

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

            if (PurchaseOrder.Count() == 0)
            {
                PurchaseOrder.Add(new PurchaseOrderModel()
                {
                    purchase_order_pk = 0,
                    purchase_order_fk = 0,
                    purchase_order_detail_pk = 0,
                    supplier_fk = 0,
                    invoice_number = "0",
                    purchase_date = DateTime.MinValue,
                    lot_cost = 0,
                    lot_quantity = 0,
                    lot_number = "0",
                    lot_name = "Name",
                    lot_description = "",
                    sales_tax = 0,
                    shipping_cost = 0
                }); 
            }

            foreach (var item in PurchaseOrder)
            {
                MessageBox.Show(item.lot_number);
            }
   
            /*ObservableCollectionCategoryModel = new ObservableCollection<CategoryModel>();
            ObservableCollectionCategoryModel = CategoryRepo.GetAll().Result.ToObservable();
            ObservableCollectionCategoryModelclean = ObservableCollectionCategoryModel;
            CategoryRepo.Commit();
            CategoryRepo.Dispose();
            */
        }
    }
}