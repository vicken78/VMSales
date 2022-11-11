using VMSales.Models;
using VMSales.Database;
using System.Windows;
using System;
using System.Collections.Generic;
using System.Data;
using System.Text;

namespace VMSales.ViewModels
{
    public class TestDatabaseOps
    {
        public TestDatabaseOps()
        {

            // select * from purchase_order, purchase_order_detail, supplier 
            // WHERE purchase_order.purchaseorder_pk = purchase_order_detail.purchaseorder_fk
            // AND supplier.supplier_pk = purchase_order.supplier_fk
            // AND supplier.supplier_pk="1"; (parameter)

            //select

            // then table names

            // optional WHERE String



            // Where parameter and value
            var whereParams = new List<Tuple<string, string>>
            {
                Tuple.Create("supplier_pk","1")
            };



            // format table names,  where fk pk are equal, and final parameter
            string tableNames = "purchase_order, purchase_order_detail, supplier";
            string whereSQL = "purchase_order.purchaseorder_pk = purchase_order_detail.purchaseorder_fk AND supplier.supplier_pk = purchase_order.supplier_fk AND supplier.";

            DataTable dt;
            dt = DataBaseOps.select(tableNames, whereSQL,whereParams);

            string data = string.Empty;
            StringBuilder sb = new StringBuilder();

            if (null != dt && null != dt.Rows)
            {
                foreach (DataRow dataRow in dt.Rows)
                {
                    foreach (var item in dataRow.ItemArray)
                    {
                        sb.Append(item);
                        sb.Append(',');
                    }
                    sb.AppendLine();
                }

                data = sb.ToString();
                MessageBox.Show(data);
            }



        }
    }
}
