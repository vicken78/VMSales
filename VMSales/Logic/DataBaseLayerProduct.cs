using Caliburn.Micro;
using Dapper;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Security.Policy;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Forms;
using System.Windows.Shapes;
using VMSales.Models;


namespace VMSales.Logic
{
    public class ProductRepository : Repository<ProductModel>
    {
        public ProductRepository(IDatabaseProvider dbProvider) : base(dbProvider) { }
        //map search text to proper column
        internal Dictionary<string, string> mapping = new Dictionary<string, string>
        {
            { "Product Name", "product_name" },
            { "Description", "description" },
            { "Quantity", "quantity" },
            { "Cost", "cost" },
            { "Listed Price", "listed_price" },
            { "Listed Number", "listed_number" },
            { "Listed URL", "listing_url" },
            { "Listed Date", "listing_date" }
        };



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
            //check why is this failing, insert if dont exist maybe
            bool insertproductcategory = (await Connection.ExecuteAsync("INSERT INTO product_category (product_fk, category_fk) VALUES (@product_fk, @category_fk);", new
            {
                product_fk = entity.product_fk,
                category_fk = entity.category_fk
            }, Transaction)) == 1;
            return insertproductcategory;
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

        public async Task<IEnumerable<ProductModel>> GetAllWithID(int supplier_fk, int category_fk, string selected_product_filter, string searchtext)
        {


            var actions = new Dictionary<(bool, bool, bool), Func<Task<IEnumerable<ProductModel>>>>
        {
            { (true, false, false), () => ReturnCategoryOnly() },
            { (false, true, false), () => ReturnSupplierOnly() },
            { (true, true, false), () => ReturnCategoryAndSupplier() },
            { (false, false, true), () => ReturnProductSearchOnly(selected_product_filter, searchtext) },
            { (true, false, true), () => ReturnCategoryAndProductSearch(selected_product_filter, searchtext,category_fk) },
            { (false, true, true), () => ReturnSupplierAndProductSearch(selected_product_filter, searchtext,supplier_fk) },
            { (true, true, true), () => ReturnAll(selected_product_filter, searchtext, category_fk, supplier_fk) }
        };
            // Determine if each variable is non-empty
            bool hasCategory = category_fk > 0;
            bool hasSupplier = supplier_fk > 0;
            bool hasProductSearch = !string.IsNullOrEmpty(searchtext);
            // Look up the appropriate action in the dictionary
            if (actions.TryGetValue((hasCategory, hasSupplier, hasProductSearch), out var func))
            {
                // Return the corresponding value
                return await func.Invoke();
            }

            async Task<IEnumerable<ProductModel>> ReturnCategoryOnly()
            {
                // Logic for "category only" case
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

            async Task<IEnumerable<ProductModel>> ReturnSupplierOnly()
            {
                // Logic for "supplier only" case
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

            async Task<IEnumerable<ProductModel>> ReturnCategoryAndSupplier()
            // Logic for "supplier and category" case
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

        async Task<IEnumerable<ProductModel>> ReturnProductSearchOnly(string selected_product_filter, string searchtext)
        {
            if (mapping.TryGetValue(selected_product_filter, out string columnName))
            {
                // Perform your search logic here, using the columnName and searchtext
                string query = $"SELECT * FROM product WHERE {columnName} LIKE @searchtext";

                // Ensure the search text is formatted correctly with SQL wildcards
                var parameters = new { searchtext = $"%{searchtext}%" };  // Adding '%' for wildcard search

                // Execute the query and return the results
                return await Connection.QueryAsync<ProductModel>(query, parameters);
            }
            else
            {
                throw new ArgumentException("Invalid product filter selected.");
            }
        }

        async Task<IEnumerable<ProductModel>> ReturnCategoryAndProductSearch(string selected_product_filter, string searchtext, int category_fk)
        {

            if (mapping.TryGetValue(selected_product_filter, out string columnName))
            {

                

                // Perform your search logic here, using the columnName , searchtext and category_fk
                string query =
                $"SELECT DISTINCT c.category_pk, c.category_name, p.*, ps.* " +
                $"FROM product p " +
                $"INNER JOIN product_supplier ps ON p.product_pk = ps.product_fk " +
                $"INNER JOIN supplier s ON s.supplier_pk = ps.supplier_fk " +
                $"LEFT JOIN product_category pc ON p.product_pk = pc.product_fk " +
                $"LEFT JOIN category c ON c.category_pk = pc.category_fk " +
                $"WHERE pc.category_fk = @category_fk " +
                $"AND p.{columnName} LIKE @searchtext " +
                $"UNION " +
                $"SELECT DISTINCT c.category_pk, c.category_name, p.*, ps.* " +
                $"FROM product p " +
                $"INNER JOIN product_supplier ps ON p.product_pk = ps.product_fk " +
                $"INNER JOIN supplier s ON s.supplier_pk = ps.supplier_fk " +
                $"INNER JOIN product_category pc ON p.product_pk = pc.product_fk " +
                $"INNER JOIN category c ON c.category_pk = pc.category_fk " +
                $"WHERE pc.category_fk = @category_fk " +
                $"AND p.{columnName} LIKE @searchtext";

                // Ensure the search text is formatted correctly with SQL wildcards
                var parameters = new { searchtext = $"%{searchtext}%", category_fk };  // Adding '%' for wildcard search

                // Execute the query and return the results
                return await Connection.QueryAsync<ProductModel>(query, parameters);
            }
            else
            {
                throw new ArgumentException("Invalid product filter selected.");
            }
        }

        async Task<IEnumerable<ProductModel>> ReturnSupplierAndProductSearch(string selected_product_filter, string searchtext, int supplier_fk)
        {
            if (mapping.TryGetValue(selected_product_filter, out string columnName))
            {
                // Perform your search logic here, using the columnName , searchtext and category_fk
                string query =
                $"SELECT DISTINCT c.category_pk, c.category_name, p.*, ps.* " +
                $"FROM product p " +
                $"INNER JOIN product_supplier ps ON p.product_pk = ps.product_fk " +
                $"INNER JOIN supplier s ON s.supplier_pk = ps.supplier_fk " +
                $"LEFT JOIN product_category pc ON p.product_pk = pc.product_fk " +
                $"LEFT JOIN category c ON c.category_pk = pc.category_fk " +
                $"WHERE ps.supplier_fk = @supplier_fk " +
                $"AND p.{columnName} LIKE @searchtext " +
                $"UNION " +
                $"SELECT DISTINCT c.category_pk, c.category_name, p.*, ps.* " +
                $"FROM product p " +
                $"INNER JOIN product_supplier ps ON p.product_pk = ps.product_fk " +
                $"INNER JOIN supplier s ON s.supplier_pk = ps.supplier_fk " +
                $"INNER JOIN product_category pc ON p.product_pk = pc.product_fk " +
                $"INNER JOIN category c ON c.category_pk = pc.category_fk " +
                $"WHERE ps.supplier_fk = @supplier_fk " +
                $"AND p.{columnName} LIKE @searchtext";


                // Ensure the search text is formatted correctly with SQL wildcards
                var parameters = new { searchtext = $"%{searchtext}%", supplier_fk };  // Adding '%' for wildcard search

                // Execute the query and return the results
                return await Connection.QueryAsync<ProductModel>(query, parameters);
            }
            else
            {
                throw new ArgumentException("Invalid product filter selected.");
            }
        }






        async Task<IEnumerable<ProductModel>> ReturnAll(string selected_product_filter, string searchtext, int supplier_fk, int category_fk)
        // Logic for "supplier and category" and product search case
        {
            if (mapping.TryGetValue(selected_product_filter, out string columnName))
            {
                string query =
                $"SELECT DISTINCT c.category_pk, c.category_name, p.*, ps.* " +
                $"FROM product p " +
                $"INNER JOIN product_supplier ps ON p.product_pk = ps.product_fk " +
                $"INNER JOIN supplier s ON s.supplier_pk = ps.supplier_fk " +
                $"LEFT JOIN product_category pc ON p.product_pk = pc.product_fk " +
                $"LEFT JOIN category c ON c.category_pk = pc.category_fk " +
                $"WHERE ps.supplier_fk = @supplier_fk AND pc.category_fk = @category_fk " +
                $"AND p.{columnName} LIKE @searchtext " +
                $"UNION SELECT DISTINCT " +
                $"c.category_pk, c.category_name, p.*, ps.* " +
                $"FROM product p " +
                $"INNER JOIN product_supplier ps ON p.product_pk = ps.product_fk " +
                $"INNER JOIN supplier s ON s.supplier_pk = ps.supplier_fk " +
                $"INNER JOIN product_category pc ON p.product_pk = pc.product_fk " +
                $"INNER JOIN category c ON c.category_pk = pc.category_fk " +
                $"WHERE ps.supplier_fk = @supplier_fk AND pc.category_fk = @category_fk " +
                $"AND p.{columnName} LIKE @searchtext";

                var parameters = new { searchtext = $"%{searchtext}%", supplier_fk, category_fk};  // Adding '%' for wildcard search
                return await Connection.QueryAsync<ProductModel>(query, parameters);

            }
            else
        {
                {
                    throw new ArgumentException("Invalid product filter selected.");
                }
            }
        }

        


    // end filter

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

        // get product category
        public async Task<bool> Get_Product_Category(ProductModel entity)
        {
            try
            {
                // Check if the record exists in the database
                var result = await Connection.QueryFirstAsync<int>("SELECT 1 FROM product_category WHERE product_fk = @product_fk AND category_fk = @category_fk", new
                {
                    product_fk = entity.product_fk,
                    category_fk = entity.category_fk
                }, Transaction);

                // If the query succeeded, a record was found
                return result == 1;
            }
            catch (InvalidOperationException)
            {
                // No records found, return false
                return false;
            }
        }

        /*
     //filter product
     public async Task<IEnumerable<ProductModel>> FilterProduct(string selected_product_filter, string searchtext)
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
             new { searchterm, selected_search }, Transaction);
     }

     */

        /*  public async Task<IEnumerable<ProductModel>> FilterProduct(string selected_product_filter, string searchtext)
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
                  new { searchterm, selected_search }, Transaction);
          }
 */

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
            "product_fk = @product_fk, category_fk = @category_fk WHERE product_fk = @product_fk", new
            {
                product_fk = entity.product_fk,
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
            }, Transaction)) == 1;
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

            }, Transaction)) == 1;
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
    }
}
