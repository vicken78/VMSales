using VMSales.Models;
using System;
using System.Data;
using System.Windows.Controls;
using System.Windows;
using System.Collections.ObjectModel;
using System.Collections.Generic;
using VMSales.ChangeTrack;
using VMSales.Database;
using VMSales.Logic;
using System.Globalization;

namespace VMSales.ViewModels
{
    public class CategoryViewModel
    { 
        CultureInfo US = new CultureInfo("en-US");
        DataValidator DV = new DataValidator();
        CategoryModel CM = new CategoryModel();
        public DataGrid CategoryDataGrid;
        public ObservableCollection<CategoryModel> ObservableCollectionCategoryModel { get; private set; }
        public ChangeTracker<CategoryModel> changetracker { get; set; }
        List<CategoryModel> SMListUpdate = new List<CategoryModel>();
        List<CategoryModel> SMListCreate = new List<CategoryModel>();
        List<CategoryModel> SMListDelete = new List<CategoryModel>();
        

            //Commands
            public void SaveCommand()
        {
            Boolean Validated = true;
            SMListUpdate = changetracker.RowsUpdated;
            int rowcount = SMListUpdate.Count;
            var sqlvalues = new List<Tuple<string, string>>();
            string sqlparams = "UPDATE category SET cname = @cname, description = @description WHERE category_pk = @category_pk;";

            var kvp = new List<Tuple<string, string>>();

            foreach (CategoryModel SM in SMListUpdate)
            {
                if ((SM.categoryName ?? SM.Description) == "")
                {
                    MessageBox.Show("Name and Description cannot be empty.");
                    Validated = false;
                }
                if ((SM.categoryName.Length > 255 || SM.Description.Length > 255))
                {
                    MessageBox.Show("Name or Description exceeds max values.");
                    Validated = false;
                }
                if (Validated == true)
                {
                    kvp.Add(new Tuple<string, string>("Cname", SM.categoryName));
                    kvp.Add(new Tuple<string, string>("Description", SM.Description));
                    kvp.Add(new Tuple<string, string>("Date", "now"));
                    kvp.Add(new Tuple<string, string>("Category_pk", SM.Category_pk));
                    DataBaseOps.IUDTable(sqlparams, kvp, rowcount);
                    changetracker.Dispose();
                }
            }
            changetracker.ClearTracking();
        }

        public void AddCommand()
        {
            try
            {
                Boolean Validated = true;
                SMListCreate = changetracker.RowsCreated;
                int rowcount = SMListCreate.Count;
                var kvp = new List<Tuple<string, string>>();
                string sqlparams = "INSERT INTO category (cname, description, creationdate) VALUES (@cname, @description, DateTime('now'));";
                //check if any rows
                MessageBox.Show(rowcount.ToString());
                if (rowcount > 0)
                {
                    foreach (CategoryModel SM in SMListCreate)
                    {

                        if (SM.categoryName == null || SM.Description == null)
                        {
                            MessageBox.Show("Name and Description must contain a value");
                            Validated = false;
                        }
                        else if ((SM.categoryName.Length > 255 || SM.Description.Length > 255))
                        {
                            MessageBox.Show("Name or Description exceeds max values");
                            Validated = false;
                        }
                        if (Validated == true)
                        {
                            kvp.Add(new Tuple<string, string>("cname", SM.categoryName));
                            kvp.Add(new Tuple<string, string>("description", SM.Description));
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
                        if (SM.Category_pk == null || SM.Category_pk == "")
                        {
                            Validated = false;
                        }
                        if (Validated == true)
                            kvp.Add(new Tuple<string, string>("Category_pk", SM.Category_pk));
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
                    Category_pk = (string)row["category_pk"].ToString(),
                    categoryName = (string)row["cname"],
                    Description = (string)row["description"],
                    CreationDate = (DateTime)row["creationdate"]
                };
            }
            if (CM.CreationDate == DateTime.MinValue) 
                {
                CM.CreationDate = (DateTime)DV.Convert(CM.CreationDate, typeof(DateTime), DateTime.Now, US);
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
                    Category_pk = (string)row["category_pk"].ToString(),
                    categoryName = (string)row["cname"],
                    Description = (string)row["description"],
                    CreationDate = (DateTime)row["creationdate"]
                };
                ObservableCollectionCategoryModel.Add(CM);
            }


            if (CM.CreationDate == DateTime.MinValue)
            {
                CM.CreationDate = (DateTime)DV.Convert(CM.CreationDate, typeof(DateTime), DateTime.Now, US);
                ObservableCollectionCategoryModel.Add(CM);
            }
          
            changetracker = new ChangeTracker<CategoryModel>(ObservableCollectionCategoryModel);
            changetracker.StartTracking(ObservableCollectionCategoryModel);
        }
    }
}