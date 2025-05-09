using LicitAR.Core.Data.Models;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LicitAR.Core.Data
{
    public class LicitacionesDbContext : DbContext
    {
 

        /*Licitaciones*/
        public DbSet<Licitacion> Licitaciones { get; set; }

        public LicitacionesDbContext(DbContextOptions<LicitacionesDbContext> options)
            : base(options)
        {


        }


        protected override void OnModelCreating(ModelBuilder builder)
        {
            builder.Entity<Licitacion>().OwnsOne(p => p.Audit); 

            base.OnModelCreating(builder);
        }
    }
}
