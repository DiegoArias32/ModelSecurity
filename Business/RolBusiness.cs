// Business/RolBusiness.cs
using Entity.DTOs;
using Entity.Model;
using Microsoft.Extensions.Logging;
using System.Collections.Generic;
using System.Threading.Tasks;
using Utilities.Interfaces;

namespace Business
{
    public class RolBusiness : BaseBusiness<Rol, RolDto>
    {
        public RolBusiness(IService<Rol, RolDto> service, ILogger<RolBusiness> logger)
            : base(service, logger)
        {
        }

        public async Task<IEnumerable<RolDto>> GetAllRolesAsync()
        {
            return await GetAllAsync();
        }

        public async Task<RolDto> GetRolByIdAsync(int id)
        {
            return await GetByIdAsync(id);
        }

        public async Task<RolDto> CreateRolAsync(RolDto rolDto)
        {
            return await CreateAsync(rolDto);
        }

        public async Task<RolDto> UpdateRolAsync(RolDto rolDto)
        {
            return await UpdateAsync(rolDto);
        }

        public async Task<bool> DeleteRolAsync(int id)
        {
            return await DeleteAsync(id);
        }

        public async Task<bool> PermanentDeleteRolAsync(int id)
        {
            return await PermanentDeleteAsync(id);
        }
    }
}