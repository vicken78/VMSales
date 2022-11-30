using System;
using VMSales.ViewModels;
using Dapper.Contrib.Extensions;
using System.Collections.Generic;
using System.Collections.ObjectModel;

namespace VMSales.Models
{
    
public partial class PurchaseOrderModel : BaseViewModel
    {

        private ObservableCollection<SupplierModel> ObservableCollectionSupplierModelclean { get; set; }
        public ObservableCollection<SupplierModel> ObservableCollectionSupplierModel { get; set; }

        public List<PurchaseOrderModel> OrderDetails { get; set; }
        private string _purchase_order_pk { get; set; }
        private string _invoice_number { get; set; }
        private DateTime _purchase_date { get; set; }
        public int supplier_fk { get; set; }

        [ExplicitKey]
        public string purchase_order_pk
        {
            get { return _purchase_order_pk; }
            set
            {
                if (_purchase_order_pk == value) return;
                _purchase_order_pk = value;
                RaisePropertyChanged("purchase_order_pk");
            }
        }
        public string invoice_number
        {
            get { return _invoice_number; }
            set
            {
                if (_invoice_number == value) return;
                _invoice_number = value;
                RaisePropertyChanged("invoice_number");
            }
        }
        public DateTime purchase_date
        {
            get { return _purchase_date; }
            set
            {
                if (_purchase_date == value) return;
                _purchase_date = value;
                RaisePropertyChanged("Purchase_date");
            }
        }
        //totals
        public decimal Totallotcost { get; set; }
        public decimal Totalsalestax { get; set; }
        public decimal Totalshippingcost { get; set; }
        public decimal Totalcost { get; set; }
    }
}