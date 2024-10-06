using Dapper;
using System.Collections.Generic;
using System.Threading.Tasks;
using VMSales.Models;

namespace VMSales.Logic
{
    public class SupplierRepository : Repository<SupplierModel>
    {
        public SupplierRepository(IDatabaseProvider dbProvider) : base(dbProvider) { }

        //get supplier by name
        public async Task<int> Get_by_supplier_name(string supplier_name)
        {
            return await Connection.QuerySingleAsync<int>("SELECT supplier_pk FROM supplier WHERE supplier_name = @supplier_name ORDER BY supplier_name", new { supplier_name }, Transaction);
        }

        // get suppliers for purchase orders
        public async Task<IEnumerable<SupplierModel>> GetSupplier()
        {

            return await Connection.QueryAsync<SupplierModel>("SELECT DISTINCT " +
                "s.*, COALESCE(po.supplier_fk, 0) AS supplier_fk " +
                "FROM supplier s " +
                "LEFT JOIN purchase_order po ON s.supplier_pk = po.supplier_fk " +
                "ORDER BY s.supplier_name");
        }

        //get product_supplier
        public async Task<IEnumerable<string>> Selected_Supplier(int product_pk)
        {
            return await Connection.QueryAsync<string>("SELECT " +
            "supplier_name as selected_supplier_name from product_supplier ps " +
            "INNER JOIN supplier s on s.supplier_pk = ps.supplier_fk " +
            "WHERE ps.product_fk = @product_pk"
            , new { product_pk }, Transaction);
        }

        public override async Task<int> Insert(SupplierModel entity)
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

            return newId;
        }

        public override async Task<SupplierModel> Get(int id)
        {
            return await Connection.QuerySingleAsync<SupplierModel>("SELECT supplier_pk FROM supplier WHERE supplier_pk = @id", new { id }, Transaction);
        }

        public override async Task<IEnumerable<SupplierModel>> GetAll()
        {
            return await Connection.QueryAsync<SupplierModel>("SELECT supplier_pk, supplier_name, address, city, zip, state, country, phone, email FROM supplier ORDER BY supplier_name", null, Transaction);
        }

        public override async Task<bool> Update(SupplierModel entity)
        {
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
            bool result = await Connection.QueryFirstAsync<bool>("SELECT CASE WHEN EXISTS (SELECT supplier_fk FROM product_supplier WHERE supplier_fk = @supplier_pk) THEN 1 ELSE 0 END as result", new { supplier_pk = entity.supplier_pk }, null);
            if (result)
                return false;
            else

                return (await Connection.ExecuteAsync("DELETE FROM supplier WHERE supplier_pk = @id", new { id = entity.supplier_pk }, Transaction)) == 1;
        }
        
        
        public override Task<IEnumerable<SupplierModel>> GetAllWithID(int id)
        {
            throw new System.NotImplementedException();
        }
    }
}