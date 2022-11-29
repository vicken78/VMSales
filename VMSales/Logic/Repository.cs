using System;
using System.Collections.Generic;
using System.Data;
using System.Threading.Tasks;
using System.Windows;

namespace VMSales.Logic
{

    public abstract class Repository<T> : IRepository<T> where T : class
    {
        protected IDbConnection Connection { get; }

        protected IDbTransaction Transaction { get; private set; }

        public Repository(IDatabaseProvider dbProvider)
        {
            Connection = dbProvider.ObtainConnection();
            Connection.Open();
            Transaction = Connection.BeginTransaction();
        }

        /// <summary>
        /// Commits the current transaction or reverts on failure.
        /// </summary>
        public void Commit()
        {
            try
            {
                Transaction.Commit();
            }
            catch (Exception)
            {
                Revert(false);
            }
        //    finally
        //    {
        //        Transaction = Connection.BeginTransaction();
        //    }
        }

        /// <summary>
        /// Rolls back the transaction, and begins a new transaction afterwards.
        /// </summary>
        public void Revert()
        {
            Revert(true);
        }

        /// <summary>
        /// Rolls back the transaction and optionally begins a new transaction afterwards.
        /// </summary>
        /// <param name="renewTransaction">If true a new transaction will be started once the previous has been rolled back.</param>
        private void Revert(bool renewTransaction)
        {
            Transaction.Rollback();

            if (renewTransaction)
                Transaction = Connection.BeginTransaction();
        }

        public abstract Task<bool> Insert(T entity);
        public abstract Task<T> Get(int id);
        public abstract Task<IEnumerable<T>> GetAll();
        public abstract Task<IEnumerable<T>> GetAll(int id);
        public abstract Task<bool> Update(T entity);
        public abstract Task<bool> Delete(T entity);

        #region Dispose Implementation

        private bool disposed = false;

        ~Repository()
        {
            Dispose(false);
        }

        public void Dispose()
        {
              if (Connection.State.ToString() == "Open")
              {
                Dispose(true);
              }
            GC.SuppressFinalize(this);
        }

        protected virtual void Dispose(bool disposing)
        {
            if (disposing == true)
            {   Transaction.Dispose();
                Connection.Dispose();
                return;
            }
            if (disposed)
            {
                Connection.Dispose();
                return;
            }
            disposed = true;
        }
        #endregion
  
    }

}