using Dapper.Contrib.Extensions;

namespace VMSales.Models
{
    public partial class PhotoModel
    {
            [ExplicitKey]
            public int product_photo_pk { get; set; }
            public int product_fk { get; set; }
            public int photo_fk { get; set; }
    }
}
