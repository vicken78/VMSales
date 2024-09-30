using Dapper;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Forms;
using VMSales.Models;

namespace VMSales.Logic
{
    public class DataBaseLayer 
    {
    
        #region Photo includes Product_Photo
        public class PhotoRepository : Repository<PhotoModel>
        {
            // in development, need Delete
            public PhotoRepository(IDatabaseProvider dbProvider) : base(dbProvider) { }

            public async Task<string> GetPhotoPath(string product_photo_path)
            {
                return await Connection.QuerySingleOrDefaultAsync<string>(
                    "SELECT photo_path FROM photo WHERE photo_path = @photo_path",
                    new { photo_path = product_photo_path },
                    Transaction
                );
            }

            public async Task<ObservableCollection<string>> GetFileList(int product_pk)
            {
                var queryResult = await Connection.QueryAsync<string>(
                    "SELECT photo_path FROM photo " +
                    "INNER JOIN product_photo " +
                    "ON photo.photo_pk = product_photo.photo_fk " +
                    "INNER JOIN product " +
                    "ON product.product_pk = product_photo.product_fk " +
                    "WHERE product_photo.product_fk = @product_pk " +
                    "ORDER BY photo.photo_order_number",
                    new { product_pk },
                    Transaction);
                return queryResult.ToObservable();
            }

            public async Task<IEnumerable<int>> GetImagePos(int product_fk)
            {
                return await Connection.QueryAsync<int>(
                "SELECT COALESCE(MIN(photo_order_number), 1) AS photo_order_number " +
                "FROM " +
                "(SELECT photo_order_number " +
                "FROM photo " +
                "LEFT JOIN product_photo ON photo.photo_pk = product_photo.photo_fk " +
                "WHERE product_fk = @product_fk) " +
                "UNION " +
                "SELECT photo_order_number " +
                "FROM photo " +
                "LEFT JOIN product_photo ON photo.photo_pk = product_photo.photo_fk " +
                "WHERE product_fk = @product_fk"
                , new { product_fk }, Transaction);
            }

            public async Task<IEnumerable<int>> GetNextPos(int product_fk)
            {
                return await Connection.QueryAsync<int>(
                "SELECT COALESCE(MAX(photo_order_number), 0) + 1 AS photo_order_number " +
                "FROM (" +
                "SELECT photo_order_number " +
                "FROM photo " +
                "LEFT JOIN product_photo ON photo.photo_pk = product_photo.photo_fk " +
                "WHERE product_fk = @product_fk " +
                "UNION " +
                "SELECT MAX(photo_order_number) " +
                "FROM photo " +
                "LEFT JOIN product_photo ON photo.photo_pk = product_photo.photo_fk " +
                "WHERE product_fk = @product_fk) " +
                "AS subquery WHERE PHOTO_ORDER_NUMBER IS NOT NULL"
                , new { product_fk }, Transaction);
            }
            // insert
            //public async Task<int> Insert(int product_fk,int photo_order_number, string photofilePath)
            public override async Task<int> Insert(PhotoModel entity)
            {
                int photo_pk = (await Connection.QueryFirstOrDefaultAsync<int>("INSERT INTO photo " +
                "(photo_order_number, photo_path) VALUES (@photo_order_num, @photo_path); SELECT last_insert_rowid()",
            new
            {
                photo_order_num = entity.photo_order_number,
                photo_path = entity.photo_path
            }, Transaction));
                return photo_pk;
            }

            public async Task<int> InsertProductPhoto(PhotoModel entity)
            {
                int pphoto_pk = (await Connection.QueryFirstOrDefaultAsync<int>("INSERT INTO product_photo " +
                "(product_fk, photo_fk) VALUES (@product_fk, @photo_fk); SELECT last_insert_rowid()",
            new
            {
                product_fk = entity.product_fk,
                photo_fk = entity.photo_fk
            }, Transaction));
                return pphoto_pk;
            }


            // update
            public override async Task<bool> Update(PhotoModel entity)
            {
                return (await Connection.ExecuteAsync("UPDATE photo SET " +
                        "photo_order_number = @photo_order_number " +
                        "WHERE photo_pk = @photo_pk", new
                        {
                            photo_fk = entity.photo_fk,
                        }, Transaction)) == 1;
            }
            // delete
            public override async Task<bool> Delete(PhotoModel entity)
            {
                bool deleterow = (await Connection.ExecuteAsync(
                "DELETE FROM photo WHERE photo_pk = @id",
                new { id = entity.photo_pk }, null)) == 1;
                return deleterow;
            }

            //get all by id
            public override async Task<IEnumerable<PhotoModel>> GetAllWithID(int id)
            {
                return await Connection.QueryAsync<PhotoModel>("SELECT " +
                    "product_order_number, photo_path FROM photo WHERE photo_pk=@id", new { id }, Transaction);
            }

            //get Photo_Path
            public async Task<IEnumerable<string>> GetPhotoPath(int id)
            {
                return await Connection.QueryAsync<string>("SELECT " +
                    "photo_path FROM photo WHERE photo_pk=@id", new { id }, Transaction);
            }

            //get all
            public override async Task<IEnumerable<PhotoModel>> GetAll()
            {
                // needs query fixing here.
                return await Connection.QueryAsync<PhotoModel>("SELECT " +
                    "photo_order_number, photo_fk FROM photo", null, Transaction);
            }
            //get by product id
            public override async Task<PhotoModel> Get(int id)
            {
                //FIX
                //needs query fixing here
                return await Connection.QuerySingleAsync<PhotoModel>("SELECT ... FROM product_photo as pp INNER JOIN product on purchase_order.purchase_order_pk = pod.purchase_order_fk WHERE supplier_fk = @id;", new { id }, Transaction);
            }

        }

        #endregion

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
        #endregion
        */
            #region CustomerModel
            // Customer
            public class CustomerRepository : Repository<CustomerModel>
        {
            public CustomerRepository(IDatabaseProvider dbProvider) : base(dbProvider) { }

            /*
                //get customer by name
                public async Task<int> Get_by_customer_name(string supplier_name)
                {
                    return await Connection.QuerySingleAsync<int>("SELECT supplier_pk FROM supplier WHERE supplier_name = @supplier_name", new { supplier_name }, Transaction);
                }
            */
            /*
                //get product_supplier
                public async Task<IEnumerable<string>> Selected_Supplier(int product_pk)
                {
                    return await Connection.QueryAsync<string>("SELECT " +
                    "supplier_name as selected_supplier_name from product_supplier ps " +
                    "INNER JOIN supplier s on s.supplier_pk = ps.supplier_fk " +
                    "WHERE ps.product_fk = @product_pk"
                    , new { product_pk }, Transaction);
                }
            */

            public override async Task<int> Insert(CustomerModel entity)
            {

                entity.user_name = entity.user_name ?? "";
                entity.first_name = entity.first_name ?? "";
                entity.last_name = entity.last_name ?? "";
                entity.address = entity.address ?? "";
                entity.city = entity.city ?? "";
                entity.state = entity.state ?? "";
                entity.zip = entity.zip ?? "";
                entity.country = entity.country ?? "";
                entity.phone = entity.phone ?? "";
                entity.shipping_address = entity.shipping_address ?? "";
                entity.shipping_city = entity.shipping_city ?? "";
                entity.shipping_state = entity.shipping_state ?? "";
                entity.shipping_zip = entity.shipping_zip ?? "";
                entity.shipping_country = entity.shipping_country ?? "";

                int newId = await Connection.QuerySingleAsync<int>("INSERT INTO customer " +
                    "(user_name, first_name, last_name, address, city, state, zip, country, phone, " +
                    "shipping_address, shipping_city, shipping_state, shipping_zip, shipping_country, same_shipping_address) " +
                    "VALUES (@user_name, @first_name, @last_name, @address, @city, @state, @zip, @country, @phone, " +
                    "@shipping_address, @shipping_city, @shipping_state, @shipping_zip, @shipping_country, @same_shipping_address); SELECT last_insert_rowid()", new
                    {
                        user_name = entity.user_name,
                        first_name = entity.first_name,
                        last_name = entity.last_name,
                        address = entity.address,
                        city = entity.city,
                        state = entity.state,
                        zip = entity.zip,
                        country = entity.country,
                        phone = entity.phone,
                        shipping_address = entity.shipping_address,
                        shipping_city = entity.shipping_city,
                        shipping_state = entity.shipping_state,
                        shipping_zip = entity.shipping_zip,
                        shipping_country = entity.shipping_country,
                        same_shipping_address = entity.same_shipping_address
                    }, Transaction);
                return newId;
            }


            public async Task<int> Get_cust_pk(int id)
            {
                var result = await Connection.QueryFirstOrDefaultAsync<int?>("SELECT customer_pk FROM customer WHERE customer_pk = @id", new { id }, Transaction);

                if (result.HasValue)
                {
                    return result.Value;
                }
                else
                {
                    return 0;
                }
            }

            public override async Task<CustomerModel> Get(int id)
            {
                return await Connection.QuerySingleAsync<CustomerModel>("SELECT customer_pk FROM customer WHERE customer_pk = @id", new { id }, Transaction);
            }

            public override async Task<IEnumerable<CustomerModel>> GetAll()
            {
                return await Connection.QueryAsync<CustomerModel>("SELECT customer_pk, user_name, first_name, last_name, address, city, state, zip, country, phone, shipping_address, shipping_city, shipping_state, shipping_zip, shipping_country, same_shipping_address FROM customer ORDER BY last_name", null, Transaction);
            }

            public override async Task<IEnumerable<CustomerModel>> GetAllWithID(int id)
            {
                return await Connection.QueryAsync<CustomerModel>("SELECT " +
                "customer_pk, user_name, first_name, last_name, address, city, state, zip, " +
                "country, phone, shipping_address, shipping_city, shipping_state, shipping_zip, " +
                "shipping_country, same_shipping_address " +
                "FROM customer " +
                "WHERE customer_pk = @id " +
                "ORDER BY last_name", new { id }, Transaction);
            }

            public override async Task<bool> Update(CustomerModel entity)
            {
                return (await Connection.ExecuteAsync("UPDATE customer SET user_name = @user_name, " +
                    "first_name = @first_name, last_name = @last_name, address = @address, " +
                    "state = @state, city = @city, zip = @zip, country = @country, phone = @phone, " +
                    "shipping_address = @shipping_address, shipping_city = @shipping_city, " +
                    "shipping_state = @shipping_state, shipping_zip = @shipping_zip, " +
                    "shipping_country = @shipping_country, same_shipping_address = @same_shipping_address " +
                    "WHERE customer_pk = @id", new
                    {
                        id = entity.customer_pk,
                        user_name = entity.user_name,
                        first_name = entity.first_name,
                        last_name = entity.last_name,
                        address = entity.address,
                        state = entity.state,
                        city = entity.city,
                        zip = entity.zip,
                        country = entity.country,
                        phone = entity.phone,
                        shipping_address = entity.shipping_address,
                        shipping_city = entity.shipping_city,
                        shipping_state = entity.shipping_state,
                        shipping_zip = entity.shipping_zip,
                        shipping_country = entity.shipping_country,
                        same_shipping_address = entity.same_shipping_address
                    }, Transaction)) == 1;
            }

            public override async Task<bool> Delete(CustomerModel entity)
            {
                return (await Connection.ExecuteAsync("DELETE FROM customer WHERE customer_pk = @id", new { id = entity.customer_pk }, Transaction)) == 1;
            }
        }
        #endregion

        #region CustomerOrderModel
        // table customer_order and customer_order_detail
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
            
        #endregion
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