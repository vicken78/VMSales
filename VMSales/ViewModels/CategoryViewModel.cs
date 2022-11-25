using VMSales.Models;
using System.Data;
using System.Collections.Generic;
using VMSales.Logic;
using System.Threading.Tasks;
using System.Collections.ObjectModel;
using System.Windows;

namespace VMSales.ViewModels
{
    
    public class CategoryViewModel : DataBaseLayer
    {


        public ObservableCollection<CategoryModel> ObservableCollectionCategoryModel;

        public CategoryViewModel()
        {
            ObservableCollectionCategoryModel = new ObservableCollection<CategoryModel>();
            IDatabaseProvider dataBaseProvider;
            dataBaseProvider = BaseViewModel.getprovider();

            CategoryRepository CategoryRepo = new CategoryRepository(dataBaseProvider);
            ObservableCollectionCategoryModel = CategoryRepo.GetAll().Result.ToObservable();

        }


        /*       private ObservableCollection<CategoryModel> _ObservableCollectionCategoryModel;
               public ObservableCollection<CategoryModel> ObservableCollectionCategoryModel
               {
                   get { return _ObservableCollectionCategoryModel; }
                   set
                   {
                       _ObservableCollectionCategoryModel = value;
                       RaisePropertyChanged("ObservableCollectionCategoryModel");
                   }
               }
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