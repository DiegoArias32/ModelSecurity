using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Storage;
using Dapper;
using System.Data;
using System.Reflection;
using Entity.Model;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.Configuration;
using Module = Entity.Model.Module;

namespace Entity.Contexts
{
    public class ApplicationDbContext : DbContext
    {
        protected readonly IConfiguration _configuration;

        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options, IConfiguration configuration)
            : base(options)
        {
            _configuration = configuration;
        }

        // DBSETS
        public DbSet<Rol> Roles { get; set; }
        public DbSet<Form> Forms { get; set; }
        public DbSet<Module> Modules { get; set; }
        public DbSet<FormModule> FormModules { get; set; }
        public DbSet<User> Users { get; set; }
        public DbSet<Worker> Workers { get; set; }
        public DbSet<Login> Logins { get; set; }
        public DbSet<WorkerLogin> WorkerLogins { get; set; }
        public DbSet<Permission> Permissions { get; set; }
        public DbSet<ActivityLog> ActivityLogs { get; set; }
        public DbSet<RolUser> RolUsers { get; set; }

        public DbSet<RolFormPermission> RolFormPermissions { get; set;}

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            // Relación User - Worker
            modelBuilder.Entity<User>()
                .HasOne(u => u.Worker)
                .WithOne(w => w.User)
                .HasForeignKey<User>(u => u.WorkerId);

            // Relación WorkerLogin - Worker
            modelBuilder.Entity<WorkerLogin>()
                .HasOne(wl => wl.Worker)
                .WithMany(w => w.WorkerLogins)
                .HasForeignKey(wl => wl.WorkerId)
                .OnDelete(DeleteBehavior.Restrict);

            modelBuilder.ApplyConfigurationsFromAssembly(Assembly.GetExecutingAssembly());
        }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            optionsBuilder.EnableSensitiveDataLogging();
        }

        protected override void ConfigureConventions(ModelConfigurationBuilder configurationBuilder)
        {
            configurationBuilder.Properties<decimal>().HavePrecision(18, 2);
        }

        public override int SaveChanges()
        {
            EnsureAudit();
            return base.SaveChanges();
        }

        public override Task<int> SaveChangesAsync(bool acceptAllChangesOnSuccess, CancellationToken cancellationToken = default)
        {
            EnsureAudit();
            return base.SaveChangesAsync(acceptAllChangesOnSuccess, cancellationToken);
        }

        private void EnsureAudit()
        {
            ChangeTracker.DetectChanges();
            // Lógica de auditoría si la deseas implementar
        }

        // MÉTODOS CON DAPPER

        public async Task<IEnumerable<T>> QueryAsync<T>(string text, object parameters = null, int? timeout = null, CommandType? type = null)
        {
            using var command = new DapperEFCoreCommand(this, text, parameters, timeout, type, CancellationToken.None);
            var connection = this.Database.GetDbConnection();
            return await connection.QueryAsync<T>(command.Definition);
        }

        public async Task<T> QueryFirstOrDefaultAsync<T>(string text, object parameters = null, int? timeout = null, CommandType? type = null)
        {
            using var command = new DapperEFCoreCommand(this, text, parameters, timeout, type, CancellationToken.None);
            var connection = this.Database.GetDbConnection();
            return await connection.QueryFirstOrDefaultAsync<T>(command.Definition);
        }

        public async Task<int> ExecuteAsync(string text, object parametres = null, int? timeout = null, CommandType? type = null)
        {
            using var command = new DapperEFCoreCommand(this, text, parametres, timeout, type, CancellationToken.None);
            var connection = this.Database.GetDbConnection();
            return await connection.ExecuteAsync(command.Definition);
        }

        public async Task<T> ExecuteScalarAsync<T>(string query, object parameters = null, int? timeout = null, CommandType? type = null)
        {
            using var command = new DapperEFCoreCommand(this, query, parameters, timeout, type, CancellationToken.None);
            var connection = this.Database.GetDbConnection();
            return await connection.ExecuteScalarAsync<T>(command.Definition);
        }

        public readonly struct DapperEFCoreCommand : IDisposable
        {
            public DapperEFCoreCommand(DbContext context, string text, object parameters, int? timeout, CommandType? type, CancellationToken ct)
            {
                var transaction = context.Database.CurrentTransaction?.GetDbTransaction();
                var commandType = type ?? CommandType.Text;
                var commandTimeout = timeout ?? context.Database.GetCommandTimeout() ?? 30;

                Definition = new CommandDefinition(
                    text,
                    parameters,
                    transaction,
                    commandTimeout,
                    commandType,
                    cancellationToken: ct
                );
            }

            public CommandDefinition Definition { get; }

            public void Dispose() { }
        }
    }
}
