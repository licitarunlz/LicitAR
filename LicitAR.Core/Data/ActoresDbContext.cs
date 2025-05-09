using LicitAR.Core.Data.Models.Parametros;
using LicitAR.Core.Data.Models;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Reflection.Emit;

namespace LicitAR.Core.Data
{
    public class ActoresDbContext : DbContext
    {
        public ActoresDbContext(DbContextOptions<ActoresDbContext> options)
            : base(options)
        {


        }


        /*Identificacion*/
        public DbSet<Persona> Personas { get; set; }
        public DbSet<EntidadLicitante> EntidadesLicitantes { get; set; }
        /*Fin Identificacion*/
        /*Parametria*/
        public DbSet<Localidad> Localidades { get; set; }
        public DbSet<Provincia> Provincias { get; set; }

        public DbSet<TipoContacto> TiposContacto { get; set; }
        public DbSet<TipoPersona> TiposPersona { get; set; }



        protected override void OnModelCreating(ModelBuilder builder)
        {
            builder.Entity<Persona>().OwnsOne(p => p.Audit);
            builder.Entity<Persona>().HasOne(l => l.Provincia)
                    .WithMany()
                    .HasForeignKey(l => l.IdProvincia);

            builder.Entity<Persona>().HasOne(l => l.Localidad)
                    .WithMany()
                    .HasForeignKey(l => l.IdLocalidad);
            
            builder.Entity<Persona>().HasOne(l => l.TipoPersona)
                    .WithMany()
                    .HasForeignKey(l => l.IdTipoPersona);

            builder.Entity<Localidad>().OwnsOne(p => p.Audit);
            builder.Entity<Provincia>().OwnsOne(p => p.Audit);
            builder.Entity<TipoContacto>().OwnsOne(p => p.Audit);
            builder.Entity<TipoPersona>().OwnsOne(p => p.Audit);
            builder.Entity<EntidadLicitanteUsuario>().OwnsOne(e => e.Audit);

            builder.Entity<EntidadLicitanteUsuario>()
                .HasKey(eu => new { eu.IdEntidadLicitante, eu.IdUsuario });

            builder.Entity<EntidadLicitanteUsuario>()
                .HasOne(eu => eu.EntidadLicitante)
                .WithMany(e => e.Usuarios)
                .HasForeignKey(eu => eu.IdEntidadLicitante);

            builder.Entity<EntidadLicitanteUsuario>()
                .HasOne(eu => eu.Usuario)
                .WithMany(u => u.EntidadesLicitantes)
                .HasForeignKey(eu => eu.IdUsuario);


            base.OnModelCreating(builder);
        }


    } 
}
