// Business/WorkerBusiness.cs
using Entity.DTOs;
using Entity.Model;
using Microsoft.Extensions.Logging;
using System.Collections.Generic;
using System.Threading.Tasks;
using Utilities.Interfaces;

namespace Business
{
    public class WorkerBusiness : BaseBusiness<Worker, WorkerDto>
    {
        public WorkerBusiness(IService<Worker, WorkerDto> service, ILogger<WorkerBusiness> logger)
            : base(service, logger)
        {
        }

        public async Task<WorkerDto> CreateAsync(WorkerDto workerDto)
        {
            return await base.CreateAsync(workerDto);
        }

        public async Task<IEnumerable<WorkerDto>> GetAllAsync()
        {
            return await base.GetAllAsync();
        }

        public async Task<WorkerDto> GetByIdAsync(int id)
        {
            return await base.GetByIdAsync(id);
        }

        public async Task<bool> UpdateAsync(WorkerDto workerDto)
        {
            await base.UpdateAsync(workerDto);
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