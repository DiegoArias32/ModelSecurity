// Business/RolUserBusiness.cs
using Entity.DTOs;
using Entity.Model;
using Microsoft.Extensions.Logging;
using System.Collections.Generic;
using System.Threading.Tasks;
using Utilities.Interfaces;

namespace Business
{
    public class RolUserBusiness : BaseBusiness<RolUser, RolUserDto>
    {
        public RolUserBusiness(IService<RolUser, RolUserDto> service, ILogger<RolUserBusiness> logger)
            : base(service, logger)
        {
        }

        public async Task<RolUserDto> CreateAsync(RolUserDto rolUserDto)
        {
            return await base.CreateAsync(rolUserDto);
        }

        public async Task<IEnumerable<RolUserDto>> GetAllAsync()
        {
            return await base.GetAllAsync();
        }

        public async Task<RolUserDto> GetByIdAsync(int id)
        {
            return await base.GetByIdAsync(id);
        }

        public async Task<bool> UpdateAsync(RolUserDto rolUserDto)
        {
            await base.UpdateAsync(rolUserDto);
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