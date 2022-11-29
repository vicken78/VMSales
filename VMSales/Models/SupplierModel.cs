using System;
using VMSales.Logic;
using Dapper.Contrib.Extensions;


namespace VMSales.Models
{
    [Table("supplier")]
    public class SupplierModel : DataBaseLayer
    {
        public string selected_supplier_pk { get; set; }
        private int _supplier_pk { get; set; }
        private string _supplier_name { get; set; }
        private string _address { get; set; }
        private string _city { get; set; }
        private string _state { get; set; }
        private string _zip { get; set; }
        private string _country { get; set; }
        private string _phone { get; set; }
        private string _email { get; set; }

        [ExplicitKey]
        public int supplier_pk
        {
            get { return _supplier_pk; }
            set
            {
                if (_supplier_pk == value) return;
                _supplier_pk = value;
                RaisePropertyChanged("supplier_pk");
            }
        }
        public string supplier_name
        {
            get { return _supplier_name; }
            set
           {
                if (_supplier_name == value) return;
                _supplier_name = value;
                RaisePropertyChanged("supplier_name");
            }
        }

        public string address
        {
            get { return _address; }
            set
            {
                if (_address == value) return;
                _address = value;
                RaisePropertyChanged("address");
            }
        }
        public string city
        {
            get { return _city; }
            set
            {
                if (_city == value) return;
                _city = value;
                RaisePropertyChanged("city");
            }
        }
        public string state
        {
            get { return _state; }
            set
            {
                if (_state == value) return;
                _state = value;
                RaisePropertyChanged("state");
            }
        }
        public string country
        {
            get { return _country; }
            set
            {
                if (_country == value) return;
                _country = value;
                RaisePropertyChanged("country");
            }
        }
        public string zip
        {
            get { return _zip; }
            set
            {
                if (_zip == value) return;
                _zip = value;
                RaisePropertyChanged("zip");
            }
        }
        public string phone
        {
            get { return _phone; }
            set
            {
                if (_phone == value) return;
                _phone = value;
                RaisePropertyChanged("phone");
            }
        }
        public string email
        {
            get { return _email; }
            set
            {
                if (_email == value) return;
                _email = value;
                RaisePropertyChanged("email");
            }
        }   
    }
}