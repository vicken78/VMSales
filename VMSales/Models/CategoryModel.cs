using VMSales.ViewModels;
namespace VMSales.Models
{
    public class CategoryModel : BaseViewModel 
    {
        public int CnameLength = 255;
        public int DescriptionLength = 255;
        private string _cname;
        private string _description;
        private string _category_pk;
        public string Cname
        {
            get { return _cname; }
            set
            {
                if (_cname == value) return;
                _cname = value;
                RaisePropertyChanged("Cname");
            }
        }
        public string Description
        {
            get { return _description; }
            set
            {
                if (_description == value) return;
                _description = value;
                RaisePropertyChanged("Description");
            }
        }
        public System.DateTime CreationDate { get; set; }
        public string Category_pk
        {
            get { return _category_pk; }
            set
            {
                if (_category_pk == value) return;
                _category_pk = value;
                RaisePropertyChanged("Category_pk");
            }
        }

    }
}
