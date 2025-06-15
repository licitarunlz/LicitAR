using DocumentFormat.OpenXml.Spreadsheet;
using LicitAR.Core.Data;
using LicitAR.Core.Data.Models;
using LicitAR.Core.Utils;
using LicitAR.FileStorage;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LicitAR.Core.Business.Documentacion
{
    public interface ILicitacionDocumentacionManager
    {
        Task AddLicitacionDocumentacionAsync(int idLicitacion, LicitacionDocumentacion licitacionDocumentacion, IFormFile formFile, int idUsuario);
        Task<List<LicitacionDocumentacion>> GetAllDocumentacionByIdLicitacionAsync(int idLicitacion);
        Task<LicitacionDocumentacion?> GetByIdAsync(int id);
        Task RemoveLicitacionDocumentacionAsync(int idLicitacionDocumentacion, int idUsuario);
    }

    public class LicitacionDocumentacionManager : ILicitacionDocumentacionManager
    {
        private LicitARDbContext _context;
        private IBlobStorageService _blobStorageService;

        public LicitacionDocumentacionManager(LicitARDbContext context, IBlobStorageService blobStorageService)
        {
            _context = context;
            _blobStorageService = blobStorageService;
        }


        public async Task<List<LicitacionDocumentacion>> GetAllDocumentacionByIdLicitacionAsync(int idLicitacion)
        {
            return await _context.LicitacionDocumentacion
                           .Where(x => x.Audit.FechaBaja == null && x.IdLicitacion == idLicitacion)
                           .ToListAsync();
        }
        public async Task<LicitacionDocumentacion?> GetByIdAsync(int id)
        {
            return await _context.LicitacionDocumentacion
                           .FirstOrDefaultAsync(x => x.IdLicitacionDocumentacion == id );
        }
        public async Task AddLicitacionDocumentacionAsync(int idLicitacion, LicitacionDocumentacion licitacionDocumentacion, IFormFile formFile, int idUsuario)
        {
            try
            {
                licitacionDocumentacion.FechaCarga = DateTime.Now;
                licitacionDocumentacion.Audit = AuditHelper.GetCreationData(idUsuario);
                licitacionDocumentacion.BlobUri = await _blobStorageService.SubirArchivoAsync(formFile);
                licitacionDocumentacion.NombreArchivoOriginal = formFile.FileName;
                licitacionDocumentacion.IdLicitacionDocumentacion = 0;
                _context.LicitacionDocumentacion.Add(licitacionDocumentacion);


                await _context.SaveChangesAsync();
            }
            catch
            {
                throw;
            }

        }
        public async Task RemoveLicitacionDocumentacionAsync(int idLicitacionDocumentacion, int idUsuario)
        {
            try
            {
                var licitacionDocumentacion = await this.GetByIdAsync(idLicitacionDocumentacion);

                if (licitacionDocumentacion == null)
                    return;

                licitacionDocumentacion.Audit = AuditHelper.SetDeletionData(licitacionDocumentacion.Audit, idUsuario);
                _context.LicitacionDocumentacion.Update(licitacionDocumentacion);

                await _context.SaveChangesAsync();
            }
            catch
            {
                throw;
            }
        }
    }
}
