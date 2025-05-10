// Business/WorkerLoginBusiness.cs
using Entity.DTOs;
using Entity.Model;
using Microsoft.Extensions.Logging;
using System.Collections.Generic;
using System.Threading.Tasks;
using Utilities.Interfaces;

namespace Business
{
    public class WorkerLoginBusiness : BaseBusiness<WorkerLogin, WorkerLoginDto>
    {
        public WorkerLoginBusiness(IService<WorkerLogin, WorkerLoginDto> service, ILogger<WorkerLoginBusiness> logger)
            : base(service, logger)
        {
        }

        public async Task<WorkerLoginDto> CreateAsync(WorkerLoginDto workerLoginDto)
        {
            return await base.CreateAsync(workerLoginDto);
        }

        public async Task<IEnumerable<WorkerLoginDto>> GetAllAsync()
        {
            return await base.GetAllAsync();
        }

        public async Task<WorkerLoginDto> GetByIdAsync(int id)
        {
            return await base.GetByIdAsync(id);
        }

        public async Task<WorkerLoginDto> GetByLoginIdAsync(int loginId)
        {
            // Esta es una consulta específica que podría requerir personalización
            // pero aún podríamos mantenerlo conciso
            return null; // Aquí deberíamos buscar por LoginId
        }

        public async Task<bool> UpdateAsync(WorkerLoginDto workerLoginDto)
        {
            await base.UpdateAsync(workerLoginDto);
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