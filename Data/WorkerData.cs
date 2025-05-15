using Entity.Contexts;
using Entity.Model;
using Microsoft.Extensions.Logging;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;

namespace Data
{
    public class WorkerData : GenericData<Worker>
    {
        public WorkerData(ApplicationDbContext context, ILogger<WorkerData> logger)
            : base(context, logger)
        {
        }

        public override async Task<IEnumerable<Worker>> GetAllAsync()
        {
            return await _dbSet
                .Include(w => w.User)
                .Include(w => w.WorkerLogins)
                .ToListAsync();
        }

        public override async Task<Worker> GetByIdAsync(object id)
        {
            return await _dbSet
                .Include(w => w.User)
                .Include(w => w.WorkerLogins)
                .FirstOrDefaultAsync(w => w.WorkerId == (int)id);
        }

        // Método para verificar si existe un documento de identidad duplicado
        public async Task<bool> ExistsIdentityDocumentAsync(string identityDocument, int excludeId = 0)
        {
            return await _dbSet
                .AnyAsync(w => w.IdentityDocument == identityDocument && w.WorkerId != excludeId);
        }
    }
}