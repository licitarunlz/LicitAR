using LicitAR.Core.Data;
using LicitAR.Core.Data.Models;
using LicitAR.Core.Utils;
using LicitAR.Core.Data.Models.Parametros;
using Microsoft.EntityFrameworkCore;
using System.Data;

namespace LicitAR.Core.Business.Licitaciones
{
    public interface ILicitacionManager
    {
        Task<List<Licitacion>> GetAllLicitacionesAsync();
        Task<List<Licitacion>> GetAllActiveLicitacionesAsync();
        Task<Licitacion?> GetLicitacionByIdAsync(int id);
        Task CreateLicitacionAsync(Licitacion licitacion, int userId);
        Task<bool> UpdateLicitacionAsync(Licitacion licitacion, int userId);
        Task<bool> DeleteLicitacionAsync(int id, int idUsuario);
        Task<List<Licitacion>> GetLicitacionesByEstadoAsync(int idEstadoLicitacion);
        Task<EstadoLicitacion?> GetEstadoLicitacionByIdAsync(int idEstadoLicitacion);
        Task<CategoriaLicitacion?> GetCategoriaLicitacionByIdAsync(int idCategoriaLicitacion);
        Task<List<CategoriaLicitacion>> GetAllCategoriasLicitacionAsync();
        Task<List<EstadoLicitacion>> GetAllEstadosLicitacionAsync();
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
            return await _dbContext.Licitaciones
                .Include(l => l.EstadoLicitacion) // Include EstadoLicitacion
                .Include(l => l.CategoriaLicitacion) // Include CategoriaLicitacion
                .ToListAsync();
        }

        public async Task<List<Licitacion>> GetAllActiveLicitacionesAsync()
        {
            return await _dbContext.Licitaciones.Where(x=> x.Audit.FechaBaja == null).ToListAsync();
        }

        public async Task<Licitacion?> GetLicitacionByIdAsync(int id)
        {
            return await _dbContext.Licitaciones
                .Include(l => l.EstadoLicitacion) // Include EstadoLicitacion
                .Include(l => l.CategoriaLicitacion) // Include CategoriaLicitacion
                .Include(d => d.Items)  // Include Los items
                .FirstOrDefaultAsync(l => l.IdLicitacion == id);
        }

        public async Task CreateLicitacionAsync(Licitacion licitacion, int userId)
        {
            try
            {
                licitacion.CodigoLicitacion = await this.ObtenerProximoCodigoAsync();
                licitacion.IdEstadoLicitacion = 1; // Default state: Planificación
                licitacion.Audit = AuditHelper.GetCreationData(userId);
                _dbContext.Licitaciones.Add(licitacion);
                foreach (var detalle in licitacion.Items)
                {
                    detalle.IdLicitacion = licitacion.IdLicitacion;
                    detalle.Audit = AuditHelper.GetCreationData(userId);
                }

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

                /*foreach (var detalle in licitacion.Items)
                {
                    if (detalle.Audit.FechaBaja != null)
                    {
                        detalle.IdLicitacion = licitacion.IdLicitacion;
                        //Si es distinto de null, estoy eliminando un registro
                        continue;
                    }

                    if (detalle.IdLicitacionDetalle != 0)
                    {
                        //estoy editando

                        detalle.IdLicitacion = licitacion.IdLicitacion;
                        detalle.
                    }
                    detalle.Audit = AuditHelper.GetCreationData(userId);
                }*/
                licitacionFromDdbb.Items = licitacion.Items;

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
            licitacion.Enabled = false;
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

        public async Task<List<Licitacion>> GetLicitacionesByEstadoAsync(int idEstadoLicitacion)
        {
            return await _dbContext.Licitaciones
                .Include(l => l.EstadoLicitacion) // Incluir la relación con EstadoLicitacion
                .Where(l => l.IdEstadoLicitacion == idEstadoLicitacion)
                .ToListAsync();
        }

        public async Task<EstadoLicitacion?> GetEstadoLicitacionByIdAsync(int idEstadoLicitacion)
        {
            return await _dbContext.EstadosLicitacion
                .FirstOrDefaultAsync(e => e.IdEstadoLicitacion == idEstadoLicitacion);
        }

        public async Task<CategoriaLicitacion?> GetCategoriaLicitacionByIdAsync(int idCategoriaLicitacion)
        {
            return await _dbContext.CategoriasLicitacion
                .FirstOrDefaultAsync(c => c.IdCategoriaLicitacion == idCategoriaLicitacion);
        }

        public async Task<List<CategoriaLicitacion>> GetAllCategoriasLicitacionAsync()
        {
            return await _dbContext.CategoriasLicitacion.ToListAsync();
        }

        public async Task<List<EstadoLicitacion>> GetAllEstadosLicitacionAsync()
        {
            return await _dbContext.EstadosLicitacion.ToListAsync();
        }
    }
}
