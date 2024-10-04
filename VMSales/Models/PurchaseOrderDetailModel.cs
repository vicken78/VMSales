using Dapper.Contrib.Extensions;
using System.Collections.Generic;
using System.Drawing.Text;
using System.Text.RegularExpressions;
using VMSales.ViewModels;

namespace VMSales.Models
{
    public partial class PurchaseOrderModel : BaseViewModel
    {
        // to move to view
        public bool isproductinventory { get; set; }
   
        private int _purchase_order_detail_pk;
        [ExplicitKey]
        public int purchase_order_detail_pk
        {
            get => _purchase_order_detail_pk; 
            set
            {
                if (_purchase_order_detail_pk != value)
                _purchase_order_detail_pk = value;
                NotifyOfPropertyChange(() => purchase_order_detail_pk);
            }
        }
        private int _purchase_order_fk;
        public int purchase_order_fk
        {
            get => _purchase_order_fk; 
            set
            {
                if (_purchase_order_fk != value) 
                _purchase_order_fk = value;
                NotifyOfPropertyChange(() => purchase_order_fk);
            }
        }

        private string _lot_number;
        public string lot_number
        {
            get => _lot_number; 
            set
            {
                if (_lot_number != value) 
                _lot_number = value;
                NotifyOfPropertyChange(() => lot_number);
            }
        }

        private decimal _lot_cost;
        public decimal lot_cost
        {
            get => _lot_cost;
            set
            {
                if (_lot_cost != value) 
                _lot_cost = value;
                var regex = new Regex(@"^\d+\.\d{2}?$"); // ^\d+(\.|\,)\d{2}?$ use this incase your dec separator can be comma or decimal.
                var flg = regex.IsMatch(_lot_cost.ToString());
                if (flg)
                {
                    NotifyOfPropertyChange(() => lot_cost);
                }

            }
        }
   
        private int _lot_quantity;
        public int lot_quantity
        {
            get => _lot_quantity; 
            set
            {
                if (_lot_quantity != value) 
                _lot_quantity = value;
                NotifyOfPropertyChange(() => lot_quantity);
            }
        }

        private string _lot_name;
        public string lot_name
        {
            get => _lot_name; 
            set
            {
                if (_lot_name != value)
                _lot_name = value;
                NotifyOfPropertyChange(() => lot_name);
            }
        }

        private string _lot_description;
        public string lot_description
        {
            get { return _lot_description; }
            set
            {
                if (_lot_description != value)
                _lot_description = value;
                NotifyOfPropertyChange(() => lot_description);
            }
        }

        private decimal _sales_tax;
        public decimal sales_tax
        {
            get => _sales_tax; 
            set
            {
                if (_sales_tax != value)
                _sales_tax = value;
                NotifyOfPropertyChange(() => sales_tax);
            }
        }

        private decimal _shipping_cost;
        public decimal shipping_cost
        {
            get => _shipping_cost; 
            set
            {
                if (_shipping_cost != value)
                _shipping_cost = value;
                NotifyOfPropertyChange(() => shipping_cost);
            }
        }

        private decimal _total_lot;
        public decimal total_lot
        {
            get => _total_lot; 
            set
            {
                if (_total_lot != value) 
                _total_lot = value;
                NotifyOfPropertyChange(() => total_lot);
            }
        }

        private decimal _total_sales_tax;
        public decimal total_sales_tax
        {
            get => _total_sales_tax; 
            set
            {
                if (_total_sales_tax != value)
                _total_sales_tax = value;
                NotifyOfPropertyChange(() => total_sales_tax);
            }
        }

        private decimal _total_shipping;
        public decimal total_shipping
        {
            get => _total_shipping; 
            set
            {
                if (_total_shipping != value)
                _total_shipping = value;
                NotifyOfPropertyChange(() => total_shipping);
            }
        }

        private decimal _total_cost;
        public decimal total_cost
        {
            get => _total_cost; 
            set
            {
                if (_total_cost != value) 
                _total_cost = value;
                NotifyOfPropertyChange(() => total_cost);
            }
        }

        private int _quantity_check;
        public int quantity_check
        {
            get => _quantity_check; 
            set
            {
                if (_quantity_check != value) 
                _quantity_check = value;
                NotifyOfPropertyChange(() => quantity_check);
            }
        }
    }
}
