// Business/FormModuleBusiness.cs
using Entity.DTOs;
using Entity.Model;
using Microsoft.Extensions.Logging;
using System.Collections.Generic;
using System.Threading.Tasks;
using Utilities.Interfaces;

namespace Business
{
    public class FormModuleBusiness : BaseBusiness<FormModule, FormModuleDto>
    {
        public FormModuleBusiness(IService<FormModule, FormModuleDto> service, ILogger<FormModuleBusiness> logger)
            : base(service, logger)
        {
        }

        public async Task<FormModuleDto> CreateAsync(FormModuleDto formModuleDto)
        {
            return await base.CreateAsync(formModuleDto);
        }

        public async Task<IEnumerable<FormModuleDto>> GetAllAsync()
        {
            return await base.GetAllAsync();
        }

        public async Task<FormModuleDto> GetByIdAsync(int id)
        {
            return await base.GetByIdAsync(id);
        }

        public async Task<bool> UpdateAsync(FormModuleDto formModuleDto)
        {
            await base.UpdateAsync(formModuleDto);
            return true;
        }

        public async Task<bool> DeleteAsync(int id)
        {
            return await base.DeleteAsync(id);
        }

        public async Task<bool> PermanentDeleteAsync(int id)
        {
            return await base.PermanentDeleteAsync(id);
        }
    }
}