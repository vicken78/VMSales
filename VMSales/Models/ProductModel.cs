using System;
using System.Collections.Generic;
using VMSales.ViewModels;
using Dapper.Contrib.Extensions;
using System.Collections.ObjectModel;
using Caliburn.Micro;
using System.ComponentModel;

namespace VMSales.Models
{
    [Table("product")]
    public partial class ProductModel : BaseViewModel
    {

        private int _product_category_pk { get; set; }
        private int _product_pk { get; set; }
        private string _condition { get; set; }
        private string _brand_name { get; set; }
        private string _product_name { get; set; }
        private string _description { get; set; }
        private int _quantity { get; set; }
        private decimal _cost { get; set; }
        private string _sku { get; set; }
        private decimal _listed_price { get; set; }
        private int _instock { get; set; }
        private string _listing_url { get; set; }
        private string _listing_number { get; set; }
        private DateTime _listing_date { get; set; }

        private List<string> _category_list { get; set; }

        private string _selected_category { get; set; }
        public int category_pk { get; set; }
        [ExplicitKey]
        [DefaultValue(0)]
        public int product_pk
        {
            get { return _product_pk; }
            set
            {
                if (_product_pk == value) return;
                _product_pk = value;
                RaisePropertyChanged("product_pk");
            }
        }      
        public string product_name
        {
            get { return _product_name; }
            set
            {
                if (_product_name == value) return;
                _product_name = value;
                RaisePropertyChanged("product_name");
            }
        }
        public string description
        {
            get { return _description; }
            set
            {
                if (_description == value) return;
                _description = value;
                RaisePropertyChanged("description");
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
        public decimal cost
        {
            get { return _cost; }
            set
            {
                if (_cost == value) return;
                _cost = value;
                RaisePropertyChanged("cost");
            }

        }
        public string sku
        {
            get { return _sku; }
            set
            {
                if (_sku == value) return;
                _sku = value;
                RaisePropertyChanged("sku");
            }
        }
        public decimal listed_price
        {
            get { return _listed_price; }
            set
            {
                if (_listed_price == value) return;
                _listed_price = value;
                RaisePropertyChanged("listed_price");
            }

        }
        public int instock
        {
            get { return _instock; }
            set
            {
                if (_instock == value) return;
                _instock = value;
                RaisePropertyChanged("instock");
            }

        }

        private List<string> _conditionlist;
        public List<string> conditionlist
        {
            get { return _conditionlist = new List<string> { "New","Used" }; }
            set
            {
                if (_conditionlist == value) return;
                _conditionlist = value;
                RaisePropertyChanged("conditionlist");
            }

        }

        public string condition
        {
            get { return _condition; }
            set
            {
                if (_condition == value) return;
                _condition = value;
                RaisePropertyChanged("condition");
            }
        }

        public string listing_url
        {
            get { return _listing_url; }
            set
            {
                if (_listing_url == value) return;
                _listing_url = value;
                RaisePropertyChanged("listing_url");
            }

        }
        public string listing_number
        {
            get { return _listing_number; }
            set
            {
                if (_listing_number == value) return;
                _listing_number = value;
                RaisePropertyChanged("listing_number");
            }


        }
        public DateTime listing_date
        {
            get { return _listing_date; }
            set
            {
                if (_listing_date == value) return;
                _listing_date = value;
                RaisePropertyChanged("listing_date");
            }

        }

        public string brand_name
        {
            get { return _brand_name; }
            set
            {
                if (_brand_name == value) return;
                _brand_name = value;
                RaisePropertyChanged("brand_name");
            }


        }

        private bool _IsSelected;
        public bool IsSelected
        {
            get { return _IsSelected; }
            set
            {
                _IsSelected = value;
                RaisePropertyChanged("IsSelected");
            }
        }
        [ExplicitKey]
        public int product_category_pk
        { 
        get { return _product_category_pk; }
        set {
                if (_product_category_pk == value) return;
                _product_category_pk = value;
                RaisePropertyChanged("product_category_pk");
            }
        }
        
        private int _category_fk { get; set; }
        public int category_fk
        {
            get { return _category_fk; }
            set {
                if (_category_fk == value) return;
                _category_fk = value;
                RaisePropertyChanged("category_fk");
                }
        }

        private string _category_name { get; set; }
        public string category_name
        {
            get { return _category_name; }
            set
            {
                if (_category_name == value) return;
                _category_name = value;
                RaisePropertyChanged("category_name");
            }
        }

        private int _purchase_order_detail_fk { get; set; }
        public int purchase_order_detail_fk
        {
            get { return _purchase_order_detail_fk; }
            set
            {
                _purchase_order_detail_fk = value;
                RaisePropertyChanged("purchase_order_detail_fk");
            }
        }
    }
}
