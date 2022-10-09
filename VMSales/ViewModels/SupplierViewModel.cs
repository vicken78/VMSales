using Caliburn.Micro;
using VMSales.Models;
using System;
using System.Data;
using System.Windows.Controls;
using System.Windows;
using System.Collections.ObjectModel;
using System.Collections.Generic;
using VMSales.ChangeTrack;
using System.Collections.Specialized;
using System.Reflection;
using System.Diagnostics;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;
using System.Windows.Data;
using VMSales.Database;

namespace VMSales.ViewModels
{
    public class SupplierViewModel
    {
        public DataGrid SupplierDataGrid;
        public ObservableCollection<SupplierModel> ObservableCollectionSupplierModel { get; private set; }
        public ChangeTracker<SupplierModel> changetracker { get; set; }
        List<SupplierModel> SMListUpdate = new List<SupplierModel>();
        List<SupplierModel> SMListCreate = new List<SupplierModel>();
        List<SupplierModel> SMListDelete = new List<SupplierModel>();

        //Commands
        public void SaveCommand()
        {
            Boolean Validated = true;
            SMListUpdate = changetracker.RowsUpdated;
            int rowcount = SMListUpdate.Count;
            var sqlvalues = new List<Tuple<string, string>>();
            string sqlparams = "UPDATE supplier SET sname = @sname, address = @address, city = @city, state = @state, Zip = @zip, country = @country, phone = @phone,  email = @email WHERE supplier_pk = @supplier_pk;";

            var kvp = new List<Tuple<string, string>>();

            foreach (SupplierModel SM in SMListUpdate)
            {
                // Email is allowed to be null.  if null, replace value
                if (SM.Email == "")
                {
                    SM.Email = "null";
                }
                if ((SM.Sname ?? SM.Address ?? SM.City ?? SM.State ?? SM.Zip ?? SM.Country ?? SM.Phone ?? SM.Supplier_pk) == "")
                {
                    MessageBox.Show("Name, Address, City, State, Zip, Country, and Phone must contain a value");
                    Validated = false;
                }
                if ((SM.Sname.Length > 255 || SM.Address.Length > 255 || SM.City.Length > 255 || SM.Zip.Length > 9 || SM.State.Length > 50 || SM.Country.Length > 50 || SM.Phone.Length > 25 || SM.Email.Length > 50))
                {
                    MessageBox.Show("Name, Address, City, State, Zip, Country, and Phone exceeds max values");
                    Validated = false;
                }
                if (Validated == true)
                {
                    kvp.Add(new Tuple<string, string>("Sname", SM.Sname));
                    kvp.Add(new Tuple<string, string>("Address", SM.Address));
                    kvp.Add(new Tuple<string, string>("City", SM.City));
                    kvp.Add(new Tuple<string, string>("State", SM.State));
                    kvp.Add(new Tuple<string, string>("Zip", SM.Zip));
                    kvp.Add(new Tuple<string, string>("Country", SM.Country));
                    kvp.Add(new Tuple<string, string>("Phone", SM.Phone));
                    kvp.Add(new Tuple<string, string>("Email", SM.Email));
                    kvp.Add(new Tuple<string, string>("Supplier_pk", SM.Supplier_pk));
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
                string sqlparams = "INSERT INTO supplier (sname, address, city, state, zip, country, phone, email) VALUES (@sname, @address, @city, @state, @zip, @country, @phone, @email);";
                //check if any rows created
                if (rowcount > 0)
                {
                    foreach (SupplierModel SM in SMListCreate)
                    {
             
                        // Email is allowed to be null.  if null, replace value
                        if (SM.Email == null)
                        {
                            SM.Email = "null";
                        }
                        if (SM.Address == null || SM.City == null || SM.Country== null || SM.Phone == null)
                        {
                            MessageBox.Show("Name, Address, City, State, Zip, Country, and Phone must contain a value");
                            Validated = false;
                        }
                        else if ((SM.Sname.Length > 255 || SM.Address.Length > 255 || SM.City.Length > 255 || SM.Zip.Length > 9 || SM.State.Length > 50 || SM.Country.Length > 50 || SM.Phone.Length > 25 || SM.Email.Length > 50))
                        {
                            MessageBox.Show("Name, Address, City, State, Zip, Country, and Phone exceeds max values");
                            Validated = false;
                        }
                        if (Validated == true)
                        {
                            kvp.Add(new Tuple<string, string>("Sname", SM.Sname));
                            kvp.Add(new Tuple<string, string>("Address", SM.Address));
                            kvp.Add(new Tuple<string, string>("City", SM.City));
                            kvp.Add(new Tuple<string, string>("State", SM.State));
                            kvp.Add(new Tuple<string, string>("Zip", SM.Zip));
                            kvp.Add(new Tuple<string, string>("Country", SM.Country));
                            kvp.Add(new Tuple<string, string>("Phone", SM.Phone));
                            kvp.Add(new Tuple<string, string>("Email", SM.Email));
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
                var kvp = new List<Tuple<string, string>>();
                string sqlparams = "DELETE FROM supplier WHERE supplier_pk = @supplier_pk;";
                //check if any rows created
                if (rowcount > 0)
                {
                    foreach (SupplierModel SM in SMListDelete)
                    {

                        if (SM.Supplier_pk == null || SM.Supplier_pk == "")
                        {
                            Validated = false;
                        }
                        if (Validated == true)
                        kvp.Add(new Tuple<string, string>("Supplier_pk", SM.Supplier_pk));
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
            ObservableCollectionSupplierModel.Clear();
            string parameter = "supplier";
            DataTable dt = DataBaseOps.SQLiteDataTableWithQuery(parameter);
            foreach (DataRow row in dt.Rows)
            {
                var obj = new SupplierModel()
                {
                    Supplier_pk = (string)row["supplier_pk"].ToString(),
                    Sname = (string)row["sname"],
                    Address = (string)row["address"],
                    City = (string)row["city"],
                    State = (string)row["state"],
                    Country = (string)row["country"],
                    Zip = (string)row["zip"],
                    Phone = (string)row["phone"],
                    Email = (string)row["email"]
                };
                ObservableCollectionSupplierModel.Add(obj);
            }
           
            changetracker = new ChangeTracker<SupplierModel>(ObservableCollectionSupplierModel);
            changetracker.StartTracking(ObservableCollectionSupplierModel);


        }

        public SupplierViewModel()
        {
            ObservableCollectionSupplierModel = new ObservableCollection<SupplierModel>();
            string parameter = "supplier";
            DataTable dt = DataBaseOps.SQLiteDataTableWithQuery(parameter);
            foreach (DataRow row in dt.Rows)
            {
                var obj = new SupplierModel()
                {
                    Supplier_pk = (string)row["supplier_pk"].ToString(),
                    Sname = (string)row["sname"],
                    Address = (string)row["address"],
                    City = (string)row["city"],
                    State = (string)row["state"],
                    Country = (string)row["country"],
                    Zip = (string)row["zip"],
                    Phone = (string)row["phone"],
                    Email = (string)row["email"]
                };
                ObservableCollectionSupplierModel.Add(obj);
            }

            changetracker = new ChangeTracker<SupplierModel>(ObservableCollectionSupplierModel);
            changetracker.StartTracking(ObservableCollectionSupplierModel);
        }
    }
}