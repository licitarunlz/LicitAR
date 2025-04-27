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

        public LicitacionesDbContext(DbContextOptions<LicitacionesDbContext> options)
            : base(options)
        {


        }
    }
}
