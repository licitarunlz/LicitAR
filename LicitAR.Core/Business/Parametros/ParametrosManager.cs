using LicitAR.Core.Data;
using LicitAR.Core.Data.Models.Parametros;
using Microsoft.EntityFrameworkCore;

namespace LicitAR.Core.Business.Parametros
{
    public interface IParametrosManager
    {
        Task<List<Provincia>> GetAllProvinciasAsync();
        Task<Provincia?> GetProvinciaByIdAsync(int idProvincia);
        Task<List<Localidad>> GetLocalidadesByProvinciaAsync(int idProvincia);
        Task<Localidad?> GetLocalidadByIdAsync(int idLocalidad);
    }

    public class ParametrosManager : IParametrosManager
    {
        private readonly LicitARDbContext _dbContext;

        public ParametrosManager(LicitARDbContext dbContext)
        {
            _dbContext = dbContext;
        }

        public async Task<List<Provincia>> GetAllProvinciasAsync()
        {
            return await _dbContext.Provincias
                .ToListAsync();
        }

        public async Task<Provincia?> GetProvinciaByIdAsync(int idProvincia)
        {
            return await _dbContext.Provincias
                .FirstOrDefaultAsync(p => p.IdProvincia == idProvincia && p.Enabled);
        }

        public async Task<List<Localidad>> GetLocalidadesByProvinciaAsync(int idProvincia)
        {
            Console.WriteLine($"Entering GetLocalidadesByProvinciaAsync with idProvincia: {idProvincia}");

            var query = _dbContext.Localidades
                .Include(l => l.Provincia) // Include Provincia navigation property
                .AsQueryable();

            if (idProvincia != 0)
            {
                Console.WriteLine($"Filtering localidades by IdProvincia: {idProvincia}");
                query = query.Where(l => l.IdProvincia == idProvincia);
            }

            var result = await query
                .ToListAsync();

            Console.WriteLine($"Fetched {result.Count} localidades from the database.");
            return result;
        }

        public async Task<Localidad?> GetLocalidadByIdAsync(int idLocalidad)
        {
            return await _dbContext.Localidades
                .FirstOrDefaultAsync(l => l.IdLocalidad == idLocalidad && l.Enabled);
        }
    }
}
