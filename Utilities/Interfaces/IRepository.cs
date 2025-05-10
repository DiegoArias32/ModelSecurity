using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Utilities.Interfaces
{
    public interface IRepository<T> where T : class
    {
        Task<IEnumerable<T>> GetAllAsync();
        Task<T> GetByIdAsync(object id);
        Task<T> CreateAsync(T entity);
        Task<bool> UpdateAsync(T entity);
        Task<bool> DeleteAsync(object id);
        Task<bool> PermanentDeleteAsync(object id);
    }
}