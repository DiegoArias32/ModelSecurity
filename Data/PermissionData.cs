// Data/PermissionData.cs
using Entity.Model;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Utilities.Interfaces;

namespace Data
{
    public class PermissionData : BaseData<Permission>
    {
        public PermissionData(IRepository<Permission> repository, ILogger<PermissionData> logger)
            : base(repository, logger)
        {
        }

        // Sobrescribir GetAllAsync para incluir solo permissions activos
        public override async Task<IEnumerable<Permission>> GetAllAsync()
        {
            try
            {
                var permissions = await base.GetAllAsync();
                return permissions.Where(p => p.DeleteAt == null);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al obtener los permisos");
                throw;
            }
        }

        // Sobrescribir DeleteAsync para implementar borrado lógico
        public override async Task<bool> DeleteAsync(object id)
        {
            try
            {
                var permission = await GetByIdAsync(id);
                if (permission == null)
                    return false;

                // Borrado lógico: establecer la fecha actual
                permission.DeleteAt = DateTime.UtcNow;
                
                return await UpdateAsync(permission);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al eliminar el permiso: {ErrorMessage}", ex.Message);
                return false;
            }
        }
    }
}