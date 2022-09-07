using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Data;
using System.Data.SQLite;
using System.Collections.Specialized;
using System.Reflection;
using System.Diagnostics;

namespace VMSales.ViewModels
{
    public class DataBaseOps
    {
    const String dbConnection = "Data Source = C:\\Users\\Vicken\\source\\repos\\VMSales\\VMSales\\db\\testsales2.db; FailIfMissing = True; Foreign Keys = True;";
     
    //query one column value - good for getting primary key. 
    public string ExecuteScalar(string tablename, string columnname, string where)
        {
            SQLiteConnection con = new SQLiteConnection(dbConnection);
            con.Open();
            SQLiteCommand mycommand = new SQLiteCommand(con);
            string sqlstatement = String.Format("select {0} from {1} where {2};", columnname, tablename, where);
            mycommand.CommandText = sqlstatement;
            object value = mycommand.ExecuteScalar();
            con.Close();
            if (value != null)
            {
                return value.ToString();
            }
            return "";
        }

        // select from database and inserts into a database table
        public DataTable LoadDataTable(string sql)
        {
            try
            {
                DataTable mydatatable = new DataTable("Supplier");
                SQLiteConnection con = new SQLiteConnection(dbConnection);
                con.Open();
                SQLiteCommand mycommand = new SQLiteCommand(con);
                mycommand.CommandText = sql;
                SQLiteDataReader reader = mycommand.ExecuteReader();
                mydatatable.Load(reader);
                reader.Close();
                con.Close();
                return mydatatable;
                
            }
            catch (Exception e)
            {
                throw new Exception(e.Message);
            }
        }

        // insert update or delete based on query, single query only.
        public int ExecuteNonQuery(string sql)
        {
            SQLiteConnection con = new SQLiteConnection(dbConnection);
            con.Open();
            SQLiteCommand mycommand = new SQLiteCommand(con);
            mycommand.CommandText = sql;
            int rowsUpdated = mycommand.ExecuteNonQuery();
            con.Close();
            return rowsUpdated;
        }

        // insert operations
        public bool Insert(String tableName, Dictionary<string, string> data)
        {

            String columns = "";
            String values = "";
            Boolean returnCode = true;
            foreach (KeyValuePair<string, string> val in data)
            {
                columns += String.Format(" {0},", val.Key.ToString());
                values += String.Format(" '{0}',", val.Value);
            }
            columns = columns.Substring(0, columns.Length - 1);
            values = values.Substring(0, values.Length - 1);
            try
            {
                this.ExecuteNonQuery(String.Format("insert into {0}({1}) values({2});", tableName, columns, values));
                //Debug Console.WriteLine(String.Format("insert into {0}({1}) values ({2});", tableName, columns, values));
            }
            catch (Exception fail)
            {
                Console.WriteLine(fail);
                returnCode = false;
            }
            return returnCode;
        }

        // update operation - supports multiple
        public bool Update(String tableName, Dictionary<string, string> updatedata, String where)
        {
            String vals = "";
            Boolean returnCode = true;
            if (updatedata.Count >= 1)
            {
                foreach (KeyValuePair<string, string> val in updatedata)
                {
                    vals += String.Format("{0} = \"{1}\",", val.Key.ToString(), val.Value.ToString());
                }
                vals = vals.Substring(0, vals.Length - 1);
            }
            try
            {
                this.ExecuteNonQuery(String.Format("update {0} set {1} where {2};", tableName, vals, where));
            }
            catch
            {
                returnCode = false;
            }
            return returnCode;
        }

        //delete operation
        public bool Delete(String tableName, String where)
        {
            Boolean returnCode = true;
            try
            {
                this.ExecuteNonQuery(String.Format("delete from {0} where {1};", tableName, where));
                //debug only Console.WriteLine(String.Format("delete from {0} where {1};", tableName, where));
            }
            catch (Exception fail)
            {
                Console.WriteLine(fail);
                returnCode = false;
            }
            return returnCode;
        }
    }
}