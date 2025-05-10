// Data/RolFormPermissionData.cs
using Entity.Model;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Utilities.Interfaces;

namespace Data
{
    public class RolFormPermissionData : BaseData<RolFormPermission>
    {
        public RolFormPermissionData(IRepository<RolFormPermission> repository, ILogger<RolFormPermissionData> logger)
            : base(repository, logger)
        {
        }

        public async Task<IEnumerable<RolFormPermission>> GetByRolIdAsync(int rolId)
        {
            try
            {
                var allRfps = await base.GetAllAsync();
                return allRfps.Where(rfp => rfp.RolId == rolId);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al obtener permisos de formularios para rol con ID {RolId}", rolId);
                throw;
            }
        }

        public async Task<IEnumerable<RolFormPermission>> GetByFormIdAsync(int formId)
        {
            try
            {
                var allRfps = await base.GetAllAsync();
                return allRfps.Where(rfp => rfp.FormId == formId);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al obtener permisos de formularios para formulario con ID {FormId}", formId);
                throw;
            }
        }
    }
}