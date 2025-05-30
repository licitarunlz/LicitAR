using LicitAR.Core.Data.Models;
using LicitAR.Core.Data;
using LicitAR.Core.Utils;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using System.Data;

namespace LicitAR.Core.Business.Licitaciones
{
    public interface IOfertaManager
    {
        Task CreateOfertaAsync(Oferta oferta, int userId);
        Task<bool> UpdateOfertaAsync(Oferta oferta, int userId);
        Task<bool> PublicarOfertaAsync(int idOferta, int userId);
        Task<bool> CancelarOfertaAsync(int id, int idUsuario);
        Task<List<Oferta>> GetAllOfertasPorLicitacionAsync(int IdLicitacion);
        Task<List<Oferta>> GetAllOfertasPorPersonaAsync(int IdPersona);
        Task<Oferta?> GetOfertaByIdAsync(int idOferta);
        Task<List<OferenteResumen>> GetOferentesPorLicitacionAsync(int idLicitacion);
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
                .Include(o => o.Persona)
                .ToListAsync();

        }

        public async Task<List<Oferta>> GetAllOfertasPorLicitacionAsync(int IdLicitacion)
        {
            return await _dbContext.Ofertas
                .Where(o => o.IdLicitacion == IdLicitacion && o.Audit.FechaBaja == null)
                .Include(o => o.EstadoOferta) // Include EstadoLicitacion
                .Include(o => o.Persona)
                .Include(o=> o.Items)
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

                oferta.CodigoOfertaLicitacion = await this.ObtenerProximoCodigoOfertaAsync(oferta.IdLicitacion);
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

        public async Task<bool> UpdateOfertaAsync(Oferta oferta, int userId)
        {
            try
            {
                var ofertaFromDdbb = await _dbContext.Ofertas.FirstOrDefaultAsync(x => x.IdOferta == oferta.IdOferta);
                if (ofertaFromDdbb == null)
                    return false;

                ofertaFromDdbb.IdEstadoOferta = oferta.IdEstadoOferta;
                ofertaFromDdbb.FechaOferta = oferta.FechaOferta;
                
                ofertaFromDdbb.Audit = AuditHelper.SetModificationData(ofertaFromDdbb.Audit, userId);
                
                ofertaFromDdbb.Items = oferta.Items;

                _dbContext.Ofertas.Update(ofertaFromDdbb);

                await _dbContext.SaveChangesAsync();
                return true;
            }
            catch (DbUpdateConcurrencyException)
            {
                return _dbContext.Ofertas.Any(e => e.IdOferta == oferta.IdOferta);
            }
        }
       
        public async Task<bool> PublicarOfertaAsync(int idOferta, int userId) {
            try
            {
                var ofertaFromDdbb = _dbContext.Ofertas.ToList().FirstOrDefault(x => x.IdOferta == idOferta);
                if (ofertaFromDdbb == null)
                    return false;

                ofertaFromDdbb.IdEstadoOferta = 2;
                ofertaFromDdbb.FechaOferta = DateTime.Now;

                ofertaFromDdbb.Audit = AuditHelper.SetModificationData(ofertaFromDdbb.Audit, userId);
                                
                _dbContext.Ofertas.Update(ofertaFromDdbb);

                await _dbContext.SaveChangesAsync();
                return true;
            }
            catch (DbUpdateConcurrencyException)
            {
                return _dbContext.Ofertas.Any(e => e.IdOferta == idOferta);
            }
        }

        public async Task<bool> CancelarOfertaAsync(int id, int idUsuario)
        {
            var oferta = _dbContext.Ofertas.ToList().FirstOrDefault(x=> x.IdOferta == id);
            if (oferta == null)
            {
                return false;
            }
            oferta.Audit = AuditHelper.SetModificationData(oferta.Audit, idUsuario);
            oferta.IdEstadoOferta = 3;

            _dbContext.Ofertas.Update(oferta);
            await _dbContext.SaveChangesAsync();
            return true;
        }

        public async Task<List<OferenteResumen>> GetOferentesPorLicitacionAsync(int idLicitacion)
        {
            return await _dbContext.Ofertas
                .Where(o => o.IdLicitacion == idLicitacion && o.Audit.FechaBaja == null)
                .Include(o => o.Persona)
                .Include(o => o.EstadoOferta)
                .Select(o => new OferenteResumen
                {
                    IdPersona = o.IdPersona,
                    RazonSocial = o.Persona.RazonSocial,
                    FechaOferta = o.FechaOferta,
                    EstadoOferta = o.EstadoOferta != null ? o.EstadoOferta.Descripcion : "",
                    IdOferta = o.IdOferta
                })
                .ToListAsync();
        }

        private async Task<string?> ObtenerProximoCodigoOfertaAsync(int idLicitacion)
        {
            var connection = _dbContext.Database.GetDbConnection();
            if (connection.State != ConnectionState.Open)
                await connection.OpenAsync();

            using (var command = connection.CreateCommand())
            {
                command.CommandText = "sp_ObtenerProximoCodigoOfertaLicitacion";
                command.CommandType = CommandType.StoredProcedure;

                var param = command.CreateParameter();
                param.ParameterName = "@pIdLicitacion";
                param.Value = idLicitacion;
                param.DbType = DbType.Int32;
                command.Parameters.Add(param);


                var result = await command.ExecuteScalarAsync();
                // No cierres la conexi�n manualmente
                return result?.ToString();
            }
        }
    }

    public class OferenteResumen
    {
        public int IdPersona { get; set; }
        public string? RazonSocial { get; set; }
        public DateTime FechaOferta { get; set; }
        public string? EstadoOferta { get; set; }
        public int IdOferta { get; set; }
    }
}
