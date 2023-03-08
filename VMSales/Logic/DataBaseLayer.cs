﻿using Dapper;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Threading.Tasks;
using System.Windows;
using VMSales.Models;
// Example implementation of the rRepository

// Our model.

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

            public override async Task<bool> Insert(CategoryModel entity)
            {
                int newId = await Connection.QuerySingleAsync<int>("INSERT INTO category (category_name, description, creation_date) VALUES (@category_name, @description, @creation_date); SELECT last_insert_rowid()", new
                {
                    category_name = entity.category_name,
                    description = entity.description,
                    creation_date = System.DateTime.Now
                }, Transaction);

                entity.category_pk = newId;

                return true;
            }

            public async Task<int> Get_by_category_name(string category_name)
            {
                return await Connection.QuerySingleAsync<int>("SELECT category_pk FROM category WHERE category_name = @category_name", new { category_name }, Transaction);
            }

            // keep for now, may not be needed.
            public async Task<CategoryModel> Get_by_category_pk(int category_pk)
            {
                return await Connection.QuerySingleAsync<CategoryModel>("SELECT category_name FROM category WHERE category_pk = @category_pk", new { category_pk }, Transaction);
            }


            public async Task<IEnumerable<CategoryModel>> Get_all_category_name()
            {
                return await Connection.QueryAsync<CategoryModel>("SELECT category_name FROM category ORDER BY category_name", null, Transaction);
            }
            // keep for now, may not be needed.

            public async Task<IEnumerable<CategoryModel>> Get_Product_Category()
            {
                return await Connection.QueryAsync<CategoryModel>(
                /*                  "SELECT DISTINCT category_pk, category_name, (category_pk - 1) as selected_category  FROM category as c, product_category as pc " +
                                  "INNER JOIN product_category on pc.category_fk = c.category_pk " +
                                  "WHERE pc.product_fk = @product_pk " +
                                  "UNION " +
                                  "SELECT DISTINCT category_pk, category_name, null  FROM category as c, product_category as pc " +
                                  "INNER JOIN product_category on pc.category_fk != c.category_pk"
                                   , product_pk }, Transaction);
                */
                "SELECT DISTINCT category_pk, category_name, (category_pk - 1) as selected_category  " +
                "FROM category as c, product_category as pc " +
                "INNER JOIN product_category on pc.category_fk = c.category_pk UNION " +
                "SELECT category_pk, category_name, null  FROM category as c"
                , null, Transaction);



            }

            // end


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
                return await Connection.QuerySingleAsync<int>("SELECT supplier_pk FROM supplier WHERE supplier_name = @supplier_name", new { supplier_name }, Transaction);
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


            public override async Task<bool> Insert(SupplierModel entity)
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

                return true;
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
                // Could check for unset id, and throw exception. // fix
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
                // Could check for unset id, and throw exception.
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

            public override async Task<bool> Insert(PurchaseOrderModel entity)
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
                        bool insertrow = (await Connection.ExecuteAsync("INSERT INTO purchase_order_detail " +
                        "(purchase_order_fk, lot_cost, lot_quantity, lot_number, lot_name, lot_description, sales_tax, shipping_cost, quantity_check) " +
                        "VALUES (@db_purchase_order_fk, @lot_cost, @lot_quantity, @lot_number, @lot_name, @lot_description, @sales_tax, @shipping_cost, @quantity_check)",
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
                        return insertrow;
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

                        bool insertrow = (await Connection.ExecuteAsync("INSERT INTO purchase_order_detail " +
                          "(purchase_order_fk, lot_cost, lot_quantity, lot_number, lot_name, lot_description, sales_tax, shipping_cost, quantity_check) " +
                            "VALUES (@purchase_order_fk, @lot_cost, @lot_quantity, @lot_number, @lot_name, @lot_description, @sales_tax, @shipping_cost, @quantity_check)",
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
                            }, Transaction)) == 1;
                        return insertrow;
                    }
                    return false;
                }
                catch (Exception e)
                {
                    MessageBox.Show(e.ToString());
                    return false;
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

            //             }, null)) == 1;



            //get last insert
            public async Task<int> Get_last_insert()
            {
                var result = await Connection.QuerySingleAsync<int>("SELECT MAX(purchase_order_detail_pk) FROM purchase_order_detail;", Transaction);
                return result;
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
            public async Task<IEnumerable<PurchaseOrderModel>> Get_PurchaseOrderDetail_by_pk(int purchase_order_detail_pk)
            {
                return await Connection.QueryAsync<PurchaseOrderModel>("SELECT DISTINCT " +
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

                    // getting fk constraint fialed here, why.
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
                //         bool deleterow = (await Connection.ExecuteAsync("DELETE FROM purchase_order_detail WHERE purchase_order_fk = @id", new { id = entity.purchase_order_fk }, null)) == 1;
                //         return (await Connection.ExecuteAsync("DELETE FROM purchase_order WHERE purchase_order_pk = @id", new { id = entity.purchase_order_pk }, Transaction)) == 1;
                return false;
            }

            // generate product command
            public async Task<IEnumerable<int>> Eligible_Products()
            {

                /*             string sql = "SELECT purchase_order_detail_pk " +
                             "FROM purchase_order_detail as pod " +
                             "LEFT JOIN product_purchase_order as ppo " +
                             "on pod.purchase_order_detail_pk != ppo.product_purchase_order_detail_fk " +
                             "WHERE pod.quantity_check = 0 " +
                             "AND pod.lot_quantity > '0' " +
                             "AND iif(exists(SELECT * FROM product_purchase_order), 1, 0) = 0 " +
                             "UNION SELECT purchase_order_detail_pk FROM purchase_order_detail as pod " +
                             "INNER JOIN product_purchase_order as ppo on pod.purchase_order_detail_pk " +
                             "!= ppo.product_purchase_order_detail_fk " +
                             "WHERE pod.quantity_check = 0 " +
                             "AND pod.lot_quantity > '0' " +
                             "AND iif(exists(SELECT * FROM product_purchase_order), 1, 0) = 0 " +
                             "ORDER BY purchase_order_detail_pk";
                */

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
            public async Task<int> InsertProduct(ProductModel entity)
            {
                int product_pk = await Connection.QuerySingleAsync<int>("INSERT INTO product " +
            "(brand_name, product_name, description, quantity, cost, sku, sold_price, instock, condition, listing_url, listing_number, listing_date) " +
            "VALUES (@brand_name, @product_name, @description, @quantity, @cost, @sku, @sold_price, @instock, @condition, @listing_url, @listing_number, @listing_date); SELECT last_insert_rowid()",
            new
            {
                brand_name = entity.brand_name,
                product_name = entity.product_name,
                description = entity.description,
                quantity = entity.quantity,
                cost = entity.cost,
                sku = entity.sku,
                sold_price = entity.sold_price,
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
                bool insertproductcategory = (await Connection.ExecuteAsync("INSERT INTO product_category (product_fk, category_fk) VALUES (@product_fk, @category_fk);", new
                {
                    product_fk = entity.product_fk,
                    category_fk = entity.category_fk
                }, Transaction)) == 1;
                return insertproductcategory;
            }

            public override async Task<bool> Insert(ProductModel entity)
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
                    bool insert_product_supplier = (await Connection.ExecuteAsync("INSERT INTO product_supplier (supplier_fk, product_fk) VALUES (@supplier_fk, @product_fk);", new
                    {
                        supplier_fk = entity.supplier_fk,
                        product_fk = entity.product_pk,
                    }, Transaction)) == 1;
                    return insert_product_supplier;
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
                /*    
                    old --- return await Connection.QueryAsync<ProductModel>("SELECT DISTINCT " +
                    "c.category_pk, c.category_name, brand_name, product_name, p.description, quantity, cost, sku, sold_price, instock, condition, listing_url, " +
                    "listing_number, listing_date " +
                    "FROM product as p, product_purchase_order as ppo, product_category as pc, category as c " +
                    "INNER JOIN product_purchase_order on p.product_pk = ppo.product_fk " +
                    "INNER JOIN product_category on p.product_pk = pc.product_fk " +
                    "INNER JOIN category on c.category_pk = pc.category_fk;", null, Transaction);
                */
                return await Connection.QueryAsync<ProductModel>("SELECT DISTINCT " +

                "p.product_pk, p.brand_name, p.product_name, p.description, p.quantity, p.cost, p.sku, p.sold_price, p.instock, p.condition, p.listing_url, p.listing_number, p.listing_date, c.category_pk, c.category_name " +
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
                    "p.cost, p.sku, p.sold_price, p.instock, p.condition, p.listing_url, p.listing_number, p.listing_date " +
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
                // done
                bool updaterow = (await Connection.ExecuteAsync("UPDATE product_category SET " +
                    "purchase_category_pk = @id, " +
                    "product_fk = @product_fk, " +
                    "category_fk = @category_fk, " +
                    "WHERE purchase_category_pk = @id", new
                    {
                        id = entity.product_category_pk,
                        product_fk = entity.product_fk,
                        category_fk = entity.category_fk
                    }, null)) == 1;


                return (await Connection.ExecuteAsync("UPDATE product SET " +
                        "brand_name = @purchase_order_fk, " +
                        "product_name = @lot_cost, " +
                        "description = @lot_quantity, " +
                        "quantity = @lot_number, " +
                        "cost = @lot_name, " +
                        "sku = @lot_description, " +
                        "sold_price = @sales_tax, " +
                        "instock = @instock, " +
                        "condition = @sales_tax, " +
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
                            sold_price = entity.sold_price,
                            instock = entity.instock,
                            condition = entity.condition,
                            listing_url = entity.listing_url,
                            listing_number = entity.listing_number,
                            listing_date = entity.listing_date
                        }, Transaction)) == 1;
            }

            public override async Task<bool> Delete(ProductModel entity)
            {
                // fix
                //bool deleterow = (await Connection.ExecuteAsync("DELETE FROM purchase_order_detail WHERE purchase_order_fk = @id", new { id = entity.purchase_order_fk }, null)) == 1;
                //return (await Connection.ExecuteAsync("DELETE FROM purchase_order WHERE purchase_order_pk = @id", new { id = entity.purchase_order_pk }, Transaction)) == 1;
                return false;
            }

            public override Task<IEnumerable<ProductModel>> GetAllWithID(int id)
            {
                throw new NotImplementedException();
            }
            //product_supplier
        }
        #endregion





        // needs fixing here
        /*
        #region Product_Purchase_Order
        public class ProductPurchaseOrderRepository : Repository<ProductPurchaseOrder>
        {
            public ProductPurchaseOrderRepository(IDatabaseProvider dbProvider) : base(dbProvider) { }

            // Insert
            public override async Task<bool> Insert(ProductPurchaseOrder entity)
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

            public override async Task<ProductPurchaseOrder> Get(int id)
            {
                return await Connection.QuerySingleAsync<ProductPurchaseOrder>("SELECT purchase_order_detail_pk FROM purchase_order_detail as pod INNER JOIN purchase_order on purchase_order.purchase_order_pk = pod.purchase_order_fk WHERE supplier_fk = @id;", new { id }, Transaction);
            }

            // get all purchase_order and purchase_order_detail
            public override async Task<IEnumerable<ProductPurchaseOrder>> GetAll()
            {
                return await Connection.QueryAsync<ProductPurchaseOrder>("SELECT " +
                    "distinct po.purchase_order_pk, po.purchase_date, po.invoice_number, pod.purchase_order_detail_pk, " +
                    "pod.purchase_order_fk, pod.lot_number, pod.lot_cost, pod.lot_quantity," +
                    "pod.lot_name, pod.lot_description, pod.sales_tax, pod.shipping_cost " +
                    "FROM purchase_order as po, purchase_order_detail as pod, supplier as sup " +
                    "INNER JOIN purchase_order_detail on po.purchase_order_pk = pod.purchase_order_fk " +
                    "INNER JOIN supplier on sup.supplier_pk = po.supplier_fk;", null, Transaction);
            }

            // get all products with product category
            public override async Task<IEnumerable<ProductPurchaseOrder>> GetAllWithID(int id)
            {
                return await Connection.QueryAsync<ProductPurchaseOrder>("SELECT " +
                    "distinct po.purchase_order_pk, po.purchase_date, po.invoice_number, " +
                    "pod.purchase_order_detail_pk, pod.purchase_order_fk, pod.lot_number, pod.lot_cost, pod.lot_quantity," +
                    "pod.lot_name, pod.lot_description, pod.sales_tax, pod.shipping_cost " +
                    "FROM purchase_order as po, purchase_order_detail as pod, supplier as sup " +
                    "INNER JOIN purchase_order_detail on po.purchase_order_pk = pod.purchase_order_fk " +
                    "INNER JOIN supplier on sup.supplier_pk = po.supplier_fk " +
                    "AND po.supplier_fk=@id", new { id }, Transaction);
            }

            public override async Task<bool> Update(ProductPurchaseOrder entity)
            {

                return (await Connection.ExecuteAsync("UPDATE product SET " +
                        "brand_name = @purchase_order_fk, " +
                        "product_name = @lot_cost, " +
                        "description = @lot_quantity, " +
                        "quantity = @lot_number, " +
                        "cost = @lot_name, " +
                        "sku = @lot_description, " +
                        "sold_price = @sales_tax, " +
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

            public override async Task<bool> Delete(ProductPurchaseOrder entity)
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

            public override async Task<bool> Insert(CustomerModel entity)
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
                return true;
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
        public class CustomerOrderRepository : Repository<CustomerOrderModel>
        {
            public CustomerOrderRepository(IDatabaseProvider dbProvider) : base(dbProvider) { }


            public override async Task<IEnumerable<CustomerOrderModel>> GetAllWithID(int id)
            {
                return null;
            }

            public override async Task<CustomerOrderModel> Get(int id)
            {
                //change
                return await Connection.QuerySingleAsync<CustomerOrderModel>("SELECT customer_pk FROM customer WHERE customer_pk = @id", new { id }, Transaction);
            }
            

            public override async Task<IEnumerable<CustomerOrderModel>> GetAll()
            {
                // change
                //            return await Connection.QueryAsync<CustomerOrderModel>("SELECT customer_pk, user_name, first_name, last_name, address, city, state, zip, country, phone, shipping_address, shipping_city, shipping_state, shipping_zip, shipping_country, same_shipping_address FROM customer ORDER BY last_name", null, Transaction);
                return null;
            }

            public override async Task<bool> Insert(CustomerOrderModel entity)
            {
                //change
                return false;
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