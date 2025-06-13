using LicitAR.Core.Data;
using Microsoft.EntityFrameworkCore;

namespace LicitAR.Core.Business.Dashboard
{
    public class AdminDashboardDto
    {
        public int TotalLicitaciones { get; set; }
        public int LicitacionesActivas { get; set; }
        public int Adjudicaciones { get; set; }

    }

    public class OferenteDashboardDto
    {
        public int LicitacionesDisponibles { get; set; }
        public int LicitacionesEnCurso { get; set; }
        public int AdjudicacionesGanadas { get; set; }

    }

    public class EntidadLicitanteDashboardDto
    {
        public int LicitacionesPublicadas { get; set; }
        public int LicitacionesEnEvaluacion { get; set; }
        public int AdjudicacionesRealizadas { get; set; }

    }

    public interface IDashboardManager
    {
        Task<AdminDashboardDto> GetAdminDashboardAsync();
        Task<OferenteDashboardDto> GetOferenteDashboardAsync(int idOferente);
        Task<EntidadLicitanteDashboardDto> GetEntidadLicitanteDashboardAsync(int idEntidad);
    }

    public class DashboardManager : IDashboardManager
    {
        private readonly LicitARDbContext _dbContext;
        public DashboardManager(LicitARDbContext dbContext)
        {
            _dbContext = dbContext;
        }

        public async Task<AdminDashboardDto> GetAdminDashboardAsync()
        {
            var totalLicitaciones = await _dbContext.Licitaciones.CountAsync();
            var licitacionesActivas = await _dbContext.Licitaciones.CountAsync(l => l.IdEstadoLicitacion == 3);
            var adjudicaciones = await _dbContext.Licitaciones.CountAsync(l => l.IdEstadoLicitacion == 10);
            return new AdminDashboardDto
            {
                TotalLicitaciones = totalLicitaciones,
                LicitacionesActivas = licitacionesActivas,
                Adjudicaciones = adjudicaciones
            };
        }

        public async Task<OferenteDashboardDto> GetOferenteDashboardAsync(int idOferente)
        {
            var licitacionesDisponibles = await _dbContext.Licitaciones.CountAsync(l => l.IdEstadoLicitacion == 3);
            var licitacionesEnCurso = await _dbContext.Ofertas.CountAsync(o => o.IdPersona == idOferente && o.IdEstadoOferta == 1);
            var adjudicacionesGanadas = await _dbContext.Ofertas.CountAsync(o => o.IdPersona == idOferente && o.IdEstadoOferta == 2);
            return new OferenteDashboardDto
            {
                LicitacionesDisponibles = licitacionesDisponibles,
                LicitacionesEnCurso = licitacionesEnCurso,
                AdjudicacionesGanadas = adjudicacionesGanadas
            };
        }

        public async Task<EntidadLicitanteDashboardDto> GetEntidadLicitanteDashboardAsync(int idEntidad)
        {
            var licitacionesPublicadas = await _dbContext.Licitaciones.CountAsync(l => l.IdEntidadLicitante == idEntidad);
            var licitacionesEnEvaluacion = await _dbContext.Licitaciones.CountAsync(l => l.IdEntidadLicitante == idEntidad && l.IdEstadoLicitacion == 7);
            var adjudicacionesRealizadas = await _dbContext.Licitaciones.CountAsync(l => l.IdEntidadLicitante == idEntidad && l.IdEstadoLicitacion == 10);
            return new EntidadLicitanteDashboardDto
            {
                LicitacionesPublicadas = licitacionesPublicadas,
                LicitacionesEnEvaluacion = licitacionesEnEvaluacion,
                AdjudicacionesRealizadas = adjudicacionesRealizadas
            };
        }
    }
}
