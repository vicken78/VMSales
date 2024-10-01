using Dapper;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using VMSales.Logic;
using VMSales.Models;

namespace VMSales.Logic
{
    public class CustomerRepository : Repository<CustomerModel>
    {
        public CustomerRepository(IDatabaseProvider dbProvider) : base(dbProvider) { }

        public override async Task<CustomerModel> Get(int id)
        {
            return await Connection.QuerySingleAsync<CustomerModel>("SELECT customer_pk FROM customer WHERE customer_pk = @id", new { id }, Transaction);
        }

        public async Task<int> Get_cust_pk(int id)
        {
            var result = await Connection.QueryFirstOrDefaultAsync<int?>("SELECT customer_pk FROM customer WHERE customer_pk = @id", new { id }, Transaction);

            if (result.HasValue)
            {
                return result.Value;
            }
            else
            {
                return 0;
            }
        }


        public override async Task<int> Insert(CustomerModel entity)
        {

            entity.user_name = entity.user_name ?? "";
            entity.first_name = entity.first_name ?? "";
            entity.last_name = entity.last_name ?? "";
            entity.address = entity.address ?? "";
            entity.city = entity.city ?? "";
            entity.state = entity.state ?? "";
            entity.zip = entity.zip ?? "";
            entity.country = entity.country ?? "";
            entity.phone = entity.phone ?? "";
            entity.shipping_address = entity.shipping_address ?? "";
            entity.shipping_city = entity.shipping_city ?? "";
            entity.shipping_state = entity.shipping_state ?? "";
            entity.shipping_zip = entity.shipping_zip ?? "";
            entity.shipping_country = entity.shipping_country ?? "";

            int newId = await Connection.QuerySingleAsync<int>("INSERT INTO customer " +
                "(user_name, first_name, last_name, address, city, state, zip, country, phone, " +
                "shipping_address, shipping_city, shipping_state, shipping_zip, shipping_country, same_shipping_address) " +
                "VALUES (@user_name, @first_name, @last_name, @address, @city, @state, @zip, @country, @phone, " +
                "@shipping_address, @shipping_city, @shipping_state, @shipping_zip, @shipping_country, @same_shipping_address); SELECT last_insert_rowid()", new
                {
                    user_name = entity.user_name,
                    first_name = entity.first_name,
                    last_name = entity.last_name,
                    address = entity.address,
                    city = entity.city,
                    state = entity.state,
                    zip = entity.zip,
                    country = entity.country,
                    phone = entity.phone,
                    shipping_address = entity.shipping_address,
                    shipping_city = entity.shipping_city,
                    shipping_state = entity.shipping_state,
                    shipping_zip = entity.shipping_zip,
                    shipping_country = entity.shipping_country,
                    same_shipping_address = entity.same_shipping_address
                }, Transaction);
            return newId;
        }


        public override async Task<IEnumerable<CustomerModel>> GetAll()
        {
            return await Connection.QueryAsync<CustomerModel>("SELECT customer_pk, user_name, first_name, last_name, address, city, state, zip, country, phone, shipping_address, shipping_city, shipping_state, shipping_zip, shipping_country, same_shipping_address FROM customer ORDER BY last_name", null, Transaction);
        }

        public override async Task<IEnumerable<CustomerModel>> GetAllWithID(int id)
        {
            return await Connection.QueryAsync<CustomerModel>("SELECT " +
            "customer_pk, user_name, first_name, last_name, address, city, state, zip, " +
            "country, phone, shipping_address, shipping_city, shipping_state, shipping_zip, " +
            "shipping_country, same_shipping_address " +
            "FROM customer " +
            "WHERE customer_pk = @id " +
            "ORDER BY last_name", new { id }, Transaction);
        }


        public override async Task<bool> Update(CustomerModel entity)
        {
            return (await Connection.ExecuteAsync("UPDATE customer SET user_name = @user_name, " +
                "first_name = @first_name, last_name = @last_name, address = @address, " +
                "state = @state, city = @city, zip = @zip, country = @country, phone = @phone, " +
                "shipping_address = @shipping_address, shipping_city = @shipping_city, " +
                "shipping_state = @shipping_state, shipping_zip = @shipping_zip, " +
                "shipping_country = @shipping_country, same_shipping_address = @same_shipping_address " +
                "WHERE customer_pk = @id", new
                {
                    id = entity.customer_pk,
                    user_name = entity.user_name,
                    first_name = entity.first_name,
                    last_name = entity.last_name,
                    address = entity.address,
                    state = entity.state,
                    city = entity.city,
                    zip = entity.zip,
                    country = entity.country,
                    phone = entity.phone,
                    shipping_address = entity.shipping_address,
                    shipping_city = entity.shipping_city,
                    shipping_state = entity.shipping_state,
                    shipping_zip = entity.shipping_zip,
                    shipping_country = entity.shipping_country,
                    same_shipping_address = entity.same_shipping_address
                }, Transaction)) == 1;
        }

        public override async Task<bool> Delete(CustomerModel entity)
        {
            return (await Connection.ExecuteAsync("DELETE FROM customer WHERE customer_pk = @id", new { id = entity.customer_pk }, Transaction)) == 1;
        }
    }
}

// Customer

/*
    //get customer by name
    public async Task<int> Get_by_customer_name(string supplier_name)
    {
        return await Connection.QuerySingleAsync<int>("SELECT supplier_pk FROM supplier WHERE supplier_name = @supplier_name", new { supplier_name }, Transaction);
    }
*/
/*
    //get product_supplier
    public async Task<IEnumerable<string>> Selected_Supplier(int product_pk)
    {
        return await Connection.QueryAsync<string>("SELECT " +
        "supplier_name as selected_supplier_name from product_supplier ps " +
        "INNER JOIN supplier s on s.supplier_pk = ps.supplier_fk " +
        "WHERE ps.product_fk = @product_pk"
        , new { product_pk }, Transaction);
    }
*/
