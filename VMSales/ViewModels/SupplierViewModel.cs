using Caliburn.Micro;
using VMSales.Models;
using System;
using System.Data;
using System.Collections.ObjectModel;
using System.Collections.Generic;
using VMSales.ChangeTrack;
using VMSales.Database;
using System.Windows.Forms;

namespace VMSales.ViewModels
{
    public class SupplierViewModel : BaseViewModel
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
            string sqlparams = "UPDATE supplier SET supplier_name = @supplier_name, address = @address, city = @city, state = @state, Zip = @zip, country = @country, phone = @phone,  email = @email WHERE supplier_pk = @supplier_pk;";

            var kvp = new List<Tuple<string, string>>();

            foreach (SupplierModel SM in SMListUpdate)
            {
                // Email is allowed to be null.  if null, replace value
                if (SM.email == "")
                {
                    SM.email = "null";
                }
                if ((SM.supplier_name ?? SM.address ?? SM.city ?? SM.state ?? SM.zip ?? SM.country ?? SM.phone ?? SM.supplier_pk) == "")
                {
                    MessageBox.Show("Name, Address, City, State, Zip, Country, and Phone must contain a value");
                    Validated = false;
                }
                if ((SM.supplier_name.Length > 255 || SM.address.Length > 255 || SM.city.Length > 255 || SM.zip.Length > 9 || SM.state.Length > 50 || SM.country.Length > 50 || SM.phone.Length > 25 || SM.email.Length > 50))
                {
                    MessageBox.Show("Name, Address, City, State, Zip, Country, and Phone exceeds max values");
                    Validated = false;
                }
                if (Validated == true)
                {
                    kvp.Add(new Tuple<string, string>("supplier_name", SM.supplier_name));
                    kvp.Add(new Tuple<string, string>("address", SM.address));
                    kvp.Add(new Tuple<string, string>("city", SM.city));
                    kvp.Add(new Tuple<string, string>("state", SM.state));
                    kvp.Add(new Tuple<string, string>("zip", SM.zip));
                    kvp.Add(new Tuple<string, string>("country", SM.country));
                    kvp.Add(new Tuple<string, string>("phone", SM.phone));
                    kvp.Add(new Tuple<string, string>("email", SM.email));
                    kvp.Add(new Tuple<string, string>("supplier_pk", SM.supplier_pk));
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
                string sqlparams = "INSERT INTO supplier (supplier_name, address, city, state, zip, country, phone, email) VALUES (@supplier_name, @address, @city, @state, @zip, @country, @phone, @email);";
                //check if any rows created
                if (rowcount > 0)
                {
                    foreach (SupplierModel SM in SMListCreate)
                    {
             
                        // Email is allowed to be null.  if null, replace value
                        if (SM.email == null)
                        {
                            SM.email = "null";
                        }
                        if (SM.address == null || SM.city == null || SM.country== null || SM.phone == null)
                        {
                            MessageBox.Show("Name, Address, City, State, Zip, Country, and Phone must contain a value");
                            Validated = false;
                        }
                        else if ((SM.supplier_name.Length > 255 || SM.address.Length > 255 || SM.city.Length > 255 || SM.zip.Length > 9 || SM.state.Length > 50 || SM.country.Length > 50 || SM.phone.Length > 25 || SM.email.Length > 50))
                        {
                            MessageBox.Show("Name, Address, City, State, Zip, Country, and Phone exceeds max values");
                            Validated = false;
                        }
                        if (Validated == true)
                        {
                            kvp.Add(new Tuple<string, string>("supplier_name", SM.supplier_name));
                            kvp.Add(new Tuple<string, string>("address", SM.address));
                            kvp.Add(new Tuple<string, string>("city", SM.city));
                            kvp.Add(new Tuple<string, string>("state", SM.state));
                            kvp.Add(new Tuple<string, string>("zip", SM.zip));
                            kvp.Add(new Tuple<string, string>("country", SM.country));
                            kvp.Add(new Tuple<string, string>("phone", SM.phone));
                            kvp.Add(new Tuple<string, string>("email", SM.email));
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

                        if (SM.supplier_pk == null || SM.supplier_pk == "")
                        {
                            Validated = false;
                        }
                        if (Validated == true)
                        kvp.Add(new Tuple<string, string>("supplier_pk", SM.supplier_pk));
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
                    supplier_pk = (string)row["supplier_pk"].ToString(),
                    supplier_name = (string)row["supplier_name"],
                    address = (string)row["address"],
                    city = (string)row["city"],
                    state = (string)row["state"],
                    country = (string)row["country"],
                    zip = (string)row["zip"],
                    phone = (string)row["phone"],
                    email = (string)row["email"]
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
                    supplier_pk = (string)row["supplier_pk"].ToString(),
                    supplier_name = (string)row["supplier_name"],
                    address = (string)row["address"],
                    city = (string)row["city"],
                    state = (string)row["state"],
                    country = (string)row["country"],
                    zip = (string)row["zip"],
                    phone = (string)row["phone"],
                    email = (string)row["email"]
                };
                ObservableCollectionSupplierModel.Add(obj);
            }

            changetracker = new ChangeTracker<SupplierModel>(ObservableCollectionSupplierModel);
            changetracker.StartTracking(ObservableCollectionSupplierModel);
        }
    }
}