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
    /// <summary>
    /// Representa el contexto de base de datos principal de la aplicación.
    /// Contiene configuraciones de entidades y utilidades para Dapper.
    /// </summary>
    public class ApplicationDbContext : DbContext
    {
        protected readonly IConfiguration _configuration;

        /// <summary>
        /// Constructor del contexto de base de datos.
        /// </summary>
        /// <param name="options">Opciones del contexto.</param>
        /// <param name="configuration">Configuración de la aplicación.</param>
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options, IConfiguration configuration)
            : base(options)
        {
            _configuration = configuration;
        }

        //
        // DBSETS
        //
        public DbSet<Rol> Roles { get; set; }
        public DbSet<Client> Clients { get; set; }
        public DbSet<Form> Forms { get; set; }
        public DbSet<Module> Modules { get; set; }
        public DbSet<FormModule> FormModules { get; set; }
        public DbSet<User> Users { get; set; }
        public DbSet<Worker> Workers { get; set; }
        public DbSet<Permission> Permissions { get; set; }
        public DbSet<RolUser> RolUsers { get; set; }
        public DbSet<Pqr> Pqr { get; set;}

        /// <summary>
        /// Configura las relaciones entre entidades.
        /// </summary>
        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            // Relación User - Worker
            modelBuilder.Entity<User>()
                .HasOne(u => u.Worker)
                .WithOne(w => w.User)
                .HasForeignKey<User>(u => u.WorkerId);

            modelBuilder.ApplyConfigurationsFromAssembly(Assembly.GetExecutingAssembly());
        }

        /// <summary>
        /// Configura opciones adicionales como el registro de datos sensibles.
        /// </summary>
        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            optionsBuilder.EnableSensitiveDataLogging();
        }

        /// <summary>
        /// Configura la convención de precisión para propiedades decimales.
        /// </summary>
        protected override void ConfigureConventions(ModelConfigurationBuilder configurationBuilder)
        {
            configurationBuilder.Properties<decimal>().HavePrecision(18, 2);
        }

        /// <summary>
        /// Guarda los cambios y aplica auditoría previa.
        /// </summary>
        public override int SaveChanges()
        {
            EnsureAudit();
            return base.SaveChanges();
        }

        /// <summary>
        /// Guarda los cambios de manera asíncrona y aplica auditoría previa.
        /// </summary>
        public override Task<int> SaveChangesAsync(bool acceptAllChangesOnSuccess, CancellationToken cancellationToken = default)
        {
            EnsureAudit();
            return base.SaveChangesAsync(acceptAllChangesOnSuccess, cancellationToken);
        }

        /// <summary>
        /// Aplica lógica de auditoría a las entidades antes de guardar.
        /// </summary>
        private void EnsureAudit()
        {
            ChangeTracker.DetectChanges();
            // Aquí puedes agregar lógica para auditoría de usuario, timestamps, etc.
        }

        //
        // MÉTODOS CON DAPPER
        //

        /// <summary>
        /// Ejecuta una consulta SQL con Dapper y retorna una colección de tipo T.
        /// </summary>
        public async Task<IEnumerable<T>> QueryAsync<T>(string text, object parameters = null, int? timeout = null, CommandType? type = null)
        {
            using var command = new DapperEFCoreCommand(this, text, parameters, timeout, type, CancellationToken.None);
            var connection = this.Database.GetDbConnection();
            return await connection.QueryAsync<T>(command.Definition);
        }

        /// <summary>
        /// Ejecuta una consulta SQL con Dapper y retorna el primer resultado o null.
        /// </summary>
        public async Task<T> QueryFirstOrDefaultAsync<T>(string text, object parameters = null, int? timeout = null, CommandType? type = null)
        {
            using var command = new DapperEFCoreCommand(this, text, parameters, timeout, type, CancellationToken.None);
            var connection = this.Database.GetDbConnection();
            return await connection.QueryFirstOrDefaultAsync<T>(command.Definition);
        }

        /// <summary>
        /// Ejecuta una instrucción SQL (INSERT, UPDATE, DELETE) y retorna el número de filas afectadas.
        /// </summary>
        public async Task<int> ExecuteAsync(string text, object parametres = null, int? timeout = null, CommandType? type = null)
        {
            using var command = new DapperEFCoreCommand(this, text, parametres, timeout, type, CancellationToken.None);
            var connection = this.Database.GetDbConnection();
            return await connection.ExecuteAsync(command.Definition);
        }

        /// <summary>
        /// Ejecuta una consulta SQL y retorna el valor escalar (ej. COUNT, MAX).
        /// </summary>
        public async Task<T> ExecuteScalarAsync<T>(string query, object parameters = null, int? timeout = null, CommandType? type = null)
        {
            using var command = new DapperEFCoreCommand(this, query, parameters, timeout, type, CancellationToken.None);
            var connection = this.Database.GetDbConnection();
            return await connection.ExecuteScalarAsync<T>(command.Definition);
        }

        /// <summary>
        /// Estructura para definir comandos SQL usados por Dapper.
        /// </summary>
        public readonly struct DapperEFCoreCommand : IDisposable
        {
            /// <summary>
            /// Constructor del comando Dapper.
            /// </summary>
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

            /// <summary>
            /// Definición del comando para ser ejecutado por Dapper.
            /// </summary>
            public CommandDefinition Definition { get; }

            public void Dispose() { }
        }
    }
}
