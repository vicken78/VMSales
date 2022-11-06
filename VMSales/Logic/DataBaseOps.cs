using System;
using System.Diagnostics;
using System.Data;
using System.Data.SQLite;
using System.Collections.Generic;
using Dapper;
using VMSales.Models;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using System.Data.Common;
using System.Configuration;
using System.Windows;

namespace VMSales.Database
{
    public static class DataBaseOps
    {

        private static string getConnectionString()
        {
            const string ConnectionString = "Data Source=C:\\Users\\Vicken\\source\\repos\\testsqllite\\testsqllite\\db\\testsales2.db; FailIfMissing = True; Foreign Keys = True; Version=3;";
            return ConnectionString;
        }

        // Data Params Map
        public static List<Tuple<string, string>> getDataParams(string tablename)
        {
            var dataBaseParams = new List<Tuple<string, string>>();

            switch (tablename)
            {
                case "category":
                    dataBaseParams.Add(new Tuple<string, string>("category_pk", "@category_pk"));
                    dataBaseParams.Add(new Tuple<string, string>("cname", "@cname"));
                    dataBaseParams.Add(new Tuple<string, string>("description", "@description"));
                    dataBaseParams.Add(new Tuple<string, string>("creationdate", "@creationdate"));
                    return dataBaseParams;
                case "customer":
                    dataBaseParams.Add(new Tuple<string, string>("customer_pk", "@customer_pk"));
                    dataBaseParams.Add(new Tuple<string, string>("username", "@username"));
                    dataBaseParams.Add(new Tuple<string, string>("firstname", "@firstname"));
                    dataBaseParams.Add(new Tuple<string, string>("lastname", "@lastname"));
                    dataBaseParams.Add(new Tuple<string, string>("address", "@address"));
                    dataBaseParams.Add(new Tuple<string, string>("state", "@state"));
                    dataBaseParams.Add(new Tuple<string, string>("zip", "@zip"));
                    dataBaseParams.Add(new Tuple<string, string>("country", "@country"));
                    dataBaseParams.Add(new Tuple<string, string>("phone", "@phone"));
                    dataBaseParams.Add(new Tuple<string, string>("shiptoaddress", "@shiptoaddress"));
                    dataBaseParams.Add(new Tuple<string, string>("shiptostate", "@shiptostate"));
                    dataBaseParams.Add(new Tuple<string, string>("shiptozip", "@shiptozip"));
                    dataBaseParams.Add(new Tuple<string, string>("shiptocountry", "@shiptocountry"));
                    return dataBaseParams;
                case "customer_order":
                    dataBaseParams.Add(new Tuple<string, string>("corder_pk", "@corder_pk"));
                    dataBaseParams.Add(new Tuple<string, string>("customer_fk", "@customer_fk"));
                    dataBaseParams.Add(new Tuple<string, string>("ordernum", "@ordernum"));
                    dataBaseParams.Add(new Tuple<string, string>("shipstatus", "@shipstatus"));
                    dataBaseParams.Add(new Tuple<string, string>("shippingservice", "@shippingservice"));
                    dataBaseParams.Add(new Tuple<string, string>("trackingnum", "@trackingnum"));
                    dataBaseParams.Add(new Tuple<string, string>("orderdate", "@orderdate"));
                    dataBaseParams.Add(new Tuple<string, string>("shipdate", "@shipdate"));
                    dataBaseParams.Add(new Tuple<string, string>("shippingfee", "@shippingfee"));
                    return dataBaseParams;
                case "customer_order_detail":
                    dataBaseParams.Add(new Tuple<string, string>("pcustomerorder_pk", "@pcustomerorder_pk"));
                    dataBaseParams.Add(new Tuple<string, string>("order_fk", "@order,fk"));
                    dataBaseParams.Add(new Tuple<string, string>("product_fk", "@product_fk"));
                    dataBaseParams.Add(new Tuple<string, string>("qty", "@qty"));
                    dataBaseParams.Add(new Tuple<string, string>("price", "@price"));
                    dataBaseParams.Add(new Tuple<string, string>("sellingfee", "@sellingfee"));
                    dataBaseParams.Add(new Tuple<string, string>("salestax", "@salestax"));
                    dataBaseParams.Add(new Tuple<string, string>("salestaxrate", "@salestaxrate"));
                    return dataBaseParams;
                case "photo":
                    dataBaseParams.Add(new Tuple<string, string>("photo_pk", "@photo_pk"));
                    dataBaseParams.Add(new Tuple<string, string>("photoordernum", "@photoordernum"));
                    dataBaseParams.Add(new Tuple<string, string>("photofile", "@photofile"));
                    return dataBaseParams;
                case "product":
                    dataBaseParams.Add(new Tuple<string, string>("product_pk", "@product_pk"));
                    dataBaseParams.Add(new Tuple<string, string>("bname", "@bname"));
                    dataBaseParams.Add(new Tuple<string, string>("pname", "@pname"));
                    dataBaseParams.Add(new Tuple<string, string>("description", "@description"));
                    dataBaseParams.Add(new Tuple<string, string>("quantity", "@quantity"));
                    dataBaseParams.Add(new Tuple<string, string>("cost", "@cost"));
                    dataBaseParams.Add(new Tuple<string, string>("sku", "@sku"));
                    dataBaseParams.Add(new Tuple<string, string>("soldprice", "@soldprice"));
                    dataBaseParams.Add(new Tuple<string, string>("instock", "@instock"));
                    dataBaseParams.Add(new Tuple<string, string>("condition", "@condition"));
                    dataBaseParams.Add(new Tuple<string, string>("listingurl", "@listingurl"));
                    dataBaseParams.Add(new Tuple<string, string>("listingnum", "@listingnum"));
                    dataBaseParams.Add(new Tuple<string, string>("listingdate", "@listingdate"));
                    return dataBaseParams;
                case "product_category":
                    dataBaseParams.Add(new Tuple<string, string>("pcategory_pk", "@pcategory_pk"));
                    dataBaseParams.Add(new Tuple<string, string>("product_fk", "@product_fk"));
                    dataBaseParams.Add(new Tuple<string, string>("category_fk", "@category_fk"));
                    return dataBaseParams;
                case "product_photo":
                    dataBaseParams.Add(new Tuple<string, string>("pphoto_pk", "@pphoto_pk"));
                    dataBaseParams.Add(new Tuple<string, string>("product_fk", "@product_fk"));
                    dataBaseParams.Add(new Tuple<string, string>("photo_fk", "@photo_fk"));
                    return dataBaseParams;
                case "product_purchase_order":
                    dataBaseParams.Add(new Tuple<string, string>("ppurchaseorder_pk", "@ppurchaseorder_pk"));
                    dataBaseParams.Add(new Tuple<string, string>("porder_fk", "@porder_fk"));
                    dataBaseParams.Add(new Tuple<string, string>("product_fk", "@product_fk"));
                    return dataBaseParams;
                case "purchase_order":
                    dataBaseParams.Add(new Tuple<string, string>("purchaseorder_pk", "@purchaseorder_pk"));
                    dataBaseParams.Add(new Tuple<string, string>("supplier_fk", "@supplier_fk"));
                    dataBaseParams.Add(new Tuple<string, string>("invoicenum", "@invoicenum"));
                    dataBaseParams.Add(new Tuple<string, string>("purchasedate", "@purchasedate"));
                    return dataBaseParams;
                case "purchase_order_detail":
                    dataBaseParams.Add(new Tuple<string, string>("porder_pk", "@porder_pk"));
                    dataBaseParams.Add(new Tuple<string, string>("purchaseorder_fk", "@purchaseorder_fk"));
                    dataBaseParams.Add(new Tuple<string, string>("lotcost", "@lotcost"));
                    dataBaseParams.Add(new Tuple<string, string>("lotqty", "@lotqty"));
                    dataBaseParams.Add(new Tuple<string, string>("lotnum", "@lotnum"));
                    dataBaseParams.Add(new Tuple<string, string>("lotname", "@lotname"));
                    dataBaseParams.Add(new Tuple<string, string>("lotdescription", "@lotdescription"));
                    dataBaseParams.Add(new Tuple<string, string>("salestax", "@salestax"));
                    dataBaseParams.Add(new Tuple<string, string>("shippingcost", "@shippingcost"));
                    return dataBaseParams;
            }
            return null;
        }

        // compare input to data map.
        public static string compareParams (List<Tuple<string,string>> columnValuePair, List<Tuple<string, string>> dataParams)
            {
            string sqlqueryparams = null;
                foreach (var columnValuePairItem in columnValuePair) // columnvaluepair tuple1
                            foreach (var dataParamsItem in dataParams) // dataparams tuple2
                                {
                                    if (dataParamsItem.Item1 == columnValuePairItem.Item1)
                                    {
                                    sqlqueryparams += (dataParamsItem.Item1) + "=";
                                    sqlqueryparams += (dataParamsItem.Item2)+" ,";
                                    }
                                 }
            sqlqueryparams.Remove(sqlqueryparams.Length - 1, 1);
            return sqlqueryparams;
            }     




// Update
public static int update(string tablename, List<Tuple<string, string>> columnValuePair, string whereParam, string whereValue) 
        //Tuple 1

        {
            var dataParams = getDataParams(tablename); // Tuple 2
            int dataBaseParams = -1;
            string sqlquery;
            using (SQLiteConnection conn = new SQLiteConnection(getConnectionString()))
            {
                //conn.Open();
                using (SQLiteCommand cmd = new SQLiteCommand(conn))
                {
                    try
                    {
                        sqlquery = "UPDATE " + tablename + " SET ";

                        // get the data params 
                        sqlquery += compareParams(columnValuePair, dataParams);
                        sqlquery.Remove(sqlquery.Length - 1, 1);
                        sqlquery += whereParam;
                        //cmd.CommandText = sqlquery;
                        MessageBox.Show(sqlquery);
                    }
                    catch (SQLiteException e)
                    {
                        MessageBox.Show(e.ToString());
                    }
                }
                //conn.Close();
            }
            return dataBaseParams;
        }

        // Select PK and FK
        public static int selectPKFK(string tablename, List<Tuple<string, string>> columnValuePair, string whereParam, string whereValue)
        //Tuple 1

        {
            var dataParams = getDataParams(tablename); // Tuple 2
            int dataBaseParams = -1;
            string sqlquery;
            using (SQLiteConnection conn = new SQLiteConnection(getConnectionString()))
            {
                //conn.Open();
                using (SQLiteCommand cmd = new SQLiteCommand(conn))
                {
                    try
                    {
                        //      sqlquery = "SELECT * FROM " + tablename + "WHERE ";

                        foreach (var columnValuePairItem in columnValuePair) // columnvaluepair tuple1
                            foreach (var dataParamsItem in dataParams) // dataparams tuple2
                            {
                                if (dataParamsItem.Item1 == columnValuePairItem.Item1)
                                {
                                    //sqlqueryparams += (dataParamsItem.Item1) + "=";
                                    //sqlqueryparams += (dataParamsItem.Item2) + " ,";
                                }
                            }



                        //      sqlquery = "SELECT * FROM " + tablename + "WHERE ";

                        // get the data params 
                        //      sqlquery += compareParams(columnValuePair, dataParams);
                        //      sqlquery += whereParam;
                        //cmd.CommandText = sqlquery;
                        //       MessageBox.Show(sqlquery);
                    }
                    catch (SQLiteException e)
                    {
                        MessageBox.Show(e.ToString());
                    }
                }
                //conn.Close();
            }
            return dataBaseParams;
        }



        // Select Parameter

        public static int selectParameter(string tablename, List<Tuple<string, string>> columnValuePair, string whereParam, string whereValue)
        //Tuple 1

        {
            var dataParams = getDataParams(tablename); // Tuple 2
            int dataBaseParams = -1;
            string sqlquery;
            using (SQLiteConnection conn = new SQLiteConnection(getConnectionString()))
            {
                //conn.Open();
                using (SQLiteCommand cmd = new SQLiteCommand(conn))
                {
                    try
                    {
                        sqlquery = "SELECT * FROM " + tablename + "WHERE ";

                        // get the data params 
                        sqlquery += compareParams(columnValuePair, dataParams);
                        sqlquery += whereParam;
                        //cmd.CommandText = sqlquery;
                        MessageBox.Show(sqlquery);
                    }
                    catch (SQLiteException e)
                    {
                        MessageBox.Show(e.ToString());
                    }
                }
                //conn.Close();
            }
            return dataBaseParams;
        }


        // to be deprecated
        public static void IUDTable(string sqlparams, List<Tuple<string, string>> sqlvalues, int rowcount)
        {
        SQLiteConnection con;
        con = new SQLiteConnection(getConnectionString());
        con.Open();
       
            using (var transaction = con.BeginTransaction())
            {
                try
                {
                        using (var SQLiteCommand = new SQLiteCommand(sqlparams, con))
                        {
                                    foreach (var item in sqlvalues)
                                    {
                                        SQLiteCommand.Parameters.AddWithValue(item.Item1, item.Item2);
                                    }
                                    SQLiteCommand.ExecuteNonQuery();
                                    transaction.Commit();
                                    con.Close();
                        }
                }
                catch (Exception ex)
                {
                    transaction.Rollback();
                    con.Close();
                    MessageBox.Show("An Error has occured.  No rows were updated." + ex);              
                }
            }
        }

        // reads all data in table, returns data table
        public static DataTable SQLiteDataTableWithQuery(string tablename)
        {
            DataTable dt = new DataTable();
            SQLiteConnection con;
            con = new SQLiteConnection(getConnectionString());
            con.Open();
            SQLiteCommand SQLiteCommand;
            SQLiteCommand = con.CreateCommand();
            SQLiteCommand.CommandText = "SELECT * FROM " + tablename;
            SQLiteDataAdapter DataAdapter = new SQLiteDataAdapter(SQLiteCommand);
            DataAdapter.Fill(dt);
            con.Close();
            return dt;
        }



        // unused for future customization.
        /*
        public static bool isColumnExist(String tableName, String columnName)
        {
            SQLiteConnection con;
            con = new SQLiteConnection(getConnectionString());
            con.Open();
            var sql = "SELECT name FROM sqlite_master WHERE type='table' AND name='" + tableName + "';";
            SQLiteCommand command = new SQLiteCommand(sql,con);
            SQLiteDataReader dataReader = command.ExecuteReader();
                if (dataReader.HasRows)
                {
                bool hascol = dataReader.GetString(0).Contains(String.Format("\"{0}\"", columnName));
                con.Close();
                return hascol;
                }    
            else
            {
                con.Close();
                return false;
            }  
        }
        */
    }
}
