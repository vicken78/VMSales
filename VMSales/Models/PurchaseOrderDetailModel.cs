using Dapper.Contrib.Extensions;
using VMSales.ViewModels;

namespace VMSales.Models
{
    public partial class PurchaseOrderModel : BaseViewModel
    {
        private string _purchase_order_detail_pk { get; set; }
        private string _purchase_order_fk { get; set; }
        private decimal _lot_cost { get; set; }
        private int _lot_qty { get; set; }
        private string _lot_name { get; set; }
        private string _lot_number { get; set; }
        private string _lot_description { get; set; }
        private decimal _sales_tax { get; set; }
        private decimal _shipping_cost { get; set; }

        // purchase order detail
        [ExplicitKey]
        public string Porder_pk
        {
            get { return _purchase_order_detail_pk; }
            set
            {
                if (_purchase_order_detail_pk == value) return;
                _purchase_order_detail_pk = value;
                RaisePropertyChanged("purchase_order_detail_pk");
            }
        }
        public string purchase_order_fk
        {
            get { return _purchase_order_fk; }
            set
            {
                if (_purchase_order_fk == value) return;
                _purchase_order_fk = value;
                RaisePropertyChanged("purchase_order_fk");
            }
        }
        public string lot_number
        {
            get { return _lot_number; }
            set
            {
                if (_lot_number == value) return;
                _lot_number = value;
                RaisePropertyChanged("lot_number");
            }
        }
        public decimal lot_cost
        {
            get { return _lot_cost; }
            set
            {
                if (_lot_cost == value) return;
                _lot_cost = value;
                RaisePropertyChanged("lot_cost");
            }
        }
        public int lot_qty
        {
            get { return _lot_qty; }
            set
            {
                if (_lot_qty == value) return;
                _lot_qty = value;
                RaisePropertyChanged("lot_qty");
            }
        }

        public string lot_name
        {
            get { return _lot_name; }
            set
            {
                if (_lot_name == value) return;
                _lot_name = value;
                RaisePropertyChanged("lot_name");
            }
        }

        public string lot_description
        {
            get { return _lot_description; }
            set
            {
                if (_lot_description == value) return;
                _lot_description = value;
                RaisePropertyChanged("lot_description");
            }
        }
        public decimal sales_tax
        {
            get { return _sales_tax; }
            set
            {
                if (_sales_tax == value) return;
                _sales_tax = value;
                RaisePropertyChanged("sales_tax");
            }
        }
        public decimal shipping_cost
        {
            get { return _shipping_cost; }
            set
            {
                if (_shipping_cost == value) return;
                _shipping_cost = value;
                RaisePropertyChanged("shipping_cost");
            }
        }
    }
}
