using VMSales.Logic;
using Dapper.Contrib.Extensions;
using System;
using System.Windows.Forms;

namespace VMSales.Models
{
    [Table("category")]
    public class CategoryModel : DataBaseLayer
    {
        [ExplicitKey]
        public int category_pk { get; set; }
        public string category_name { get; set; }
        public string description { get; set; }
        public DateTime creation_date { get; set; }
    }
}
