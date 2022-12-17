using System;
using System.Collections.Generic;
using VMSales.ViewModels;
using Dapper.Contrib.Extensions;
using System.Collections.ObjectModel;

namespace VMSales.Models
{
    [Table("product")]
    public class ProductModel : BaseViewModel
    {
        // for category
        private ObservableCollection<CategoryModel> ObservableCollectionCategoryModelclean { get; set; }
        public ObservableCollection<CategoryModel> ObservableCollectionCategoryModel { get; set; }
        private ObservableCollection<ProductModel> ObservableCollectionProductModelClean { get; set; }

        public ObservableCollection<ProductModel> ObservableCollectionProductModel 
        {
            get { return ObservableCollectionProductModelClean; }
            set {
                ObservableCollectionProductModelClean = value;
                RaisePropertyChanged("ObservableCollectionProductModel");
                    } 
        }


        public Dictionary<int, string> category_dictionary { get; set; }
        public List<int> category_pk { get; set; }
        
        private List<string> _conditionlist { get; set; }
        private string _condition { get; set; }

        private int _product_pk { get; set; }
        private string _brand_name { get; set; }
        private string _product_name { get; set; }
        private string _description { get; set; }
        private int _quantity { get; set; }
        private decimal _cost { get; set; }
        private string _sku { get; set; }
        private decimal _sold_price { get; set; }
        private int _instock { get; set; }
        private string _listing_url { get; set; }
        private string _listing_number { get; set; }
        private DateTime _listing_date { get; set; }

        [ExplicitKey]
        public int product_pk { get; set; }
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
        public int quantity
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
        public List<string> conditionlist
        {
            get { return _conditionlist; }

            set
            {
                _conditionlist = value;
                RaisePropertyChanged("conditionlist");
            }
        }

        public string condition
        {
            get { return _condition; }

            set
            {
                _condition = value;
                RaisePropertyChanged("condition");
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
