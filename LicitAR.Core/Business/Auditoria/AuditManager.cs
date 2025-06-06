using LicitAR.Core.Data;
using LicitAR.Core.Data.Models;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace LicitAR.Core.Business.Auditoria
{
    public interface IAuditManager
    {
        Task LogSystemEvent(AuditTrail trail);
        Task LogSystemEvent(int usuarioId, string accion, string entidad, int? entidadId, string descripcion, string ip, string userAgent);
        Task LogLicitacionChange(AuditLicitacion audit);
        Task LogLicitacionChange(int idLicitacion, int usuarioId, string accion, string? campo, string? valorAnterior, string? valorNuevo);

        Task<(List<AuditTrail> Items, int Total)> GetAuditTrailsAsync(
            string accion, int? usuarioId, string entidad, DateTime? desde, DateTime? hasta, 
            HashSet<int>? usuarioIdsFilter, int page, int pageSize);

        Task<(List<AuditLicitacion> Items, int Total)> GetAuditLicitacionesAsync(
            string accion, int? usuarioId, int? idLicitacion, string campo, DateTime? desde, DateTime? hasta,
            HashSet<int>? usuarioIdsFilter, int page, int pageSize);
    }

    public class AuditManager : IAuditManager
    {
        private readonly LicitARDbContext _dbContext;

        public AuditManager(LicitARDbContext dbContext)
        {
            _dbContext = dbContext;
        }

        public async Task LogSystemEvent(AuditTrail trail)
        {
            _dbContext.AuditTrails.Add(trail);
            await _dbContext.SaveChangesAsync();
        }

        public async Task LogSystemEvent(int usuarioId, string accion, string entidad, int? entidadId, string descripcion, string ip, string userAgent)
        {
            var trail = new AuditTrail
            {
                UsuarioId = usuarioId,
                FechaHora = DateTime.UtcNow,
                Accion = accion,
                Entidad = entidad,
                EntidadId = entidadId,
                Descripcion = descripcion,
                IpCliente = ip,
                UserAgent = userAgent
            };
            await LogSystemEvent(trail);
        }

        public async Task LogLicitacionChange(AuditLicitacion audit)
        {
            _dbContext.AuditLicitaciones.Add(audit);
            await _dbContext.SaveChangesAsync();
        }

        public async Task LogLicitacionChange(int idLicitacion, int usuarioId, string accion, string? campo, string? valorAnterior, string? valorNuevo)
        {
            var audit = new AuditLicitacion
            {
                IdLicitacion = idLicitacion,
                FechaHora = DateTime.UtcNow,
                UsuarioId = usuarioId,
                Accion = accion,
                CampoModificado = campo,
                ValorAnterior = valorAnterior,
                ValorNuevo = valorNuevo
            };
            await LogLicitacionChange(audit);
        }

        public async Task<(List<AuditTrail> Items, int Total)> GetAuditTrailsAsync(
            string accion, int? usuarioId, string entidad, DateTime? desde, DateTime? hasta, 
            HashSet<int>? usuarioIdsFilter, int page, int pageSize)
        {
            var query = _dbContext.AuditTrails.AsQueryable();

            if (!string.IsNullOrEmpty(accion))
                query = query.Where(x => x.Accion.Contains(accion));
            if (usuarioId.HasValue)
                query = query.Where(x => x.UsuarioId == usuarioId.Value);
            if (!string.IsNullOrEmpty(entidad))
                query = query.Where(x => x.Entidad.Contains(entidad));
            if (desde.HasValue)
                query = query.Where(x => x.FechaHora >= desde.Value);
            if (hasta.HasValue)
                query = query.Where(x => x.FechaHora <= hasta.Value);
            if (usuarioIdsFilter != null)
                query = query.Where(x => usuarioIdsFilter.Contains(x.UsuarioId));

            query = query.OrderByDescending(x => x.FechaHora);

            var total = await query.CountAsync();
            var items = await query
                .Skip((page - 1) * pageSize)
                .Take(pageSize)
                .ToListAsync();

            return (items, total);
        }

        public async Task<(List<AuditLicitacion> Items, int Total)> GetAuditLicitacionesAsync(
            string accion, int? usuarioId, int? idLicitacion, string campo, DateTime? desde, DateTime? hasta,
            HashSet<int>? usuarioIdsFilter, int page, int pageSize)
        {
            var query = _dbContext.AuditLicitaciones
                .Include(x => x.Licitacion)
                .AsQueryable();

            if (!string.IsNullOrEmpty(accion))
                query = query.Where(x => x.Accion.Contains(accion));
            if (usuarioId.HasValue)
                query = query.Where(x => x.UsuarioId == usuarioId.Value);
            if (idLicitacion.HasValue)
                query = query.Where(x => x.IdLicitacion == idLicitacion.Value);
            if (!string.IsNullOrEmpty(campo))
                query = query.Where(x => x.CampoModificado.Contains(campo));
            if (desde.HasValue)
                query = query.Where(x => x.FechaHora >= desde.Value);
            if (hasta.HasValue)
                query = query.Where(x => x.FechaHora <= hasta.Value);
            if (usuarioIdsFilter != null)
                query = query.Where(x => usuarioIdsFilter.Contains(x.UsuarioId));

            query = query.OrderByDescending(x => x.FechaHora);

            var total = await query.CountAsync();
            var items = await query
                .Skip((page - 1) * pageSize)
                .Take(pageSize)
                .ToListAsync();

            return (items, total);
        }
    }
}
