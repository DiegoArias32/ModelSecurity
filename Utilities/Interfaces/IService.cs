using System.Collections.Generic;
using System.Threading.Tasks;

namespace Utilities.Interfaces
{
    public interface IService<TEntity, TDto>
        where TEntity : class
        where TDto : class
    {
        Task<IEnumerable<TDto>> GetAllAsync();
        Task<TDto> GetByIdAsync(object id);
        Task<TDto> CreateAsync(TDto dto);
        Task<bool> UpdateAsync(TDto dto);
        Task<bool> DeleteAsync(object id);
        Task<bool> PermanentDeleteAsync(object id);
    }
}