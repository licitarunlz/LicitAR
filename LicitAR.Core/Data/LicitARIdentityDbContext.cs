using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using LicitAR.Core.Data.Models;
using LicitAR.Core.Data.Models.Identidad;
using LicitAR.Core.Data.Models.Parametros;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;

namespace LicitAR.Core.Data
{
    public class LicitARIdentityDbContext : IdentityDbContext<LicitArUser, IdentityRole, string>
    {
        public LicitARIdentityDbContext(DbContextOptions<LicitARIdentityDbContext> options)
            : base(options)
        {
        }

        /*Identificacion*/
        protected override void OnModelCreating(ModelBuilder builder)
        {
            builder.Entity<LicitArUser>().OwnsOne(p => p.Audit); 

            // Ignorar escritura de IdUsuario después del insert
            builder.Entity<LicitArUser>()
                .Property(u => u.IdUsuario)
                .ValueGeneratedOnAdd()
                .Metadata.SetAfterSaveBehavior(Microsoft.EntityFrameworkCore.Metadata.PropertySaveBehavior.Ignore);
           
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