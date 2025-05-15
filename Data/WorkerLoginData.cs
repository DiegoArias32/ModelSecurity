using Entity.Contexts;
using Entity.Model;
using Microsoft.Extensions.Logging;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;

namespace Data
{
    public class WorkerLoginData : GenericData<WorkerLogin>
    {
        public WorkerLoginData(ApplicationDbContext context, ILogger<WorkerLoginData> logger)
            : base(context, logger)
        {
        }

        public override async Task<IEnumerable<WorkerLogin>> GetAllAsync()
        {
            return await _dbSet
                .Include(w => w.Worker)
                .ToListAsync();
        }

        public override async Task<WorkerLogin> GetByIdAsync(object id)
        {
            return await _dbSet
                .Include(w => w.Worker)
                .FirstOrDefaultAsync(w => w.id == (int)id);
        }

        public async Task<WorkerLogin> GetByLoginIdAsync(int loginId)
        {
            return await _dbSet
                .Include(w => w.Worker)
                .FirstOrDefaultAsync(w => w.LoginId == loginId);
        }

        // Método para verificar si existe un username duplicado
        public async Task<bool> ExistsUsernameAsync(string username, int excludeId = 0)
        {
            return await _dbSet
                .AnyAsync(w => w.Username == username && w.id != excludeId);
        }
    }
}