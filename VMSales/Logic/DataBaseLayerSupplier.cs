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
            return (await Connection.ExecuteAsync("DELETE FROM supplier WHERE supplier_pk = @id", new { id = entity.supplier_pk }, Transaction)) == 1;
        }
        public override Task<IEnumerable<SupplierModel>> GetAllWithID(int id)
        {
            throw new System.NotImplementedException();
        }
    }
}