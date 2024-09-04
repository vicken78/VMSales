using VMSales.Models;
using VMSales.Logic;
using System.Threading.Tasks;
using System.Collections.ObjectModel;
using System.Windows;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Windows.Media;

namespace VMSales.ViewModels
{

    public class CategoryViewModel : BaseViewModel
    {

        #region INotify Private
        private int _category_pk;
        private string _category_name;
        private string _description;
        private DateTime _creation_date;
        private HashSet<CategoryModel> modifiedItems;
        private CategoryModel select_request;
        private ObservableCollection<CategoryModel> ObservableCollectionCategoryModelclean { get; set; }
        #endregion
        #region INotify Public
        public int category_pk
        {
            get { return _category_pk; }
            set
            {
                if (_category_pk == value) return;
                _category_pk = value;
                RaisePropertyChanged("category_pk");
            }
        }
        public string category_name
        {
            get { return _category_name; }
            set
            {
                //if (_category_name == value) return;
                _category_name = value;
                IsDirty = true;
                FontColor = Brushes.Red;
                RaisePropertyChanged(nameof(category_name));
                MessageBox.Show("S");
            }
        }
        public string description
        {
            get { return _description; }
            set
            {
                //if (_description == value) return;
                _description = value;
                IsDirty = true;
                FontColor = Brushes.Red;
                RaisePropertyChanged(nameof(description));
                MessageBox.Show("S");
            }
        }
        public System.DateTime creation_date
        {
            get { return _creation_date; }
            set
            {
                if (_creation_date == value) return;
                _creation_date = value;
                IsDirty = true;
                FontColor = Brushes.Red;
                RaisePropertyChanged(nameof(creation_date));
            }
        }

        public CategoryModel Select_Request
        {
            get { return select_request; }
            set
            {
                if (select_request == value) return;
                select_request = value;
                RaisePropertyChanged("Select_Request");
  
            }
        }
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


        #endregion

        IDatabaseProvider dataBaseProvider;

        //Commands
        public void SaveCommand()
        {

            foreach (var item in modifiedItems)
            {
                // Implement logic to save changes to the database
                MessageBox.Show(item.category_name);
                MessageBox.Show(item.description);
            }

            // Clear the modified items after saving

            /*
            // on successful save
            changecolor = 0;
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
            dataBaseProvider = getprovider();
            DataBaseLayer.CategoryRepository CategoryRepo = new DataBaseLayer.CategoryRepository(dataBaseProvider);
            try
            {
                db_category_pk = CategoryRepo.Get(select_request.category_pk).Result.category_pk.ToString();
            }
            catch (AggregateException) // primary key does not exist
            {
                // insert
                Task<int> insertCategory = CategoryRepo.Insert(Select_Request);
                if (insertCategory.Result > 0)
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

            */
        }

        public void ResetCommand()
        {

            // needs to be implemented.
            ObservableCollectionCategoryModel.Clear();
            initial_load();
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
            dataBaseProvider = getprovider();
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

        // Event handler for when a property in a CategoryModel changes
        private void OnCategoryModelChanged(object sender, PropertyChangedEventArgs e)
        {
            if (sender is CategoryModel model)
            {
                modifiedItems.Add(model);
                MessageBox.Show(model.description);
            }

        }

        public void initial_load() 
            {
                ObservableCollectionCategoryModel = new ObservableCollection<CategoryModel>();
                modifiedItems = new HashSet<CategoryModel>();
                dataBaseProvider = getprovider();
                DataBaseLayer.CategoryRepository CategoryRepo = new DataBaseLayer.CategoryRepository(dataBaseProvider);
                ObservableCollectionCategoryModel = CategoryRepo.GetAll().Result.ToObservable();
                ObservableCollectionCategoryModelclean = ObservableCollectionCategoryModel;
                CategoryRepo.Commit();
                CategoryRepo.Dispose();
                Initialize();
          


        }


            public CategoryViewModel()
        {
            initial_load();
        }
      
        }
}