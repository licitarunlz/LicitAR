using LicitAR.Core.Data;
using LicitAR.Core.Data.Models;
using LicitAR.Core.Data.Models.Parametros;
using LicitAR.Core.Utils;
using Microsoft.EntityFrameworkCore;

namespace LicitAR.Core.Business.Licitaciones
{
    public interface IEntidadLicitanteManager
    {
        Task<EntidadLicitante?> GetEntidadLicitanteByIdAsync(int idEntidadLicitante);
        Task<List<EntidadLicitante>> GetAllEntidadesLicitantesAsync();
        Task<List<EntidadLicitante>> GetAllActiveEntidadesLicitantesAsync();
        Task<Provincia?> GetProvinciaByIdAsync(int idProvincia);
        Task<Localidad?> GetLocalidadByIdAsync(int idLocalidad);
        Task<IMessageManager> AgregarAsync(EntidadLicitante entidadLicitante, int idUser);
        Task<IMessageManager> ModificarAsync(EntidadLicitante entidadLicitante, int idEntidadLicitante, int idUser);
        Task<IMessageManager> DeleteEntidadLicitanteAsync(int idEntidadLicitante, int idUser);
        Task<IMessageManager> AsociarUsuarioAsync(int idEntidadLicitante, string idUsuario, int idUser);
        Task<IMessageManager> DesasociarUsuarioAsync(int idEntidadLicitante, string idUsuario, int idUser);
    }

    public class EntidadLicitanteManager : IEntidadLicitanteManager
    {
        private readonly LicitARDbContext _dbContext;
        private readonly IMessageManager _messageManager;

        public EntidadLicitanteManager(LicitARDbContext dbContext, IMessageManager messageManager)
        {
            _dbContext = dbContext;
            _messageManager = messageManager;
        }

        public async Task<EntidadLicitante?> GetEntidadLicitanteByIdAsync(int idEntidadLicitante)
        {
            return await _dbContext.EntidadesLicitantes
                .Include(e => e.Provincia)
                .Include(e => e.Localidad)
                .FirstOrDefaultAsync(e => e.IdEntidadLicitante == idEntidadLicitante);
        }

        public async Task<List<EntidadLicitante>> GetAllEntidadesLicitantesAsync()
        {
            return await _dbContext.EntidadesLicitantes
                .Include(e => e.Provincia)
                .Include(e => e.Localidad)
                .ToListAsync();
        }

        public async Task<List<EntidadLicitante>> GetAllActiveEntidadesLicitantesAsync()
        {
            return await _dbContext.EntidadesLicitantes
                .Where(e => e.Audit.FechaBaja == null)
                .Include(e => e.Provincia)
                .Include(e => e.Localidad)
                .ToListAsync();
        }

        public async Task<Provincia?> GetProvinciaByIdAsync(int idProvincia)
        {
            return await _dbContext.Provincias.FirstOrDefaultAsync(p => p.IdProvincia == idProvincia);
        }

        public async Task<Localidad?> GetLocalidadByIdAsync(int idLocalidad)
        {
            return await _dbContext.Localidades.FirstOrDefaultAsync(l => l.IdLocalidad == idLocalidad);
        }

        public async Task<IMessageManager> AgregarAsync(EntidadLicitante entidadLicitante, int idUser)
        {
            try
            {
                entidadLicitante.Audit = AuditHelper.GetCreationData(idUser);
                _dbContext.EntidadesLicitantes.Add(entidadLicitante);
                await _dbContext.SaveChangesAsync();
                _messageManager.OkMessage("Entidad Licitante agregada exitosamente.");
            }
            catch (Exception ex)
            {
                _messageManager.ErrorMessage($"Error al agregar la Entidad Licitante: {ex.Message}");
            }
            return _messageManager;
        }

        public async Task<IMessageManager> ModificarAsync(EntidadLicitante entidadLicitante, int idEntidadLicitante, int idUser)
        {
            try
            {
                var entidadFromDb = await _dbContext.EntidadesLicitantes.FirstOrDefaultAsync(e => e.IdEntidadLicitante == idEntidadLicitante);
                if (entidadFromDb == null)
                {
                    _messageManager.ErrorMessage("Entidad Licitante no encontrada.");
                    return _messageManager;
                }

                entidadFromDb.Cuit = entidadLicitante.Cuit;
                entidadFromDb.RazonSocial = entidadLicitante.RazonSocial;
                entidadFromDb.IdProvincia = entidadLicitante.IdProvincia;
                entidadFromDb.IdLocalidad = entidadLicitante.IdLocalidad;
                entidadFromDb.DireccionBarrio = entidadLicitante.DireccionBarrio;
                entidadFromDb.DireccionCalle = entidadLicitante.DireccionCalle;
                entidadFromDb.DireccionNumero = entidadLicitante.DireccionNumero;
                entidadFromDb.DireccionPiso = entidadLicitante.DireccionPiso;
                entidadFromDb.DireccionDepto = entidadLicitante.DireccionDepto;
                entidadFromDb.DireccionCodigoPostal = entidadLicitante.DireccionCodigoPostal;
                entidadFromDb.Audit = AuditHelper.SetModificationData(entidadFromDb.Audit, idUser);

                _dbContext.EntidadesLicitantes.Update(entidadFromDb);
                await _dbContext.SaveChangesAsync();
                _messageManager.OkMessage("Entidad Licitante modificada exitosamente.");
            }
            catch (Exception ex)
            {
                _messageManager.ErrorMessage($"Error al modificar la Entidad Licitante: {ex.Message}");
            }
            return _messageManager;
        }

        public async Task<IMessageManager> DeleteEntidadLicitanteAsync(int idEntidadLicitante, int idUser)
        {
            try
            {
                var entidadFromDb = await _dbContext.EntidadesLicitantes.FirstOrDefaultAsync(e => e.IdEntidadLicitante == idEntidadLicitante);
                if (entidadFromDb == null)
                {
                    _messageManager.ErrorMessage("Entidad Licitante no encontrada.");
                    return _messageManager;
                }
                entidadFromDb.Enabled = false;
                entidadFromDb.Audit = AuditHelper.SetDeletionData(entidadFromDb.Audit, idUser);
                _dbContext.EntidadesLicitantes.Update(entidadFromDb);
                await _dbContext.SaveChangesAsync();
                _messageManager.OkMessage("Entidad Licitante eliminada exitosamente.");
            }
            catch (Exception ex)
            {
                _messageManager.ErrorMessage($"Error al eliminar la Entidad Licitante: {ex.Message}");
            }
            return _messageManager;
        }

        public async Task<IMessageManager> AsociarUsuarioAsync(int idEntidadLicitante, string idUsuario, int idUser)
        {
            try
            {
                var association = new EntidadLicitanteUsuario
                {
                    IdEntidadLicitante = idEntidadLicitante,
                    IdUsuario = idUsuario,
                    Audit = AuditHelper.GetCreationData(idUser)
                };

                _dbContext.EntidadLicitanteUsuarios.Add(association);
                await _dbContext.SaveChangesAsync();
                _messageManager.OkMessage("Usuario asociado exitosamente.");
            }
            catch (Exception ex)
            {
                _messageManager.ErrorMessage($"Error al asociar usuario: {ex.Message}");
            }
            return _messageManager;
        }

        public async Task<IMessageManager> DesasociarUsuarioAsync(int idEntidadLicitante, string idUsuario, int idUser)
        {
            try
            {
                var association = await _dbContext.EntidadLicitanteUsuarios
                    .FirstOrDefaultAsync(eu => eu.IdEntidadLicitante == idEntidadLicitante && eu.IdUsuario == idUsuario);

                if (association == null)
                {
                    _messageManager.ErrorMessage("Asociación no encontrada.");
                    return _messageManager;
                }

                _dbContext.EntidadLicitanteUsuarios.Remove(association);
                await _dbContext.SaveChangesAsync();
                _messageManager.OkMessage("Usuario desasociado exitosamente.");
            }
            catch (Exception ex)
            {
                _messageManager.ErrorMessage($"Error al desasociar usuario: {ex.Message}");
            }
            return _messageManager;
        }
    }
}
