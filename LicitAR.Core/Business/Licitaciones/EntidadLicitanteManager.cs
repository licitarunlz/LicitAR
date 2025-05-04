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

        public EntidadLicitanteManager (IMessageManager messageManager, ActoresDbContext actoresDbContext)
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
                _messageManager.ErrorMessage("Error al intnetar actualizar la base de datos " + ex.ToString());
            }

            return _messageManager;
        }
        public async Task<IMessageManager> ModificarAsync(EntidadLicitante entidadLicitante, int idEntidadLicitante, int idUser)
        {
            return _messageManager;
        }
        public async Task<IMessageManager> BajaLogicaAsync(int idEntidadLicitante, int idUser)
        {

            return _messageManager;
        }
    }
}
