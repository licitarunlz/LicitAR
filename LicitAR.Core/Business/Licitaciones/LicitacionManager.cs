using LicitAR.Core.Data;
using LicitAR.Core.Data.Models;
using Microsoft.EntityFrameworkCore;
using System.Data;

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
            try
            {
                licitacion.CodigoLicitacion = await this.ObtenerProximoCodigoAsync();
                licitacion.IdEstadoLicitacion = 1; //arranca en Borrador

                _context.Licitaciones.Add(licitacion);
                await _context.SaveChangesAsync();
            }
            catch
            {
                throw;
            }
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


        public async Task<string> ObtenerProximoCodigoAsync()
        {
            var connection = _context.Database.GetDbConnection();
            if (connection.State != ConnectionState.Open)
                await connection.OpenAsync();

            using (var command = connection.CreateCommand())
            {
                command.CommandText = "sp_ObtenerProximoCodigoLicitacion";
                command.CommandType = CommandType.StoredProcedure;

                var result = await command.ExecuteScalarAsync();
                // No cierres la conexión manualmente
                return result?.ToString();
            }
        }
    }
}
