using Dapper;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Forms;
using VMSales.Models;

namespace VMSales.Logic
{
    public class PhotoRepository : Repository<PhotoModel>
    {
        // in development, need Delete
        public PhotoRepository(IDatabaseProvider dbProvider) : base(dbProvider) { }

        public async Task<string> GetPhotoPath(string product_photo_path)
        {
            return await Connection.QuerySingleOrDefaultAsync<string>(
                "SELECT photo_path FROM photo WHERE photo_path = @photo_path",
                new { photo_path = product_photo_path },
                Transaction
            );
        }

        public async Task<ObservableCollection<string>> GetFileList(int product_pk)
        {
            var queryResult = await Connection.QueryAsync<string>(
                "SELECT photo_path FROM photo " +
                "INNER JOIN product_photo " +
                "ON photo.photo_pk = product_photo.photo_fk " +
                "INNER JOIN product " +
                "ON product.product_pk = product_photo.product_fk " +
                "WHERE product_photo.product_fk = @product_pk " +
                "ORDER BY photo.photo_order_number",
                new { product_pk },
                Transaction);
            return queryResult.ToObservable();
        }

        public async Task<IEnumerable<int>> GetImagePos(int product_fk)
        {
            return await Connection.QueryAsync<int>(
            "SELECT COALESCE(MIN(photo_order_number), 1) AS photo_order_number " +
            "FROM " +
            "(SELECT photo_order_number " +
            "FROM photo " +
            "LEFT JOIN product_photo ON photo.photo_pk = product_photo.photo_fk " +
            "WHERE product_fk = @product_fk) " +
            "UNION " +
            "SELECT photo_order_number " +
            "FROM photo " +
            "LEFT JOIN product_photo ON photo.photo_pk = product_photo.photo_fk " +
            "WHERE product_fk = @product_fk"
            , new { product_fk }, Transaction);
        }

        public async Task<IEnumerable<int>> GetNextPos(int product_fk)
        {
            return await Connection.QueryAsync<int>(
            "SELECT COALESCE(MAX(photo_order_number), 0) + 1 AS photo_order_number " +
            "FROM (" +
            "SELECT photo_order_number " +
            "FROM photo " +
            "LEFT JOIN product_photo ON photo.photo_pk = product_photo.photo_fk " +
            "WHERE product_fk = @product_fk " +
            "UNION " +
            "SELECT MAX(photo_order_number) " +
            "FROM photo " +
            "LEFT JOIN product_photo ON photo.photo_pk = product_photo.photo_fk " +
            "WHERE product_fk = @product_fk) " +
            "AS subquery WHERE PHOTO_ORDER_NUMBER IS NOT NULL"
            , new { product_fk }, Transaction);
        }
        // insert
        //public async Task<int> Insert(int product_fk,int photo_order_number, string photofilePath)
        public override async Task<int> Insert(PhotoModel entity)
        {
            int photo_pk = (await Connection.QueryFirstOrDefaultAsync<int>("INSERT INTO photo " +
            "(photo_order_number, photo_path) VALUES (@photo_order_num, @photo_path); SELECT last_insert_rowid()",
        new
        {
            photo_order_num = entity.photo_order_number,
            photo_path = entity.photo_path
        }, Transaction));
            return photo_pk;
        }

        public async Task<int> InsertProductPhoto(PhotoModel entity)
        {
            int pphoto_pk = (await Connection.QueryFirstOrDefaultAsync<int>("INSERT INTO product_photo " +
            "(product_fk, photo_fk) VALUES (@product_fk, @photo_fk); SELECT last_insert_rowid()",
        new
        {
            product_fk = entity.product_fk,
            photo_fk = entity.photo_fk
        }, Transaction));
            return pphoto_pk;
        }


        // update
        public override async Task<bool> Update(PhotoModel entity)
        {
            return (await Connection.ExecuteAsync("UPDATE photo SET " +
                    "photo_order_number = @photo_order_number " +
                    "WHERE photo_pk = @photo_pk", new
                    {
                        photo_fk = entity.photo_fk,
                    }, Transaction)) == 1;
        }
        // delete
        public override async Task<bool> Delete(PhotoModel entity)
        {
            bool deleterow = (await Connection.ExecuteAsync(
            "DELETE FROM photo WHERE photo_pk = @id",
            new { id = entity.photo_pk }, null)) == 1;
            return deleterow;
        }

        //get all by id
        public override async Task<IEnumerable<PhotoModel>> GetAllWithID(int id)
        {
            return await Connection.QueryAsync<PhotoModel>("SELECT " +
                "product_order_number, photo_path FROM photo WHERE photo_pk=@id", new { id }, Transaction);
        }

        //get Photo_Path
        public async Task<IEnumerable<string>> GetPhotoPath(int id)
        {
            return await Connection.QueryAsync<string>("SELECT " +
                "photo_path FROM photo WHERE photo_pk=@id", new { id }, Transaction);
        }

        //get all
        public override async Task<IEnumerable<PhotoModel>> GetAll()
        {
            // needs query fixing here.
            return await Connection.QueryAsync<PhotoModel>("SELECT " +
                "photo_order_number, photo_fk FROM photo", null, Transaction);
        }
        //get by product id
        public override async Task<PhotoModel> Get(int id)
        {
            //FIX
            //needs query fixing here
            return await Connection.QuerySingleAsync<PhotoModel>("SELECT ... FROM product_photo as pp INNER JOIN product on purchase_order.purchase_order_pk = pod.purchase_order_fk WHERE supplier_fk = @id;", new { id }, Transaction);
        }

    }
}