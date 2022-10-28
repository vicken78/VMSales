using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using VMSales.ViewModels;

namespace VMSales.Models
{
    public class ProductModel : BaseViewModel
    {

       

        private string _product_pk { get; set; }
        private string _productname { get; set; }
        private string _productdescription { get; set; }
        private string _productquantity { get; set; }
        private decimal _productcost { get; set; }
        private string _productsku { get; set; }
        private decimal _productsoldprice { get; set; }
        private int _productstock { get; set; }
        private string _productlistingurl { get; set; }
        private string _productlistingnumber { get; set; }
        private DateTime _productlistingdate { get; set; }
        private string _productbrandname { get; set; }

        public string product_PK { get; set; }
        public string productName
        {
            get { return _productname; }
            set
            {
                _productname = value;
                RaisePropertyChanged("productName");
            }
        }
        public string productDescription 
        {
            get { return _productdescription; }
            set
            {
                _productdescription = value;
                RaisePropertyChanged("productDescription");
            }
        }
        public string productQuantity 
        {
            get { return _productquantity; }
            set
            {
                _productquantity = value;
                RaisePropertyChanged("productQuantity");
            }
        }
        public decimal productCost
        {
            get { return _productcost; }
            set
            {
            _productcost = value;
            RaisePropertyChanged("productCost");
            }
        }
        public string productSKU
        {
            get { return _productsku; }
            set
            {
                _productsku = value;
                RaisePropertyChanged("productSKU");
            }
        }
        public decimal productSoldPrice 
        {
            get { return _productsoldprice; }
            set
            {
            _productsoldprice = value;
            RaisePropertyChanged("productSoldPrice");
            }
        }
        public int productStock 
        {
            get { return _productstock; }
            set
            {
            _productstock = value;
            RaisePropertyChanged("productStock");
            }
        }

        // Lists
        public List<string> productCondition { get; set; }
        public List<string> supplierName { get; set; }
        public List<string> categoryName { get; set; }


        public string productListingURL 
        {
            get { return _productlistingurl; }
            set
            {
            _productlistingurl = value;
            RaisePropertyChanged("productListingURL");
            }
        }
        public string productListingNumber 
        {
            get { return _productlistingnumber; }
            set
            {
            _productlistingnumber = value;
            RaisePropertyChanged("productListingNumber");
            }
        }
        public DateTime productListingDate 
        {
            get { return _productlistingdate; }
            set
            {
            _productlistingdate = value;
            RaisePropertyChanged("productListingDate");
            }
        }

        public string productBrandName 
        {
            get { return _productbrandname; }
            set
                {
                _productbrandname = value;
                RaisePropertyChanged("productBrandName");
                }
        }
    }
}
