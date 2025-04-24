using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using LicitAR.Core.Data.Models;
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

        protected override void OnModelCreating(ModelBuilder builder)
        {
            builder.Entity<LicitArUser>().OwnsOne(p => p.Audit); 

            base.OnModelCreating(builder);
        }
    }
}