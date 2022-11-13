using VMSales.ViewModels;
using System.ComponentModel.DataAnnotations;
using System.Collections.Generic;
using Dapper.Contrib.Extensions;

    namespace VMSales.Models
{
    [Table("category")]
    public class CategoryModel : BaseViewModel 
    {
        public int categorynameLength = 255;
        public int DescriptionLength = 255;
        private string _categoryname;
        private string _description;
        private string _category_pk;

        public ICollection<ProductModel> Products { get; set; }

        public string categoryName
        {
            get { return _categoryname; }
            set
            {
                if (_categoryname == value) return;
                _categoryname = value;
                RaisePropertyChanged("categoryName");
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

        public List<ProductModel> ProductModel { get; set; } = new List<ProductModel>();

        [ExplicitKey]
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
