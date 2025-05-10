// Data/FormData.cs
using Entity.Model;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Utilities.Interfaces;

namespace Data
{
    public class FormData : BaseData<Form>
    {
        public FormData(IRepository<Form> repository, ILogger<FormData> logger)
            : base(repository, logger)
        {
        }

        // Sobrescribir GetAllAsync para incluir solo forms activos
        public override async Task<IEnumerable<Form>> GetAllAsync()
        {
            try
            {
                var forms = await base.GetAllAsync();
                return forms.Where(f => f.DeleteAt == null);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al obtener todos los formularios");
                throw;
            }
        }
    }
}