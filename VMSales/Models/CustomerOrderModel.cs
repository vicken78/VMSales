using Dapper.Contrib.Extensions;

namespace VMSales.Models
{
    [Table("customer_order")]
    class CustomerOrderModel
    {
        [ExplicitKey]
        public string customer_order_pk { get; set; }
    }
}
