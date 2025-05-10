// Business/LoginBusiness.cs
using Entity.DTOs;
using Entity.Model;
using Microsoft.Extensions.Logging;
using System.Collections.Generic;
using System.Threading.Tasks;
using Utilities.Interfaces;

namespace Business
{
    public class LoginBusiness : BaseBusiness<Login, LoginDto>
    {
        public LoginBusiness(IService<Login, LoginDto> service, ILogger<LoginBusiness> logger)
            : base(service, logger)
        {
        }

        public async Task<LoginDto> CreateAsync(LoginDto loginDto)
        {
            return await base.CreateAsync(loginDto);
        }

        public async Task<IEnumerable<LoginDto>> GetAllAsync()
        {
            return await base.GetAllAsync();
        }

        public async Task<LoginDto> GetByIdAsync(int id)
        {
            return await base.GetByIdAsync(id);
        }

        public async Task<bool> UpdateAsync(LoginDto loginDto)
        {
            await base.UpdateAsync(loginDto);
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