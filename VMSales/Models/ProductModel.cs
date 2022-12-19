﻿using System;
using System.Collections.Generic;
using VMSales.ViewModels;
using Dapper.Contrib.Extensions;
using System.Collections.ObjectModel;

namespace VMSales.Models
{
    [Table("product")]
    public class ProductModel : BaseViewModel
    {
        private List<string> _conditionlist { get; set; }
        private string _condition { get; set; }
        private string _brand_name;
        private string _product_name;
        private string _description;
        private int _quantity;
        private decimal _cost;
        private string _sku;
        private decimal _sold_price;
        private int _instock;
        private string _listing_url;
        private string _listing_number;
        private DateTime _listing_date;

        private Dictionary<int,string> _category_dictionary {get; set; }
        public Dictionary<int, string> category_dictionary
        { 
            get {return _category_dictionary; } 
            set { if (_category_dictionary == value) return;
                _category_dictionary = value;
                RaisePropertyChanged("category_dictionary");
            }
        }




        public int category_pk { get; set; }
        [ExplicitKey]
        public int product_pk { get; set; }
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
        public List<string> conditionlist
        {
            get { return _conditionlist; }
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
        [ExplicitKey]
        public int product_category_pk { get; set; }
        public int product_fk { get; set; }
        private int _category_fk { get; set; }
        public int category_fk
        {
            get { return _category_fk; }
            set { 
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
