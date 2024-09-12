using VMSales.Logic;
using Dapper.Contrib.Extensions;



namespace VMSales.Models
{
    [Table("customer")]
    public class CustomerModel : DataBaseLayer
    {
        [ExplicitKey]
        private int _customer_pk { get; set; }
        private string _user_name { get; set; }
        private string _first_name { get; set; }
        private string _last_name { get; set; }
        private string _address { get; set; }
        private string _state { get; set; }
        private string _city { get; set; }
        private string _zip { get; set; }
        private string _country { get; set; }
        private string _phone { get; set; }
        private string _shipping_address { get; set; }
        private string _shipping_state { get; set; }
        private string _shipping_city { get; set; }
        private string _shipping_zip { get; set; }
        private string _shipping_country { get; set; }
       
        public int customer_pk 
        {
            get { return _customer_pk; }
            set
            {
                if (_customer_pk == value) return;
                _customer_pk = value;
                //RaisePropertyChanged("customer_pk");
            }
        }
        public string user_name 
        {
            get { return _user_name; }
            set
            {
                if (_user_name == value) return;
                _user_name = value;
                //RaisePropertyChanged("user_name");
            }
        }
        public string first_name 
        {
            get { return _first_name; }
            set
            {
                if (_first_name == value) return;
                _first_name = value;
                //RaisePropertyChanged("first_name");
            }
        }
        public string last_name 
        {
            get { return _last_name; }
            set
            {
                if (_last_name == value) return;
                _last_name = value;
                //RaisePropertyChanged("last_name");
            }
        }
        public string address 
        {
            get { return _address; }
            set
            {
                if (_address == value) return;
                _address = value;
                //RaisePropertyChanged("address");
            }
        }
        public string state 
        {
            get { return _state; }
            set
            {
                if (_state == value) return;
                _state = value;
                //RaisePropertyChanged("state");
            }
        }
        public string city
        {
            get { return _city; }
            set
            {
                if (_city == value) return;
                _city = value;
                //RaisePropertyChanged("city");
            }
        }

        public string zip 
        {
            get { return _zip; }
            set
            {
                if (_zip == value) return;
                _zip = value;
                //RaisePropertyChanged("zip");
            }
        }
        public string country 
        {
            get { return _country; }
            set
            {
                if (_country == value) return;
                _country = value;
                //RaisePropertyChanged("country");
            }
        }
        public string phone 
        {
            get { return _phone; }
            set
            {
                if (_phone == value) return;
                _phone = value;
                //RaisePropertyChanged("phone");
            }
        }
        public string shipping_address 
        {
            get { return _shipping_address; }
            set
            {
                if (_shipping_address == value) return;
                _shipping_address = value;
                //RaisePropertyChanged("shipping_address");
            }
        }
        public string shipping_city
        {
            get { return _shipping_city; }
            set
            {
                if (_shipping_city == value) return;
                _shipping_city = value;
                //RaisePropertyChanged("shipping_city");
            }
        }
        public string shipping_state 
        {
            get { return _shipping_state; }
            set
            {
                if (_shipping_state == value) return;
                _shipping_state = value;
                //RaisePropertyChanged("shipping_state");
            }
        }
        public string shipping_zip 
        {
            get { return _shipping_zip; }
            set
            {
                if (_shipping_zip == value) return;
                _shipping_zip = value;
                //RaisePropertyChanged("shipping_zip");
            }
        }
        public string shipping_country
        {
            get { return _shipping_country; }
            set
            {
                if (_shipping_country == value) return;
                _shipping_country = value;
                //RaisePropertyChanged("shipping_country");
            }
        }
        private int _same_shipping_address { get; set; }
        public int same_shipping_address
        {
            get { return _same_shipping_address; }
            set
            {
                if (_same_shipping_address == value) return;
                _same_shipping_address = value;
                //RaisePropertyChanged("same_shipping_address");
            }
        }


    }
}
