using Caliburn.Micro;
using System;
using System.Linq;
using System.Windows;
using System.Windows.Media.Imaging;
using VMSales.Models;

namespace VMSales.ViewModels
{
    public class ProductPhotoViewModel : BaseViewModel
    {

        public int product_fk { get; set; }
        public string product_name { get; set; }
        private BitmapImage _selectedImage;
        public BitmapImage SelectedImage
        {
            get { return _selectedImage; }
            set
            {
                if (_selectedImage == value) return;
                _selectedImage = value;
                RaisePropertyChanged("SelectedImage");
            }
        }

        public void SaveCommand() 
        {
            // 1. get next photo order
            // 2. Save to photo. get photo_pk
            // 3. Save to product_photo, insert photo_pk and product_fk
        }
        public void CancelCommand() 
        {
            var window = Application.Current.Windows.OfType<Window>().SingleOrDefault(w => w.DataContext == this);
            if (window != null)
            {
                window.Close();
            }
        }

        public ProductPhotoViewModel(ProductModel SelectedItem, string filePath)
        {
            product_fk = SelectedItem.product_pk;
            product_name = SelectedItem.product_name;
            SelectedImage = new BitmapImage(new Uri(filePath));
        }
    }
}