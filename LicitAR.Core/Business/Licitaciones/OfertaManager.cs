using LicitAR.Core.Data.Models;
using LicitAR.Core.Data;
using LicitAR.Core.Utils;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;

namespace LicitAR.Core.Business.Licitaciones
{
    public interface IOfertaManager
    {
        Task CreateOfertaAsync(Oferta oferta, int userId);
        Task<List<Oferta>> GetAllOfertasPorLicitacionAsync(int IdLicitacion);
        Task<List<Oferta>> GetAllOfertasPorPersonaAsync(int IdPersona);
        Task<Oferta?> GetOfertaByIdAsync(int idOferta);
    }

    public class OfertaManager : IOfertaManager
    {
        private readonly LicitARDbContext _dbContext;

        public OfertaManager(LicitARDbContext dbContext)
        {
            _dbContext = dbContext;
        }

        public async Task<List<Oferta>> GetAllOfertasPorPersonaAsync(int IdPersona)
        {
            return await _dbContext.Ofertas
                .Where(o => o.IdPersona == IdPersona && o.Audit.FechaBaja == null)
                .Include(o => o.EstadoOferta) // Include EstadoLicitacion
                .Include(o => o.Licitacion)
                .ToListAsync();
        }

        public async Task<List<Oferta>> GetAllOfertasPorLicitacionAsync(int IdLicitacion)
        {
            return await _dbContext.Ofertas
                .Where(o => o.IdLicitacion == IdLicitacion && o.Audit.FechaBaja == null)
                .Include(o => o.EstadoOferta) // Include EstadoLicitacion
                .Include(o => o.Persona)
                .ToListAsync();
        }


        public async Task<Oferta?> GetOfertaByIdAsync(int idOferta)
        {
            return await _dbContext.Ofertas
                .Include(o => o.EstadoOferta) // Include EstadoLicitacion
                .Include(o => o.Persona) // Include CategoriaLicitacion
                .Include(o => o.Licitacion)
                .Include(d => d.Items)  // Include Los items
                .FirstOrDefaultAsync(l => l.IdOferta == idOferta);
        }

        public async Task CreateOfertaAsync(Oferta oferta, int userId)
        {
            try
            {
                oferta.IdEstadoOferta = 1;
                oferta.FechaOferta = DateTime.Now;
                
                oferta.Audit = AuditHelper.GetCreationData(userId);
                _dbContext.Ofertas.Add(oferta);
                foreach (var detalle in oferta.Items)
                {
                    detalle.IdOferta = oferta.IdOferta;
                    detalle.Audit = AuditHelper.GetCreationData(userId);
                }

                await _dbContext.SaveChangesAsync();

            }
            catch
            {
                throw;
            }
        }

    }
}
