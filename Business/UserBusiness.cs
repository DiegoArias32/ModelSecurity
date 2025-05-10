// Business/UserBusiness.cs
using Entity.DTOs;
using Entity.Model;
using Microsoft.Extensions.Logging;
using System.Collections.Generic;
using System.Threading.Tasks;
using Utilities.Interfaces;

namespace Business
{
    public class UserBusiness : BaseBusiness<User, UserDto>
    {
        public UserBusiness(IService<User, UserDto> service, ILogger<UserBusiness> logger)
            : base(service, logger)
        {
        }

        public async Task<IEnumerable<UserDto>> GetAllAsync()
        {
            return await base.GetAllAsync();
        }

        public async Task<UserDto> GetByIdAsync(int id)
        {
            return await base.GetByIdAsync(id);
        }

        public async Task<UserDto> CreateAsync(UserDto userDto)
        {
            return await base.CreateAsync(userDto);
        }

        public async Task<bool> UpdateAsync(UserDto userDto)
        {
            await base.UpdateAsync(userDto);
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