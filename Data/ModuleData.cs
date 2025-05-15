using Entity.Model;
using Entity.Contexts;
using Microsoft.Extensions.Logging;

namespace Data
{
    public class ModuleData : GenericData<Module>
    {
        public ModuleData(ApplicationDbContext context, ILogger<ModuleData> logger)
            : base(context, logger)
        {
        }
        
        // Si necesitas métodos específicos para Module, puedes añadirlos aquí
    }
}