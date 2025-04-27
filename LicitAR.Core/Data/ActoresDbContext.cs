using LicitAR.Core.Data.Models.Parametros;
using LicitAR.Core.Data.Models;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

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


        protected override void OnModelCreating(ModelBuilder builder)
        {
            builder.Entity<Persona>().OwnsOne(p => p.Audit);
            builder.Entity<EntidadLicitante>().OwnsOne(p=> p.Audit);

            base.OnModelCreating(builder);
        }


    } 
}
