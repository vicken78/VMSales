using Dapper.Contrib.Extensions;

namespace VMSales.Models
{
        [Table("product_photo")]
    class ProductPhotoModel
    {
            [ExplicitKey]
            public string pphoto_pk { get; set; }
    }
}
