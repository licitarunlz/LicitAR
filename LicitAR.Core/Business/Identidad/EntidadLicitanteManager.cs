using LicitAR.Core.Data;
using LicitAR.Core.Data.Models;
using LicitAR.Core.Utils;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LicitAR.Core.Business.Identidad
{
    public interface IEntidadLicitanteManager
    {
        Task<IMessageManager> AgregarAsync(EntidadLicitante entidadLicitante);
        Task<IMessageManager> BajaLogicaAsync(int idEntidadLicitante);
        Task<IEnumerable<EntidadLicitante>> GetAllEntidadesLicitantesAsync();
        Task<EntidadLicitante> GetEntidadLicitanteByIdAsync(int idEntidadLicitante);
        Task<IMessageManager> ModificarAsync(EntidadLicitante entidadLicitante, int idEntidadLicitante);
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
        public async Task<IMessageManager> AgregarAsync(EntidadLicitante entidadLicitante)
        {
            return _messageManager;
        }
        public async Task<IMessageManager> ModificarAsync(EntidadLicitante entidadLicitante, int idEntidadLicitante)
        {
            return _messageManager;
        }
        public async Task<IMessageManager> BajaLogicaAsync(int idEntidadLicitante)
        {

            return _messageManager;
        }
    }
}
