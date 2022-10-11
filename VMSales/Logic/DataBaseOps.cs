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
        private static string LoadConnectionString()
        {
            const string ConnectionString = "Data Source=C:\\Users\\Vicken\\source\\repos\\testsqllite\\testsqllite\\db\\testsales2.db; FailIfMissing = True; Foreign Keys = True; Version=3;";
            return ConnectionString;
        }


        public static void IUDTable(string sqlparams, List<Tuple<string, string>> sqlvalues, int rowcount)
        {
        SQLiteConnection con;
        con = new SQLiteConnection(LoadConnectionString());
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
            con = new SQLiteConnection(LoadConnectionString());
            con.Open();
            SQLiteCommand SQLiteCommand;
            SQLiteCommand = con.CreateCommand();
            SQLiteCommand.CommandText = "SELECT * FROM " + tablename;
            SQLiteDataAdapter DataAdapter = new SQLiteDataAdapter(SQLiteCommand);
            DataAdapter.Fill(dt);
            con.Close();
            return dt;
        }


        public static bool isColumnExist(String tableName, String columnName)
        {
            SQLiteConnection con;
            con = new SQLiteConnection(LoadConnectionString());
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

    }
}
