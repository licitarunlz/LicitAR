using LicitAR.Core.Data.Models;
using LicitAR.Core.Data.Models.Identidad;
using LicitAR.Core.Data.Models.Parametros;
using LicitAR.Core.Data.Models.Historial;
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
        public DbSet<Rubro> Rubros { get; set; }
        public DbSet<TipoContacto> TiposContacto { get; set; }
        public DbSet<TipoPersona> TiposPersona { get; set; }
        public DbSet<Parametria> Parametria { get; set; }
        public DbSet<EstadoLicitacion> EstadosLicitacion { get; set; }
        public DbSet<CategoriaLicitacion> CategoriasLicitacion { get; set; }
        public DbSet<Licitacion> Licitaciones { get; set; }
        public DbSet<LicitacionDetalle> LicitacionesDetalle { get; set; }
        public DbSet<LicitacionChecklistItem> LicitacionChecklistItems { get; set; }
        public DbSet<EstadoOferta> EstadosOferta { get; set; }
        public DbSet<Oferta> Ofertas { get; set; }
        public DbSet<OfertaDetalle> OfertasDetalle { get; set; }
        public DbSet<OfertaChecklistItem> OfertaChecklistItems { get; set; }
        public DbSet<Evaluacion> Evaluaciones { get; set; }
        public DbSet<EvaluacionOferta> EvaluacionOfertas { get; set; }
        public DbSet<EvaluacionOfertaDetalle> EvaluacionOfertasDetalle { get; set; }
        public DbSet<EstadoEvaluacion> EstadoEvaluacion { get; set; }
        public DbSet<LicitacionEstadoHistorial> LicitacionEstadoHistorial { get; set; }
        public DbSet<LicitacionInvitacion> LicitacionInvitacion { get; set; }
        public DbSet<LicitacionDocumentacion> LicitacionDocumentacion { get; set; }
        public DbSet<AuditTrail> AuditTrails { get; set; }
        public DbSet<AuditLicitacion> AuditLicitaciones { get; set; }

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
            modelBuilder.Entity<EstadoEvaluacion>().OwnsOne(p => p.Audit);
            modelBuilder.Entity<EstadoOferta>().OwnsOne(p => p.Audit);
            modelBuilder.Entity<CategoriaLicitacion>().OwnsOne(p => p.Audit);

            modelBuilder.Entity<Evaluacion>().OwnsOne(p => p.Audit);
            modelBuilder.Entity<EvaluacionOferta>().OwnsOne(p => p.Audit);
            modelBuilder.Entity<EvaluacionOfertaDetalle>().OwnsOne(p => p.Audit);


            modelBuilder.Entity<Evaluacion>()
                        .HasOne(p => p.Licitacion)
                        .WithMany()
                        .HasForeignKey(p => p.IdLicitacion);
 

            modelBuilder.Entity<Evaluacion>()
                        .HasMany(l => l.EvaluacionOfertas)
                        .WithOne(i => i.Evaluacion)
                        .HasForeignKey(i => i.IdEvaluacion)
                        .OnDelete(DeleteBehavior.Cascade);

            modelBuilder.Entity<Evaluacion>()
                        .HasMany(l => l.EvaluacionOfertasDetalles)
                        .WithOne(i => i.Evaluacion)
                        .HasForeignKey(i => i.IdEvaluacion)
                        .OnDelete(DeleteBehavior.Cascade);

            modelBuilder.Entity<Evaluacion>()
                .HasOne(p => p.EstadoEvaluacion)
                .WithMany()
                .HasForeignKey(p => p.IdEstadoEvaluacion);

            modelBuilder.Entity<EvaluacionOferta>()
                .HasOne(p => p.Evaluacion)
                .WithMany(ld => ld.EvaluacionOfertas)
                .HasForeignKey(p => p.IdEvaluacionOferta)
                .OnDelete(DeleteBehavior.NoAction);


            modelBuilder.Entity<EvaluacionOferta>()
                .HasOne(p => p.Oferta)
                .WithMany()
                .HasForeignKey(p => p.IdOferta)
                .OnDelete(DeleteBehavior.NoAction);

            modelBuilder.Entity<EvaluacionOfertaDetalle>()
                .HasOne(p => p.OfertaDetalle)
                .WithMany()
                .HasForeignKey(p => p.IdOfertaDetalle)
                .OnDelete(DeleteBehavior.NoAction);



            modelBuilder.Entity<EvaluacionOfertaDetalle>()
                .HasOne(p => p.Evaluacion)
                .WithMany(p=> p.EvaluacionOfertasDetalles)
                .HasForeignKey(p => p.IdEvaluacion)
                .OnDelete(DeleteBehavior.NoAction);

            modelBuilder.Entity<Licitacion>().OwnsOne(p => p.Audit);

            modelBuilder.Entity<Licitacion>()
                        .HasMany(l => l.Items)
                        .WithOne(i => i.Licitacion)
                        .HasForeignKey(i => i.IdLicitacion)
                        .OnDelete(DeleteBehavior.Cascade); // Borra ítems si se borra la licitación

            modelBuilder.Entity<Licitacion>()
                .HasOne(l => l.Rubro)
                .WithMany()
                .HasForeignKey(l => l.IdRubro);

            modelBuilder.Entity<Licitacion>()
                        .HasOne(l => l.EntidadLicitante)
                        .WithMany(e => e.Licitaciones)
                        .HasForeignKey(l => l.IdEntidadLicitante)
                        .OnDelete(DeleteBehavior.NoAction); // o Cascade, según tu lógica

            modelBuilder.Entity<Licitacion>()
                    .HasMany(l => l.DocumentosAsociados)
                    .WithOne(i => i.Licitacion)
                    .HasForeignKey(i => i.IdLicitacion)
                    .OnDelete(DeleteBehavior.Cascade); // Borra ítems si se borra la licitación



            modelBuilder.Entity<LicitacionDetalle>().OwnsOne(p => p.Audit);


            modelBuilder.Entity<LicitacionChecklistItem>().OwnsOne(p => p.Audit);



            modelBuilder.Entity<LicitacionDocumentacion>().OwnsOne(p => p.Audit);

            modelBuilder.Entity<LicitacionDocumentacion>().HasOne(p => p.Licitacion)
                .WithMany(p => p.DocumentosAsociados)
                .HasForeignKey(p => p.IdLicitacion)
                .OnDelete(DeleteBehavior.NoAction);

            modelBuilder.Entity<Oferta>().OwnsOne(p => p.Audit);


            modelBuilder.Entity<Oferta>()
                .HasOne(p => p.EstadoOferta)
                .WithMany()
                .HasForeignKey(p => p.IdEstadoOferta);


            modelBuilder.Entity<Oferta>()
                .HasOne(p => p.Licitacion)
                .WithMany()
                .HasForeignKey(p => p.IdLicitacion);

            modelBuilder.Entity<Oferta>()
                        .HasMany(l => l.Items)
                        .WithOne(i => i.Oferta)
                        .HasForeignKey(i => i.IdOferta)
                        .OnDelete(DeleteBehavior.Cascade); // Borra ítems si se borra la licitación

            modelBuilder.Entity<OfertaDetalle>().OwnsOne(p => p.Audit);
            
            modelBuilder.Entity<OfertaDetalle>()
                .HasOne(p => p.LicitacionDetalle)
                .WithMany(ld => ld.OfertasDetalle)
                .HasForeignKey(p => p.IdLicitacionDetalle)
                .OnDelete(DeleteBehavior.NoAction);


            modelBuilder.Entity<OfertaChecklistItem>().OwnsOne(p => p.Audit);

            modelBuilder.Entity<OfertaChecklistItem>()
                .HasOne(p => p.LicitacionChecklistItem)
                .WithMany(ld => ld.OfertasChecklistItems)
                .HasForeignKey(p => p.IdLicitacionChecklistItem)
                .OnDelete(DeleteBehavior.NoAction);

            modelBuilder.Entity<OfertaChecklistItem>()
                .HasOne(p=> p.Oferta)
                .WithMany()
                .HasForeignKey(p=> p.IdOferta)
                .OnDelete(DeleteBehavior.NoAction);

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
                .HasOne(p => p.Rubro)
                .WithMany()
                .HasForeignKey(p => p.IdRubro);

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

            modelBuilder.Entity<LicitacionEstadoHistorial>()
                .HasOne(h => h.Licitacion)
                .WithMany()
                .HasForeignKey(h => h.IdLicitacion);

            modelBuilder.Entity<LicitacionEstadoHistorial>()
                .HasOne(h => h.EstadoNuevo)
                .WithMany()
                .HasForeignKey(h => h.IdEstadoNuevo)
                .OnDelete(DeleteBehavior.Restrict);

            modelBuilder.Entity<LicitacionEstadoHistorial>()
                .HasOne(h => h.EstadoAnterior)
                .WithMany()
                .HasForeignKey(h => h.IdEstadoAnterior)
                .OnDelete(DeleteBehavior.Restrict);

            modelBuilder.Entity<LicitacionInvitacion>()
                .HasKey(x => new { x.IdLicitacion, x.IdPersona });
            modelBuilder.Entity<LicitacionInvitacion>()
                .HasOne(x => x.Licitacion)
                .WithMany()
                .HasForeignKey(x => x.IdLicitacion);
            modelBuilder.Entity<LicitacionInvitacion>()
                .HasOne(x => x.Persona)
                .WithMany()
                .HasForeignKey(x => x.IdPersona);

            modelBuilder.Entity<LicitacionInvitacion>().OwnsOne(p => p.Audit);

            // Ignorar escritura de IdUsuario después del insert
            modelBuilder.Entity<LicitArUser>()
                .Property(u => u.IdUsuario)
                .ValueGeneratedOnAdd()
                .Metadata.SetAfterSaveBehavior(Microsoft.EntityFrameworkCore.Metadata.PropertySaveBehavior.Ignore);

            // Configuración directa de AuditTrail
            modelBuilder.Entity<AuditTrail>(builder =>
            {
                builder.HasKey(a => a.Id);
                builder.Property(a => a.Accion).HasMaxLength(100).IsRequired();
                builder.Property(a => a.Entidad).HasMaxLength(100).IsRequired();
                builder.Property(a => a.Descripcion).HasMaxLength(2000);
                builder.Property(a => a.IpCliente).HasMaxLength(50);
                builder.Property(a => a.UserAgent).HasMaxLength(512);
            });

            // Configuración directa de AuditLicitacion
            modelBuilder.Entity<AuditLicitacion>(builder =>
            {
                builder.HasKey(a => a.Id);
                builder.Property(a => a.Accion).HasMaxLength(100).IsRequired();
                builder.Property(a => a.CampoModificado).HasMaxLength(100);
                builder.Property(a => a.ValorAnterior).HasMaxLength(2000);
                builder.Property(a => a.ValorNuevo).HasMaxLength(2000);

                builder.HasOne(a => a.Licitacion)
                    .WithMany()
                    .HasForeignKey(a => a.IdLicitacion)
                    .OnDelete(DeleteBehavior.Cascade);
            });
        }
    }
}