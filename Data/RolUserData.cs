using Entity.Contexts;
using Entity.Model;
using Microsoft.Extensions.Logging;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;

namespace Data
{
    public class RolUserData : GenericData<RolUser>
    {
        public RolUserData(ApplicationDbContext context, ILogger<RolUserData> logger)
            : base(context, logger)
        {
        }

        public override async Task<IEnumerable<RolUser>> GetAllAsync()
        {
            return await _dbSet
                .Include(ru => ru.User)
                .Include(ru => ru.Rol)
                .ToListAsync();
        }

        public override async Task<RolUser> GetByIdAsync(object id)
        {
            return await _dbSet
                .Include(ru => ru.User)
                .Include(ru => ru.Rol)
                .FirstOrDefaultAsync(ru => ru.Id == (int)id);
        }

        public async Task<IEnumerable<RolUser>> GetByUserIdAsync(int userId)
        {
            return await _dbSet
                .Include(ru => ru.Rol)
                .Where(ru => ru.UserId == userId)
                .ToListAsync();
        }

        public async Task<IEnumerable<RolUser>> GetByRolIdAsync(int rolId)
        {
            return await _dbSet
                .Include(ru => ru.User)
                .Where(ru => ru.RolId == rolId)
                .ToListAsync();
        }
    }
}