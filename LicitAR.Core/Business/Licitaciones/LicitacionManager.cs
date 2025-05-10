using LicitAR.Core.Data;
using LicitAR.Core.Data.Models;
using LicitAR.Core.Utils;
using Microsoft.EntityFrameworkCore;
using System.Data;

namespace LicitAR.Core.Business.Licitaciones
{
    public interface ILicitacionManager
    {
        Task<List<Licitacion>> GetAllLicitacionesAsync();
        Task<Licitacion?> GetLicitacionByIdAsync(int id);
        Task CreateLicitacionAsync(Licitacion licitacion, int userId);
        Task<bool> UpdateLicitacionAsync(Licitacion licitacion, int userId);
        Task<bool> DeleteLicitacionAsync(int id, int idUsuario);
    }

    public class LicitacionManager : ILicitacionManager
    {
        private readonly LicitARDbContext _dbContext;

        public LicitacionManager(LicitARDbContext dbContext)
        {
            _dbContext = dbContext;
        }

        public async Task<List<Licitacion>> GetAllLicitacionesAsync()
        {
            return await _dbContext.Licitaciones.Where(x=> x.Audit.FechaBaja == null).ToListAsync();
        }

        public async Task<Licitacion?> GetLicitacionByIdAsync(int id)
        {
            return await _dbContext.Licitaciones.FirstOrDefaultAsync(l => l.IdLicitacion == id);
        }

        public async Task CreateLicitacionAsync(Licitacion licitacion, int userId)
        {
            try
            {
                licitacion.CodigoLicitacion = await this.ObtenerProximoCodigoAsync();
                licitacion.IdEstadoLicitacion = 1; //arranca en Borrador
                licitacion.Audit = AuditHelper.GetCreationData(userId);
                _dbContext.Licitaciones.Add(licitacion);
                await _dbContext.SaveChangesAsync();
            }
            catch
            {
                throw;
            }
        }

        public async Task<bool> UpdateLicitacionAsync(Licitacion licitacion, int userId)
        {
            try
            {
                var licitacionFromDdbb = await _dbContext.Licitaciones.FirstOrDefaultAsync(x => x.IdLicitacion == licitacion.IdLicitacion);
                if (licitacionFromDdbb == null)
                    return false;

                licitacionFromDdbb.Titulo = licitacion.Titulo;
                licitacionFromDdbb.Descripcion = licitacion.Descripcion;
                licitacionFromDdbb.IdCategoriaLicitacion = licitacion.IdCategoriaLicitacion;
                licitacionFromDdbb.FechaPublicacion = licitacion.FechaPublicacion;
                licitacionFromDdbb.FechaCierre = licitacion.FechaCierre;

                licitacionFromDdbb.Audit = AuditHelper.SetModificationData(licitacionFromDdbb.Audit, userId);
                /*
                // Marcar solo los campos modificados 
                _context.Entry(licitacionFromDdbb).Property(u => u.Titulo).IsModified = true;
                _context.Entry(licitacionFromDdbb).Property(u => u.Descripcion).IsModified = true;
                _context.Entry(licitacionFromDdbb).Property(u => u.IdCategoriaLicitacion).IsModified = true;
                _context.Entry(licitacionFromDdbb).Property(u => u.FechaPublicacion).IsModified = true;
                _context.Entry(licitacionFromDdbb).Property(u => u.FechaCierre).IsModified = true;
                */
                _dbContext.Licitaciones.Update(licitacionFromDdbb);
            
                await _dbContext.SaveChangesAsync();
                return true;
            }
            catch (DbUpdateConcurrencyException)
            {
                return _dbContext.Licitaciones.Any(e => e.IdLicitacion == licitacion.IdLicitacion);
            }
        }

        public async Task<bool> DeleteLicitacionAsync(int id, int idUsuario)
        {
            var licitacion = await _dbContext.Licitaciones.FindAsync(id);
            if (licitacion == null)
            {
                return false;
            }
            licitacion.Audit = AuditHelper.SetDeletionData(licitacion.Audit, idUsuario);

            _dbContext.Licitaciones.Update(licitacion);
            await _dbContext.SaveChangesAsync();
            return true;
        }


        public async Task<string> ObtenerProximoCodigoAsync()
        {
            var connection = _dbContext.Database.GetDbConnection();
            if (connection.State != ConnectionState.Open)
                await connection.OpenAsync();

            using (var command = connection.CreateCommand())
            {
                command.CommandText = "sp_ObtenerProximoCodigoLicitacion";
                command.CommandType = CommandType.StoredProcedure;

                var result = await command.ExecuteScalarAsync();
                // No cierres la conexi�n manualmente
                return result?.ToString();
            }
        }
    }
}
