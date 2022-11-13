using Dapper.Contrib.Extensions;

namespace VMSales.Models
{
    [Table("customer_order")]
    class CustomerOrderModel
    {
        [ExplicitKey]
        public string corder_pk { get; set; }
    }
}
