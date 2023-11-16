using Dapper;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Threading.Tasks;
using System.Windows;
using VMSales.Models;

namespace VMSales.Logic
{
    public class DataBaseLayer : INotifyPropertyChanged
    {

        public event PropertyChangedEventHandler PropertyChanged;
        protected void RaisePropertyChanged(string property)
        {
            if (PropertyChanged != null)
                PropertyChanged(this, new PropertyChangedEventArgs(property));
        }

        #region Category
        public class CategoryRepository : Repository<CategoryModel>
        {

            public CategoryRepository(IDatabaseProvider dbProvider) : base(dbProvider) { }

            public override async Task<int> Insert(CategoryModel entity)
            {
                int newId = await Connection.QuerySingleAsync<int>("INSERT INTO category (category_name, description, creation_date) VALUES (@category_name, @description, @creation_date); SELECT last_insert_rowid()", new
                {
                    category_name = entity.category_name,
                    description = entity.description,
                    creation_date = System.DateTime.Now
                }, Transaction);

                entity.category_pk = newId;

                return newId;
            }

            public async Task<int> Get_by_category_name(string category_name)
            {
                return await Connection.QuerySingleAsync<int>("SELECT category_pk FROM category WHERE category_name = @category_name", new { category_name }, Transaction);
            }

            public async Task<CategoryModel> Get_by_category_pk(int category_pk)
            {
                return await Connection.QuerySingleAsync<CategoryModel>("SELECT category_name FROM category WHERE category_pk = @category_pk", new { category_pk }, Transaction);
            }

            public async Task<IEnumerable<CategoryModel>> Get_all_category_name()
            {
                return await Connection.QueryAsync<CategoryModel>("SELECT category_name FROM category ORDER BY category_name", null, Transaction);
            }

            public async Task<IEnumerable<CategoryModel>> Get_Product_Category()
            {
                return await Connection.QueryAsync<CategoryModel>(
                "SELECT DISTINCT category_pk, category_name, (category_pk - 1) as selected_category  " +
                "FROM category as c, product_category as pc " +
                "INNER JOIN product_category on pc.category_fk = c.category_pk UNION " +
                "SELECT category_pk, category_name, null  FROM category as c"
                , null, Transaction);

            }

            public override async Task<CategoryModel> Get(int id)
            {
                return await Connection.QuerySingleAsync<CategoryModel>("SELECT category_pk FROM category WHERE category_pk = @id", new { id }, Transaction);
            }

            public override async Task<IEnumerable<CategoryModel>> GetAll()
            {
                return await Connection.QueryAsync<CategoryModel>("SELECT category_pk, category_name, description, creation_date FROM category", null, Transaction);
            }

            public async Task<IEnumerable<CategoryModel>> GetCategory()
            {
                return await Connection.QueryAsync<CategoryModel>("SELECT category_pk, category_name FROM category", null, Transaction);
            }


            public override async Task<bool> Update(CategoryModel entity)
            {
                // Could check for unset id, and throw exception.
                return (await Connection.ExecuteAsync("UPDATE category SET category_name = @category_name, description = @description WHERE category_pk = @id", new
                {
                    id = entity.category_pk,
                    category_name = entity.category_name,
                    description = entity.description

                }, Transaction)) == 1;
            }

            public override async Task<bool> Delete(CategoryModel entity)
            {
                // Could check for unset id, and throw exception.
                return (await Connection.ExecuteAsync("DELETE FROM category WHERE category_pk = @id", new { id = entity.category_pk }, Transaction)) == 1;
            }

            public override Task<IEnumerable<CategoryModel>> GetAllWithID(int id)
            {
                throw new System.NotImplementedException();
            }
        }
        #endregion

        #region Supplier
        public class SupplierRepository : Repository<SupplierModel>
        {

            public SupplierRepository(IDatabaseProvider dbProvider) : base(dbProvider) { }


            //get supplier by name
            public async Task<int> Get_by_supplier_name(string supplier_name)
            {
                return await Connection.QuerySingleAsync<int>("SELECT supplier_pk FROM supplier WHERE supplier_name = @supplier_name ORDER BY supplier_name", new { supplier_name }, Transaction);
            }


            //get product_supplier
            public async Task<IEnumerable<string>> Selected_Supplier(int product_pk)
            {
                return await Connection.QueryAsync<string>("SELECT " +
                "supplier_name as selected_supplier_name from product_supplier ps " +
                "INNER JOIN supplier s on s.supplier_pk = ps.supplier_fk " +
                "WHERE ps.product_fk = @product_pk"
                , new { product_pk }, Transaction);
            }

            public override async Task<int> Insert(SupplierModel entity)
            {
                int newId = await Connection.QuerySingleAsync<int>("INSERT INTO supplier (supplier_name, address, city, zip, state, country, phone, email) VALUES (@supplier_name, @address, @city, @zip, @state, @country, @phone, @email); SELECT last_insert_rowid()", new
                {
                    supplier_name = entity.supplier_name,
                    address = entity.address,
                    city = entity.city,
                    zip = entity.zip,
                    state = entity.state,
                    country = entity.country,
                    phone = entity.phone,
                    email = entity.email
                }, Transaction);

                entity.supplier_pk = newId;

                return newId;
            }

            public override async Task<SupplierModel> Get(int id)
            {
                return await Connection.QuerySingleAsync<SupplierModel>("SELECT supplier_pk FROM supplier WHERE supplier_pk = @id", new { id }, Transaction);
            }

            public override async Task<IEnumerable<SupplierModel>> GetAll()
            {
                return await Connection.QueryAsync<SupplierModel>("SELECT supplier_pk, supplier_name, address, city, zip, state, country, phone, email FROM supplier ORDER BY supplier_name", null, Transaction);
            }

            public override async Task<bool> Update(SupplierModel entity)
            {
                return (await Connection.ExecuteAsync("UPDATE supplier SET supplier_name = @supplier_name, address = @address, city = @city, zip = @zip, state = @state, country = @country, phone = @phone, email = @email WHERE supplier_pk = @id", new
                {
                    id = entity.supplier_pk,
                    supplier_name = entity.supplier_name,
                    address = entity.address,
                    city = entity.city,
                    zip = entity.zip,
                    state = entity.state,
                    country = entity.country,
                    phone = entity.phone,
                    email = entity.email

                }, Transaction)) == 1;
            }

            public override async Task<bool> Delete(SupplierModel entity)
            {
                return (await Connection.ExecuteAsync("DELETE FROM supplier WHERE supplier_pk = @id", new { id = entity.supplier_pk }, Transaction)) == 1;
            }
            public override Task<IEnumerable<SupplierModel>> GetAllWithID(int id)
            {
                throw new System.NotImplementedException();
            }

        }
        #endregion
        #region PurchaseOrder
        public class PurchaseOrderRepository : Repository<PurchaseOrderModel>
        {
            public PurchaseOrderRepository(IDatabaseProvider dbProvider) : base(dbProvider) { }

            public override async Task<int> Insert(PurchaseOrderModel entity)
            {
                // get invoice number
                try
                {
                    string invoice_number = await Connection.QuerySingleOrDefaultAsync<string>("SELECT invoice_number FROM purchase_order WHERE invoice_number = @invoice_number", new { entity.invoice_number }, Transaction);

                    if (invoice_number == entity.invoice_number)
                    {
                        /// <summary>
                        /// scenerio 1
                        /// same invoice number, INSERT purchase_order_detail using existing po_fk
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

                    if (invoice_number != entity.invoice_number && entity.invoice_number != "0" && entity.invoice_number != null)
                    {
                        //  scenerio 2
                        // INSERT new invoice number, INSERT new purchase_order detail pod_pk = 0
                        // insert into purchase_order and purchase_order_detail using new purchase_order_fk
                        int newId = await Connection.QuerySingleAsync<int>("INSERT INTO purchase_order (supplier_fk, invoice_number, purchase_date) VALUES (@supplier_fk, @invoice_number, @purchase_date); SELECT last_insert_rowid()", new
                        {
                            supplier_fk = entity.supplier_fk,
                            invoice_number = entity.invoice_number,
                            purchase_date = entity.purchase_date
                        }, Transaction);
                        entity.purchase_order_fk = newId;
                        // now for purchase_order_detail

                        // erroring below cannot convert int to object?

                       
                        // cannot convert object to int

                        int purchase_order_detail_pk = await Connection.QueryFirstOrDefaultAsync("INSERT INTO purchase_order_detail " +
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
                catch (Exception e)
                {
                    MessageBox.Show(e.ToString());
                    return 0;
                }
            }

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
                    "distinct po.purchase_order_pk, po.purchase_date, po.invoice_number, pod.purchase_order_detail_pk, " +
                    "pod.purchase_order_fk, pod.lot_number, pod.lot_cost, pod.lot_quantity," +
                    "pod.lot_name, pod.lot_description, pod.sales_tax, pod.shipping_cost, pod.quantity_check " +
                    "FROM purchase_order as po, purchase_order_detail as pod, supplier as sup " +
                    "INNER JOIN purchase_order_detail on po.purchase_order_pk = pod.purchase_order_fk " +
                    "INNER JOIN supplier on sup.supplier_pk = po.supplier_fk;", null, Transaction);

            }


            public async Task<IEnumerable<PurchaseOrderModel>> GetAllTotalWithID(int id)
            {
                return await Connection.QueryAsync<PurchaseOrderModel>("SELECT " +
                    "SUM(pod.sales_tax) AS total_sales_tax, " +
                    "SUM(pod.shipping_cost) AS total_shipping, " +
                    "SUM(pod.lot_cost) AS total_lot, " +
                    "SUM(pod.sales_tax + pod.shipping_cost + pod.lot_cost) AS total_cost " +
                    "FROM purchase_order AS po " +
                    "INNER JOIN purchase_order_detail AS pod ON po.purchase_order_pk = pod.purchase_order_fk " +
                    "INNER JOIN supplier AS sup ON sup.supplier_pk = po.supplier_fk " +
                    "AND po.supplier_fk=#id", new { id }, Transaction);
            }

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

            // scenerio 3 (UPDATE)
            // same invoice number, UPDATE purchase_order_detail.



            // scenerio 4  (may not be needed)
            // INSERT new invoice number, UPDATE purchase_order_details to that invoice number.

            public override async Task<bool> Update(PurchaseOrderModel entity)
            {
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

        #endregion
        #region Product

        public class ProductRepository : Repository<ProductModel>
        {
            public ProductRepository(IDatabaseProvider dbProvider) : base(dbProvider) { }

            //Get Product Purchase Order
            public async Task<int> Get_product_purchase_order(int product_fk)
            {
                return await Connection.QuerySingleAsync<int>("SELECT product_purchase_order_detail_fk FROM product_purchase_order WHERE product_fk = @product_fk", new { product_fk }, Transaction);
            }

            // Insert
            public override async Task<int> Insert(ProductModel entity)
            {
                int product_pk = await Connection.QuerySingleAsync<int>("INSERT INTO product " +
            "(brand_name, product_name, description, quantity, cost, sku, listed_price, instock, condition, listing_url, listing_number, listing_date) " +
            "VALUES (@brand_name, @product_name, @description, @quantity, @cost, @sku, @listed_price, @instock, @condition, @listing_url, @listing_number, @listing_date); SELECT last_insert_rowid()",
            new
            {
                brand_name = entity.brand_name,
                product_name = entity.product_name,
                description = entity.description,
                quantity = entity.quantity,
                cost = entity.cost,
                sku = entity.sku,
                listed_price = entity.listed_price,
                instock = entity.instock,
                condition = entity.condition,
                listing_url = entity.listing_url,
                listing_number = entity.listing_number,
                listing_date = entity.listing_date

            }, Transaction);
                entity.product_pk = product_pk;

                return product_pk;
            }

            public async Task<bool> InsertProductCategory(ProductModel entity)
            {

                // failing
                bool insertproductcategory = (await Connection.ExecuteAsync("INSERT INTO product_category (product_fk, category_fk) VALUES (@product_fk, @category_fk);", new
                {
                    product_fk = entity.product_fk,
                    category_fk = entity.category_fk
                }, Transaction)) == 1;
                return insertproductcategory;
            }

            //fix
            public async Task<bool> Insert_Product_Purchase_Order(ProductModel entity)
            {
                // insert into product_purchase_order
                bool insert_product_purchase_order = (await Connection.ExecuteAsync("INSERT INTO product_purchase_order (product_purchase_order_detail_fk, product_fk) VALUES (@purchase_order_detail_fk, @product_fk);", new
                {
                    purchase_order_detail_fk = entity.purchase_order_detail_fk,
                    product_fk = entity.product_fk,
                }, Transaction)) == 1;

                if (insert_product_purchase_order == true)
                {
                    // insert into product_supplier
                    bool insert_product_supplier = (await Connection.QueryFirstOrDefaultAsync<int>("INSERT INTO product_supplier (supplier_fk, product_fk) VALUES (@supplier_fk, @product_fk); SELECT last_insert_rowid()", new
                    {
                        supplier_fk = entity.supplier_fk,
                        product_fk = entity.product_pk,
                    }, Transaction)) == 1;
                    return true;
                }
                return false;
            }


            public override async Task<ProductModel> Get(int id)
            {
                return await Connection.QuerySingleAsync<ProductModel>("SELECT purchase_order_detail_pk FROM purchase_order_detail as pod INNER JOIN purchase_order on purchase_order.purchase_order_pk = pod.purchase_order_fk WHERE supplier_fk = @id;", new { id }, Transaction);
            }

            // get all products
            public override async Task<IEnumerable<ProductModel>> GetAll()
            {
                return await Connection.QueryAsync<ProductModel>("SELECT DISTINCT " +

                "p.product_pk, p.brand_name, p.product_name, p.description, p.quantity, p.cost, p.sku, p.listed_price, p.instock, p.condition, p.listing_url, p.listing_number, p.listing_date, c.category_pk, c.category_name " +
                "FROM product as p " +
                "LEFT OUTER JOIN product_category as pc on p.product_pk = pc.product_fk " +
                "LEFT OUTER JOIN category as c on c.category_pk = pc.category_fk " +
                "ORDER BY product_pk"
                 , null, Transaction);
            }

            // get all products by supplier

            public async Task<IEnumerable<ProductModel>> GetAllWithID(int supplier_fk, int category_fk)
            {

                // category only
                if (category_fk != 0 && supplier_fk == 0)
                {
                    return await Connection.QueryAsync<ProductModel>(
                    "SELECT DISTINCT c.category_pk, c.category_name, p.*, ps.* " +
                    "FROM product p " +
                    "INNER JOIN product_supplier ps ON p.product_pk = ps.product_fk " +
                    "INNER JOIN supplier s ON s.supplier_pk = ps.supplier_fk " +
                    "LEFT JOIN product_category pc ON p.product_pk = pc.product_fk " +
                    "LEFT JOIN category c ON c.category_pk = pc.category_fk " +
                    "WHERE pc.category_fk = @category_fk " +
                    "UNION SELECT DISTINCT " +
                    "c.category_pk, c.category_name, p.*, ps.* " +
                    "FROM product p " +
                    "INNER JOIN product_supplier ps ON p.product_pk = ps.product_fk " +
                    "INNER JOIN supplier s ON s.supplier_pk = ps.supplier_fk " +
                    "INNER JOIN product_category pc ON p.product_pk = pc.product_fk " +
                    "INNER JOIN category c ON c.category_pk = pc.category_fk " +
                    "WHERE pc.category_fk = @category_fk ", new { category_fk }, Transaction);
                }
                // supplier only
                if (category_fk == 0 && supplier_fk != 0)
                {
                    return await Connection.QueryAsync<ProductModel>(
                "SELECT DISTINCT c.category_pk, c.category_name, p.*, ps.* " +
                "FROM product p " +
                "INNER JOIN product_supplier ps ON p.product_pk = ps.product_fk " +
                "INNER JOIN supplier s ON s.supplier_pk = ps.supplier_fk " +
                "LEFT JOIN product_category pc ON p.product_pk = pc.product_fk " +
                "LEFT JOIN category c ON c.category_pk = pc.category_fk " +
                "WHERE ps.supplier_fk = @supplier_fk " +
                "UNION SELECT DISTINCT " +
                "c.category_pk, c.category_name, p.*, ps.* " +
                "FROM product p " +
                "INNER JOIN product_supplier ps ON p.product_pk = ps.product_fk " +
                "INNER JOIN supplier s ON s.supplier_pk = ps.supplier_fk " +
                "INNER JOIN product_category pc ON p.product_pk = pc.product_fk " +
                "INNER JOIN category c ON c.category_pk = pc.category_fk " +
                "WHERE ps.supplier_fk = @supplier_fk", new { supplier_fk }, Transaction);
                }
                // supplier and category
                if (category_fk != 0 && supplier_fk != 0)
                {
                    return await Connection.QueryAsync<ProductModel>(
                "SELECT DISTINCT c.category_pk, c.category_name, p.*, ps.* " +
                "FROM product p " +
                "INNER JOIN product_supplier ps ON p.product_pk = ps.product_fk " +
                "INNER JOIN supplier s ON s.supplier_pk = ps.supplier_fk " +
                "LEFT JOIN product_category pc ON p.product_pk = pc.product_fk " +
                "LEFT JOIN category c ON c.category_pk = pc.category_fk " +
                "WHERE ps.supplier_fk = @supplier_fk AND pc.category_fk = @category_fk " +
                "UNION SELECT DISTINCT " +
                "c.category_pk, c.category_name, p.*, ps.* " +
                "FROM product p " +
                "INNER JOIN product_supplier ps ON p.product_pk = ps.product_fk " +
                "INNER JOIN supplier s ON s.supplier_pk = ps.supplier_fk " +
                "INNER JOIN product_category pc ON p.product_pk = pc.product_fk " +
                "INNER JOIN category c ON c.category_pk = pc.category_fk " +
                "WHERE ps.supplier_fk = @supplier_fk AND pc.category_fk = @category_fk"
                , new { supplier_fk, category_fk }, Transaction);
                }
                return null;
            }

            // get all products by supplier and purchase_order
            public async Task<IEnumerable<ProductModel>> GetAllWithAllID(int supplier_fk, int purchase_order_detail_pk)
            {

                return await Connection.QueryAsync<ProductModel>("SELECT DISTINCT " +
                    "ppo.*, c.category_pk, c.category_name, p.product_pk, p.brand_name, p.product_name, p.description, p.quantity, " +
                    "p.cost, p.sku, p.listed_price, p.instock, p.condition, p.listing_url, p.listing_number, p.listing_date " +
                    "FROM product_purchase_order as ppo, product as p, product_category as pc, " +
                    "category as c, product_supplier as ps, supplier as s, purchase_order_detail as pod " +
                    "INNER JOIN product_purchase_order on pod.purchase_order_detail_pk = ppo.purchase_order_detail_fk " +
                    "INNER JOIN product_purchase_order on p.product_pk = ppo.product_fk " +
                    "INNER JOIN product_category on c.category_pk = pc.category_fk " +
                    "INNER JOIN product on p.product_pk = pc.product_fk " +
                    "INNER JOIN product_supplier on ps.supplier_fk = s.supplier_pk " +
                    "INNER JOIN product_supplier on ps.product_fk = p.product_pk " +
                    "WHERE s.supplier_pk = @supplier_fk AND pod.purchase_order_detail_pk = @purchase_order_detail_pk",
                    new { supplier_fk, purchase_order_detail_pk }, Transaction);
            }

            public override async Task<bool> Update(ProductModel entity)
            {
                bool update_product = (await Connection.ExecuteAsync("UPDATE product SET " +
                        "brand_name = @brand_name, " +
                        "product_name = @product_name, " +
                        "description = @description, " +
                        "quantity = @quantity, " +
                        "cost = @cost, " +
                        "sku = @sku, " +
                        "listed_price = @listed_price, " +
                        "instock = @instock, " +
                        "condition = @condition, " +
                        "listing_url = @listing_url, " +
                        "listing_number = @listing_number, " +
                        "listing_date = @listing_date " +
                        "WHERE product_pk = @product_pk", new
                        {
                            product_pk = entity.product_pk,
                            brand_name = entity.brand_name,
                            product_name = entity.product_name,
                            description = entity.description,
                            quantity = entity.quantity,
                            cost = entity.cost,
                            sku = entity.sku,
                            listed_price = entity.listed_price,
                            instock = entity.instock,
                            condition = entity.condition,
                            listing_url = entity.listing_url,
                            listing_number = entity.listing_number,
                            listing_date = entity.listing_date
                        }, Transaction)) == 1;
                return update_product;
            }

            public async Task<bool> Update_Product_Category(ProductModel entity)
            {
                bool update_product_category = (await Connection.ExecuteAsync("UPDATE product_category SET " +
                "product_fk = @product_fk, " +
                "category_fk = @category_fk " +
                "WHERE product_fk = @id", new
                {
                    id = entity.product_pk,
                    product_fk = entity.product_pk,
                    category_fk = entity.category_fk
                }, null)) == 1;
                return update_product_category;
            }

            public async Task<bool> Update_Product_Supplier(ProductModel entity)
            {
                bool update_product_supplier = (await Connection.ExecuteAsync("UPDATE product_supplier SET " +
                "product_fk = @product_fk, " +
                "supplier_fk = @supplier_fk " +
                "WHERE product_fk = @id", new
                {
                    id = entity.product_pk,
                    product_fk = entity.product_pk,
                    supplier_fk = entity.supplier_fk
                }, null)) == 1;
                return update_product_supplier;
            }

            public async Task<bool> Update_Product_Purchase_Order(ProductModel entity)
            {
                bool update_product_purchase_order = (await Connection.ExecuteAsync("UPDATE product_purchase_order SET " +
                "product_purchase_order_detail_fk = @product_purchase_order_detail_fk, " +
                "product_fk = @product_fk " +
                "WHERE product_fk = @id", new
                {
                    id = entity.product_pk,
                    product_fk = entity.product_pk,
                    product_purchase_order_detail_fk = entity.purchase_order_detail_fk

                }, null)) == 1;
                return update_product_purchase_order;
            }

            public async Task<string> Check_Product_Customer(ProductModel entity)
            {
                    string orderNumber = await Connection.QuerySingleOrDefaultAsync<string>(
                    "SELECT order_number FROM customer_order " +
                    "INNER JOIN customer_order_detail on customer_order.customer_order_pk = customer_order_fk " +
                    "WHERE customer_order_detail.product_fk = @product_fk",
                    new { product_fk = entity.product_pk });
                    return orderNumber;
            }

            //FIX -- Not fully implemented
            public override async Task<bool> Delete(ProductModel entity)
            
            {
                bool deleterow = (await Connection.ExecuteAsync("...DELETE FROM purchase_order_detail WHERE purchase_order_fk = @id", new { id = entity.product_fk }, null)) == 1;
                return (await Connection.ExecuteAsync("...DELETE FROM purchase_order WHERE purchase_order_pk = @id", new { id = entity.product_fk }, Transaction)) == 1;
            }

            public async Task<bool> Delete_Product_Category(ProductModel entity)
            {
                //bool deleterow = (await Connection.ExecuteAsync("...DELETE FROM purchase_order_detail WHERE purchase_order_fk = @id", new { id = entity.product_fk }, null)) == 1;
                //return (await Connection.ExecuteAsync("...DELETE FROM purchase_order WHERE purchase_order_pk = @id", new { id = entity.product_fk }, Transaction)) == 1;
                return false;
            }

            public async Task<bool> Delete_Product_Supplier(ProductModel entity)
            {
                //bool deleterow = (await Connection.ExecuteAsync("...DELETE FROM purchase_order_detail WHERE purchase_order_fk = @id", new { id = entity.product_fk }, null)) == 1;
                //return (await Connection.ExecuteAsync("...DELETE FROM purchase_order WHERE purchase_order_pk = @id", new { id = entity.product_fk }, Transaction)) == 1;
                return false;
            }

            public async Task<bool> Delete_Product_Purchase_Order(ProductModel entity)
            {
                //bool deleterow = (await Connection.ExecuteAsync("...DELETE FROM purchase_order_detail WHERE purchase_order_fk = @id", new { id = entity.product_fk }, null)) == 1;
                //return (await Connection.ExecuteAsync("...DELETE FROM purchase_order WHERE purchase_order_pk = @id", new { id = entity.product_fk }, Transaction)) == 1;
                return false;
            }







            public override Task<IEnumerable<ProductModel>> GetAllWithID(int id)
            {
                throw new NotImplementedException();
            }
            //product_supplier
        }
        #endregion


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