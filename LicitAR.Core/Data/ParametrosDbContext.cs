using LicitAR.Core.Data.Models;
using LicitAR.Core.Data.Models.Parametros;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LicitAR.Core.Data
{
    public class ParametrosDbContext: DbContext
    {

        public ParametrosDbContext(DbContextOptions<ParametrosDbContext> options)
           : base(options)
        {


        }


        /*Parametria*/
        public DbSet<Localidad> Localidades { get; set; }
        public DbSet<Provincia> Provincias { get; set; }

        public DbSet<TipoContacto> TiposContacto { get; set; }
        public DbSet<TipoPersona> TiposPersona { get; set; }

        public DbSet<Parametria> Parametria { get; set; }

        public DbSet<EstadoLicitacion> EstadosLicitacion { get; set; }
        public DbSet<CategoriaLicitacion> CategoriasLicitacion { get; set; }

        /*Fin Parametria*/

        protected override void OnModelCreating(ModelBuilder builder)
        {
            builder.Entity<Localidad>().OwnsOne(p => p.Audit);
            builder.Entity<Provincia>().OwnsOne(p => p.Audit);
            builder.Entity<TipoContacto>().OwnsOne(p => p.Audit);
            builder.Entity<TipoPersona>().OwnsOne(p => p.Audit);
            builder.Entity<Parametria>().OwnsOne(p => p.Audit);
            builder.Entity<EstadoLicitacion>().OwnsOne(p => p.Audit);
            builder.Entity<CategoriaLicitacion>().OwnsOne(p => p.Audit);

            base.OnModelCreating(builder);
        }


    }
}
