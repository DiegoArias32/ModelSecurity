// Archivo: Data/IGenericData.cs
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Data
{
    // Definimos la interfaz en un archivo separado para mayor claridad
    public interface IGenericData<TEntity> where TEntity : class
    {
        Task<TEntity> CreateAsync(TEntity entity);
        Task<IEnumerable<TEntity>> GetAllAsync();
        Task<TEntity> GetByIdAsync(object id);
        Task<bool> UpdateAsync(TEntity entity);
        Task<bool> DeleteAsync(object id);
        Task<bool> PermanentDeleteAsync(object id);
    }
}