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
        public ObservableCollection<SupplierModel> ObservableCollectionSupplierModel { get; private set; }
        public ChangeTracker<SupplierModel> changetracker { get; set; }
        List<SupplierModel> SMListUpdated = new List<SupplierModel>();
        List<SupplierModel> SMListCreated = new List<SupplierModel>();
        List<SupplierModel> SMListDeleted = new List<SupplierModel>();

        //Commands
        public void SaveCommand()
        {

            // SMLISTupdated is class<SupplierModel> and is returned from change tracker with values that is read fom a db.
            // see SupplierModel.cs


            List<String> updatelist = new List<string>();
            string updateline;
            var sb = new System.Text.StringBuilder();

            SMListUpdated = changetracker.RowsUpdated;


            // this works but it is VERY unpleasant looking.
            // I couldn't find out how to change class<supplierModel> into a list of string
            // im aware i could of put updateline in one line but this is easier to debug.
            // this generates updates. sadly changetracker pulls the entire row, not just the changed value, so we update all values in row.
            // this is not ideal efficiency but it will work.
            // sample output where 2 rows were changed.
            //UPDATE supplier SET Sname='TEST1', Address = '111 MAIN ST', City = 'NEW YORK', State = '11111', Zip = 'NY', Country = 'USA', Phone = '555-111-1111', Email = 'TEST@GMAIL.COM'WHERE Supplier_pk='1';
            //UPDATE supplier SET Sname = 'FREDS', Address = '555 SAND ST', City = 'ATLANTA', State = 'GA', Zip = '44444', Country = 'USA', Phone = '777-777-7777', Email = 'fred@gmail.com'WHERE Supplier_pk = '2';
            foreach (SupplierModel svm in SMListUpdated)
            {
                updateline = "UPDATE supplier SET Sname='";
                updateline += svm.Sname;
                updateline += "', ";
                updateline += "Address = '";
                updateline += svm.Address;
                updateline += "', "; 
                updateline +="City = '";
                updateline += svm.City;
                updateline += "', ";
                updateline += "State = '";
                updateline += svm.State;
                updateline +="', ";
                updateline +="Zip = '";
                updateline += svm.Zip;
                updateline +="', ";
                updateline +="Country = '";
                updateline += svm.Country;
                updateline += "', ";
                updateline +="Phone = '";
                updateline +=svm.Phone;
                updateline +="', ";
                updateline +="Email = '";
                updateline +=svm.Email;
                updateline +="' ";
                updateline +="WHERE Supplier_pk='";
                updateline +=svm.Supplier_pk;
                updateline +="';";
                updatelist.Add(updateline);
            }

            foreach (var item in updatelist)
            {
                Debug.WriteLine(item);
            }


            changetracker.ClearTracking();


        }
        public void DeleteCommand()
        {
            List<SupplierModel> Deleted;
            //var ChangeTrack = new ChangeTracker<SupplierModel>();
            //changeTrack = new ChangeTracker<SupplierModel>(ObservableCollectionSupplierModel);
            //changeTrack.StartTracking(ObservableCollectionSupplierModel);
            //Deleted = changeTrack.RowsDeleted;

            //for (var i = 0; i < Deleted.Count; i++)
            //{
            //    Debug.WriteLine(Deleted[i]);
            // }
            //int count = Deleted.Count;

            //MessageBox.Show("Delete");
            //MessageBox.Show(count.ToString());

        }

        public void AddCommand()
        {
            MessageBox.Show("Add");
        }

        public void ResetCommand()
        {
            MessageBox.Show("Reset");
        }


        public SupplierViewModel()
        {
            ObservableCollectionSupplierModel = new ObservableCollection<SupplierModel>();
            string sql = "SELECT * FROM supplier;";
            DataTable dt = Database.DataBaseOps.SQLiteDataTableWithQuery(sql);
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