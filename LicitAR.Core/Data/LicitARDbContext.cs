using LicitAR.Core.Data.Models;
using LicitAR.Core.Data.Models.Identidad;
using LicitAR.Core.Data.Models.Parametros;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;

namespace LicitAR.Core.Data
{
    public class LicitARDbContext : IdentityDbContext<LicitArUser, IdentityRole, string>
    {
        public LicitARDbContext(DbContextOptions<LicitARDbContext> options)
            : base(options)
        {
        }

        // Entidades de todos los DbContext
        public DbSet<EntidadLicitante> EntidadesLicitantes { get; set; }
        public DbSet<EntidadLicitanteUsuario> EntidadLicitanteUsuarios { get; set; }
        public DbSet<Persona> Personas { get; set; }
        public DbSet<PersonaUsuario> PersonaUsuarios { get; set; }
        public DbSet<Localidad> Localidades { get; set; }
        public DbSet<Provincia> Provincias { get; set; }
        public DbSet<Provincia> Rubros { get; set; }
        public DbSet<TipoContacto> TiposContacto { get; set; }
        public DbSet<TipoPersona> TiposPersona { get; set; }
        public DbSet<Parametria> Parametria { get; set; }
        public DbSet<EstadoLicitacion> EstadosLicitacion { get; set; }
        public DbSet<CategoriaLicitacion> CategoriasLicitacion { get; set; }
        public DbSet<Licitacion> Licitaciones { get; set; }
        public DbSet<LicitacionDetalle> LicitacionesDetalle { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            // Configuración de Audit
            modelBuilder.Entity<EntidadLicitante>().OwnsOne(p => p.Audit);
            modelBuilder.Entity<EntidadLicitanteUsuario>().OwnsOne(e => e.Audit);
            modelBuilder.Entity<Persona>().OwnsOne(p => p.Audit);
            modelBuilder.Entity<PersonaUsuario>().OwnsOne(e => e.Audit);
            modelBuilder.Entity<Localidad>().OwnsOne(p => p.Audit);
            modelBuilder.Entity<Provincia>().OwnsOne(p => p.Audit);
            modelBuilder.Entity<Rubro>().OwnsOne(p => p.Audit);
            modelBuilder.Entity<TipoContacto>().OwnsOne(p => p.Audit);
            modelBuilder.Entity<TipoPersona>().OwnsOne(p => p.Audit);
            modelBuilder.Entity<Parametria>().OwnsOne(p => p.Audit);
            modelBuilder.Entity<EstadoLicitacion>().OwnsOne(p => p.Audit);
            modelBuilder.Entity<CategoriaLicitacion>().OwnsOne(p => p.Audit);
            modelBuilder.Entity<Licitacion>().OwnsOne(p => p.Audit);

            modelBuilder.Entity<Licitacion>()
                        .HasMany(l => l.Items)
                        .WithOne(i => i.Licitacion)
                        .HasForeignKey(i => i.IdLicitacion)
                        .OnDelete(DeleteBehavior.Cascade); // Borra ítems si se borra la licitación

            modelBuilder.Entity<LicitacionDetalle>().OwnsOne(p => p.Audit);

            modelBuilder.Entity<LicitArUser>(entity =>
            {
                entity.OwnsOne(a => a.Audit); // Explicitly configure the Audit property
            });

            // Relaciones
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

            modelBuilder.Entity<Persona>()
                .HasOne(p => p.Provincia)
                .WithMany()
                .HasForeignKey(p => p.IdProvincia);

            modelBuilder.Entity<Persona>()
                .HasOne(p => p.Localidad)
                .WithMany()
                .HasForeignKey(p => p.IdLocalidad);

            modelBuilder.Entity<Persona>()
                .HasOne(p => p.TipoPersona)
                .WithMany()
                .HasForeignKey(p => p.IdTipoPersona);

            modelBuilder.Entity<Localidad>()
                .HasOne(l => l.Provincia)
                .WithMany()
                .HasForeignKey(l => l.IdProvincia)
                .OnDelete(DeleteBehavior.Restrict); // Prevent cascading deletes

            // Relaciones
            modelBuilder.Entity<PersonaUsuario>()
                .HasKey(eu => new { eu.IdPersona, eu.IdUsuario });

            modelBuilder.Entity<PersonaUsuario>()
                .HasOne(eu => eu.Persona)
                .WithMany(e => e.Usuarios)
                .HasForeignKey(eu => eu.IdPersona);

            modelBuilder.Entity<PersonaUsuario>()
                .HasOne(eu => eu.Usuario)
                .WithMany(u => u.Personas)
                .HasForeignKey(eu => eu.IdUsuario);

            // Ignorar escritura de IdUsuario después del insert
            modelBuilder.Entity<LicitArUser>()
                .Property(u => u.IdUsuario)
                .ValueGeneratedOnAdd()
                .Metadata.SetAfterSaveBehavior(Microsoft.EntityFrameworkCore.Metadata.PropertySaveBehavior.Ignore);
        }
    }
}