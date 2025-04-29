using LicitAR.Core.Data;
using LicitAR.Core.Data.Models;
using Microsoft.EntityFrameworkCore;

namespace LicitAR.Core.Business.Licitaciones
{
    public interface ILicitacionManager
    {
        Task<List<Licitacion>> GetAllLicitacionesAsync();
        Task<Licitacion?> GetLicitacionByIdAsync(int id);
        Task CreateLicitacionAsync(Licitacion licitacion);
        Task<bool> UpdateLicitacionAsync(Licitacion licitacion);
        Task<bool> DeleteLicitacionAsync(int id);
    }
    
    public class LicitacionManager : ILicitacionManager
    {
        private readonly LicitacionesDbContext _context;

        public LicitacionManager(LicitacionesDbContext context)
        {
            _context = context;
        }

        public async Task<List<Licitacion>> GetAllLicitacionesAsync()
        {
            return await _context.Licitaciones.ToListAsync();
        }

        public async Task<Licitacion?> GetLicitacionByIdAsync(int id)
        {
            return await _context.Licitaciones.FirstOrDefaultAsync(l => l.IdLicitacion == id);
        }

        public async Task CreateLicitacionAsync(Licitacion licitacion)
        {
            _context.Licitaciones.Add(licitacion);
            await _context.SaveChangesAsync();
        }

        public async Task<bool> UpdateLicitacionAsync(Licitacion licitacion)
        {
            _context.Licitaciones.Update(licitacion);
            try
            {
                await _context.SaveChangesAsync();
                return true;
            }
            catch (DbUpdateConcurrencyException)
            {
                return _context.Licitaciones.Any(e => e.IdLicitacion == licitacion.IdLicitacion);
            }
        }

        public async Task<bool> DeleteLicitacionAsync(int id)
        {
            var licitacion = await _context.Licitaciones.FindAsync(id);
            if (licitacion == null)
            {
                return false;
            }

            _context.Licitaciones.Remove(licitacion);
            await _context.SaveChangesAsync();
            return true;
        }
    }
}
