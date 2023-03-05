using Dapper.Contrib.Extensions;
using System;
using System.Collections.Generic;
using VMSales.ViewModels;

namespace VMSales.Models
{
    [Table("customer_order")]
    public partial class CustomerOrderModel : BaseViewModel
    {
        [ExplicitKey]
        private int _customer_order_pk { get; set; }
        private int _customer_fk { get; set; }
        private string _order_number { get; set; }
        private List<string> _shipping_status { get; set; }
        private List<string> _shipping_service { get; set; }
        private string _tracking_number { get; set; }
        private DateTime _order_date { get; set; }
        private DateTime _shipping_date { get; set; }
        private decimal _shipping_cost_collected { get; set; }
        private decimal _actual_shipping_cost { get; set; }

        public int customer_order_pk { get; set; }
        public int customer_fk
        {
            get { return _customer_fk; }
            set
            {
                if (_customer_fk == value) return;
                _customer_fk = value;
                RaisePropertyChanged("customer_fk");

            }
        }
        public string order_number
        {
            get { return _order_number; }
            set
            {
                if (_order_number == value) return;
                _order_number = value;
                RaisePropertyChanged("order_number");
            }
        }

        public List<string> shipping_status 
        {
            get { return _shipping_status = new List<string> { "Shipped", "Ship Ready", "Unshipped"}; }
            set 
            {
                if (_shipping_status == value) return;
                _shipping_status = value;
                RaisePropertyChanged("shipping_status");
            } 
        }
        
        public List<string> shipping_service
        {
            get { return _shipping_service = new List<string> { "Fedex Ground", "Fedex 2-Day", "Fedex 3-Day", "USPS First Class", "USPS Media", "USPS Priority", "UPS Ground", "UPS 2-Day", "UPS 3-Day" }; }
                set
                {
                    if (_shipping_service == value) return;
                    _shipping_service = value;
                    RaisePropertyChanged("shipping_service");
                }
            
        }
        public string tracking_number 
        {
            get { return _tracking_number; }
            set { } 
        }


        public DateTime order_date 
        { 
            get 
            { return _order_date; } 
            set 
            {
                if (_order_date == value) return;
                _order_date = value;
                RaisePropertyChanged("order_date");
            } 
        }
        public DateTime shipping_date 
        { 
            get { return _shipping_date; } 
            set 
            {
                if (_shipping_date == value) return;
                _shipping_date = value;
                RaisePropertyChanged("shipping_date");
            }
        }
 
        public decimal shipping_cost_collected 
        { 
            get 
            { 
                return _shipping_cost_collected; 
            } 
            set 
            {
                if (_shipping_cost_collected == value) return;
                _shipping_cost_collected = value;
                RaisePropertyChanged("shipping_cost_collected");
            }
        }
        public decimal actual_shipping_cost 
        { 
            get 
            { 
                return _actual_shipping_cost; 
            } 
            set 
            {
                if (_actual_shipping_cost == value) return;
                _actual_shipping_cost = value;
                RaisePropertyChanged("actual_shipping_cost");
            }
        }
    }
}
