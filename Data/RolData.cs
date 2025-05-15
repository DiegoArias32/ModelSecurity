using Entity.Model;
using Entity.Contexts;
using Microsoft.Extensions.Logging;

namespace Data
{
    public class RolData : GenericData<Rol>
    {
        public RolData(ApplicationDbContext context, ILogger<RolData> logger)
            : base(context, logger)
        {
        }
        
        // Si necesitas métodos específicos para Rol, puedes añadirlos aquí
    }
}