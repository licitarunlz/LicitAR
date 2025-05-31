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
    public interface IEvaluacionManager
    {
        Task<bool> ConfirmarEvaluacionAsync(int idEvaluacion, DateTime fechaCierre, int idUsuario);
        Task CreateEvaluacionAsync(Evaluacion evaluacion, int userId);
        Task<bool> DeleteEvaluacionAsync(int idEvaluacion, int idUsuario);
        Task<List<Evaluacion>> GetAllActiveEvaluacionesAsync();
        Task<List<Evaluacion>> GetAllEvaluacionesAsync();
        Task<Evaluacion?> GetEvaluacionByIdAsync(int id);
        Task<bool> UpdateEvaluacionAsync(Evaluacion evaluacion, int userId);
        Task<Evaluacion?> GetEvaluacionByLicitacionAsync(int idLicitacion);
    }

    public class EvaluacionManager : IEvaluacionManager
    {
        private LicitARDbContext _dbContext;

        public EvaluacionManager(LicitARDbContext licitARDbContext)
        {
            _dbContext = licitARDbContext;
        }


        public async Task<List<Evaluacion>> GetAllEvaluacionesAsync()
        {
            return await _dbContext.Evaluaciones
                .Include(e=> e.EstadoEvaluacion)
                .Include(l => l.EvaluacionOfertasDetalles) // Include EstadoLicitacion
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
                .Include(l => l.EvaluacionOfertas) // Include EstadoLicitacion
                .Include(l => l.EstadoEvaluacion) // Include CategoriaLicitacion
                .Include(l=> l.EvaluacionOfertasDetalles)
                .FirstOrDefaultAsync(l => l.IdEvaluacion == id);
        }



        public async Task<Evaluacion?> GetEvaluacionByLicitacionAsync(int idLicitacion)
        {
            return await _dbContext.Evaluaciones
                .Include(l => l.EvaluacionOfertas) // Include EstadoLicitacion
                .Include(l => l.EstadoEvaluacion) // Include CategoriaLicitacion
                .Include(l => l.EvaluacionOfertasDetalles)
                .FirstOrDefaultAsync(l => l.IdLicitacion == idLicitacion);
        }
        public async Task CreateEvaluacionAsync(Evaluacion evaluacion, int userId)
        {
            try
            {
                evaluacion.FechaInicioEvaluacion = DateTime.Now;
                evaluacion.FechaFinEvaluacion = null;//las primeras arrancan en estado "Borrador" y dps se confirman
                evaluacion.IdEstadoEvaluacion = 1; // Default state: Planificación
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
                var evaluacionFromDdbb = await _dbContext.Evaluaciones.FirstOrDefaultAsync(x => x.IdEvaluacion == evaluacion.IdEvaluacion);
                if (evaluacionFromDdbb == null)
                    return false;


                evaluacionFromDdbb.Audit = AuditHelper.SetModificationData(evaluacionFromDdbb.Audit, userId);

                evaluacionFromDdbb.EvaluacionOfertas = evaluacion.EvaluacionOfertas;
                evaluacionFromDdbb.EvaluacionOfertasDetalles = evaluacion.EvaluacionOfertasDetalles;

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
