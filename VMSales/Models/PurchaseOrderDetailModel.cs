using Dapper.Contrib.Extensions;
using VMSales.ViewModels;

namespace VMSales.Models
{
    [Table("purchase_order_detail")]
    public class PurchaseOrderDetailModel : BaseViewModel
    {
        private string _porder_pk { get; set; }
        private string _purchaseorder_id { get; set; }
        private decimal _lotcost { get; set; }
        private int _lotqty { get; set; }
        private string _lotname { get; set; }
        private string _lotnum { get; set; }
        private string _lotdescription { get; set; }
        private decimal _salestax { get; set; }
        private decimal _shippingcost { get; set; }

        // purchase order detail
        [ExplicitKey]
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
            get { return _purchaseorder_id; }
            set
            {
                if (_purchaseorder_id == value) return;
                _purchaseorder_id = value;
                RaisePropertyChanged("Purchaseorder_id");
            }
        }
        public string lotnum
        {
            get { return _lotnum; }
            set
            {
                if (_lotnum == value) return;
                _lotnum = value;
                RaisePropertyChanged("lotnum");
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



    }
}
