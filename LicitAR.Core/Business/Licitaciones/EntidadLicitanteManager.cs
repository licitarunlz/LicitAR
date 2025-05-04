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
    }

    public class EntidadLicitanteManager : IEntidadLicitanteManager
    {
        private IMessageManager _messageManager;
        private ActoresDbContext _actoresDbContext;

        public EntidadLicitanteManager(IMessageManager messageManager, ActoresDbContext actoresDbContext)
        {
            this._messageManager = messageManager;
            this._actoresDbContext = actoresDbContext;
        }

        public async Task<IEnumerable<EntidadLicitante>> GetAllEntidadesLicitantesAsync()
        {
            return this._actoresDbContext.EntidadesLicitantes.Where(x => x.Audit.FechaBaja == null);
        }
        public async Task<EntidadLicitante?> GetEntidadLicitanteByIdAsync(int idEntidadLicitante)
        {
            return await this._actoresDbContext.EntidadesLicitantes.FirstOrDefaultAsync(x => x.IdEntidadLicitante == idEntidadLicitante);
        }
        public async Task<IMessageManager> AgregarAsync(EntidadLicitante entidadLicitante, int idUser)
        {
            try
            {
                entidadLicitante.Audit = AuditHelper.GetCreationData(idUser);
                _actoresDbContext.EntidadesLicitantes.Add(entidadLicitante);
                await _actoresDbContext.SaveChangesAsync();

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
                var entidadFromDdbb = await _actoresDbContext.EntidadesLicitantes.FirstOrDefaultAsync(x => x.IdEntidadLicitante == idEntidadLicitante);
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

                    _actoresDbContext.EntidadesLicitantes.Update(entidadFromDdbb);

                    await _actoresDbContext.SaveChangesAsync();

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
                var entidadLicitante = await _actoresDbContext.EntidadesLicitantes.FirstOrDefaultAsync(x => x.IdEntidadLicitante == idEntidadLicitante);
                if (entidadLicitante == null)
                {
                    _messageManager.ErrorMessage("Entidad Licitante no encontrada");
                }
                else
                {
                    entidadLicitante.Audit = AuditHelper.SetDeletionData(entidadLicitante.Audit, idUser);

                    _actoresDbContext.EntidadesLicitantes.Update(entidadLicitante);
                    await _actoresDbContext.SaveChangesAsync();

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
    }
}
