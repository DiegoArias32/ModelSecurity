// Business/PermissionBusiness.cs
using Entity.DTOs;
using Entity.Model;
using Microsoft.Extensions.Logging;
using System.Collections.Generic;
using System.Threading.Tasks;
using Utilities.Interfaces;

namespace Business
{
    public class PermissionBusiness : BaseBusiness<Permission, PermissionDto>
    {
        public PermissionBusiness(IService<Permission, PermissionDto> service, ILogger<PermissionBusiness> logger)
            : base(service, logger)
        {
        }

        public async Task<PermissionDto> CreateAsync(PermissionDto permissionDto)
        {
            return await base.CreateAsync(permissionDto);
        }

        public async Task<IEnumerable<PermissionDto>> GetAllAsync()
        {
            return await base.GetAllAsync();
        }

        public async Task<PermissionDto> GetByIdAsync(int id)
        {
            return await base.GetByIdAsync(id);
        }

        public async Task<bool> UpdateAsync(PermissionDto permissionDto)
        {
            await base.UpdateAsync(permissionDto);
            return true;
        }

        public async Task<bool> DeleteAsync(int id)
        {
            return await base.DeleteAsync(id);
        }

        public async Task<bool> PermanentDeleteAsync(int id)
        {
            return await base.PermanentDeleteAsync(id);
        }
    }
}