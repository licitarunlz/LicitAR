using LicitAR.Core.Data.Models;
using LicitAR.Core.Data.Models.Identidad;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;

namespace LicitAR.Core.Data
{
    public class LicitARDbContext : IdentityDbContext<LicitArUser>
    {
        public LicitARDbContext(DbContextOptions<LicitARDbContext> options)
            : base(options)
        {
        }

        public DbSet<EntidadLicitante> EntidadesLicitantes { get; set; }
        public DbSet<EntidadLicitanteUsuario> EntidadLicitanteUsuarios { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            // Configurar Audit para EntidadLicitante
            modelBuilder.Entity<EntidadLicitante>().OwnsOne(p => p.Audit);

            modelBuilder.Entity<EntidadLicitanteUsuario>()
                .HasKey(eu => new { eu.IdEntidadLicitante, eu.IdUsuario });

            modelBuilder.Entity<EntidadLicitanteUsuario>()
                .HasOne(eu => eu.EntidadLicitante)
                .WithMany(e => e.Usuarios)
                .HasForeignKey(eu => eu.IdEntidadLicitante);

            modelBuilder.Entity<EntidadLicitanteUsuario>()
                .HasOne(eu => eu.Usuario)
                .WithMany(u => u.EntidadesLicitantes)
                .HasForeignKey(eu => eu.IdUsuario);
        }
    }
}