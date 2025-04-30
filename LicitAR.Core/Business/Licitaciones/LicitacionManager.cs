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
        private readonly LicitacionesDbContext _context;

        public LicitacionManager(LicitacionesDbContext context)
        {
            _context = context;
        }

        public async Task<List<Licitacion>> GetAllLicitacionesAsync()
        {
            return await _context.Licitaciones.Where(x=> x.Audit.FechaBaja == null).ToListAsync();
        }

        public async Task<Licitacion?> GetLicitacionByIdAsync(int id)
        {
            return await _context.Licitaciones.FirstOrDefaultAsync(l => l.IdLicitacion == id);
        }

        public async Task CreateLicitacionAsync(Licitacion licitacion, int userId)
        {
            try
            {
                licitacion.CodigoLicitacion = await this.ObtenerProximoCodigoAsync();
                licitacion.IdEstadoLicitacion = 1; //arranca en Borrador
                licitacion.Audit = AuditHelper.GetCreationData(userId);
                _context.Licitaciones.Add(licitacion);
                await _context.SaveChangesAsync();
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
                var licitacionFromDdbb = await _context.Licitaciones.FirstOrDefaultAsync(x => x.IdLicitacion == licitacion.IdLicitacion);
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
                _context.Licitaciones.Update(licitacionFromDdbb);
            
                await _context.SaveChangesAsync();
                return true;
            }
            catch (DbUpdateConcurrencyException)
            {
                return _context.Licitaciones.Any(e => e.IdLicitacion == licitacion.IdLicitacion);
            }
        }

        public async Task<bool> DeleteLicitacionAsync(int id, int idUsuario)
        {
            var licitacion = await _context.Licitaciones.FindAsync(id);
            if (licitacion == null)
            {
                return false;
            }
            licitacion.Audit = AuditHelper.SetDeletionData(licitacion.Audit, idUsuario);

            _context.Licitaciones.Update(licitacion);
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
