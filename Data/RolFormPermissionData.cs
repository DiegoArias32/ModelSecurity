using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Entity.Contexts;
using Entity.Model;

namespace Data
{
    public class RolFormPermissionData : GenericData<RolFormPermission>
    {
        public RolFormPermissionData(ApplicationDbContext context, ILogger<RolFormPermissionData> logger)
            : base(context, logger)
        {
        }

        // Métodos necesarios para la funcionalidad existente
        public async Task<IEnumerable<RolFormPermission>> GetByRolIdAsync(int rolId)
        {
            return await _dbSet.Where(rfp => rfp.RolId == rolId)
                               .Include(rfp => rfp.Rol)
                               .Include(rfp => rfp.Form)
                               .Include(rfp => rfp.Permission)
                               .ToListAsync();
        }

        public async Task<IEnumerable<RolFormPermission>> GetByFormIdAsync(int formId)
        {
            return await _dbSet.Where(rfp => rfp.FormId == formId)
                               .Include(rfp => rfp.Rol)
                               .Include(rfp => rfp.Form)
                               .Include(rfp => rfp.Permission)
                               .ToListAsync();
        }
    }
}