using System;
using System.Collections.Generic;
using VMSales.ViewModels;
using Dapper.Contrib.Extensions;


namespace VMSales.Models
{
    [Table("product")]
    public class ProductModel : BaseViewModel
    {
        private List<string> _condition { get; set; }
        private string _product_pk { get; set; }
        private string _brand_name { get; set; }
        private string _product_name { get; set; }
        private string _description { get; set; }
        private string _quantity { get; set; }
        private decimal _cost { get; set; }
        private string _sku { get; set; }
        private decimal _sold_price { get; set; }
        private int _instock { get; set; }
        private string _listing_url { get; set; }
        private string _listing_number { get; set; }
        private DateTime _listing_date { get; set; }
     

        [ExplicitKey]
        public string product_pk { get; set; }
        public string product_name
        {
            get { return _product_name; }
            set
            {
                _product_name = value;
                RaisePropertyChanged("product_name");
            }
        }
        public string description 
        {
            get { return _description; }
            set
            {
                _description = value;
                RaisePropertyChanged("description");
            }
        }
        public string quantity 
        {
            get { return _quantity; }
            set
            {
                _quantity = value;
                RaisePropertyChanged("quantity");
            }
        }
        public decimal cost
        {
            get { return _cost; }
            set
            {
            _cost = value;
            RaisePropertyChanged("cost");
            }
        }
        public string sku
        {
            get { return _sku; }
            set
            {
                _sku = value;
                RaisePropertyChanged("sku");
            }
        }
        public decimal sold_price 
        {
            get { return _sold_price; }
            set
            {
            _sold_price = value;
            RaisePropertyChanged("sold_price");
            }
        }
        public int instock 
        {
            get { return _instock; }
            set
            {
            _instock = value;
            RaisePropertyChanged("instock");
            }
        }

        public string listing_url 
        {
            get { return _listing_url; }
            set
            {
            _listing_url = value;
            RaisePropertyChanged("listing_url");
            }
        }
        public string listing_number 
        {
            get { return _listing_number; }
            set
            {
            _listing_number = value;
            RaisePropertyChanged("listing_number");
            }
        }
        public DateTime listing_date 
        {
            get { return _listing_date; }
            set
            {
            _listing_date = value;
            RaisePropertyChanged("listing_date");
            }
        }

        public string brand_name 
        {
            get { return _brand_name; }
            set
                {
                _brand_name = value;
                RaisePropertyChanged("brand_name");
                }
        }
    }
}
