using LicitAR.Core.Data;
using LicitAR.Core.Data.Models;
using Microsoft.EntityFrameworkCore;
using System.Threading.Tasks;

namespace LicitAR.Core.Business.Auditoria
{
    public interface IAuditManager
    {
        Task LogSystemEvent(AuditTrail trail);
        Task LogSystemEvent(int usuarioId, string accion, string entidad, int? entidadId, string descripcion, string ip, string userAgent);
        Task LogLicitacionChange(AuditLicitacion audit);
        Task LogLicitacionChange(int idLicitacion, int usuarioId, string accion, string? campo, string? valorAnterior, string? valorNuevo);
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
                FechaHora = DateTime.Now,
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
                FechaHora = DateTime.Now,
                UsuarioId = usuarioId,
                Accion = accion,
                CampoModificado = campo,
                ValorAnterior = valorAnterior,
                ValorNuevo = valorNuevo
            };
            await LogLicitacionChange(audit);
        }
    }
}
