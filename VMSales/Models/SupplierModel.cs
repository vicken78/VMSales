using VMSales.BaseType;
using System.Diagnostics;
using System.Windows;
using System.ComponentModel;

namespace VMSales.Models
{
    public class SupplierModel : BaseModel
    {
        private string _supplier_pk;
        private string _sname;
        private string _address;
        private string _city;
        private string _state;
        private string _zip;
        private string _country;
        private string _phone;
        private string _email;
        public string Supplier_pk
        {
            get { return _supplier_pk; }
            set
            {
                if (_supplier_pk == value) return;
                _supplier_pk = value;
                OnPropertyChanged(nameof(Supplier_pk));
            }
        }
        public string Sname
        {
            get { return _sname; }
            set
            {
                if (_sname == value) return;
                _sname = value;
                OnPropertyChanged(nameof(Sname));
            }
        }
        public string Address
        {
            get { return _address; }
            set
            {
                if (_address == value) return;
                _address = value;
                OnPropertyChanged(nameof(Address));
            }
        }
        public string City
        {
            get { return _city; }
            set
            {
                if (_city == value) return;
                _city = value;
                OnPropertyChanged(nameof(City));
            }
        }

        public string State
        {
            get { return _state; }
            set
            {
                if (_state == value) return;
                _state = value;
                OnPropertyChanged(nameof(State));
            }
        }

        public string Country
        {
            get { return _country; }
            set
            {
                if (_country == value) return;
                _country = value;
                OnPropertyChanged(nameof(Country));
            }
        }

        public string Zip
        {
            get { return _zip; }
            set
            {
                if (_zip == value) return;
                _zip = value;
                OnPropertyChanged(nameof(Zip));
            }
        }

        public string Phone
        {
            get { return _phone; }
            set
            {
                if (_phone == value) return;
                _phone = value;
                OnPropertyChanged(nameof(Phone));
            }
        }

        public string Email
        {
            get { return _email; }
            set
            {
                if (_email == value) return;
                _email = value;
                OnPropertyChanged(nameof(Email));
            }
        }
        /*    private string _selectedSname;
            public string SelectedSname
            {
            get { return SelectedSname; }

            set
            {
                if (selectedSname != value)
                {
                    selectedSname = value;
                    OnPropertyChanged(nameof(selectedSname));
                }
            }
        */
    }
}
