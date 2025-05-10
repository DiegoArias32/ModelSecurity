// Business/ModuleBusiness.cs
using Entity.DTOs;
using Entity.Model;
using Microsoft.Extensions.Logging;
using System.Collections.Generic;
using System.Threading.Tasks;
using Utilities.Interfaces;
using Module = Entity.Model.Module;

namespace Business
{
    public class ModuleBusiness : BaseBusiness<Module, ModuleDto>
    {
        public ModuleBusiness(IService<Module, ModuleDto> service, ILogger<ModuleBusiness> logger)
            : base(service, logger)
        {
        }

        public async Task<IEnumerable<ModuleDto>> GetAllModulesAsync()
        {
            return await GetAllAsync();
        }

        public async Task<ModuleDto> GetModuleByIdAsync(int id)
        {
            return await GetByIdAsync(id);
        }

        public async Task<ModuleDto> CreateModuleAsync(ModuleDto moduleDto)
        {
            return await CreateAsync(moduleDto);
        }

        public async Task<bool> UpdateModuleAsync(ModuleDto moduleDto)
        {
            await UpdateAsync(moduleDto);
            return true;
        }

        public async Task<bool> DeleteModuleAsync(int id)
        {
            return await DeleteAsync(id);
        }

        public async Task<bool> PermanentDeleteModuleAsync(int id)
        {
            return await PermanentDeleteAsync(id);
        }
    }
}