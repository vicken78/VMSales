using System;
using VMSales.Logic;

namespace VMSales.Models
{   
    public class PurchaseOrderModel : BaseModel    {
        public int lotnum = 10;
        public int invoicenumber = 255;
        public int lotname = 255;
        public int lotdescription = 255;
        // purchase_order_detail table
        public decimal Totallotcost { get; set; }
        public decimal Totalsalestax { get; set; }
        public decimal Totalshippingcost { get; set; }
        public decimal Totalcost { get; set; }
        private string _porder_pk { get; set; }
        private string _purchaseorder_fk { get; set; }
        private decimal _lotcost { get; set; }
        private int _lotqty { get; set; }
        private string _lotname { get; set; }
        private string _lotnumber { get; set; }
        private string _lotdescription { get; set; }
        private decimal _salestax { get; set; }
        private decimal _shippingcost { get; set; }
      
        // purchaseorder table
        private string _purchaseorder_pk { get; set; }
        private string _supplier_fk { get; set; }
        private string _invoicenumber { get; set; }
        private DateTime _purchasedate;


        private string _selectedSname;

        public string SelectedSname
        {
            get
            {
                return _selectedSname;
            }
            set
            {
                _selectedSname = value;
                RaisePropertyChanged("SelectedSname");
            }
        }
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
            get { return _porder_pk; }
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
        // purchaseorder table
        public string Purchaseorder_pk
        {
            get { return _purchaseorder_pk; }
            set
            {
                if (_purchaseorder_pk == value) return;
                _purchaseorder_pk = value;
                RaisePropertyChanged("Purchaseorder_pk");
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
            get
            {
                return (this._purchasedate == default(DateTime))
                   ? this._purchasedate = DateTime.Now
                   : this._purchasedate;
            }
            set
            {
                this._purchasedate = value;
                RaisePropertyChanged("Purchasedate");
            }
        }
    }
}
