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
        public SupplierModel SelectedSname    // we need SelectedInvoice.  Datagrid with Purchase Date Invoice #
        {
            get { return _selectedsname; }
            set
            {
                _selectedsname = value;
                RaisePropertyChanged("SelectedSname");
                Supplier_pk = _selectedsname.Supplier_pk.ToString();
                LoadInvoiceDates(Supplier_pk);
            }
        }
        public ObservableCollection<SupplierModel> Supplier { get; set; } = new ObservableCollection<SupplierModel>();
  
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
            //PurchaseOrder.Clear();
            //LoadPurchaseOrder(Supplier_pk);
        }


        public void LoadInvoiceDates(string supplier_pk)
        {
            ObservableCollection<PurchaseOrderModel> ObservableCollectionPOD; 
            ObservableCollectionPOD = new ObservableCollection<PurchaseOrderModel>();
            string sqlquery = "purchase_order WHERE supplier_fk='" + supplier_pk + "'";
            DataTable Purchase_orderDT = DataBaseOps.SQLiteDataTableWithQuery(sqlquery);
            foreach (DataRow row in Purchase_orderDT.Rows)
            {
                var obj = new PurchaseOrderModel()
                {
                    Invoicenumber = (string)row["invoicenum"],
                    Purchasedate = (DateTime)row["Purchasedate"],
                 };
                ObservableCollectionPOD.Add(obj);
            }
        }

        public void LoadPurchaseOrder(string supplier_pk)
        {
            List<String> InvoicenumList = new List<String>();
            List<DateTime> PurchasedateList = new List<DateTime>();

            //string sqlquery = "purchase_order, purchase_order_detail, supplier WHERE purchase_order.purchaseorder_pk = purchase_order_detail.purchaseorder_fk AND supplier.supplier_pk = purchase_order.supplier_fk AND supplier.supplier_pk = '" + supplier_pk + "'";
            string sqlquery = "purchase_order, purchase_order_detail, supplier WHERE purchase_order.purchaseorder_pk = purchase_order_detail.purchaseorder_fk AND supplier.supplier_pk = purchase_order.supplier_fk AND supplier.supplier_pk='"+supplier_pk+"'";

            DataTable Purchaseorderdt = DataBaseOps.SQLiteDataTableWithQuery(sqlquery);
            foreach (DataRow row in Purchaseorderdt.Rows)
            {
                var obj = new PurchaseOrderModel()
                {
                    Lotnumber = (string)row["lotnum"],
                    Lotname = (string)row["Lotname"],
                    Lotdescription = (string)row["Lotdescription"],
                    Lotqty = (int)row["Lotqty"],
                    Lotcost = (decimal)row["Lotcost"],
                    Salestax = (decimal)row["Salestax"],
                    Shippingcost = (decimal)row["Shippingcost"],
                    Invoicenumber = (string)row["invoicenum"],
                    Purchasedate = (DateTime)row["Purchasedate"],
                    Totallotcost = Totallotcost + (decimal)row["Lotcost"],
                    Totalsalestax = Totalsalestax + (decimal)row["Salestax"],
                    Totalshippingcost = Totalshippingcost + (decimal)row["Shippingcost"]
                };
                
                if (!InvoicenumList.Contains(obj.Invoicenumber))
                    InvoicenumList.Add(obj.Invoicenumber);

                if (!PurchasedateList.Contains(obj.Purchasedate))
                    PurchasedateList.Add(obj.Purchasedate);

                obj.Totallotcost += Totallotcost;
                obj.Totalsalestax += Totalsalestax;
                obj.Totalshippingcost = Totalshippingcost;
                obj.Totalcost = obj.Totalcost + obj.Totalsalestax + obj.Totalshippingcost;
                //PurchaseOrder.Add(obj);
            }
            

            //Purchasedate.Add (Convert.ToDateTime(obj.Purchasedate));
            //Invoicenumber.Add(obj.Invoicenumber);



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