﻿using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace VMSales.Logic
{

    public interface IRepository<T> : IDisposable where T : class
    {
        Task<int> Insert(T entity);
        Task<T> Get(int id);
        Task<IEnumerable<T>> GetAll();
        Task<IEnumerable<T>> GetAllWithID(int id);
        Task<bool> Update(T entity);
        Task<bool> Delete(T entity);
        void Commit();
        void Revert();
    }
}