using Caliburn.Micro;
using Dapper;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Threading.Tasks;
using System.Windows;
using VMSales.Models;


namespace VMSales.Logic
{
    public class PurchaseOrderRepository : Repository<PurchaseOrderModel>
    {
        public PurchaseOrderRepository(IDatabaseProvider dbProvider) : base(dbProvider) { }

        // NEED CODE REWRITE HERE.

        // INSERTS
        public override async Task<int> Insert(PurchaseOrderModel entity)
        {
            // get invoice number
            try
            {
                string invoice_number = await Connection.QuerySingleOrDefaultAsync<string>("SELECT invoice_number FROM purchase_order WHERE invoice_number = @invoice_number", new { entity.invoice_number }, Transaction);


                /// <summary>
                /// scenerio 1
                /// same invoice number, INSERT purchase_order_detail using existing purchase_order_fk
                /// adding row to existing invoice

                if (invoice_number == entity.invoice_number)
                {
                    // get purchase_order_fk

                    int db_purchase_order_fk = await Connection.QuerySingleOrDefaultAsync<int>("SELECT purchase_order_pk FROM purchase_order WHERE invoice_number = @invoice_number", new { entity.invoice_number }, Transaction);

                    bool rowsAffected = (await Connection.ExecuteAsync("INSERT INTO purchase_order_detail " +
                    "(purchase_order_fk, lot_cost, lot_quantity, lot_number, lot_name, lot_description, sales_tax, shipping_cost, quantity_check) " +
                    "VALUES (@db_purchase_order_fk, @lot_cost, @lot_quantity, @lot_number, @lot_name, @lot_description, @sales_tax, @shipping_cost, @quantity_check);  SELECT last_insert_rowid()",
                     new
                     {
                         db_purchase_order_fk,
                         lot_cost = entity.lot_cost,
                         lot_quantity = entity.lot_quantity,
                         lot_number = entity.lot_number,
                         lot_name = entity.lot_name,
                         lot_description = entity.lot_description,
                         sales_tax = entity.sales_tax,
                         shipping_cost = entity.shipping_cost,
                         quantity_check = entity.quantity_check
                     }, Transaction)) == 1;
                    if (rowsAffected == true)
                    {
                        return 1;
                    }
                    else
                    {
                        return 0; // Return 0 to indicate that no rows were inserted or an error occurred.
                    }
                }

                //  scenerio 2
                // INSERT new invoice number, INSERT new purchase_order detail (purchase_order_detail_pk = 0)
                // insert into purchase_order and purchase_order_detail using new purchase_order_fk

                if (invoice_number != entity.invoice_number && entity.invoice_number != "0" && entity.invoice_number != null)
                {
                    int newId = await Connection.QuerySingleAsync<int>("INSERT INTO purchase_order (supplier_fk, invoice_number, purchase_date) VALUES (@supplier_fk, @invoice_number, @purchase_date); SELECT last_insert_rowid()", new
                    {
                        supplier_fk = entity.supplier_fk,
                        invoice_number = entity.invoice_number,
                        purchase_date = entity.purchase_date
                    }, Transaction);
                    entity.purchase_order_fk = newId;
                    // now for purchase_order_detail

                    int purchase_order_detail_pk = await Connection.ExecuteScalarAsync<int>("INSERT INTO purchase_order_detail " +
                      "(purchase_order_fk, lot_cost, lot_quantity, lot_number, lot_name, lot_description, sales_tax, shipping_cost, quantity_check) " +
                        "VALUES (@purchase_order_fk, @lot_cost, @lot_quantity, @lot_number, @lot_name, @lot_description, @sales_tax, @shipping_cost, @quantity_check); SELECT last_insert_rowid()",
                        new
                        {
                            purchase_order_fk = entity.purchase_order_fk,
                            lot_cost = entity.lot_cost,
                            lot_quantity = entity.lot_quantity,
                            lot_number = entity.lot_number,
                            lot_name = entity.lot_name,
                            lot_description = entity.lot_description,
                            sales_tax = entity.sales_tax,
                            shipping_cost = entity.shipping_cost,
                            quantity_check = entity.quantity_check
                        }, Transaction);
                    return purchase_order_detail_pk;
                }
                return 0;
            }
            catch (Exception)
            {
                return 0;
            }
        }

        // SELECTS

        public async Task<PurchaseOrderModel> get_supplier_fk(int id)
        {
            return await Connection.QuerySingleAsync<PurchaseOrderModel>("SELECT supplier_fk FROM purchase_order WHERE purchase_order_pk = @id", new { id }, Transaction);
        }

        public override async Task<PurchaseOrderModel> Get(int id)
        {
            return await Connection.QuerySingleAsync<PurchaseOrderModel>("SELECT purchase_order_detail_pk FROM purchase_order_detail as pod INNER JOIN purchase_order on purchase_order.purchase_order_pk = pod.purchase_order_fk WHERE supplier_fk = @id;", new { id }, Transaction);
        }

        public async Task<bool> Get_purchase_order_detail_pk(PurchaseOrderModel entity)
        {
            return await Connection.QuerySingleAsync<bool>("SELECT purchase_order_detail_pk FROM purchase_order_detail WHERE purchase_order_detail_pk = @id;", new
            {
                id = entity.purchase_order_detail_pk
            }, null);
        }

        public async Task<IEnumerable<PurchaseOrderModel>> GetProductPurchase_Order(int product_pk)
        {

            return await Connection.QueryAsync<PurchaseOrderModel>("SELECT DISTINCT " +
                "supplier_name, lot_number, invoice_number, purchase_date " +
                "FROM product p " +
                "INNER JOIN product_supplier ps ON p.product_pk = ps.product_fk " +
                "INNER JOIN supplier s ON ps.supplier_fk = s.supplier_pk " +
                "INNER JOIN product_purchase_order ppo ON p.product_pk = ppo.product_fk " +
                "INNER JOIN product on p.product_pk = ps.product_fk " +
                "INNER JOIN purchase_order_detail pod ON ppo.product_purchase_order_detail_fk = pod.purchase_order_detail_pk " +
                "INNER JOIN purchase_order po ON ppo.product_purchase_order_detail_fk = po.purchase_order_pk " +
                "WHERE p.product_pk = @product_pk",
                new { product_pk }, Transaction);
        }

        //get last insert
        public async Task<int> Get_last_insert()
        {
            var result = await Connection.QuerySingleAsync<int>("SELECT MAX(purchase_order_detail_pk) FROM purchase_order_detail;", Transaction);
            return result;
        }

        public async Task<IEnumerable<PurchaseOrderModel>> GetAllTotal(int supplier_fk)
        {
            string query = "SELECT " +
                           "SUM(pod.sales_tax) AS total_sales_tax, " +
                           "SUM(pod.shipping_cost) AS total_shipping, " +
                           "SUM(pod.lot_cost) AS total_lot, " +
                           "SUM(pod.sales_tax + pod.shipping_cost + pod.lot_cost) AS total_cost " +
                           "FROM purchase_order AS po " +
                           "INNER JOIN purchase_order_detail AS pod ON po.purchase_order_pk = pod.purchase_order_fk " +
                           "INNER JOIN supplier AS sup ON sup.supplier_pk = po.supplier_fk";

            if (supplier_fk != 0)
            {
                query += " WHERE supplier_fk = @supplier_fk";
            }

            var parameters = new { supplier_fk };

            return await Connection.QueryAsync<PurchaseOrderModel>(query, parameters, Transaction);
        }

        // get all purchase_order and purchase_order_detail
        public override async Task<IEnumerable<PurchaseOrderModel>> GetAll()
        {

            return await Connection.QueryAsync<PurchaseOrderModel>("SELECT " +
         "DISTINCT po.purchase_order_pk, pod.purchase_order_detail_pk, po.purchase_date, po.invoice_number, po.supplier_fk, sup.supplier_name, pod.purchase_order_fk, " +
         "pod.lot_number, pod.lot_cost, pod.lot_quantity, pod.lot_name, pod.lot_description, pod.sales_tax, pod.shipping_cost, pod.quantity_check, " +
            "CASE " +
            "WHEN product_purchase_order.product_purchase_order_detail_fk IS NULL THEN 'True' " +
            "ELSE 'False' " +
         "END AS isproductinventory " +
         "FROM " +
            "purchase_order AS po " +
         "JOIN " +
            "purchase_order_detail AS pod ON po.purchase_order_pk = pod.purchase_order_fk " +
         "JOIN " +
            "supplier AS sup ON sup.supplier_pk = po.supplier_fk " +
         "LEFT JOIN " +
            "product_purchase_order ON pod.purchase_order_detail_pk = product_purchase_order.product_purchase_order_detail_fk");
        }

        /*

       public async Task<IEnumerable<bool>> CheckForProductfk()
       {
           IEnumerable<bool> result = await Connection.QueryAsync<bool>("SELECT CASE " +
               "WHEN product_purchase_order.product_purchase_order_detail_fk IS NULL THEN 'True' " +
               "ELSE 'False' " +
           "END AS isproductinventory " +
           "FROM " +
               "purchase_order_detail " +
           "LEFT JOIN " +
               "product_purchase_order ON purchase_order_detail.purchase_order_detail_pk = product_purchase_order.product_purchase_order_detail_fk");
            return result;
       }
    */


        // get all purchase_order and purchase_order_detail
        public override async Task<IEnumerable<PurchaseOrderModel>> GetAllWithID(int id)
        {

            return await Connection.QueryAsync<PurchaseOrderModel>("SELECT " +
                "distinct po.purchase_order_pk, po.purchase_date, po.invoice_number, " +
                "pod.purchase_order_detail_pk, pod.purchase_order_fk, pod.lot_number, pod.lot_cost, pod.lot_quantity," +
                "pod.lot_name, pod.lot_description, pod.sales_tax, pod.shipping_cost, pod.quantity_check " +
                "FROM purchase_order as po, purchase_order_detail as pod, supplier as sup " +
                "INNER JOIN purchase_order_detail on po.purchase_order_pk = pod.purchase_order_fk " +
                "INNER JOIN supplier on sup.supplier_pk = po.supplier_fk " +
                "AND po.supplier_fk=@id", new { id }, Transaction);
        }


        public async Task<PurchaseOrderModel> GetAllWithPK(int purchase_order_detail_pk)
        {
            return await Connection.QuerySingleAsync<PurchaseOrderModel>("SELECT DISTINCT " +
                "pod.purchase_order_detail_pk, pod.lot_number, pod.lot_cost, pod.lot_quantity, " +
                "pod.lot_name, pod.sales_tax, pod.shipping_cost, po.supplier_fk " +
                "FROM purchase_order as po, purchase_order_detail as pod, supplier as sup " +
                "INNER JOIN purchase_order_detail on po.purchase_order_pk = pod.purchase_order_fk " +
                "INNER JOIN supplier on sup.supplier_pk = po.supplier_fk " +
                "WHERE pod.purchase_order_detail_pk =@purchase_order_detail_pk", new { purchase_order_detail_pk }, Transaction);
        }


        // get specific product order detail
        public async Task<int> Get_PurchaseOrderDetail_by_pk(int purchase_order_detail_pk)
        {
            return await Connection.QuerySingleAsync<int>("SELECT DISTINCT " +
                "pod.purchase_order_detail_pk, pod.lot_number, pod.lot_cost, pod.lot_quantity, " +
                "pod.lot_name, pod.sales_tax, pod.shipping_cost " +
                "FROM purchase_order_detail as pod " +
                "WHERE pod.purchase_order_detail_pk = @purchase_order_detail_pk", new { purchase_order_detail_pk }, Transaction);
        }




        // UPDATES

        // Update Scenerios
        // UPDATE TABLE PURCHASE_ORDER (change supplier -> supplier_fk warn user)
        // UPDATE TABLE PURCHASE_ORDER (INVOICE NUMBER OR PURCHASE DATE)

        //UPDATE TABLE PURCHASE_ORDER_DETAIL
        //(LOT_COST, LOT_QUANTITY, LOT_NUMBER, LOT_NAME, LOT_DESCRIPTION, SALES_TAX,
        //SHIPPING_COST, QUANTITY_CHECK(0,1)

        // check if supplier_fk is changing and change supplier.

        public override async Task<bool> Update(PurchaseOrderModel entity)
        {
            bool result = await Connection.QueryFirstAsync<bool>(
                "SELECT CASE WHEN EXISTS " +
                "(SELECT 1 FROM purchase_order WHERE supplier_fk = @id AND purchase_order_pk = @purchase_order_pk) " +
                "THEN 1 ELSE 0 END",
                new { id = entity.supplier_fk, purchase_order_pk = entity.purchase_order_pk }, null);

            if (result == false)
            {
                if (MessageBox.Show("Please Confirm Supplier Change.", "Question", MessageBoxButton.YesNo, MessageBoxImage.Warning) == MessageBoxResult.Yes)
                {
                    bool supplier_change = (await Connection.ExecuteAsync("UPDATE purchase_order SET " +
                    "supplier_fk = @supplier_fk WHERE purchase_order_pk = @id", new
                    {
                        id = entity.purchase_order_pk,
                        supplier_fk = entity.supplier_fk,
                    }, null)) == 1;

                    if (supplier_change)
                        MessageBox.Show("Supplier Updated.");
                }
            }
            // Update Purchase_Order

            // Update Purchase_Order_Detail
            


            return true;
        }


        /*
            // check if purchase_order really exists
            bool result = await Connection.QueryFirstAsync<bool>("SELECT CASE WHEN EXISTS (SELECT purchase_order_fk FROM purchase_order_detail WHERE purchase_order_fk = @id) THEN 1 ELSE 0 END as result", new { id = entity.purchase_order_fk }, null);

            if (result)
            {

                // getting fk constraint failed here, why.
                bool updaterow = (await Connection.ExecuteAsync("UPDATE purchase_order SET " +
                    "purchase_order_pk = @id, " +
                    "supplier_fk = @supplier_fk, " +
                    "invoice_number = @invoice_number, " +
                    "purchase_date = @purchase_date " +
                    "WHERE purchase_order_pk = @id", new
                    {
                        id = entity.purchase_order_pk,
                        supplier_fk = entity.supplier_fk,
                        invoice_number = entity.invoice_number,
                        purchase_date = entity.purchase_date
                    }, null)) == 1;
                return (await Connection.ExecuteAsync("UPDATE purchase_order_detail SET " +
                        "purchase_order_fk = @purchase_order_fk, " +
                        "lot_cost = @lot_cost, " +
                        "lot_quantity = @lot_quantity, " +
                        "lot_number = @lot_number," +
                        "lot_name = @lot_name, " +
                        "lot_description = @lot_description, " +
                        "sales_tax = @sales_tax, " +
                        "shipping_cost = @shipping_cost, " +
                        "quantity_check = @quantity_check " +
                        "WHERE purchase_order_detail_pk = @purchase_order_detail_pk", new
                        {
                            purchase_order_fk = entity.purchase_order_fk,
                            lot_cost = entity.lot_cost,
                            lot_quantity = entity.lot_quantity,
                            lot_number = entity.lot_number,
                            lot_name = entity.lot_name,
                            lot_description = entity.lot_description,
                            sales_tax = entity.sales_tax,
                            shipping_cost = entity.shipping_cost,
                            quantity_check = entity.quantity_check,
                            purchase_order_detail_pk = entity.purchase_order_detail_pk,
                        }, Transaction)) == 1;
            }
            return false;
        }

        */

        // DELETES

        // FIX
        public override async Task<bool> Delete(PurchaseOrderModel entity)
        {
            //
            // check and fix.  we need to delete purchase_order_detail pk, then delete purchase_order_pk IF its the last one.
            //
            bool deleterow = (await Connection.ExecuteAsync("...DELETE FROM purchase_order_detail WHERE purchase_order_fk = @id", new { id = entity.purchase_order_fk }, null)) == 1;
            return (await Connection.ExecuteAsync("...DELETE FROM purchase_order WHERE purchase_order_pk = @id", new { id = entity.purchase_order_pk }, Transaction)) == 1;
        }

        // generate product command
        public async Task<IEnumerable<int>> Eligible_Products()
        {
            string sql = "SELECT purchase_order_detail_pk FROM purchase_order_detail as pod " +
                         "WHERE purchase_order_detail_pk NOT IN (SELECT product_purchase_order_detail_fk " +
                         "FROM product_purchase_order) " +
                         "AND pod.lot_quantity > '0' " +
                         "AND pod.quantity_check = 0 " +
                         "ORDER BY purchase_order_detail_pk";


            var result = await Connection.QueryAsync<int>(sql, Transaction);
            return result;
        }
    }
}


// needs fixing here
/*
#region Product_Purchase_Order
public class ProductPurchaseOrderRepository : Repository<ProductPurchaseOrderModel>
{
    public ProductPurchaseOrderRepository(IDatabaseProvider dbProvider) : base(dbProvider) { }

    // Insert
    public override async Task<bool> Insert(ProductPurchaseOrderModel entity)
    {
        bool insertrow = (await Connection.ExecuteAsync("INSERT INTO product_purchase_order " +
            "(product_order_detail_fk, product_fk) VALUES (@product_order_detail_fk, @product_fk)",
              new
              {
                  product_order_detail_fk = entity.product_order_detail_fk,
                  product_fk = entity.product_fk
              }, Transaction)) == 1;
        return insertrow;
    }

    public override async Task<ProductPurchaseOrderModel> Get(int id)
    {
        return await Connection.QuerySingleAsync<ProductPurchaseOrderModel>("SELECT purchase_order_detail_pk FROM purchase_order_detail as pod INNER JOIN purchase_order on purchase_order.purchase_order_pk = pod.purchase_order_fk WHERE supplier_fk = @id;", new { id }, Transaction);
    }

    // get all purchase_order and purchase_order_detail
    public override async Task<IEnumerable<ProductPurchaseOrderModel>> GetAll()
    {
        return await Connection.QueryAsync<ProductPurchaseOrderModel>("SELECT " +
            "distinct po.purchase_order_pk, po.purchase_date, po.invoice_number, pod.purchase_order_detail_pk, " +
            "pod.purchase_order_fk, pod.lot_number, pod.lot_cost, pod.lot_quantity," +
            "pod.lot_name, pod.lot_description, pod.sales_tax, pod.shipping_cost " +
            "FROM purchase_order as po, purchase_order_detail as pod, supplier as sup " +
            "INNER JOIN purchase_order_detail on po.purchase_order_pk = pod.purchase_order_fk " +
            "INNER JOIN supplier on sup.supplier_pk = po.supplier_fk;", null, Transaction);
    }

    // get all products with product category
    public override async Task<IEnumerable<ProductPurchaseOrderModel>> GetAllWithID(int id)
    {
        return await Connection.QueryAsync<ProductPurchaseOrderModel>("SELECT " +
            "distinct po.purchase_order_pk, po.purchase_date, po.invoice_number, " +
            "pod.purchase_order_detail_pk, pod.purchase_order_fk, pod.lot_number, pod.lot_cost, pod.lot_quantity," +
            "pod.lot_name, pod.lot_description, pod.sales_tax, pod.shipping_cost " +
            "FROM purchase_order as po, purchase_order_detail as pod, supplier as sup " +
            "INNER JOIN purchase_order_detail on po.purchase_order_pk = pod.purchase_order_fk " +
            "INNER JOIN supplier on sup.supplier_pk = po.supplier_fk " +
            "AND po.supplier_fk=@id", new { id }, Transaction);
    }

    public override async Task<bool> Update(ProductPurchaseOrderModel entity)
    {

        return (await Connection.ExecuteAsync("UPDATE product SET " +
                "brand_name = @purchase_order_fk, " +
                "product_name = @lot_cost, " +
                "description = @lot_quantity, " +
                "quantity = @lot_number, " +
                "cost = @lot_name, " +
                "sku = @lot_description, " +
                "listed_price = @listed_price, " +
                "instock = @shipping_cost, " +
                "condition = @sales_tax, " +
                "listing_url = @shipping_cost, " +
                "listing_number = @listing_number, " +
                "listing_date = @listing_date " +
                "WHERE product_pk = @product_pk", new
                {
                    product_purchase_order_fk = entity.product_order_detail_fk,
                    product_fk = entity.product_fk,
                }, Transaction)) == 1;
    }

    public override async Task<bool> Delete(ProductPurchaseOrderModel entity)
    {
        // fix
        //bool deleterow = (await Connection.ExecuteAsync("DELETE FROM purchase_order_detail WHERE purchase_order_fk = @id", new { id = entity.purchase_order_fk }, null)) == 1;
        //return (await Connection.ExecuteAsync("DELETE FROM purchase_order WHERE purchase_order_pk = @id", new { id = entity.purchase_order_pk }, Transaction)) == 1;
        return false;
    }
}
*/