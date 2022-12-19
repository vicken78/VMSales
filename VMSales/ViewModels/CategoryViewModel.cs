using VMSales.Models;
using VMSales.Logic;
using System.Threading.Tasks;
using System.Collections.ObjectModel;
using System.Windows;
using System;

namespace VMSales.ViewModels
{

    public class CategoryViewModel : BaseViewModel
    {

        private CategoryModel select_request;
        public CategoryModel Select_Request
        {
            get { return select_request; }
            set
            {
                select_request = value;
                RaisePropertyChanged("Select_Request");
            }
        }

        private ObservableCollection<CategoryModel> ObservableCollectionCategoryModelclean { get; set; }
        public ObservableCollection<CategoryModel> ObservableCollectionCategoryModel
        {
            get { return ObservableCollectionCategoryModelclean; }
            set
            {
                if (ObservableCollectionCategoryModelclean == value) return;
                ObservableCollectionCategoryModelclean = value;
                RaisePropertyChanged("ObservableCollectionCategoryModel");
            }
        }

        IDatabaseProvider dataBaseProvider;

        //Commands
        public void SaveCommand()
        {
            // set db value to null until checked
            String db_category_pk = null;
            // nothing selected
            if (Select_Request == null)
            {
                MessageBox.Show("No Changes Were Made.");
                return;
            }
            // check for default values
            if (Select_Request.category_name.ToString() == "Name")
            {
                MessageBox.Show("Default Name Must Not be Used");
                return;
            }
            if (Select_Request.description.ToString() == "Description")
            {
                MessageBox.Show("Default Description Must Not be Used");
                return;
            }
            // all values good, now we must update or insert, get primary key.
            dataBaseProvider = BaseViewModel.getprovider();
            DataBaseLayer.CategoryRepository CategoryRepo = new DataBaseLayer.CategoryRepository(dataBaseProvider);
            try
            {
                db_category_pk = CategoryRepo.Get(select_request.category_pk).Result.category_pk.ToString();
            }
            catch (AggregateException e) // primary key does not exist
            {
                // insert
                Task<bool> insertCategory = CategoryRepo.Insert(Select_Request);
                if (insertCategory.Result == true)
                {
                    CategoryRepo.Commit();
                    CategoryRepo.Dispose();
                }
                else
                {
                    MessageBox.Show("An Error has occured with inserting.  Insertion Rejected");
                    CategoryRepo.Revert();
                    CategoryRepo.Dispose();
                }
                return;
            }
            catch (Exception e)
            {  // any other error
                MessageBox.Show(e.ToString());
                CategoryRepo.Revert();
                CategoryRepo.Dispose();
                return;
            }
            // if id match UPDATE
            if (db_category_pk == select_request.category_pk.ToString())
            {
                Task<bool> updateCategory = CategoryRepo.Update(Select_Request);
                if (updateCategory.Result == true)
                {
                    CategoryRepo.Commit();
                    CategoryRepo.Dispose();
                    MessageBox.Show("Saved");
                }
                else
                {
                    MessageBox.Show("An Error has occured with Updating.  Updating Rejected");
                    CategoryRepo.Revert();
                    CategoryRepo.Dispose();
                }
                return;
            }
        }

        public void ResetCommand()
        {
        }
        public void AddCommand()
        {
            var obj = new CategoryModel()
            {
                category_name = "Name",
                description = "Description",
                creation_date = System.DateTime.MinValue,
            };
            ObservableCollectionCategoryModel.Add(obj);
            RaisePropertyChanged("ObservableCollectionCategoryModel");
        }
        public void DeleteCommand()
        {
            // nothing selected
            if (Select_Request == null)
            {
                MessageBox.Show("No Changes Were Made.");
                return;
            }
            // remove from observable only if selected 0
            if (Select_Request.category_pk.ToString() == "0")
            {
                //remove code to be here
                return;
            }
            // set db value to null until checked
            String db_category_pk = null;

            // get primary key.
            dataBaseProvider = BaseViewModel.getprovider();
            DataBaseLayer.CategoryRepository CategoryRepo = new DataBaseLayer.CategoryRepository(dataBaseProvider);
            try
            {
                db_category_pk = CategoryRepo.Get(select_request.category_pk).Result.category_pk.ToString();
            }
            catch (Exception e)
            {  // catch all errors
                MessageBox.Show(e.ToString());
                CategoryRepo.Revert();
                CategoryRepo.Dispose();
                return;
            }
            // if id match DELETE
            if (db_category_pk == select_request.category_pk.ToString())
            {
                Task<bool> deleteCategory = CategoryRepo.Delete(Select_Request);
                if (deleteCategory.Result == true)
                {
                    CategoryRepo.Commit();
                    CategoryRepo.Dispose();
                    MessageBox.Show("Row Deleted");
                    // we are not refreshing for delete. we need to remove it from observable and refresh.
                }
                else
                {
                    MessageBox.Show("An Error has occured with Deleting.  Rejected");
                    CategoryRepo.Revert();
                    CategoryRepo.Dispose();
                }
                return;
            }
        }

        public CategoryViewModel()
        {
            ObservableCollectionCategoryModel = new ObservableCollection<CategoryModel>();
            dataBaseProvider = BaseViewModel.getprovider();
            DataBaseLayer.CategoryRepository CategoryRepo = new DataBaseLayer.CategoryRepository(dataBaseProvider);
            ObservableCollectionCategoryModel = CategoryRepo.GetAll().Result.ToObservable();
            ObservableCollectionCategoryModelclean = ObservableCollectionCategoryModel;
            CategoryRepo.Commit();
            CategoryRepo.Dispose();
        }
    }
}