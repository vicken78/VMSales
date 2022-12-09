using Dapper;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Threading.Tasks;
using System.Windows;
using VMSales.Models;
// Example implementation of the rRepository

// Our model.

namespace VMSales.Logic
{

    public class DataBaseLayer : INotifyPropertyChanged
    {

        public event PropertyChangedEventHandler PropertyChanged;
        protected void RaisePropertyChanged(string property)
        {
            if (PropertyChanged != null)
                PropertyChanged(this, new PropertyChangedEventArgs(property));
        }
        #region Category
        public class CategoryRepository : Repository<CategoryModel> 
        {

            public CategoryRepository(IDatabaseProvider dbProvider) : base(dbProvider) { }

            public override async Task<bool> Insert(CategoryModel entity)
            {
                int newId = await Connection.QuerySingleAsync<int>("INSERT INTO category (category_name, description, creation_date) VALUES (@category_name, @description, @creation_date); SELECT last_insert_rowid()", new
                {
                    category_name = entity.category_name,
                    description = entity.description,
                    creation_date = System.DateTime.Now
                }, Transaction);

                entity.category_pk = newId;

                return true;
            }

            public override async Task<CategoryModel> Get(int id)
            {
                return await Connection.QuerySingleAsync<CategoryModel>("SELECT category_pk FROM category WHERE category_pk = @id", new { id }, Transaction);
            }

            public override async Task<IEnumerable<CategoryModel>> GetAll()
            {
                return await Connection.QueryAsync<CategoryModel>("SELECT category_pk, category_name, description, creation_date FROM category", null, Transaction);
            }

            public override async Task<bool> Update(CategoryModel entity)
            {
                // Could check for unset id, and throw exception.
                return (await Connection.ExecuteAsync("UPDATE category SET category_name = @category_name, description = @description WHERE category_pk = @id", new
                {
                    id = entity.category_pk,
                    category_name = entity.category_name,
                    description = entity.description

                }, Transaction)) == 1;
            }

            public override async Task<bool> Delete(CategoryModel entity)
            {
                // Could check for unset id, and throw exception.
                return (await Connection.ExecuteAsync("DELETE FROM category WHERE category_pk = @id", new { id = entity.category_pk }, Transaction)) == 1;
            }

            public override Task<IEnumerable<CategoryModel>> GetAllWithID(int id)
            {
                throw new System.NotImplementedException();
            }
        }
        #endregion

        #region Supplier
        public class SupplierRepository : Repository<SupplierModel>
        {

            public SupplierRepository(IDatabaseProvider dbProvider) : base(dbProvider) { }

            public override async Task<bool> Insert(SupplierModel entity)
            {
                int newId = await Connection.QuerySingleAsync<int>("INSERT INTO supplier (supplier_name, address, city, zip, state, country, phone, email) VALUES (@supplier_name, @address, @city, @zip, @state, @country, @phone, @email); SELECT last_insert_rowid()", new
                {
                    supplier_name = entity.supplier_name,
                    address = entity.address,
                    city = entity.city,
                    zip = entity.zip,
                    state = entity.state,
                    country = entity.country,
                    phone = entity.phone,
                    email = entity.email
                }, Transaction);

                entity.supplier_pk = newId;

                return true;
            }

            public override async Task<SupplierModel> Get(int id)
            {
                return await Connection.QuerySingleAsync<SupplierModel>("SELECT supplier_pk FROM supplier WHERE supplier_pk = @id", new { id }, Transaction);
            }

            public override async Task<IEnumerable<SupplierModel>> GetAll()
            {
                return await Connection.QueryAsync<SupplierModel>("SELECT supplier_pk, supplier_name, address, city, zip, state, country, phone, email FROM supplier", null, Transaction);
            }

            public override async Task<bool> Update(SupplierModel entity)
            {
                // Could check for unset id, and throw exception. // fix
                return (await Connection.ExecuteAsync("UPDATE supplier SET supplier_name = @supplier_name, address = @address, city = @city, zip = @zip, state = @state, country = @country, phone = @phone, email = @email WHERE supplier_pk = @id", new
                {
                    id = entity.supplier_pk,
                    supplier_name = entity.supplier_name,
                    address = entity.address,
                    city = entity.city,
                    zip = entity.zip,
                    state = entity.state,
                    country = entity.country,
                    phone = entity.phone,
                    email = entity.email
          
                }, Transaction)) == 1;
            }

            public override async Task<bool> Delete(SupplierModel entity)
            {
                // Could check for unset id, and throw exception.
                return (await Connection.ExecuteAsync("DELETE FROM supplier WHERE supplier_pk = @id", new { id = entity.supplier_pk }, Transaction)) == 1;
            }
            public override Task<IEnumerable<SupplierModel>> GetAllWithID(int id)
            {
                throw new System.NotImplementedException();
            }

        }
        #endregion
        #region PurchaseOrder
        public class PurchaseOrderRepository : Repository<PurchaseOrderModel>
        {
            public PurchaseOrderRepository(IDatabaseProvider dbProvider) : base(dbProvider) { }

            // Insert
            public override async Task<bool> Insert(PurchaseOrderModel entity)
            {
                // I need to insert into purchase_order and purchase_order_detail
                // first, purchase_order, then purchase_order_detail
                  int newId = await Connection.QuerySingleAsync<int>("INSERT INTO purchase_order (supplier_fk, invoice_number, purchase_date) VALUES (@supplier_fk, @invoice_number, @purchase_date); SELECT last_insert_rowid()", new
                  {
                     supplier_fk = entity.supplier_fk,
                      invoice_number = entity.invoice_number,
                      purchase_date = entity.purchase_date
                  }, Transaction) ;

                 entity.purchase_order_fk = newId;
             
            
                // now for purchase_order_detail
                bool insertrow = (await Connection.ExecuteAsync("INSERT INTO purchase_order_detail " +
                    "(purchase_order_fk, lot_cost, lot_quantity, lot_number, lot_name, lot_description, sales_tax, shipping_cost) " +
                      "VALUES (@purchase_order_fk, @lot_cost, @lot_quantity, @lot_number, @lot_name, @lot_description, @sales_tax, @shipping_cost)", 
                      new {
                      purchase_order_fk = entity.purchase_order_fk,
                      lot_cost = entity.lot_cost,
                      lot_quantity = entity.lot_quantity,
                      lot_number = entity.lot_number,
                      lot_name = entity.lot_name,
                      lot_description = entity.lot_description,
                      sales_tax = entity.sales_tax,
                      shipping_cost = entity.shipping_cost
                      }, Transaction)) == 1;
                  return insertrow;
                
              
        }

    
            public override async Task<PurchaseOrderModel> Get(int id)
            {
                return await Connection.QuerySingleAsync<PurchaseOrderModel>("SELECT purchase_order_detail_pk FROM purchase_order_detail as pod INNER JOIN purchase_order on purchase_order.purchase_order_pk = pod.purchase_order_fk WHERE supplier_fk = @id;", new { id }, Transaction);
            }
  
            // get all purchase_order and purchase_order_detail
             public override async Task<IEnumerable<PurchaseOrderModel>> GetAll()
            {
                return await Connection.QueryAsync<PurchaseOrderModel>("SELECT " +
                    "distinct po.purchase_order_pk, po.purchase_date, po.invoice_number, pod.purchase_order_detail_pk, " +
                    "pod.purchase_order_fk, pod.lot_number, pod.lot_cost, pod.lot_quantity," +
                    "pod.lot_name, pod.lot_description, pod.sales_tax, pod.shipping_cost " +
                    "FROM purchase_order as po, purchase_order_detail as pod, supplier as sup " +
                    "INNER JOIN purchase_order_detail on po.purchase_order_pk = pod.purchase_order_fk " +
                    "INNER JOIN supplier on sup.supplier_pk = po.supplier_fk;", null, Transaction);
            }

            // get all purchase_order and purchase_order_detail
            public override async Task<IEnumerable<PurchaseOrderModel>> GetAllWithID(int id)
            {
                return await Connection.QueryAsync<PurchaseOrderModel>("SELECT " +
                    "distinct po.purchase_order_pk, po.purchase_date, po.invoice_number, " +
                    "pod.purchase_order_detail_pk, pod.purchase_order_fk, pod.lot_number, pod.lot_cost, pod.lot_quantity," +
                    "pod.lot_name, pod.lot_description, pod.sales_tax, pod.shipping_cost " +
                    "FROM purchase_order as po, purchase_order_detail as pod, supplier as sup " +
                    "INNER JOIN purchase_order_detail on po.purchase_order_pk = pod.purchase_order_fk " +
                    "INNER JOIN supplier on sup.supplier_pk = po.supplier_fk " +
                    "AND po.supplier_fk=@id", new { id }, Transaction);
            }

            public override async Task<bool> Update(PurchaseOrderModel entity)
            {

               //     Task<bool> select = (await Connection.QuerySingleAsync("SELECT 1 FROM purchase_order_detail WHERE purchase_order_fk = @id", new { id = entity.purchase_order_fk }, Transaction)) == 1;
               //     MessageBox.Show(select.Result.ToString());
              //  if (select.Result > 0)
              //  {
                    
                    
                    bool updaterow = (await Connection.ExecuteAsync("UPDATE purchase_order SET " +
                        "purchase_order_pk = @id, " +
                        "supplier_fk = @supplier_fk, " +
                        "invoice_number = @invoice_number, " +
                        "purchase_date = @purchase_date " +
                        "WHERE purchase_order_pk = @id", new
                        {
                            id = entity.purchase_order_pk,
                            supplier_fk = entity.supplier_fk,
                            invoice_number = entity.invoice_number,
                            purchase_date = entity.purchase_date
                        }, null)) == 1;

              
                return (await Connection.ExecuteAsync("UPDATE purchase_order_detail SET " +
                        "purchase_order_fk = @purchase_order_fk, " +
                        "lot_cost = @lot_cost, " +
                        "lot_quantity = @lot_quantity, " +
                        "lot_number = @lot_number," +
                        "lot_name = @lot_name, " +
                        "lot_description = @lot_description, " +
                        "sales_tax = @sales_tax, " +
                        "shipping_cost = @shipping_cost " +
                        "WHERE purchase_order_detail_pk = @purchase_order_detail_pk", new
                        {
                            purchase_order_fk = entity.purchase_order_fk,
                            lot_cost = entity.lot_cost,
                            lot_quantity = entity.lot_quantity,
                            lot_number = entity.lot_number,
                            lot_name = entity.lot_name,
                            lot_description = entity.lot_description,
                            sales_tax = entity.sales_tax,
                            shipping_cost = entity.shipping_cost,
                            purchase_order_detail_pk = entity.purchase_order_detail_pk,
                        }, Transaction)) == 1;
                //}
                //return false;
            }

            public override async Task<bool> Delete(PurchaseOrderModel entity)
            { 
                //
                // check and fix.  we need to delete purchase_order_detail pk, then delete purchase_order_pk IF its the last one.
                //


                bool deleterow = (await Connection.ExecuteAsync("DELETE FROM purchase_order_detail WHERE purchase_order_fk = @id", new { id = entity.purchase_order_fk }, null)) == 1;
                return (await Connection.ExecuteAsync("DELETE FROM purchase_order WHERE purchase_order_pk = @id", new { id = entity.purchase_order_pk }, Transaction)) == 1;

            }
        }

    }
            #endregion
}