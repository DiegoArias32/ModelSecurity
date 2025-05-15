using Entity.Contexts;
using Entity.Model;
using Microsoft.Extensions.Logging;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;

namespace Data
{
    public class UserData : GenericData<User>
    {
        public UserData(ApplicationDbContext context, ILogger<UserData> logger)
            : base(context, logger)
        {
        }

        public override async Task<IEnumerable<User>> GetAllAsync()
        {
            return await _dbSet
                .Include(u => u.Worker)
                .Where(u => u.DeleteAt == null)
                .ToListAsync();
        }

        public override async Task<User> GetByIdAsync(object id)
        {
            return await _dbSet
                .Include(u => u.Worker)
                .FirstOrDefaultAsync(u => u.Id == (int)id && u.DeleteAt == null);
        }

        // Método para verificar si existe un email duplicado
        public async Task<bool> ExistsEmailAsync(string email, int excludeId = 0)
        {
            return await _dbSet
                .AnyAsync(u => u.Email == email && u.Id != excludeId && u.DeleteAt == null);
        }
    }
}