using Caliburn.Micro;
using VMSales.Models;
using System;
using System.Data;
using System.Windows.Controls;
using System.Windows;
using System.Collections.Generic;
using VMSales.ChangeTrack;
using VMSales.Database;
using VMSales.Models;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Windows.Data;
using VMSales.Logic;

namespace VMSales.ViewModels
{

    public class PurchaseOrderViewModel : BaseViewModel
    {
        #region Totals
        private decimal _totallotcost { get; set; }
        private decimal _totalsalestax { get; set; }
        private decimal _totalshippingcost { get; set; }
        private decimal _totalcost { get; set; }
   
        public decimal Totallotcost
        {
            get { return _totallotcost; }
            set
            {
                if (_totallotcost == value) return;
                _totallotcost = value;
                RaisePropertyChanged("Totallotcost");
            }
        }

        public decimal Totalsalestax
        {
            get { return _totalsalestax; }
            set
            {
                if (_totalsalestax == value) return;
                _totalsalestax = value;
                RaisePropertyChanged("Totalsalestax");
            }
        }
        public decimal Totalshippingcost
        {
            get { return _totalshippingcost; }
            set
            {
                if (_totalshippingcost == value) return;
                _totalshippingcost = value;
                RaisePropertyChanged("Totalshippingcost");
            }
        }
        public decimal Totalcost
        {
            get { return _totalcost; }
            set
            {
                if (_totalcost == value) return;
                _totalcost = value;
                RaisePropertyChanged("Totalcost");
            }
        }

        #endregion
        public String SelectedSupplier_pk;
        private SupplierModel _selectedsname;
        public SupplierModel SelectedSname    
        {
            get { return _selectedsname; }
            set
            {
                _selectedsname = value;
                RaisePropertyChanged("SelectedSname");
                SelectedSupplier_pk = _selectedsname.Supplier_pk.ToString();
                LoadPurchaseOrder(SelectedSupplier_pk); 
            }
        }
        private PurchaseOrderModel _selectedinvoicenumber;
        public PurchaseOrderModel Selectedinvoicenumber   
        {
            get { return _selectedinvoicenumber; }
            set
            {
                _selectedinvoicenumber = value;
                //RaisePropertyChanged("Selectedinvoicenumber");
                //LoadPurchaseOrder(SelectedSupplier_pk);
            }
        }





   
        public ObservableCollection<SupplierModel> Supplier { get; set; } = new ObservableCollection<SupplierModel>();
        public ObservableCollection<PurchaseOrderModel> ObservableCollectionPOD { get; set; } = new ObservableCollection<PurchaseOrderModel>();


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


        public void LoadInvoiceDates(string SelectedSupplier_pk)
        {
            //ObservableCollectionPOD = new ObservableCollection<PurchaseOrderModel>();
            string sqlquery = "purchase_order WHERE supplier_fk='" + SelectedSupplier_pk + "'";
            DataTable Purchase_orderDT = DataBaseOps.SQLiteDataTableWithQuery(sqlquery);
            foreach (DataRow row in Purchase_orderDT.Rows)
            {
                var obj = new PurchaseOrderModel()
                {
                    Invoicenumber = (string)row["invoicenum"],
                    Purchasedate = (DateTime)row["Purchasedate"],
                 };
                //ObservableCollectionPOD.Add(obj);
                
            }
         
        }

        public void LoadPurchaseOrder(string SelectedSupplier_pk)
        {
            if (SelectedSupplier_pk != null)
              {
                ObservableCollectionPOD.Clear();
                Totallotcost = 0;
                Totalsalestax = 0;
                Totalshippingcost = 0;
                Totalcost = 0;

                string sqlquery = "purchase_order, purchase_order_detail, supplier WHERE purchase_order.purchaseorder_pk = purchase_order_detail.purchaseorder_fk AND supplier.supplier_pk = purchase_order.supplier_fk AND supplier.supplier_pk='" + SelectedSupplier_pk + "'";
                DataTable Purchaseorderdt = DataBaseOps.SQLiteDataTableWithQuery(sqlquery);
                if ((Purchaseorderdt?.Rows?.Count ?? 0) > 0) 
                {

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
                            Invoicenumber = (string)row["Invoicenum"],
                            Purchasedate = (DateTime)row["Purchasedate"],
                        };
                        Totallotcost = Totallotcost + obj.Lotcost;
                        Totalsalestax = Totalsalestax + obj.Salestax;
                        Totalshippingcost = Totalshippingcost + obj.Shippingcost;
                        RaisePropertyChanged("Totallotcost");
                        RaisePropertyChanged("Totalsalestax");
                        RaisePropertyChanged("Totalshippingcost");
                        RaisePropertyChanged("Totalcost");
                        Totalcost = Totallotcost + Totalsalestax + Totalshippingcost;
                        ObservableCollectionPOD.Add(obj);


                        // now add purchase date and invoice num to list but don't add if already in a list.
                    }
                 

                }
                CollectionViewSource.GetDefaultView(ObservableCollectionPOD).Refresh();


            }
        }

        public PurchaseOrderViewModel()
        {
            // load suppliers
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