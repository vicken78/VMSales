using Dapper.Contrib.Extensions;
using VMSales.ViewModels;

namespace VMSales.Models
{
    [Table("supplier")]
    public class SupplierModel : BaseViewModel 
    {
        public int SnameLength = 255;
        public int AddressLength = 255;
        public int CityLength = 255;
        public int StateLength = 50;
        public int ZipLength = 9;
        public int CountryLength = 50;
        public int PhoneLength = 25;

        private string _supplier_pk;
        private string _supplier_name;
        private string _address;
        private string _city;
        private string _state;
        private string _zip;
        private string _country;
        private string _phone;
        private string _email;

        [ExplicitKey]
        public string supplier_pk
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