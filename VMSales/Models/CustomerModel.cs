using Dapper.Contrib.Extensions;

namespace VMSales.Models
{
    [Table("customer")]
    class CustomerModel
    {
        [ExplicitKey]
        public string customer_pk { get; set; }
    }
}
