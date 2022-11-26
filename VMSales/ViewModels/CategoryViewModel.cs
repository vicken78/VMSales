using VMSales.Models;
using System.Data;
using System.Collections.Generic;
using VMSales.Logic;
using System.Threading.Tasks;
using System.Collections.ObjectModel;
using System.Windows;
using System.Windows.Data;
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
            set {
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
            // remove from observaable only if selected 0
            if (Select_Request.category_pk.ToString() == "0")
            {
                //remove code
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



        /*
        var sqlvalues = new List<Tuple<string, string>>();
        string sqlparams = "UPDATE category SET category_name = @categoey_name, description = @description WHERE category_pk = @category_pk;";

        var kvp = new List<Tuple<string, string>>();

        foreach (CategoryModel SM in SMListUpdate)
        {
            if ((SM.category_name ?? SM.description) == "")
            {
                MessageBox.Show("Name and Description cannot be empty.");
                Validated = false;
            }
            if ((SM.category_name.Length > 255 || SM.description.Length > 255))
            {
                MessageBox.Show("Name or Description exceeds max values.");
                Validated = false;
            }
            if (Validated == true)
            {
                kvp.Add(new Tuple<string, string>("category_name", SM.category_name));
                kvp.Add(new Tuple<string, string>("description", SM.description));
                kvp.Add(new Tuple<string, string>("creation_date", "now"));
                kvp.Add(new Tuple<string, string>("category_pk", SM.category_pk));
                DataBaseOps.IUDTable(sqlparams, kvp, rowcount);
                changetracker.Dispose();
            }
        }
        changetracker.ClearTracking();
    }

    /*
    public void AddCommand()
    {
        try
        {
            Boolean Validated = true;
            SMListCreate = changetracker.RowsCreated;
            int rowcount = SMListCreate.Count;
            var kvp = new List<Tuple<string, string>>();
            string sqlparams = "INSERT INTO category (category_name, description, creation_date) VALUES (@category_name, @description, DateTime('now'));";
            //check if any rows
            MessageBox.Show(rowcount.ToString());
            if (rowcount > 0)
            {
                foreach (CategoryModel SM in SMListCreate)
                {

                    if (SM.category_name == null || SM.description == null)
                    {
                        MessageBox.Show("Name and Description must contain a value");
                        Validated = false;
                    }
                    else if ((SM.category_name.Length > 255 || SM.description.Length > 255))
                    {
                        MessageBox.Show("Name or Description exceeds max values");
                        Validated = false;
                    }
                    if (Validated == true)
                    {
                        kvp.Add(new Tuple<string, string>("category_name", SM.category_name));
                        kvp.Add(new Tuple<string, string>("description", SM.description));
                        DataBaseOps.IUDTable(sqlparams, kvp, rowcount);
                        changetracker.Dispose();
                    }
                }
            }
            else { MessageBox.Show("No Data to be Inserted"); }
        }
        catch (Exception)
        {

        }
    }


    public void DeleteCommand()
    {
        try
        {
            Boolean Validated = true;
            SMListDelete = changetracker.RowsDeleted;
            int rowcount = SMListDelete.Count;
            MessageBox.Show(rowcount.ToString());
            var kvp = new List<Tuple<string, string>>();
            string sqlparams = "DELETE FROM category WHERE category_pk = @category_pk;";
            //check if any rows created
            if (rowcount > 0)
            {
                foreach (CategoryModel SM in SMListDelete)
                {
                    if (SM.category_pk == null || SM.category_pk == "")
                    {
                        Validated = false;
                    }
                    if (Validated == true)
                        kvp.Add(new Tuple<string, string>("category_pk", SM.category_pk));
                    DataBaseOps.IUDTable(sqlparams, kvp, rowcount);
                    changetracker.Dispose();
                }
            }
            else { MessageBox.Show("No Data to be Deleted"); }
        }
        catch (Exception)
        {

        }

    }


    public void ResetCommand()
    {
        changetracker.Dispose();
        changetracker = null;
        ObservableCollectionCategoryModel.Clear();
        string parameter = "category";
        DataTable dt = DataBaseOps.SQLiteDataTableWithQuery(parameter);
        foreach (DataRow row in dt.Rows)
        {
                CM = new CategoryModel()
            {
                category_pk = (string)row["category_pk"].ToString(),
                category_name = (string)row["category_name"],
                description = (string)row["description"],
                creation_date = (DateTime)row["creation_date"]
            };
        }
        if (CM.creation_date == DateTime.MinValue) 
            {
            CM.creation_date = (DateTime)DV.Convert(CM.creation_date, typeof(DateTime), DateTime.Now, US);
            ObservableCollectionCategoryModel.Add(CM);
            }

        changetracker = new ChangeTracker<CategoryModel>(ObservableCollectionCategoryModel);
        changetracker.StartTracking(ObservableCollectionCategoryModel);


    }








/*
public ChangeTracker<CategoryModel> changetracker { get; set; }
    List<CategoryModel> SMListUpdate = new List<CategoryModel>();
    List<CategoryModel> SMListCreate = new List<CategoryModel>();
    List<CategoryModel> SMListDelete = new List<CategoryModel>();


    public CategoryViewModel()
    {

        // get data
        string where = "";
        string table = "[category]";
        ObservableCollectionCategoryModel = new ObservableCollection<CategoryModel>();
    //    ObservableCollectionCategoryModel = DataConversion.ToObservable<CategoryModel>(DataBaseLayer.getTable<CategoryModel>(table, where));
        changetracker = new ChangeTracker<CategoryModel>();
        changetracker.StartTracking(ObservableCollectionCategoryModel);
    }


    //  CultureInfo US = new CultureInfo("en-US");
    //  DataValidator DV = new DataValidator();
    //  CategoryModel CM = new CategoryModel();


    //Commands
    public void SaveCommand()
    {
        Boolean Validated = true;
        SMListUpdate = changetracker.RowsUpdated;
        int rowcount = SMListUpdate.Count;

    }        
        /*
        var sqlvalues = new List<Tuple<string, string>>();
        string sqlparams = "UPDATE category SET category_name = @categoey_name, description = @description WHERE category_pk = @category_pk;";

        var kvp = new List<Tuple<string, string>>();

        foreach (CategoryModel SM in SMListUpdate)
        {
            if ((SM.category_name ?? SM.description) == "")
            {
                MessageBox.Show("Name and Description cannot be empty.");
                Validated = false;
            }
            if ((SM.category_name.Length > 255 || SM.description.Length > 255))
            {
                MessageBox.Show("Name or Description exceeds max values.");
                Validated = false;
            }
            if (Validated == true)
            {
                kvp.Add(new Tuple<string, string>("category_name", SM.category_name));
                kvp.Add(new Tuple<string, string>("description", SM.description));
                kvp.Add(new Tuple<string, string>("creation_date", "now"));
                kvp.Add(new Tuple<string, string>("category_pk", SM.category_pk));
                DataBaseOps.IUDTable(sqlparams, kvp, rowcount);
                changetracker.Dispose();
            }
        }
        changetracker.ClearTracking();
    }

    /*
    public void AddCommand()
    {
        try
        {
            Boolean Validated = true;
            SMListCreate = changetracker.RowsCreated;
            int rowcount = SMListCreate.Count;
            var kvp = new List<Tuple<string, string>>();
            string sqlparams = "INSERT INTO category (category_name, description, creation_date) VALUES (@category_name, @description, DateTime('now'));";
            //check if any rows
            MessageBox.Show(rowcount.ToString());
            if (rowcount > 0)
            {
                foreach (CategoryModel SM in SMListCreate)
                {

                    if (SM.category_name == null || SM.description == null)
                    {
                        MessageBox.Show("Name and Description must contain a value");
                        Validated = false;
                    }
                    else if ((SM.category_name.Length > 255 || SM.description.Length > 255))
                    {
                        MessageBox.Show("Name or Description exceeds max values");
                        Validated = false;
                    }
                    if (Validated == true)
                    {
                        kvp.Add(new Tuple<string, string>("category_name", SM.category_name));
                        kvp.Add(new Tuple<string, string>("description", SM.description));
                        DataBaseOps.IUDTable(sqlparams, kvp, rowcount);
                        changetracker.Dispose();
                    }
                }
            }
            else { MessageBox.Show("No Data to be Inserted"); }
        }
        catch (Exception)
        {

        }
    }


    public void DeleteCommand()
    {
        try
        {
            Boolean Validated = true;
            SMListDelete = changetracker.RowsDeleted;
            int rowcount = SMListDelete.Count;
            MessageBox.Show(rowcount.ToString());
            var kvp = new List<Tuple<string, string>>();
            string sqlparams = "DELETE FROM category WHERE category_pk = @category_pk;";
            //check if any rows created
            if (rowcount > 0)
            {
                foreach (CategoryModel SM in SMListDelete)
                {
                    if (SM.category_pk == null || SM.category_pk == "")
                    {
                        Validated = false;
                    }
                    if (Validated == true)
                        kvp.Add(new Tuple<string, string>("category_pk", SM.category_pk));
                    DataBaseOps.IUDTable(sqlparams, kvp, rowcount);
                    changetracker.Dispose();
                }
            }
            else { MessageBox.Show("No Data to be Deleted"); }
        }
        catch (Exception)
        {

        }

    }


    public void ResetCommand()
    {
        changetracker.Dispose();
        changetracker = null;
        ObservableCollectionCategoryModel.Clear();
        string parameter = "category";
        DataTable dt = DataBaseOps.SQLiteDataTableWithQuery(parameter);
        foreach (DataRow row in dt.Rows)
        {
                CM = new CategoryModel()
            {
                category_pk = (string)row["category_pk"].ToString(),
                category_name = (string)row["category_name"],
                description = (string)row["description"],
                creation_date = (DateTime)row["creation_date"]
            };
        }
        if (CM.creation_date == DateTime.MinValue) 
            {
            CM.creation_date = (DateTime)DV.Convert(CM.creation_date, typeof(DateTime), DateTime.Now, US);
            ObservableCollectionCategoryModel.Add(CM);
            }

        changetracker = new ChangeTracker<CategoryModel>(ObservableCollectionCategoryModel);
        changetracker.StartTracking(ObservableCollectionCategoryModel);


    }


    public CategoryViewModel()
    {
        ObservableCollectionCategoryModel = new ObservableCollection<CategoryModel>();
        string parameter = "category";
        DataTable dt = DataBaseOps.SQLiteDataTableWithQuery(parameter);
        foreach (DataRow row in dt.Rows) // won't run if null
        {
            CM = new CategoryModel()
            {
                category_pk = (string)row["category_pk"].ToString(),
                category_name = (string)row["category_name"],
                description = (string)row["description"],
                creation_date = (DateTime)row["creation_date"]
            };
            ObservableCollectionCategoryModel.Add(CM);
        }


        if (CM.creation_date == DateTime.MinValue)
        {
            CM.creation_date = (DateTime)DV.Convert(CM.creation_date, typeof(DateTime), DateTime.Now, US);
            ObservableCollectionCategoryModel.Add(CM);
        }

        changetracker = new ChangeTracker<CategoryModel>(ObservableCollectionCategoryModel);
        changetracker.StartTracking(ObservableCollectionCategoryModel);
    }*/



    }
}