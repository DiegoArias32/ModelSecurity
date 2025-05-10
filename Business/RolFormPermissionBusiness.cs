// Business/RolFormPermissionBusiness.cs
using Entity.DTOs;
using Entity.Model;
using Microsoft.Extensions.Logging;
using System.Collections.Generic;
using System.Threading.Tasks;
using Utilities.Interfaces;

namespace Business
{
    public class RolFormPermissionBusiness : BaseBusiness<RolFormPermission, RolFormPermissionDto>
    {
        public RolFormPermissionBusiness(
            IService<RolFormPermission, RolFormPermissionDto> service, 
            ILogger<RolFormPermissionBusiness> logger)
            : base(service, logger)
        {
        }

        public async Task<RolFormPermissionDto> CreateRolFormPermissionAsync(RolFormPermissionDto dto)
        {
            return await base.CreateAsync(dto);
        }

        public async Task<IEnumerable<RolFormPermissionDto>> GetAllRolFormPermissionsAsync()
        {
            return await base.GetAllAsync();
        }

        public async Task<RolFormPermissionDto> GetRolFormPermissionByIdAsync(int id)
        {
            return await base.GetByIdAsync(id);
        }

        public async Task<IEnumerable<RolFormPermissionDto>> GetRolFormPermissionsByRolIdAsync(int rolId)
        {
            // Esta es una consulta específica que podría requerir personalización
            // pero aún podríamos mantenerlo conciso
            return await _service.GetAllAsync(); // Aquí deberíamos filtrar por RolId
        }

        public async Task<bool> UpdateRolFormPermissionAsync(RolFormPermissionDto dto)
        {
            await base.UpdateAsync(dto);
            return true;
        }

        public async Task<bool> DeleteRolFormPermissionAsync(int id)
        {
            return await base.DeleteAsync(id);
        }

        public async Task<bool> PermanentDeleteRolFormPermissionAsync(int id)
        {
            return await base.PermanentDeleteAsync(id);
        }
    }
}