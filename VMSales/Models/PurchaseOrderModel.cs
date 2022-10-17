using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using VMSales.ViewModels;
namespace VMSales.Models
{

    public class PurchaseOrderModel : BaseViewModel
    {
        public int invoiceNumberLength = 255;
        public int lotNumberLength = 10;
        public int lotNameLength = 255;
        public int lotDescriptionLength = 255;
        // purchase_order_detail table

        private string _porder_pk { get; set; }
        private string _purchaseorder_fk { get; set; }
        private decimal _lotcost { get; set; }
        private int _lotqty { get; set; }
        private string _lotname { get; set; }
        private string _lotnumber { get; set; }
        private string _lotdescription { get; set; }
        private decimal _salestax { get; set; }
        private decimal _shippingcost { get; set; }

        // purchase order detail
        public string Porder_pk
        {
            get { return _porder_pk; }
            set
            {
                if (_porder_pk == value) return;
                _porder_pk = value;
                RaisePropertyChanged("Porder_pk");
            }
        }
        public string Purchaseorder_fk
        {
            get { return _purchaseorder_fk; }
            set
            {
                if (_purchaseorder_fk == value) return;
                _purchaseorder_fk = value;
                RaisePropertyChanged("Purchaseorder_fk");
            }
        }
        public string Lotnumber
        {
            get { return _lotnumber; }
            set
            {
                if (_lotnumber == value) return;
                _lotnumber = value;
                RaisePropertyChanged("Lotnumber");
            }
        }
        public decimal Lotcost
        {
            get { return _lotcost; }
            set
            {
                if (_lotcost == value) return;
                _lotcost = value;
                RaisePropertyChanged("Lotcost");
            }
        }
        public int Lotqty
        {
            get { return _lotqty; }
            set
            {
                if (_lotqty == value) return;
                _lotqty = value;
                RaisePropertyChanged("Lotqty");
            }
        }

        public string Lotname
        {
            get { return _lotname; }
            set
            {
                if (_lotname == value) return;
                _lotname = value;
                RaisePropertyChanged("Lotname");
            }
        }

        public string Lotdescription
        {
            get { return _lotdescription; }
            set
            {
                if (_lotdescription == value) return;
                _lotdescription = value;
                RaisePropertyChanged("Lotdescription");
            }
        }
        public decimal Salestax
        {
            get { return _salestax; }
            set
            {
                if (_salestax == value) return;
                _salestax = value;
                RaisePropertyChanged("Salestax");
            }
        }
        public decimal Shippingcost
        {
            get { return _shippingcost; }
            set
            {
                if (_shippingcost == value) return;
                _shippingcost = value;
                RaisePropertyChanged("Shippingcost");
            }
        }

        // purchase order
        private string _invoicenumber { get; set; }
        private DateTime _purchasedate { get; set; }

        private string _purchaseorder_pk { get; set; }
        private string _supplier_fk { get; set; }
        public string Invoicenumber
        {
            get { return _invoicenumber; }
            set
            {
                if (_invoicenumber == value) return;
                _invoicenumber = value;
                RaisePropertyChanged("Invoicenumber");
            }
        }
        public DateTime Purchasedate
        {
            get { return _purchasedate; }
            set
            {
                if (_purchasedate == value) return;
                _purchasedate = value;
                RaisePropertyChanged("Purchasedate");
            }
        }

        public string Supplier_fk
        {
            get { return _supplier_fk; }
            set
            {
                if (_supplier_fk == value) return;
                _supplier_fk = value;
            }
        }

        //totals
        public decimal Totallotcost { get; set; }
        public decimal Totalsalestax { get; set; }
        public decimal Totalshippingcost { get; set; }
        public decimal Totalcost { get; set; }
    }
}