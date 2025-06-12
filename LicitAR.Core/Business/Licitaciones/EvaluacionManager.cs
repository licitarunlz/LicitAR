using LicitAR.Core.Data;
using LicitAR.Core.Data.Models;
using LicitAR.Core.Data.Models.Historial;
using LicitAR.Core.Services;
using LicitAR.Core.Utils;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Protocols.OpenIdConnect;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LicitAR.Core.Business.Licitaciones
{
    public interface IEvaluacionManager
    {
        Task<bool> ConfirmarEvaluacionAsync(int idEvaluacion, DateTime fechaCierre, int idUsuario);
        Task CreateEvaluacionAsync(Evaluacion evaluacion, int userId);
        Task<bool> DeleteEvaluacionAsync(int idEvaluacion, int idUsuario);
        Task<bool> ResultadoEvaluacionAsync(int idEvaluacion, int idEstadoResultado, int idUsuario, string baseUrl);
        Task<List<Evaluacion>> GetAllActiveEvaluacionesAsync();
        Task<List<Evaluacion>> GetAllEvaluacionesAsync();
        Task<Evaluacion?> GetEvaluacionByIdAsync(int id);
        Task<bool> UpdateEvaluacionAsync(Evaluacion evaluacion, int userId);
        Task<Evaluacion?> GetEvaluacionByLicitacionAsync(int idLicitacion);
    }

    public class EvaluacionManager : IEvaluacionManager
    {
        private LicitARDbContext _dbContext;
        private readonly ILicitacionNotificationService _licitacionNotificationService;

        public EvaluacionManager(LicitARDbContext licitARDbContext, ILicitacionNotificationService licitacionNotificationService)
        {
            _dbContext = licitARDbContext;
            _licitacionNotificationService = licitacionNotificationService;
        }

        public async Task<List<Evaluacion>> GetAllEvaluacionesAsync()
        {
            return await _dbContext.Evaluaciones
                .Include(e => e.EstadoEvaluacion)
                .Include(e => e.Licitacion)
                .Include(e => e.Licitacion.EstadoLicitacion)
                .Include(l => l.EvaluacionOfertasDetalles)
                .ToListAsync();
        }

        public async Task<List<Evaluacion>> GetAllActiveEvaluacionesAsync()
        {
            return await _dbContext.Evaluaciones.Where(x => x.Audit.FechaBaja == null)
                .Include(l => l.EvaluacionOfertas).ToListAsync();
        }

        public async Task<Evaluacion?> GetEvaluacionByIdAsync(int id)
        {
            return await _dbContext.Evaluaciones
                .Include(l => l.EvaluacionOfertas)
                .Include(l => l.EstadoEvaluacion)
                .Include(l => l.EvaluacionOfertasDetalles)
                .FirstOrDefaultAsync(l => l.IdEvaluacion == id);
        }

        public async Task<Evaluacion?> GetEvaluacionByLicitacionAsync(int idLicitacion)
        {
            return await _dbContext.Evaluaciones
                .Include(l => l.EvaluacionOfertas)
                .Include(l => l.EstadoEvaluacion)
                .Include(l => l.EvaluacionOfertasDetalles)
                .FirstOrDefaultAsync(l => l.IdLicitacion == idLicitacion);
        }

        public async Task CreateEvaluacionAsync(Evaluacion evaluacion, int userId)
        {
            try
            {
                evaluacion.FechaInicioEvaluacion = DateTime.Now;
                evaluacion.FechaFinEvaluacion = null;
                evaluacion.IdEstadoEvaluacion = 1;
                evaluacion.Audit = AuditHelper.GetCreationData(userId);
                _dbContext.Evaluaciones.Add(evaluacion);
                foreach (var detalle in evaluacion.EvaluacionOfertasDetalles)
                {
                    detalle.IdEvaluacion = evaluacion.IdEvaluacion;
                    detalle.Audit = AuditHelper.GetCreationData(userId);
                }

                await _dbContext.SaveChangesAsync();
            }
            catch
            {
                throw;
            }
        }

        public async Task<bool> UpdateEvaluacionAsync(Evaluacion evaluacion, int userId)
        {
            try
            {
                var evaluacionFromDdbb = await _dbContext.Evaluaciones.Include(x => x.EvaluacionOfertasDetalles)
                                                .FirstOrDefaultAsync(x => x.IdEvaluacion == evaluacion.IdEvaluacion);
                if (evaluacionFromDdbb == null)
                    return false;

                evaluacionFromDdbb.Audit = AuditHelper.SetModificationData(evaluacionFromDdbb.Audit, userId);
                var _ = evaluacionFromDdbb.EvaluacionOfertasDetalles.Zip(evaluacion.EvaluacionOfertasDetalles, (a, b) =>
                {
                    a.IdOfertaDetalle = b.IdOfertaDetalle;
                    return 0;
                }).ToList();

                _dbContext.Evaluaciones.Update(evaluacionFromDdbb);

                await _dbContext.SaveChangesAsync();
                return true;
            }
            catch (DbUpdateConcurrencyException)
            {
                return _dbContext.Evaluaciones.Any(e => e.IdEvaluacion == evaluacion.IdEvaluacion);
            }
        }

        public async Task<bool> DeleteEvaluacionAsync(int idEvaluacion, int idUsuario)
        {
            var evaluacion = await _dbContext.Evaluaciones.FindAsync(idEvaluacion);
            if (evaluacion == null)
            {
                return false;
            }
            evaluacion.Audit = AuditHelper.SetDeletionData(evaluacion.Audit, idUsuario);

            _dbContext.Evaluaciones.Update(evaluacion);
            await _dbContext.SaveChangesAsync();
            return true;
        }

        public async Task<bool> ResultadoEvaluacionAsync(int idEvaluacion, int idEstadoResultado, int idUsuario, string baseUrl)
        {
            var evaluacion = await _dbContext.Evaluaciones.FindAsync(idEvaluacion);
            if (evaluacion == null)
                return false;

            if (evaluacion.IdEstadoEvaluacion != 1)
                return false;

            evaluacion.IdEstadoEvaluacion = 2;
            evaluacion.FechaFinEvaluacion = DateTime.Now;

            evaluacion.Audit = AuditHelper.SetModificationData(evaluacion.Audit, idUsuario);

            _dbContext.Evaluaciones.Update(evaluacion);

            var licitacion = await _dbContext.Licitaciones.FindAsync(evaluacion.IdLicitacion);
            var estadoAnterior = licitacion.IdEstadoLicitacion;

            licitacion.IdEstadoLicitacion = idEstadoResultado;
            licitacion.Audit = AuditHelper.SetModificationData(licitacion.Audit, idUsuario);

            _dbContext.LicitacionEstadoHistorial.Add(new LicitacionEstadoHistorial
            {
                IdLicitacion = licitacion.IdLicitacion,
                IdEstadoAnterior = estadoAnterior,
                IdEstadoNuevo = idEstadoResultado,
                FechaCambio = DateTime.Now,
                IdUsuarioCambio = idUsuario
            });

            /* if (idEstadoResultado == 6)
            {
                var model = new
                {
                    LicitacionNombre = licitacion.CodigoLicitacion + " - " + licitacion.Titulo,
                    LicitacionId = licitacion.IdLicitacion,
                    Fecha = DateTime.Now,
                    UrlDetalle = baseUrl
                };

                var mail = "pablo.numb@gmail.com";
                if (!string.IsNullOrEmpty(mail))
                {
                    await _licitacionNotificationService.NotificarLicitacionImpugnadaAsync(mail, model);
                }
            }*/
            
            if (idEstadoResultado == 9)
            {
                var model = new
                {
                    LicitacionNombre = licitacion.CodigoLicitacion + " - " + licitacion.Titulo,
                    LicitacionId = licitacion.IdLicitacion,
                    Fecha = DateTime.Now,
                    UrlDetalle = baseUrl
                };

                // Obtener lista de destinatarios desde la base de datos
                var destinatarios = await GetDestinatariosEmailsByEvaluacionAsync(idEvaluacion);

                foreach (var mail in destinatarios.Distinct())
                {
                    if (!string.IsNullOrEmpty(mail))
                    {
                        await _licitacionNotificationService.NotificarLicitacionAdjudicadaAsync(mail, model);
                    }
                }
            }

            await _dbContext.SaveChangesAsync();
            return true;
        }

        /// <summary>
        /// Obtiene los emails de los destinatarios relacionados a la evaluación.
        /// </summary>
        private async Task<List<string>> GetDestinatariosEmailsByEvaluacionAsync(int idEvaluacion)
        {
            // 1. Obtener todos los IdOfertaDetalle de EvaluacionOfertasDetalle para el idEvaluacion
            var idOfertaDetalles = await _dbContext.EvaluacionOfertasDetalle
                .Where(eod => eod.IdEvaluacion == idEvaluacion)
                .Select(eod => eod.IdOfertaDetalle)
                .ToListAsync();

            // 2. Obtener los IdOferta de OfertasDetalle para esos IdOfertaDetalle (distinct)
            var idOfertas = await _dbContext.OfertasDetalle
                .Where(od => idOfertaDetalles.Contains(od.IdOfertaDetalle))
                .Select(od => od.IdOferta)
                .Distinct()
                .ToListAsync();

            // 3. Obtener los IdPersona de Ofertas para esos IdOferta (distinct)
            var idPersonas = await _dbContext.Ofertas
                .Where(o => idOfertas.Contains(o.IdOferta))
                .Select(o => o.IdPersona)
                .Distinct()
                .ToListAsync();

            // 4. Obtener los emails de Persona para esos IdPersona
            var emails = await _dbContext.Personas
                .Where(p => idPersonas.Contains(p.IdPersona))
                .Select(p => p.Email)
                .Where(email => !string.IsNullOrEmpty(email))
                .ToListAsync();

            return emails;
        }

        public async Task<bool> ConfirmarEvaluacionAsync(int idEvaluacion, DateTime fechaCierre, int idUsuario)
        {
            var evaluacion = await _dbContext.Evaluaciones.FindAsync(idEvaluacion);
            if (evaluacion == null)
            {
                return false;
            }
            evaluacion.FechaFinEvaluacion = DateTime.Now;
            evaluacion.IdEstadoEvaluacion = 3;
            evaluacion.Audit = AuditHelper.SetModificationData(evaluacion.Audit, idUsuario);

            _dbContext.Evaluaciones.Update(evaluacion);
            await _dbContext.SaveChangesAsync();
            return true;
        }
    }
}
