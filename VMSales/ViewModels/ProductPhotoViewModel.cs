using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Media.Imaging;
using VMSales.Logic;
using VMSales.Models;

namespace VMSales.ViewModels
{
    public class ProductPhotoViewModel : BaseViewModel
    {

        public int product_fk { get; set; }
        public int photo_pk { get; set; }
        public int photo_fk { get; set; }
        public int next_photo_order_number { get; set; }
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

        public async void SaveCommand()
        {

            IDatabaseProvider dataBaseProvider;
            dataBaseProvider = getprovider();
            DataBaseLayer.PhotoRepository PhotoRepo = new DataBaseLayer.PhotoRepository(dataBaseProvider);
            // 1. get next photo order
            try
            {
                // temp for testing, remove later.
                product_fk = 5;
                // 

                IEnumerable<int> imagePositions = await PhotoRepo.GetImagePos(product_fk);
                List<int> photoOrderNum = imagePositions.Select(x => x).ToList();

                // Sort
                if (photoOrderNum.Count > 1)
                {
                    photoOrderNum.Sort();
                }

                bool isInOrder = photoOrderNum.SequenceEqual(Enumerable.Range(1, photoOrderNum.Count));

                if (isInOrder)
                {
                    // get next photo order
                    IEnumerable<int> next_photo_order_number = await PhotoRepo.GetNextPos(product_fk);
                    int photo_order_number = next_photo_order_number.Single();
                    
                    // insert into photo

                    // get photo_pk, insert into product_photo


                }
                else
                {
                    // reorg then call insert
                    MessageBox.Show("outoforg");
                }

                foreach (var item in photoOrderNum)
                {
                    MessageBox.Show(item.ToString());
                }

                PhotoRepo.Commit();
                PhotoRepo.Dispose();



            }
            catch (Exception e)
            {
                MessageBox.Show("An Unexpected Error has occured." + e);
                PhotoRepo.Dispose();
            }


            // 2. resort if needed

            // 3. Save to photo. get photo_pk

            // 4. Save to product_photo, insert photo_pk and product_fk
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