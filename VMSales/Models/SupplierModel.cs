using System.Collections.ObjectModel;

namespace VMSales.Models
{
    public class SupplierModel : BaseModel 
    {
        public int SnameLength = 255;
        public int AddressLength = 255;
        public int CityLength = 255;
        public int StateLength = 50;
        public int ZipLength = 9;
        public int CountryLength = 50;
        public int PhoneLength = 25;
    

        private string _sname;
        private string _address;
        private string _city;
        private string _state;
        private string _zip;
        private string _country;
        private string _phone;
        private string _email;
        private string _supplier_pk;

        public string Sname
        {
            get { return _sname; }
            set
           {
                if (_sname == value) return;
                _sname = value;
                RaisePropertyChanged("Sname");
            }
        }



        public string Address
        {
            get { return _address; }
            set
            {
                if (_address == value) return;
                _address = value;
                RaisePropertyChanged("Address");
            }
        }
        public string City
        {
            get { return _city; }
            set
            {
                if (_city == value) return;
                _city = value;
                RaisePropertyChanged("City");
            }
        }
        public string State
        {
            get { return _state; }
            set
            {
                if (_state == value) return;
                _state = value;
                RaisePropertyChanged("State");
            }
        }
        public string Country
        {
            get { return _country; }
            set
            {
                if (_country == value) return;
                _country = value;
                RaisePropertyChanged("Country");
            }
        }
        public string Zip
        {
            get { return _zip; }
            set
            {
                if (_zip == value) return;
                _zip = value;
                RaisePropertyChanged("Zip");
            }
        }
        public string Phone
        {
            get { return _phone; }
            set
            {
                if (_phone == value) return;
                _phone = value;
                RaisePropertyChanged("Phone");
            }
        }
        public string Email
        {
            get { return _email; }
            set
            {
                if (_email == value) return;
                _email = value;
                RaisePropertyChanged("Email");
            }
        }
        public string Supplier_pk
        {
            get { return _supplier_pk; }
            set
            {
                if (_supplier_pk == value) return;
                _supplier_pk = value;
                RaisePropertyChanged("Supplier_pk");
            }
        }

        private string _selectedsname;
        public string SelectedSname
        {
            get { return _selectedsname; }
            set
            {
                if (_selectedsname == value) return;
                _selectedsname = value;
                RaisePropertyChanged("SelectedSname");
            }
        }
    }
}