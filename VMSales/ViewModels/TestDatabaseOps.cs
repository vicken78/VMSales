using VMSales.Logic;
using VMSales.Models;

namespace VMSales.ViewModels
{
    public class TestDatabaseOps
    {
        public TestDatabaseOps()
        {

        }
    }
}


        //        public sealed class CustomerRepository : Repository<Customer>
        //        {

        //            public CustomerRepository(IDatabaseProvider dbProvider) : base(dbProvider) { }
        /*
                    public override async Task<bool> Insert(Customer entity)
                {
                    int newId = await Connection.QuerySingleAsync<int>("INSERT INTO customers (firstName, lastName) VALUES (@firstName, @lastName); SELECT last_insert_rowid()", new
                    {
                        firstName = entity.FirstName,
                        lastName = entity.LastName
                    }, Transaction);

                    entity.Id = newId;

                    return true;
                }


                public override async Task<IEnumerable<Customer>> GetAll()
                {
                    return await Connection.QueryAsync<Customer>("SELECT * FROM customers", null, Transaction);
                }

                public override async Task<bool> Update(Customer entity)
                {
                    // Could check for unset id, and throw exception.
                    return (await Connection.ExecuteAsync("UPDATE customers SET firstName = @firstName, lastName = @lastName WHERE id = @id", new
                    {
                        id = entity.Id,
                        firstName = entity.FirstName,
                        lastName = entity.LastName
                    }, Transaction)) == 1;
                }

                public override async Task<bool> Delete(Customer entity)
                {
                    // Could check for unset id, and throw exception.
                    return (await Connection.ExecuteAsync("DELETE FROM customers WHERE id = @id", new { id = entity.Id }, Transaction)) == 1;
                }

            }
        }
        */





















        /*

                    //SELECT l.name FROM pragma_table_info("Table_Name") as l WHERE l.pk <>0;

                    List<string> columns = new List<string>
                    {
                    "UserName",
                    "City"
                    };
                    //QueryBuilder is the class having the UpdateQueryBuilder()
                    string updateQueryValues = DataBaseOps.UpdateQueryBuilder(columns);

                    string updateQuery = $"UPDATE UserDetails SET {updateQueryValues} WHERE UserId=@UserId";
                    //UPDATE UserDetails SET UserName = @UserName, City = @City WHERE UserId = @UserId



                    string test = DataBaseLayer.getPrimaryKeyName("purchase_order_detail");

                    List<string> testa = new List<string>();
                    testa = DataBaseLayer.getPrimaryKeyValue(test, "purchase_order_detail", "lotnum='45'");

                    if (testa != null)
                    {
                        foreach (var item in testa)
                        {
                            //        MessageBox.Show(item);
                        }
                    }

                    List<PurchaseOrderModel> POM = new List<PurchaseOrderModel>();
                    POM = DataBaseLayer.getPurchaseOrderRelationship();

                    foreach (var item in POM)

                        foreach (var i in item.OrderDetails)
                        {
                            MessageBox.Show(item.invoice_number);
                            MessageBox.Show(
                            "lot number" + i.lotnum + "\n"
                            );



                            //   MessageBox.Show(i.Lotqty.ToString());
                                 MessageBox.Show(i.Salestax.ToString());
                            //   MessageBox.Show(i.Lotcost.ToString());
                            //   MessageBox.Show(i.Lotdescription);
                            //   MessageBox.Show(i.Lotdescription);



                        }


                    // select * from purchase_order, purchase_order_detail, supplier 
                    // WHERE purchase_order.purchaseorder_pk = purchase_order_detail.purchaseorder_fk
                    // AND supplier.supplier_pk = purchase_order.supplier_fk
                    // AND supplier.supplier_pk="1"; (parameter)

                    //select

                    // then table names

                    // optional WHERE String


                    /*
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
                        */
        //}



//    }
//}
//}
