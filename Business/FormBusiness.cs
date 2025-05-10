// Business/FormBusiness.cs
using Entity.DTOs;
using Entity.Model;
using Microsoft.Extensions.Logging;
using System.Collections.Generic;
using System.Threading.Tasks;
using Utilities.Interfaces;

namespace Business
{
    public class FormBusiness : BaseBusiness<Form, FormDto>
    {
        public FormBusiness(IService<Form, FormDto> service, ILogger<FormBusiness> logger)
            : base(service, logger)
        {
        }

        public async Task<IEnumerable<FormDto>> GetAllFormsAsync()
        {
            return await GetAllAsync();
        }

        public async Task<FormDto> GetFormByIdAsync(int id)
        {
            return await GetByIdAsync(id);
        }

        public async Task<FormDto> CreateFormAsync(FormDto formDto)
        {
            return await CreateAsync(formDto);
        }

        public async Task<bool> UpdateFormAsync(FormDto formDto)
        {
            await UpdateAsync(formDto);
            return true;
        }

        public async Task<bool> DeleteFormAsync(int id)
        {
            return await DeleteAsync(id);
        }

        public async Task<bool> PermanentDeleteFormAsync(int id)
        {
            return await PermanentDeleteAsync(id);
        }
    }
}