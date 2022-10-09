using VMSales.Models;
using System;
using System.Data;
using System.Windows.Controls;
using System.Windows;
using System.Collections.Generic;
using VMSales.ChangeTrack;
using VMSales.Database;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Windows.Data;
using VMSales.Logic;

namespace VMSales.ViewModels
{

    public class PurchaseOrderViewModel : PurchaseOrderModel
    {
     
        public String Supplier_pk;
        private SupplierModel _selectedsname;
        public SupplierModel SelectedSname
        {
            get { return _selectedsname; }
            set 
            { 
                _selectedsname = value;
                RaisePropertyChanged("SelectedSname");
                Supplier_pk = _selectedsname.Supplier_pk.ToString();
                LoadPurchaseOrder(Supplier_pk);
            }
        }
        public ObservableCollection<SupplierModel> Supplier { get; set; } = new ObservableCollection<SupplierModel>();
        public ObservableCollection<PurchaseOrderModel> PurchaseOrder { get; set; } = new ObservableCollection<PurchaseOrderModel>();

        public void SaveCommand()
        {
        }
        public void AddCommand()
        {
        }
        public void DeleteCommand()
        {
        }
        public void ResetCommand()
        {
            PurchaseOrder.Clear();
            LoadPurchaseOrder(Supplier_pk);
        }

        public void LoadPurchaseOrder(string supplier_pk)
        {

            string sqlquery = "purchase_order, purchase_order_detail, supplier WHERE purchase_order.purchaseorder_pk = purchase_order_detail.purchaseorder_fk AND supplier.supplier_pk = purchase_order.supplier_fk AND supplier.supplier_pk = '" + supplier_pk + "'";
            DataTable Purchase_order = DataBaseOps.SQLiteDataTableWithQuery(sqlquery);
            foreach (DataRow row in Purchase_order.Rows)
            {
                var obj = new PurchaseOrderModel()
                {
                    Lotnumber = (string)row["Lotnum"],
                    Lotname = (string)row["Lotname"],
                    Lotdescription = (string)row["Lotdescription"],
                    Lotqty = (int)row["Lotqty"],
                    Lotcost = (decimal)row["Lotcost"],
                    Salestax = (decimal)row["Salestax"],
                    Shippingcost = (decimal)row["Shippingcost"],
                    Invoicenumber = (string)row["Invoicenum"],
                    Purchasedate = (System.DateTime)row["Purchasedate"],
                    Totallotcost = Totallotcost + (decimal)row["Lotcost"],
                    Totalsalestax = Totalsalestax + (decimal)row["Salestax"],
                    Totalshippingcost = Totalshippingcost + (decimal)row["Shippingcost"],
                    Totalcost = Totalcost + Totalsalestax + Totalshippingcost
    };
                PurchaseOrder.Add(obj);
                MessageBox.Show(Purchasedate.ToString());
                MessageBox.Show(Invoicenumber);

            }
        }

        public PurchaseOrderViewModel()
        {

            string parametersup = "supplier";
            DataTable supplierdt = DataBaseOps.SQLiteDataTableWithQuery(parametersup);
            foreach (DataRow row in supplierdt.Rows)
            {
                var obj = new SupplierModel()
                {
                    Supplier_pk = (string)row["Supplier_pk"].ToString(),
                    Sname = (string)row["Sname"],
                    Address = (string)row["Address"],
                    City = (string)row["City"],
                    State = (string)row["State"],
                    Zip = (string)row["Zip"],
                    Country = (string)row["Country"],
                    Phone = (string)row["Phone"],
                    Email = (string)row["Email"]
                };
                   Supplier.Add(obj);
            };
        }
    }
}