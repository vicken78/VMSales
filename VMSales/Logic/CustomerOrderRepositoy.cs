using Dapper;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using VMSales.Models;

namespace VMSales.Logic
{

    public class CustomerOrderRepository : Repository<CustomerOrderModel>
    {
        public CustomerOrderRepository(IDatabaseProvider dbProvider) : base(dbProvider) { }

        public override async Task<IEnumerable<CustomerOrderModel>> GetAllWithID(int id)
        {
            return null;
        }


        public override async Task<CustomerOrderModel> Get(int id)
        {
            return await Connection.QuerySingleAsync<CustomerOrderModel>("SELECT customer_order_detail_pk FROM customer_order_detail WHERE customer__order_detail_pk = @id", new { id }, Transaction);
        }

        public override async Task<IEnumerable<CustomerOrderModel>> GetAll()
        {
            return await Connection.QueryAsync<CustomerOrderModel>(
            "SELECT customer_order_pk, customer_fk, order_number, shipping_status, shipping_service, tracking_number, " +
            "order_date, shipping_date, shipping_cost_collected, actual_shipping_cost, customer_order_detail_pk, customer_order_fk, " +
            "product_fk, customer_order_detail.quantity, sold_price, selling_fee, sales_tax_amount, sales_tax_rate, brand_name, product_name " +
            "FROM customer_order as co " +
            "INNER JOIN customer_order_detail on co.customer_order_pk = customer_order_detail.customer_order_fk " +
            "INNER JOIN customer on customer.customer_pk = co.customer_fk " +
            "INNER JOIN product on product.product_pk = product_fk " +
            "UNION " +
            "SELECT co.customer_order_pk, co.customer_fk, co.order_number, co.shipping_status, co.shipping_service, co.tracking_number, " +
            "co.order_date, co.shipping_date, co.shipping_cost_collected, co.actual_shipping_cost, cod.customer_order_detail_pk, cod.customer_order_fk, " +
            "cod.product_fk, cod.quantity, cod.sold_price, cod.selling_fee, cod.sales_tax_amount, cod.sales_tax_rate, p.brand_name, p.product_name " +
            "FROM customer_order AS co " +
            "LEFT JOIN customer_order_detail AS cod ON co.customer_order_pk = cod.customer_order_fk " +
            "LEFT JOIN customer ON customer.customer_pk = co.customer_fk " +
            "LEFT JOIN product AS p ON p.product_pk = cod.product_fk", null, Transaction);
        }

        public override async Task<int> Insert(CustomerOrderModel entity)
        {
            //change
            return 0;
        }


        public override async Task<bool> Update(CustomerOrderModel entity)
        {
            // change
            return false;
        }

        public override async Task<bool> Delete(CustomerOrderModel entity)
        {
            // change
            //return (await Connection.ExecuteAsync("DELETE FROM customer WHERE customer_pk = @id", new { id = entity.customer_pk }, Transaction)) == 1;
            return false;
        }
    }
}

/* fix
SELECT customer_order_pk, customer_fk, order_number, shipping_status, shipping_service, tracking_number,
order_date, shipping_date, shipping_cost_collected, actual_shipping_cost, customer_order_detail_pk, customer_order_fk,
product_fk, customer_order_detail.quantity, sold_price, selling_fee, sales_tax_amount, sales_tax_rate, brand_name, product_name
FROM customer_order as co
INNER JOIN customer_order_detail on co.customer_order_pk = customer_order_detail.customer_order_fk
INNER JOIN customer on customer.customer_pk = co.customer_fk
INNER JOIN product on product.product_pk = product_fk
UNION
SELECT co.customer_order_pk, co.customer_fk, co.order_number, co.shipping_status, co.shipping_service, co.tracking_number,
co.order_date, co.shipping_date, co.shipping_cost_collected, co.actual_shipping_cost, cod.customer_order_detail_pk, cod.customer_order_fk,
cod.product_fk, cod.quantity, cod.sold_price, cod.selling_fee, cod.sales_tax_amount, cod.sales_tax_rate, p.brand_name, p.product_name
FROM customer_order AS co
LEFT JOIN customer_order_detail AS cod ON co.customer_order_pk = cod.customer_order_fk
LEFT JOIN customer ON customer.customer_pk = co.customer_fk
LEFT JOIN product AS p ON p.product_pk = cod.product_fk
*/

