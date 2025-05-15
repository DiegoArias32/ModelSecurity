using Entity.Model;
using Entity.Contexts;
using Microsoft.Extensions.Logging;

namespace Data
{
    public class PermissionData : GenericData<Permission>
    {
        public PermissionData(ApplicationDbContext context, ILogger<PermissionData> logger)
            : base(context, logger)
        {
        }
        
        // Si necesitas métodos específicos para Permission, puedes añadirlos aquí
    }
}