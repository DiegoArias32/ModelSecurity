using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Entity.Contexts;
using Entity.Model;

namespace Data
{
    public class FormModuleData : GenericData<FormModule>
    {
        public FormModuleData(ApplicationDbContext context, ILogger<FormModuleData> logger)
            : base(context, logger)
        {
        }

        // Métodos específicos que no están en la interfaz genérica
        public async Task<FormModule> GetByModuleIdAndFormIdAsync(int moduleId, int formId)
        {
            try
            {
                return await _context.Set<FormModule>()
                    .Where(fm => fm.ModuleId == moduleId && fm.FormId == formId)
                    .FirstOrDefaultAsync();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al obtener la asignación de formulario a módulo con ModuleId {ModuleId} y FormId {FormId}", moduleId, formId);
                throw;
            }
        }
    }
}