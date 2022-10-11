using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;

namespace VMSales.Models
{
  
    public class PurchaseOrderModel : BaseModel 
    {
        private ObservableCollection<PurchaseOrderModel> _ObservableCollectionPOD;

        public ObservableCollection<PurchaseOrderModel> ObservableCollectionPOD
        {
            get { return _ObservableCollectionPOD; }
            set { _ObservableCollectionPOD = value; RaisePropertyChanged("ObservableCollectionPOD"); }
        }

        public int invoicenumberlength = 255;
        public int lotnumlength = 10;
        public int lotnamelength = 255;
        public int lotdescriptionlength = 255;
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
                RaisePropertyChanged("Supplier_fk");
            }
        }



        // totals
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
    }
}
