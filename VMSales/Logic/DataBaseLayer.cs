using Dapper;
using System.Collections.Generic;
using System.Threading.Tasks;
using VMSales.Models;
// Example implementation of the rRepository

// Our model.

namespace VMSales.Logic
{

    public class DataBaseLayer
    {

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
                return await Connection.QuerySingleAsync<CategoryModel>("SELECT category_pk, category_name, description, creation_date FROM category WHERE category_pk = @id", new { id }, Transaction);
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
        }
    }
}