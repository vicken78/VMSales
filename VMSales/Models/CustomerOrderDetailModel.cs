using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using VMSales.ViewModels;

namespace VMSales.Models
{
    public partial class CustomerOrderModel : BaseViewModel
    {
        private int _customer_order_detail_pk { get; set; }
        private int _customer_order_fk { get; set; }
        private int _product_fk { get; set; }
        private int _quantity { get; set; }
        private decimal _sold_price { get; set; }
        private decimal _selling_fee { get; set; }
        private decimal _sales_tax_amount { get; set; }
        private decimal _sales_tax_rate { get; set; }

        public int customer_order_detail_pk { get; set; }
        public int customer_order_fk
        {
            get { return _customer_order_fk;  }
            set 
            {
                if (_customer_order_fk == value) return;
                _customer_order_fk = value;
                RaisePropertyChanged("customer_order_fk");
            }
        }
        public int product_fk
        {
            get { return _product_fk; }
            set 
            {
                if (_product_fk == value) return;
                _product_fk = value;
                RaisePropertyChanged("product_fk");
            }
        }
        public int quantity
        {
            get { return _quantity; }
            set 
            {
                if (_quantity == value) return;
                _quantity = value;
                RaisePropertyChanged("quantity");
            }
        }
        public decimal sold_price
        {
            get { return _sold_price; }
            set 
            {
                if (_sold_price == value) return;
                _sold_price = value;
                RaisePropertyChanged("sold_price");
            }
        }
        public decimal selling_fee
        {
            get { return _selling_fee; }
            set 
            {
                if (_selling_fee == value) return;
                _selling_fee = value;
                RaisePropertyChanged("selling_fee");
            }
        }
        public decimal sales_tax_amount
        {
            get { return _sales_tax_amount; }
            set 
            {
                if (_sales_tax_amount == value) return;
                _sales_tax_amount = value;
                RaisePropertyChanged("sales_tax_amount");
            }
        }
        public decimal sales_tax_rate
        {
            get { return _sales_tax_rate; }
            set 
            {
                if (_sales_tax_rate == value) return;
                _sales_tax_rate = value;
                RaisePropertyChanged("sales_tax_rate");
            }
        }
    }
}
