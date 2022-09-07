using System;
using System.Diagnostics;
using System.Data;
using System.Data.SQLite;
using System.Collections.Generic;
using Dapper;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Data.Common;

namespace VMSales.Database
{
    public static class DataBaseOps
    {
        const String ConnectionString = "Data Source = C:\\Users\\Vicken\\source\\repos\\testsqllite\\testsqllite\\db\\testsales2.db; FailIfMissing = True; Foreign Keys = True; Version=3;";

        public static IDbConnection GetConnection()
        {
            return new SQLiteConnection(ConnectionString);
        }

        public static List<T> SQLReader<T>(string sql) where T : new()
        {
            List<T> result = new List<T>();
            using (IDbConnection db=GetConnection())
            {
                result = db.Query<T>(sql).ToList();
                if (result.Count > 0) 
                {
                    return result;
                }
            }
            return null;
        }


        public static  DataTable ExecuteReadQuery(string query)
        {
            DataTable entries = new DataTable();

            using (SQLiteConnection db = new SQLiteConnection(ConnectionString))
            {
                SQLiteCommand selectCommand = new SQLiteCommand(query, db);
                try
                {
                    db.Open();
                    SQLiteDataReader reader = selectCommand.ExecuteReader();

                    if (reader.HasRows)
                        for (int i = 0; i < reader.FieldCount; i++)
                            entries.Columns.Add(new DataColumn(reader.GetName(i)));

                    int j = 0;
                    while (reader.Read())
                    {
                        DataRow row = entries.NewRow();
                        entries.Rows.Add(row);

                        for (int i = 0; i < reader.FieldCount; i++)
                            entries.Rows[j][i] = (reader.GetValue(i));

                        j++;
                    }

                    db.Close();
                }
                catch (SQLiteException e)
                {
                    Debug.WriteLine(e);
                    db.Close();
                }
                return entries;
            }
        }


        public static DataTable SQLiteDataTableWithQuery(string sql)
        {
            DataTable dt = new DataTable();
            SQLiteConnection con;
            con = new SQLiteConnection(ConnectionString);
            con.Open();
            SQLiteCommand SQLiteCommand;
            SQLiteCommand = con.CreateCommand();
            SQLiteCommand.CommandText = string.Format(sql);
            SQLiteDataAdapter DataAdapter = new SQLiteDataAdapter(SQLiteCommand);
            DataAdapter.Fill(dt);
            con.Close();
            return dt;
        }



            
//            SqlConnection con = new SqlConnection(connection);//
//            con.Open();
  //          using (SqlCommand cmd = new SqlCommand("up_searchUsers", con))
   //         {
    //            cmd.CommandType = CommandType.StoredProcedure;
    //            cmd.Parameters.Add("@SearchName", SqlDbType.VarChar).Value = name;
    //            SqlDataAdapter da = new SqlDataAdapter(cmd);
    //            
     //           da.Fill(dt);
     //           con.Close();
     //           return dt;
      //      }
      //  }


    }
}
