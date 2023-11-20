using Dapper.Contrib.Extensions;
using System.Collections.Generic;
using System.Text.RegularExpressions;
using VMSales.ViewModels;

namespace VMSales.Models
{
    public partial class PurchaseOrderModel : BaseViewModel
    {
        public bool productinventoried { get; set; }
        private int _purchase_order_detail_pk { get; set; }
        private int _purchase_order_fk { get; set; }
        private decimal _lot_cost { get; set; }
        private int _lot_quantity { get; set; }
        private string _lot_name { get; set; }
        private string _lot_number { get; set; }
        private string _lot_description { get; set; }
        private decimal _sales_tax { get; set; }
        private decimal _shipping_cost { get; set; }

        private decimal _total_lot { get; set; }
        private decimal _total_sales_tax { get; set; }
        private decimal _total_shipping { get; set; }
        private decimal _total_cost { get; set; }

        private int _quantity_check {get; set;}

        // purchase order detail
        [ExplicitKey]
        public int purchase_order_detail_pk
        {
            get { return _purchase_order_detail_pk; }
            set
            {
                if (_purchase_order_detail_pk == value) return;
                _purchase_order_detail_pk = value;
                RaisePropertyChanged("purchase_order_detail_pk");

            }
        }
        public int purchase_order_fk
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
            get 
            {
                    return _lot_cost;
            }
            set
            {
                if (_lot_cost == value) return;
                _lot_cost = value;
                var regex = new Regex(@"^\d+\.\d{2}?$"); // ^\d+(\.|\,)\d{2}?$ use this incase your dec separator can be comma or decimal.
                var flg = regex.IsMatch(_lot_cost.ToString());
                if (flg)
                {
                    RaisePropertyChanged("lot_cost");
                }

            }
        }
        public int lot_quantity
        {
            get { return _lot_quantity; }
            set
            {
                if (_lot_quantity == value) return;
                _lot_quantity = value;
                RaisePropertyChanged("lot_quantity");
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
        public decimal total_lot
        {
            get { return _total_lot; }
            set
            {
                if (_total_lot == value) return;
                _total_lot = value;
                RaisePropertyChanged("total_lot");
            }
        }

        public decimal total_sales_tax
        {
            get { return _total_sales_tax; }
            set
            {
                if (_total_sales_tax == value) return;
                _total_sales_tax = value;
                RaisePropertyChanged("total_sales_tax");
            }
        }

        public decimal total_shipping
        {
            get { return _total_shipping; }
            set
            {
                if (_total_shipping == value) return;
                _total_shipping = value;
                RaisePropertyChanged("total_shipping");
            }
        }

        public decimal total_cost
        {
            get { return _total_cost; }
            set
            {
                if (_total_cost == value) return;
                _total_cost = value;
                RaisePropertyChanged("total_cost");
            }
        }


        public int quantity_check
        {
            get { return _quantity_check; }
            set
            {
                if (_quantity_check == value) return;
                _quantity_check = value;
                RaisePropertyChanged("quantity_check");
            }
        }

    }
}
