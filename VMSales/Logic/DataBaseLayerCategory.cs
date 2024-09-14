using Dapper;
using System.Collections.Generic;
using System.Threading.Tasks;
using VMSales.Models;

namespace VMSales.Logic
{
        public class CategoryRepository : Repository<CategoryModel>
        {

            public CategoryRepository(IDatabaseProvider dbProvider) : base(dbProvider) { }

            public override async Task<int> Insert(CategoryModel entity)
            {
                int newId = await Connection.QuerySingleAsync<int>("INSERT INTO category (category_name, description, creation_date) VALUES (@category_name, @description, @creation_date); SELECT last_insert_rowid()", new
                {
                    category_name = entity.category_name,
                    description = entity.description,
                    creation_date = System.DateTime.Now
                }, Transaction);

                entity.category_pk = newId;

                return newId;
            }

            public async Task<int> Get_by_category_name(string category_name)
            {
                return await Connection.QuerySingleAsync<int>("SELECT category_pk FROM category WHERE category_name = @category_name", new { category_name }, Transaction);
            }

            public async Task<CategoryModel> Get_by_category_pk(int category_pk)
            {
                return await Connection.QuerySingleAsync<CategoryModel>("SELECT category_name FROM category WHERE category_pk = @category_pk", new { category_pk }, Transaction);
            }

            public async Task<IEnumerable<CategoryModel>> Get_all_category_name()
            {
                return await Connection.QueryAsync<CategoryModel>("SELECT category_name FROM category ORDER BY category_name", null, Transaction);
            }

            /*     public async Task<IEnumerable<CategoryModel>> Get_Product_Category()
                 {
                     return await Connection.QueryAsync<CategoryModel>(
                     "SELECT DISTINCT category_pk, category_name, (category_pk - 1) as selected_category  " +
                     "FROM category as c, product_category as pc " +
                     "INNER JOIN product_category on pc.category_fk = c.category_pk UNION " +
                     "SELECT category_pk, category_name, null  FROM category as c"
                     , null, Transaction);

                 }
            */
            public override async Task<CategoryModel> Get(int id)
            {
                return await Connection.QuerySingleAsync<CategoryModel>("SELECT category_pk FROM category WHERE category_pk = @id", new { id }, Transaction);
            }

            public override async Task<IEnumerable<CategoryModel>> GetAll()
            {
                return await Connection.QueryAsync<CategoryModel>("SELECT category_pk, category_name, description, creation_date FROM category", null, Transaction);
            }

            public async Task<IEnumerable<CategoryModel>> GetCategory()
            {
                return await Connection.QueryAsync<CategoryModel>("SELECT category_pk, category_name FROM category", null, Transaction);
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
}

