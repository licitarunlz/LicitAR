using LicitAR.Core.Data;
using LicitAR.Core.Data.Models;
using LicitAR.Core.Utils;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LicitAR.Core.Business.Licitaciones
{
    public interface IEntidadLicitanteManager
    {
        Task<IMessageManager> AgregarAsync(EntidadLicitante entidadLicitante, int idUser);
        Task<IMessageManager> BajaLogicaAsync(int idEntidadLicitante, int idUser);
        Task<IEnumerable<EntidadLicitante>> GetAllEntidadesLicitantesAsync();
        Task<EntidadLicitante> GetEntidadLicitanteByIdAsync(int idEntidadLicitante);
        Task<IMessageManager> ModificarAsync(EntidadLicitante entidadLicitante, int idEntidadLicitante, int idUser);
        Task<IMessageManager> AsociarUsuarioAsync(int idEntidadLicitante, string idUsuario, int idUser);
        Task<IMessageManager> DesasociarUsuarioAsync(int idEntidadLicitante, string idUsuario, int idUser);
    }

    public class EntidadLicitanteManager : IEntidadLicitanteManager
    {
        private IMessageManager _messageManager;
        private readonly LicitARDbContext _dbContext;

        public EntidadLicitanteManager(IMessageManager messageManager, LicitARDbContext dbContext)
        {
            this._messageManager = messageManager;
            this._dbContext = dbContext;
        }

        public async Task<IEnumerable<EntidadLicitante>> GetAllEntidadesLicitantesAsync()
        {
            return this._dbContext.EntidadesLicitantes.Where(x => x.Audit.FechaBaja == null);
        }
        public async Task<EntidadLicitante?> GetEntidadLicitanteByIdAsync(int idEntidadLicitante)
        {
            return await this._dbContext.EntidadesLicitantes.FirstOrDefaultAsync(x => x.IdEntidadLicitante == idEntidadLicitante);
        }
        public async Task<IMessageManager> AgregarAsync(EntidadLicitante entidadLicitante, int idUser)
        {
            try
            {
                entidadLicitante.Audit = AuditHelper.GetCreationData(idUser);
                _dbContext.EntidadesLicitantes.Add(entidadLicitante);
                await _dbContext.SaveChangesAsync();

                _messageManager.OkMessage("Entidad Licitante agregada exitosamente!");
            }
            catch (Exception ex)
            {
                _messageManager.ClearMessages();
                _messageManager.ErrorMessage("Error al intentar actualizar la base de datos " + ex.ToString());
            }

            return _messageManager;
        }
        public async Task<IMessageManager> ModificarAsync(EntidadLicitante entidadLicitante, int idEntidadLicitante, int idUser)
        {
            try
            {
                var entidadFromDdbb = await _dbContext.EntidadesLicitantes.FirstOrDefaultAsync(x => x.IdEntidadLicitante == idEntidadLicitante);
                if (entidadFromDdbb == null)
                {
                    _messageManager.ErrorMessage("Entidad Licitante no encontrada");
                }
                else
                {
                    entidadFromDdbb.Cuit = entidadLicitante.Cuit;
                    entidadFromDdbb.RazonSocial = entidadLicitante.RazonSocial;
                    entidadFromDdbb.IdLocalidad = entidadLicitante.IdLocalidad;
                    entidadFromDdbb.IdProvincia = entidadLicitante.IdProvincia;
                    entidadFromDdbb.DireccionBarrio = entidadLicitante.DireccionBarrio;
                    entidadFromDdbb.DireccionCalle = entidadLicitante.DireccionCalle;
                    entidadFromDdbb.DireccionNumero = entidadLicitante.DireccionNumero;
                    entidadFromDdbb.DireccionPiso = entidadLicitante.DireccionPiso;
                    entidadFromDdbb.DireccionDepto = entidadLicitante.DireccionDepto;
                    entidadFromDdbb.DireccionCodigoPostal = entidadLicitante.DireccionCodigoPostal;

                    entidadFromDdbb.Audit = AuditHelper.SetModificationData(entidadFromDdbb.Audit, idUser);

                    _dbContext.EntidadesLicitantes.Update(entidadFromDdbb);

                    await _dbContext.SaveChangesAsync();

                    _messageManager.OkMessage("Entidad Licitante modificada exitosamente");


                }
            }
            catch (Exception ex)
            {
                _messageManager.ClearMessages();
                _messageManager.ErrorMessage("Excepcion al modificar la entidad licitante: Ex - " + ex.ToString());
            }
            return _messageManager;


        }
        public async Task<IMessageManager> BajaLogicaAsync(int idEntidadLicitante, int idUser)
        {
            try
            {
                var entidadLicitante = await _dbContext.EntidadesLicitantes.FirstOrDefaultAsync(x => x.IdEntidadLicitante == idEntidadLicitante);
                if (entidadLicitante == null)
                {
                    _messageManager.ErrorMessage("Entidad Licitante no encontrada");
                }
                else
                {
                    entidadLicitante.Audit = AuditHelper.SetDeletionData(entidadLicitante.Audit, idUser);

                    _dbContext.EntidadesLicitantes.Update(entidadLicitante);
                    await _dbContext.SaveChangesAsync();

                    _messageManager.OkMessage("Entidad Licitante eliminada exitosamente");


                }
            }
            catch (Exception ex)
            {
                _messageManager.ClearMessages();
                _messageManager.ErrorMessage("Excepcion al eliminar la entidad licitante: Ex - " + ex.ToString());

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

                _dbContext.Add(association);
                await _dbContext.SaveChangesAsync();

                _messageManager.OkMessage("Usuario asociado exitosamente.");
            }
            catch (Exception ex)
            {
                _messageManager.ErrorMessage("Error al asociar usuario: " + ex.Message);
            }

            return _messageManager;
        }

        public async Task<IMessageManager> DesasociarUsuarioAsync(int idEntidadLicitante, string idUsuario, int idUser)
        {
            try
            {
                var association = await _dbContext.Set<EntidadLicitanteUsuario>()
                    .FirstOrDefaultAsync(eu => eu.IdEntidadLicitante == idEntidadLicitante && eu.IdUsuario == idUsuario);

                if (association != null)
                {
                    _dbContext.Remove(association);
                    await _dbContext.SaveChangesAsync();

                    _messageManager.OkMessage("Usuario desasociado exitosamente.");
                }
                else
                {
                    _messageManager.ErrorMessage("Asociación no encontrada.");
                }
            }
            catch (Exception ex)
            {
                _messageManager.ErrorMessage("Error al desasociar usuario: " + ex.Message);
            }

            return _messageManager;
        }
    }
}
