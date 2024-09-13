using VMSales.Models;
using VMSales.Logic;
using System.Collections.ObjectModel;
using System.Windows;
using System.Linq;
using System;
using System.Threading.Tasks;

namespace VMSales.ViewModels
{
    public class CategoryViewModel : BaseViewModel
    {
        private ObservableCollection<CategoryModel> _ObservableCollectionCategoryModelDirty { get; set; }

        public ObservableCollection<CategoryModel> ObservableCollectionCategoryModelDirty
        {
            get => _ObservableCollectionCategoryModelDirty; 
            set
            {
                _ObservableCollectionCategoryModelDirty = value;
                NotifyOfPropertyChange(() => ObservableCollectionCategoryModelDirty);
            }
        }
        public ObservableCollection<CategoryModel> ObservableCollectionCategoryModelClean { get; protected set; }

        IDatabaseProvider dataBaseProvider;

        //Commands
        public async Task SaveCommand()
        {
            // Create an instance of DataProcessor with CategoryModel type
            var dataProcessor = new DataProcessor<CategoryModel>();

            // Call the Compare method
            ObservableCollection<CategoryModel> differences = dataProcessor.Compare(ObservableCollectionCategoryModelClean, ObservableCollectionCategoryModelDirty);

            foreach (var item in differences)
            {
                DataBaseLayer.CategoryRepository CategoryRepo = new DataBaseLayer.CategoryRepository(dataBaseProvider);

                try
                {
                    // implement check for foreign keys, if foreign key exists, warn user.
                    switch (item.Action)
                    {
                        case "Update":
                            bool Update_Category = CategoryRepo.Update(item).Result;
                            if (Update_Category == false)
                            { throw new Exception("Update Failed"); }
                            else
                            CategoryRepo.Commit();
                            break;
                        case "Insert":
                            int category_pk = await CategoryRepo.Insert(item);
                            if (category_pk == 0)
                            { throw new Exception("Insert Failed"); }
                            else
                                CategoryRepo.Commit();
                            break;
                        case "Delete":
                            bool Delete_Category = CategoryRepo.Delete(item).Result;
                            if (Delete_Category == false)
                            { throw new Exception("Delete Failed"); }
                            else
                                CategoryRepo.Commit();
                            break;
                        default:
                            throw new InvalidOperationException("Unexpected Action read, expected Update Insert or Delete.");
                    }
                    CategoryRepo.Dispose();
                    initial_load();
                }
                catch (Exception e)
                {
                    MessageBox.Show("An unexpected error has occured." + e);
                    CategoryRepo.Revert();
                    CategoryRepo.Dispose();
                }
            }
        } 

        public void ResetCommand()
        {
            ObservableCollectionCategoryModelDirty.Clear();
            ObservableCollectionCategoryModelClean.Clear();
            initial_load();
        }
     
        public void initial_load() 
            {
                ObservableCollectionCategoryModelDirty = new ObservableCollection<CategoryModel>();
                ObservableCollectionCategoryModelClean = new ObservableCollection<CategoryModel>();

                dataBaseProvider = getprovider();
                DataBaseLayer.CategoryRepository CategoryRepo = new DataBaseLayer.CategoryRepository(dataBaseProvider);
                ObservableCollectionCategoryModelDirty = CategoryRepo.GetAll().Result.ToObservable();
        
            ObservableCollectionCategoryModelClean = new ObservableCollection<CategoryModel>(ObservableCollectionCategoryModelDirty.Select(item => new CategoryModel
            {
                // Copy properties from the item, or use a copy constructor if available
                category_pk = item.category_pk,
                category_name = item.category_name,
                description = item.description,
                creation_date = item.creation_date,  
            }
            ));
            CategoryRepo.Commit();
            CategoryRepo.Dispose();
            }
        
        public CategoryViewModel()
            {
              initial_load();
            }
    }
}