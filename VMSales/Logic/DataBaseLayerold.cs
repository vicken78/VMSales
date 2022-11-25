using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Dapper;
using System.Data;
using Dapper.Contrib.Extensions;
using System.Data.SQLite;
using VMSales.Models;
using System.Windows;

namespace VMSales.Logic
{
    public static class DataBaseLayerold
    {
    }
}















        /*
        private static string getConnectionString()
        {
            const string ConnectionString = "Data Source=C:\\Users\\Vicken\\source\\repos\\testsqllite\\testsqllite\\db\\testsales2.db; FailIfMissing = True; Foreign Keys = True; Version=3;";
            return ConnectionString;
        }

        public static IDbConnection GetConnection()
        {
            return new SQLiteConnection(getConnectionString());
        }
       





















        // Category
        
        
        
        
        
        
        
        
        
        
        
        
        
        
        
        // Generic Select
        public static string queryTable(string table, string where)
        {
            using (IDbConnection db = GetConnection())
            {
                try
                {

                    string query = "SELECT * FROM " + table + " WHERE "+where+";".ToString();
                    var result = db.QueryFirstOrDefault<string>(query);
                    return result;
                }
                catch (Exception e)
                {
                    return null;
                }
            }
        }

        // Select Table

        public static List<T> getTable<T>(string table, string where)
        {
            try
            {
                if (where == "" || where == null)
                {
                    using (IDbConnection db = GetConnection())
                    {
                        var selectTable = db.Query<T>("Select * from " + table).ToList();
                        return selectTable;
                    }
                }
            else
                {
                    using (IDbConnection db = GetConnection())
                    {
                        var selectTable = db.Query<T>("Select * from " + table + " WHERE " + where).ToList();
                        return selectTable;
                    }
                }
            }
                catch (Exception e)
                {
                    return null;
                }
            }
            // Select Primary Key Value
            public static string getPrimaryKeyName(string table)
        {
            using (IDbConnection db = GetConnection())
            {
                try
                {
                  
                    string query = "SELECT l.name FROM pragma_table_info('" + table + "') as l WHERE l.pk = 1;".ToString();
                    var primaryKeyName = db.QueryFirstOrDefault<string>(query);
                    return primaryKeyName;
                }
                catch (Exception e) 
                {
                    return null;
                }
            }
        }

        public static List<string> getPrimaryKeyValue(string primaryKeyName, string table, string where)
        {
            if (where == null || where == "")
            {
                try
                {
                    using (IDbConnection db = GetConnection())
                    {
                        string query = "SELECT " + primaryKeyName + " FROM " + table + ";";
                        MessageBox.Show(query);
                        var primaryKeyValue = db.Query<string>(query).ToList();
                        return primaryKeyValue;
                    }
                }
                catch (Exception e)
                {
                    return null;
                }
            }
            else 
            {
                try
                {
                    using (IDbConnection db = GetConnection())
                    {
                        string query = "SELECT " + primaryKeyName + " FROM " + table + " WHERE " + where +";";
                        var primaryKeyValue = db.Query<string>(query).ToList();
                        return primaryKeyValue;
                    }
                }
                catch (Exception e)
                {
                    return null;
                }
            }

        }




        // Select Relationship

        // Photos photo -> product Photo -> product


        //purchase order -> supplier

        //purchase order -> purchase_order detail
        //select* from purchase_order, supplier
        //inner join purchase_order_detail
        //on purchase_order.purchaseorder_pk = purchase_order_detail.purchaseorder_fk


        //   public static async Task<Dictionary<int, string>> getPurchaseOrderRelationship()
        //    public static Dictionary<int, string> getPurchaseOrderRelationship()

        //       public static List<PurchaseOrderModel> getPurchaseOrderRelationship()

        public static List<PurchaseOrderModel> getPurchaseOrderRelationship()
        {
            using (var connection = new SQLiteConnection(getConnectionString()))
            {
                Dictionary<string, PurchaseOrderModel> orderDictionary = new Dictionary<string, PurchaseOrderModel>();
              
                List<PurchaseOrderModel> list = connection.Query<PurchaseOrderModel, PurchaseOrderDetailModel, PurchaseOrderModel>
                        (@"SELECT po.purchase_order_pk, po.supplier_fk, po.invoice_number, po.purchase_date, 
                         pod.purchase_order_fk as purchase_order_pk,  pod.lot_number, pod.lot_cost, pod.lot_qty,  
                      pod.lot_name, pod.lot_description, pod.sales_tax, pod.shipping_cost from purchase_order po 
                      INNER JOIN purchase_order_detail pod on po.purchase_order_pk = pod.purchase_order_fk", (order, orderDetail) =>
                      {
                          if (!orderDictionary.TryGetValue(order.purchase_order_pk, out PurchaseOrderModel orderEntry))
                          {
                              orderEntry = order;
                              orderEntry.OrderDetails = new List<PurchaseOrderDetailModel>();
                              orderDictionary.Add(orderEntry.purchase_order_pk, orderEntry);
                          }
                          orderEntry.OrderDetails.Add(orderDetail);
                          return orderEntry;
                      }, splitOn: "purchase_order_pk").Distinct().ToList(); 
                return list;
            }
        }//     if (!PurchaseOrderModelDictionary.TryGetValue(po.Purchaseorder_id, out POM))

        //              POM = po;
        //          POM.PODM.Add(pod);
        //POMDictionary.Add(POM.Purchaseorder_id, POM);
        //             }
        //             POM.PODM.Add(pod);
        //           return POM;
        //       }, splitOn: "Id").Distinct().ToList();
        //       return list;
        //    }  
        // }


        //  /select* from purchase_order, supplier
        //inner join purchase_order_detail
        //on purchase_order.purchaseorder_pk = purchase_order_detail.purchaseorder_fk







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


        // Insert single
        /*
        public static long insertTable(object obj)
        {
            long rows = 0;
            try
            {
                using (IDbConnection db = GetConnection())
                {
                    db.Open();

                    using (var trans = db.BeginTransaction())
                    {
                        try
                        {
                            //  insert single record with custom data model
                           
                            rows = db.Insert(obj, transaction: trans);

                            // Only save to SQL database if all require SQL operation completed successfully 
                            trans.Commit();
                            return rows;
                        }
                        catch (Exception e)
                        {
                            // If one of the SQL operation fail , roll back the whole transaction
                            trans.Rollback();
                            return rows;
                        }
                    }
                }
            }
            catch (Exception e) { return -1; }
        }

        // insert list
        public static long Insert<T>(IEnumerable<T> ClassName)
        {
            long rows = 0;
            try
            {
                using (IDbConnection db = GetConnection())
                {
                    db.Open();

                    using (var trans = db.BeginTransaction())
                    {
                        try
                        {
                    //  insert single record with custom data model
                    db.Insert(ClassName);

                    rows = db.Insert(ClassName, transaction: trans);

                            // Only save to SQL database if all require SQL operation completed successfully 
                            trans.Commit();
                            return rows;
                        }
                        catch (Exception e)
                        {
                            // If one of the SQL operation fail , roll back the whole transaction
                            trans.Rollback();
                            return rows;
                        }
                    }
                }
            }
            catch (Exception e) { return -1; }
        }



        // Update

        // Delete
    }
}
        */