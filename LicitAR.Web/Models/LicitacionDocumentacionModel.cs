using LicitAR.Core.Data.Models.Helpers;
using LicitAR.Core.Data.Models.Parametros;
using LicitAR.Core.Data.Models;
using LicitAR.Core.Utils;

namespace LicitAR.Web.Models
{
    public class LicitacionDocumentacionModel
    {
        public int IdLicitacionDocumentacion { get; set; }
        public int IdLicitacion { get; set; }
        
        public string TituloDocumento { get; set; }
        public string BlobUri { get; set; }
        public IFormFile archivo { get; set; }


        public LicitacionDocumentacion GetLicitacionDocumentacion(AuditTable audit)
        {
 
            return new LicitacionDocumentacion
            {
                IdLicitacionDocumentacion = this.IdLicitacionDocumentacion,
                IdLicitacion  = this.IdLicitacion,
                FechaCarga = DateTime.Now,
                NombreArchivoOriginal = archivo.FileName,
                TituloDocumento = this.TituloDocumento,
                Audit = audit,
                Licitacion = null,
                BlobUri = this.BlobUri
            };
        }

    }
}
