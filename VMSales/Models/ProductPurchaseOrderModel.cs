using VMSales.ViewModels;
using Dapper.Contrib.Extensions;

namespace VMSales.Models
{
    public partial class ProductModel
    {
        [ExplicitKey]
        public int product_purchase_order_pk { get; set; }
        public int product_order_detail_fk { get; set; }
        public int product_fk { get; set; }
        public int product_supplier_pk { get; set; }
        public int supplier_fk { get; set; }
    }
}

