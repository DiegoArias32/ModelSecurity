using Microsoft.EntityFrameworkCore;
using Entity.Model;

namespace Entity.Contexts
{
    public class ApplicationDbContext : DbContext
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options) : base(options) { }

        public DbSet<Rol> Roles { get; set; }
        public DbSet<Client> Clients { get; set; } // 🔹 Agregado para manejar clientes
        public DbSet<Form> Forms { get; set; }  // Asegura que la entidad esté en el contexto
        public DbSet<Module> Modules { get; set; }

        public DbSet<FormModule> FormModules { get; set; }

    }
}
