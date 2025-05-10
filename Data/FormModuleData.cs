// Data/FormModuleData.cs
using Entity.Model;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Utilities.Interfaces;

namespace Data
{
    public class FormModuleData : BaseData<FormModule>
    {
        public FormModuleData(IRepository<FormModule> repository, ILogger<FormModuleData> logger)
            : base(repository, logger)
        {
        }

        public async Task<IEnumerable<FormModule>> GetByModuleIdAsync(int moduleId)
        {
            try
            {
                var allFormModules = await base.GetAllAsync();
                return allFormModules.Where(fm => fm.ModuleId == moduleId);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al obtener formularios para módulo con ID {ModuleId}", moduleId);
                throw;
            }
        }

        public async Task<IEnumerable<FormModule>> GetByFormIdAsync(int formId)
        {
            try
            {
                var allFormModules = await base.GetAllAsync();
                return allFormModules.Where(fm => fm.FormId == formId);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al obtener módulos para formulario con ID {FormId}", formId);
                throw;
            }
        }

        public async Task<FormModule> GetByModuleIdAndFormIdAsync(int moduleId, int formId)
        {
            try
            {
                var allFormModules = await base.GetAllAsync();
                return allFormModules.FirstOrDefault(fm => fm.ModuleId == moduleId && fm.FormId == formId);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al obtener la asignación de formulario a módulo con ModuleId {ModuleId} y FormId {FormId}", moduleId, formId);
                throw;
            }
        }
    }
}