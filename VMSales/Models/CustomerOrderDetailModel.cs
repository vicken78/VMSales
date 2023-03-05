using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using VMSales.ViewModels;

namespace VMSales.Models
{
    public partial class CustomerOrderModel : BaseViewModel
    {
        private int _customer_order_detail_pk;
        private int _customer_order_fk;
        private int _product_fk;
        private int _quantity;
        private decimal _sold_price;
        private decimal _selling_fee;
        private decimal _sales_tax_amount;
        private decimal _sales_tax_rate;
    }
}
