using VMSales.Logic;
using Dapper.Contrib.Extensions;
using System;

namespace VMSales.Models
{
    [Table("category")]
    public class CategoryModel : DataBaseLayer
    {
        private int _category_pk;
        private string _category_name;
        private string _description;
        private DateTime _creation_date;
   
        [ExplicitKey]
        public int category_pk
        {
            get { return _category_pk; }
            set
            {
                if (_category_pk == value) return;
                _category_pk = value;
                RaisePropertyChanged("category_pk");
            }
        }
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
        public System.DateTime creation_date
        {
            get { return _creation_date; }
            set
            {
                if (_creation_date == value) return;
                _creation_date = value;
                RaisePropertyChanged("creation_date");
            }
        }
    }
}
