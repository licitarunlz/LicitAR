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
        Task AddLicitacionChecklistItemAsync(int idLicitacion, LicitacionChecklistItem licitacionChecklistItem, int idUsuario);
        Task<List<LicitacionChecklistItem>> GetAllChecklistItemsByIdLicitacionAsync(int idLicitacion);
        Task<LicitacionChecklistItem?> GetChecklistItemByIdAsync(int id);
        Task RemoveLicitacionChecklistItemAsync(int idLicitacionDocumentacion, int idUsuario);

        Task<List<OfertaChecklistItem>> GetAllOfertaChecklistItemByIdOfertaAsync(int idOferta);
        Task<OfertaChecklistItem?> GetOfertaChecklistItemByIdAsync(int id);

        Task AddOfertaChecklistItemAsync(int idOferta, OfertaChecklistItem ofertaChecklistItem, IFormFile archivo, int idUsuario);
        Task RemoveOfertaChecklistItemAsync(int idOfertaChecklistItem, int idUsuario);
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

        /*DocumentacionAdjunta*/
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

        /*Checklist*/
        public async Task<List<LicitacionChecklistItem>> GetAllChecklistItemsByIdLicitacionAsync(int idLicitacion)
        {
            return await _context.LicitacionChecklistItems
                           .Where(x => x.Audit.FechaBaja == null && x.IdLicitacion == idLicitacion)
                           .ToListAsync();
        }
        public async Task<LicitacionChecklistItem?> GetChecklistItemByIdAsync(int id)
        {
            return await _context.LicitacionChecklistItems
                           .FirstOrDefaultAsync(x => x.IdLicitacionChecklistItem == id);
        }
        public async Task AddLicitacionChecklistItemAsync(int idLicitacion, LicitacionChecklistItem licitacionChecklistItem, int idUsuario)
        {
            try
            {
                _context.LicitacionChecklistItems.Add(licitacionChecklistItem);


                await _context.SaveChangesAsync();
            }
            catch
            {
                throw;
            }

        }
        public async Task RemoveLicitacionChecklistItemAsync(int idLicitacionChecklistItem, int idUsuario)
        {
            try
            {
                var licitacionChecklistItem = await this.GetByIdAsync(idLicitacionChecklistItem);

                if (licitacionChecklistItem == null)
                    return;

                licitacionChecklistItem.Audit = AuditHelper.SetDeletionData(licitacionChecklistItem.Audit, idUsuario);
                _context.LicitacionDocumentacion.Update(licitacionChecklistItem);

                await _context.SaveChangesAsync();
            }
            catch
            {
                throw;
            }
        }


        /*Oferta Item*/
        public async Task<List<OfertaChecklistItem>> GetAllOfertaChecklistItemByIdOfertaAsync(int idOferta)
        {
            return await _context.OfertaChecklistItems
                           .Where(x => x.Audit.FechaBaja == null && x.IdOferta == idOferta)
                           .ToListAsync();
        }
        public async Task<OfertaChecklistItem?> GetOfertaChecklistItemByIdAsync(int id)
        {
            return await _context.OfertaChecklistItems
                           .FirstOrDefaultAsync(x => x.IdOfertaChecklistItem == id);
        }

        public async Task AddOfertaChecklistItemAsync(int idOferta, OfertaChecklistItem ofertaChecklistItem, IFormFile formFile, int idUsuario)
        {
            try
            {
                ofertaChecklistItem.Audit = AuditHelper.GetCreationData(idUsuario);
                ofertaChecklistItem.BlobUri = await _blobStorageService.SubirArchivoAsync(formFile); 
                ofertaChecklistItem.IdOfertaChecklistItem = 0;
                _context.OfertaChecklistItems.Add(ofertaChecklistItem);


                await _context.SaveChangesAsync();
            }
            catch
            {
                throw;
            }

        }

        public async Task RemoveOfertaChecklistItemAsync(int idOfertaChecklistItem, int idUsuario)
        {
            try
            {
                var ofertaChecklistItem = await this.GetOfertaChecklistItemByIdAsync(idOfertaChecklistItem);

                if (ofertaChecklistItem == null)
                    return;

                ofertaChecklistItem.Audit = AuditHelper.SetDeletionData(ofertaChecklistItem.Audit, idUsuario);
                _context.OfertaChecklistItems.Update(ofertaChecklistItem);

                await _context.SaveChangesAsync();
            }
            catch
            {
                throw;
            }
        }

    }
}
