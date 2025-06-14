using LicitAR.Core.Data;
using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;

namespace LicitAR.Core.Business.Dashboard
{
    public class LicitacionDto
    {
        public string? Nombre { get; set; }
        public string? Rubro { get; set; }
        public string? Titulo { get; set; }
        public decimal MontoEstimado { get; set; }
    }

    public class LicitacionPorSectorDto
    {
        public string? Rubro { get; set; }
        public string? Descripcion { get; set; }
        public int Cantidad { get; set; }
        public string? Nombre { get; set; } 
    }

    public class LicitacionProximaACerrarDto
    {
        public string? CodigoLicitacion { get; set; }
        public string? Titulo { get; set; }
        public decimal MontoEstimado { get; set; }
        public DateTime FechaCierre { get; set; }
        public double DiferenciaHoras { get; set; } 
        public string? Rubro { get; set; }
        public string? Nombre { get; set; }   
    }

    public class AdminDashboardDto
    {
        public int TotalLicitaciones { get; set; }
        public int TotalLicitacionesActivasMesActual { get; set; }
        public int TotalAdjudicacionesMesActual { get; set; }
        public int LicitacionesActivas { get; set; }
        public int Adjudicaciones { get; set; }
        public decimal PorcentajeLicitacionesActivasVsMesAnterior { get; set; }
        public decimal PorcentajeAdjudicacionesVsMesAnterior { get; set; }

        public List<LicitacionDto> UltimasLicitaciones { get; set; } = new List<LicitacionDto>();
        public List<LicitacionPorSectorDto> LicitacionesPorSector { get; set; } = new List<LicitacionPorSectorDto>();
        public List<LicitacionProximaACerrarDto> LicitacionesProximasACerrar { get; set; } = new List<LicitacionProximaACerrarDto>();
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
            var licitacionesActivas = await _dbContext.Licitaciones.CountAsync(l => l.Enabled == true);
            var adjudicaciones = await _dbContext.Licitaciones.CountAsync(l => l.IdEstadoLicitacion == 9);

            // Mes actual
            var now = DateTime.UtcNow;
            var firstDayOfThisMonth = new DateTime(now.Year, now.Month, 1);
            var firstDayOfLastMonth = firstDayOfThisMonth.AddMonths(-1);
            var lastDayOfLastMonth = firstDayOfThisMonth.AddDays(-1);

            // Totales del mes actual
            var totalLicitacionesActivasMesActual = await _dbContext.Licitaciones
                .CountAsync(l => l.Enabled == true  && l.Audit.FechaAlta >= firstDayOfThisMonth && l.Audit.FechaAlta <= now);
            
            var totalAdjudicacionesMesActual = await _dbContext.Licitaciones
                .CountAsync(l => l.IdEstadoLicitacion == 9 && l.Audit.FechaAlta >= firstDayOfThisMonth && l.Audit.FechaAlta <= now);

            // Totales del mes anterior
            var licitacionesActivasMesAnterior = await _dbContext.Licitaciones
                .CountAsync(l => l.Enabled == true && l.Audit.FechaAlta >= firstDayOfLastMonth && l.Audit.FechaAlta <= lastDayOfLastMonth);

            var adjudicacionesMesAnterior = await _dbContext.Licitaciones
                .CountAsync(l => l.IdEstadoLicitacion == 9 && l.Audit.FechaAlta >= firstDayOfLastMonth && l.Audit.FechaAlta <= lastDayOfLastMonth);

            // Porcentajes de variación
            decimal porcentajeLicitacionesActivasVsMesAnterior = 0;
            if (licitacionesActivasMesAnterior > 0)
            {
                porcentajeLicitacionesActivasVsMesAnterior = ((decimal)(totalLicitacionesActivasMesActual - licitacionesActivasMesAnterior) / licitacionesActivasMesAnterior) * 100;
            }

            decimal porcentajeAdjudicacionesVsMesAnterior = 0;
            if (adjudicacionesMesAnterior > 0)
            {
                porcentajeAdjudicacionesVsMesAnterior = ((decimal)(totalAdjudicacionesMesActual - adjudicacionesMesAnterior) / adjudicacionesMesAnterior) * 100;
            }

            // Ultimas licitaciones (por ejemplo, las 5 más recientes)
            var ultimasLicitaciones = await _dbContext.Licitaciones
                .OrderByDescending(l => l.Audit.FechaAlta)
                .Take(5)
                .Select(l => new LicitacionDto
                {
                    Nombre = l.EntidadLicitante != null ? l.EntidadLicitante.RazonSocial : null,
                    Rubro = l.Rubro != null ? l.Rubro.Descripcion : null,
                    Titulo = l.Titulo,
                    MontoEstimado = l.Items.Sum(i => (decimal?)i.PrecioEstimadoUnitario) ?? 0
                })
                .ToListAsync();

            // Licitaciones por sector (agrupadas por rubro)
            var licitacionesPorSector = await _dbContext.Licitaciones
                .Where(l => l.Rubro != null)
                .GroupBy(l => new { l.Rubro.Descripcion })
                .Select(g => new LicitacionPorSectorDto
                {
                    Rubro = g.Key.Descripcion,
                    Descripcion = g.Key.Descripcion,
                    Cantidad = g.Count(),
                    Nombre = g.Key.Descripcion
                })
                .ToListAsync();

            // Licitaciones próximas a cerrar (por ejemplo, las 5 más próximas)
            var licitacionesProximasACerrar = await _dbContext.Licitaciones
                .Where(l => l.FechaCierre > DateTime.UtcNow)
                .OrderBy(l => l.FechaCierre)
                .Take(5)
                // .Select(l => new LicitacionProximaACerrarDto
                // {
                //     Id = l.Id,
                //     Nombre = l.Nombre,
                //     FechaCierre = l.FechaCierre
                //     // Otros campos relevantes
                // })
                .ToListAsync();

            return new AdminDashboardDto
            {
                TotalLicitaciones = totalLicitaciones,
                TotalLicitacionesActivasMesActual = totalLicitacionesActivasMesActual,
                TotalAdjudicacionesMesActual = totalAdjudicacionesMesActual,
                LicitacionesActivas = licitacionesActivas,
                Adjudicaciones = adjudicaciones,
                PorcentajeLicitacionesActivasVsMesAnterior = porcentajeLicitacionesActivasVsMesAnterior,
                PorcentajeAdjudicacionesVsMesAnterior = porcentajeAdjudicacionesVsMesAnterior,
                UltimasLicitaciones = ultimasLicitaciones,
                LicitacionesPorSector = licitacionesPorSector
                //LicitacionesProximasACerrar = licitacionesProximasACerrar
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
