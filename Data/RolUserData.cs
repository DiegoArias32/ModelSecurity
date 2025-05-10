// Data/RolUserData.cs
using Entity.Model;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Utilities.Interfaces;

namespace Data
{
    public class RolUserData : BaseData<RolUser>
    {
        public RolUserData(IRepository<RolUser> repository, ILogger<RolUserData> logger)
            : base(repository, logger)
        {
        }

        public async Task<IEnumerable<RolUser>> GetAllIncludingDeletedAsync()
        {
            try
            {
                return await base.GetAllAsync();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al obtener todas las relaciones Rol-User (incluyendo eliminadas)");
                throw;
            }
        }

        public async Task<IEnumerable<RolUser>> GetByUserIdAsync(int userId)
        {
            try
            {
                var allRolUsers = await base.GetAllAsync();
                return allRolUsers.Where(ru => ru.UserId == userId && ru.DeleteAt == null);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al obtener las relaciones Rol-User para el usuario con ID {UserId}", userId);
                throw;
            }
        }

        public async Task<IEnumerable<RolUser>> GetByRolIdAsync(int rolId)
        {
            try
            {
                var allRolUsers = await base.GetAllAsync();
                return allRolUsers.Where(ru => ru.RolId == rolId && ru.DeleteAt == null);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al obtener las relaciones Rol-User para el rol con ID {RolId}", rolId);
                throw;
            }
        }
    }
}