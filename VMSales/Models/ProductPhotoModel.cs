using Dapper.Contrib.Extensions;

namespace VMSales.Models
{
    public partial class PhotoModel
    {
            [ExplicitKey]
            public string product_photo_pk { get; set; }
            public string product_fk { get; set; }
            public string photo_fk { get; set; }
    }
}
