using VMSales.ViewModels;
using Dapper.Contrib.Extensions;

namespace VMSales.Models
{
    [Table("product_purchase_order")]

    public class ProductPurchaseOrder
    {
        [ExplicitKey]
        public int product_purchase_order_pk { get; set; }
        public int product_order_detail_fk { get; set; }
        public int product_fk { get; set; }
    }
}

