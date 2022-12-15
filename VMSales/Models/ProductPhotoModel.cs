using Dapper.Contrib.Extensions;

namespace VMSales.Models
{
    [Table("product_photo")]
    public class ProductPhotoModel
    {
            [ExplicitKey]
            public string product_photo_pk { get; set; }
            public string product_fk { get; set; }
            public string photo_fk { get; set; }
    }
}
