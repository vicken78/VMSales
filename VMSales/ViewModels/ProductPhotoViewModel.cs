using System;
using System.Collections.Generic;
using System.Linq;
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
        public string photofilePath { get; set; }
 
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
            PhotoModel photoModel = new PhotoModel();
            photoModel.product_fk = product_fk;
            photoModel.photo_path = photofilePath;
            IDatabaseProvider dataBaseProvider;
            dataBaseProvider = getprovider();
            DataBaseLayer.PhotoRepository PhotoRepo = new DataBaseLayer.PhotoRepository(dataBaseProvider);
            // 1. get next photo order
            try
            {
                int pphoto_pk;
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
                    // 1. get next photo order
                    IEnumerable<int> next_photo_order_number = await PhotoRepo.GetNextPos(product_fk);
                    photoModel.photo_order_number = next_photo_order_number.Single();

                    // 2. Check if same photo_path exists.
                    string saved_photo_path = await PhotoRepo.GetPhotoPath(photoModel.photo_path);
                    if (saved_photo_path == photoModel.photo_path)
                    {
                        MessageBox.Show("A photo with the same path already exists");
                        PhotoRepo.Commit();
                        PhotoRepo.Dispose();
                        return;
                    }

                    // 3. Save to photo. get photo_pk

                    // insert into photo
                    photoModel.photo_fk = await PhotoRepo.Insert(photoModel);
                    // 4. Save to product_photo, insert photo_pk and product_fk
                    if (photoModel.photo_fk > 0)
                    {
                        pphoto_pk = await PhotoRepo.InsertProductPhoto(photoModel);
                        if (pphoto_pk > 0)
                        PhotoRepo.Commit();
                        PhotoRepo.Dispose();

                        MessageBox.Show("Saved.");
                            CancelCommand(); // close save window
                    }
                }
                else
                {
                    MessageBox.Show("An Error has occured. No Image Saved.");
                }
                PhotoRepo.Commit();
                PhotoRepo.Dispose();
            }
            catch (Exception e)
            {
                MessageBox.Show("An Unexpected Error has occured. No Image has been saved" + e);
                PhotoRepo.Dispose();
            }

  
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
            photofilePath = filePath;
            SelectedImage = new BitmapImage(new Uri(filePath));
        }
    }
}