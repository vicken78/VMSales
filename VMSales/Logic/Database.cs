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

//   category -> product category-> product

//purchase order -> supplier

//purchase order -> purchase_order detail

//purchase_order_detail -> product_purcahse_order -> product

// customer -> customer_order -> customer order detail

// customer order detail -> product

        public static async Task<List<T>> getRelationship<T>(Dictionary<string,string>, string table)
        {
            using (IDbConnection db = GetConnection())
            {
                var sql = "";
                string primaryKe = null;
                var selectRelationship = await db.QueryAsync<T>(sql,())
                return selectRelationship;
            }
        }






        // Insert

        // Update

        // Delete
    }
}
