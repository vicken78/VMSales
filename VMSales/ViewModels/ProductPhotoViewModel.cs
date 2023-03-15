using Caliburn.Micro;
using System.Windows;
using VMSales.Models;

namespace VMSales.ViewModels
{
    public class ProductPhotoViewModel
    {
        public string product_name { get; set; }
     
        public ProductPhotoViewModel(ProductModel SelectedItem)
        {
            product_name = SelectedItem.product_name;
        }
    
    }
}
