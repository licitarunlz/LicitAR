using LicitAR.Core.Data;
using LicitAR.Core.Data.Models;
using LicitAR.Core.Utils;
using LicitAR.Core.Data.Models.Parametros;
using LicitAR.Core.Data.Models.Historial; // Asegúrate de tener el namespace correcto
using Microsoft.EntityFrameworkCore;
using System.Data;
using Microsoft.Identity.Client;

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
        Task<List<CategoriaLicitacion>> GetAllCategoriasAsync();
        Task<bool> PublicarLicitacionAsync(int idLicitacion, DateTime fechaCierre, int idUsuario);
        Task<bool> IniciarEvaluacionLicitacionAsync(int idLicitacion, int idUsuario);
        Task<List<EstadoHistorialDto>> GetHistorialEstados(int idLicitacion);
    }

    public class EstadoHistorialDto
    {
        public DateTime Fecha { get; set; }
        public int IdEstado { get; set; }
        public string DescripcionEstado { get; set; }
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

                // Registrar historial de estado inicial
                _dbContext.LicitacionEstadoHistorial.Add(new LicitacionEstadoHistorial
                {
                    IdLicitacion = licitacion.IdLicitacion,
                    IdEstadoAnterior = null, // No hay estado anterior en creación
                    IdEstadoNuevo = licitacion.IdEstadoLicitacion,
                    FechaCambio = DateTime.Now,
                    IdUsuarioCambio = userId
                });

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

                var estadoAnterior = licitacionFromDdbb.IdEstadoLicitacion;
                bool cambioEstado = estadoAnterior != licitacion.IdEstadoLicitacion;

                licitacionFromDdbb.Titulo = licitacion.Titulo;
                licitacionFromDdbb.Descripcion = licitacion.Descripcion;
                licitacionFromDdbb.IdCategoriaLicitacion = licitacion.IdCategoriaLicitacion;
                licitacionFromDdbb.FechaPublicacion = licitacion.FechaPublicacion;
                licitacionFromDdbb.FechaCierre = licitacion.FechaCierre;
                licitacionFromDdbb.IdEntidadLicitante = licitacion.IdEntidadLicitante; // <-- Agrega esta línea

                licitacionFromDdbb.Audit = AuditHelper.SetModificationData(licitacionFromDdbb.Audit, userId);

                licitacionFromDdbb.Items = licitacion.Items;

                // Si cambió el estado, registrar en historial
                if (cambioEstado)
                {
                    licitacionFromDdbb.IdEstadoLicitacion = licitacion.IdEstadoLicitacion;
                    _dbContext.LicitacionEstadoHistorial.Add(new LicitacionEstadoHistorial
                    {
                        IdLicitacion = licitacion.IdLicitacion,
                        IdEstadoAnterior = estadoAnterior,
                        IdEstadoNuevo = licitacion.IdEstadoLicitacion,
                        FechaCambio = DateTime.Now,
                        IdUsuarioCambio = userId
                    });
                }

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

            // Registrar historial de cambio de estado si corresponde (por ejemplo, si hay un estado de "Eliminado")
            // Si existe un estado específico para "Eliminado", descomentar y ajustar el siguiente bloque:
            
            var estadoAnterior = licitacion.IdEstadoLicitacion;
            licitacion.IdEstadoLicitacion = 14; // Definir el valor correspondiente
            _dbContext.LicitacionEstadoHistorial.Add(new LicitacionEstadoHistorial
            {
                IdLicitacion = id,
                IdEstadoAnterior = estadoAnterior,
                IdEstadoNuevo = 14,
                FechaCambio = DateTime.Now,
                IdUsuarioCambio = idUsuario
            });
            
            _dbContext.Licitaciones.Update(licitacion);
            await _dbContext.SaveChangesAsync();
            return true;
        }

        public async Task<bool> PublicarLicitacionAsync(int idLicitacion, DateTime fechaCierre, int idUsuario)
        {
            var licitacion = await _dbContext.Licitaciones.FindAsync(idLicitacion);
            if (licitacion == null)
            {
                return false;
            }
            var estadoAnterior = licitacion.IdEstadoLicitacion;
            licitacion.Enabled = true;
            licitacion.FechaPublicacion = DateTime.Now;
            licitacion.IdEstadoLicitacion = 3;
            licitacion.FechaCierre = fechaCierre;
            licitacion.Audit = AuditHelper.SetModificationData(licitacion.Audit, idUsuario);

            _dbContext.Licitaciones.Update(licitacion);

            // Registrar historial de cambio de estado
            _dbContext.LicitacionEstadoHistorial.Add(new LicitacionEstadoHistorial
            {
                IdLicitacion = idLicitacion,
                IdEstadoAnterior = estadoAnterior,
                IdEstadoNuevo = 3,
                FechaCambio = DateTime.Now,
                IdUsuarioCambio = idUsuario
            });

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

        public async Task<List<CategoriaLicitacion>> GetAllCategoriasAsync()
        {
            return await GetAllCategoriasLicitacionAsync();
        }

        public async Task<bool> IniciarEvaluacionLicitacionAsync(int idLicitacion, int idUsuario)
        {
            var licitacion = await _dbContext.Licitaciones.FindAsync(idLicitacion);
            if (licitacion == null)
            {
                return false;
            }

            if (licitacion.IdEstadoLicitacion == 7)
            {
                return true; //ya fue cambiado el estado
            }

            if (licitacion.IdEstadoLicitacion != 3)
            {
                return false;
            }

            var estadoAnterior = licitacion.IdEstadoLicitacion;
            licitacion.IdEstadoLicitacion = 7;
            licitacion.Audit = AuditHelper.SetModificationData(licitacion.Audit, idUsuario);

            _dbContext.Licitaciones.Update(licitacion);

            // Registrar historial de cambio de estado
            _dbContext.LicitacionEstadoHistorial.Add(new LicitacionEstadoHistorial
            {
                IdLicitacion = idLicitacion,
                IdEstadoAnterior = estadoAnterior,
                IdEstadoNuevo = 7,
                FechaCambio = DateTime.Now,
                IdUsuarioCambio = idUsuario
            });

            await _dbContext.SaveChangesAsync();
            return true;
        }

        public async Task<List<EstadoHistorialDto>> GetHistorialEstados(int idLicitacion)
        {
            // Ahora consulta la tabla de historial real
            var historial = await _dbContext.LicitacionEstadoHistorial
                .Where(x => x.IdLicitacion == idLicitacion)
                .OrderBy(x => x.FechaCambio)
                .Select(x => new EstadoHistorialDto
                {
                    Fecha = x.FechaCambio,
                    IdEstado = x.IdEstadoNuevo,
                    DescripcionEstado = x.EstadoNuevo.Descripcion // Asegúrate de incluir la relación EstadoNuevo en el modelo
                }).ToListAsync();

            return historial;
        }
    }
}
