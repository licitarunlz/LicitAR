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
    public class LicitARIdentityDbContext: IdentityDbContext<LicitArUser>
    {
        public LicitARIdentityDbContext(DbContextOptions<LicitARIdentityDbContext> options)
            : base(options)
        {


        }

        /*Identificacion*/
        public DbSet<EmailConfirmationToken> AspNetUserEmailConfirmationTokens { get; set; }
        protected override void OnModelCreating(ModelBuilder builder)
        {
            builder.Entity<LicitArUser>().OwnsOne(p => p.Audit); 
            builder.Entity<EmailConfirmationToken>().OwnsOne(p=> p.Audit);

            base.OnModelCreating(builder);
        }



    }
}