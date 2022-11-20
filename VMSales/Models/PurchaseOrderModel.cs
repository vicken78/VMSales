using System;
using VMSales.ViewModels;
using Dapper.Contrib.Extensions;
using System.Collections.Generic;

namespace VMSales.Models
{
    
[Table("purchase_order")]
public class PurchaseOrderModel : BaseViewModel
    {
        public int invoiceNumberLength = 255;
        public int lotNumberLength = 10;
        public int lotNameLength = 255;
        public int lotDescriptionLength = 255;
        public List<PurchaseOrderDetailModel> OrderDetails { get; set; }

        private string _purchase_order_pk { get; set; }
        private string _supplier_fk { get; set; }
        private string _invoice_number { get; set; }
        private DateTime _purchase_date { get; set; }
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
        public string supplier_fk
        {
            get { return _supplier_fk; }
            set
            {
                if (_supplier_fk == value) return;
                _supplier_fk = value;
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