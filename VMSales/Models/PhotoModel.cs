using Dapper.Contrib.Extensions;


namespace VMSales.Models
{
    public partial class PhotoModel
    {
        [ExplicitKey]
        public int photo_pk { get; set; }
        public int photo_order_number { get; set; }
        public string photo_path { get; set; }

    }
}
