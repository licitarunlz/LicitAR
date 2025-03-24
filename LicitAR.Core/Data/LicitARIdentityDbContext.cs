using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using LicitAR.Core.Data.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;

namespace LicitAR.Core.Data
{
    public class LicitARIdentityDbContext: IdentityDbContext<LicitArUser>
    {
        public LicitARIdentityDbContext(DbContextOptions<LicitARIdentityDbContext> options)
            : base(options) { }
    }
}