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
        public DbSet<User> Users { get; set; }
        public DbSet<Worker> Workers { get; set; } // Agregado para manejar trabajadores

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder); // Llama a la configuración base

            // Configuración de la relación entre User y Worker
            modelBuilder.Entity<User>()
                .HasOne(u => u.Worker)        // Relación de User con Worker
                .WithOne(w => w.User)         // Relación inversa de Worker con User
                .HasForeignKey<User>(u => u.WorkerId);  // Define la clave foránea explícita
        }
    }
}
