using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Dapper;
using System.Data;
using System.Data.SQLite;
using VMSales.Models;


namespace VMSales.Logic
{
    public static class Database
    {
     
        private static string getConnectionString()
        {
            const string ConnectionString = "Data Source=C:\\Users\\Vicken\\source\\repos\\testsqllite\\testsqllite\\db\\testsales2.db; FailIfMissing = True; Foreign Keys = True; Version=3;";
            return ConnectionString;
        }

        public static IDbConnection GetConnection()
        {
            return new SQLiteConnection(getConnectionString());
        }
        // Select Table
        public static List<T> getTable<T>(string table)
        {
            using (IDbConnection db = GetConnection())
            {
                var selectTable = db.Query<T>("Select * from "+table).ToList();
                return selectTable;
            }             
        }
        // Select Primary Key
        public static string getPrimaryKey(string primaryKey, string table)
        {
            using (IDbConnection db = GetConnection())
            {
                string primaryKa = null;
                //var selectTable = db.Query<T>("Select * from " + table).ToList();
                return primaryKa;
            }
        }

        // Select Relationship

        // Photos photo -> product Photo -> product


        //purchase order -> supplier

        //purchase order -> purchase_order detail

        //purchase_order_detail -> product_purcahse_order -> product

        // customer -> customer_order -> customer order detail

        // customer order detail -> product

        // Relationship: category -> product_category -> product
     /*   public static async Task<Dictionary<int, string>> getCategoryProductRelationship(string where)
        {
            using (var connection = new SQLiteConnection(getConnectionString()))
            {
                var sql = @"select productid, productname, p.categoryid, categoryname 
                from products p 
                inner join categories c on p.categoryid = c.categoryid";
                var products = await connection.QueryAsync<ProductModel, CategoryModel, ProductModel>(sql, (product, category) => {
                    product.Category = category;
                    return product;
                },
                splitOn: "CategoryId");
                products.ToList();        
            }
            return null;
        }
     */





            /*
            using (IDbConnection db = GetConnection())
            {
                var categoryRelatonship = new Dictionary<int, string>();

                db.Query<>;

                var sql = "";
                var list = db.QueryASYNC<CategoryModel, ProductModel, CategoryModel>(@"SELECT c.*, p.* FROM Category c INNER JOIN Product p ON c.category_PK = p. WHERE f.Id = 1"),(c, p) =>
                    {
                    CategoryModel categoryEntry;
                    if (!categoryRelatonship.TryGetValue(c.Id, out categoryEntry))
                    {
                        categoryEntry = c;
                        categoryEntry.products = new List<products>;
                        categoryRelationship.Add(categoryEntry.Id, categoryEntry);
                    }
                    categoryEntry.products.Add(p);
                    return CategoryEntry;
                }).Distinct().ToList();
            }
        }

            */


        // Insert

        // Update

        // Delete
    }
}
