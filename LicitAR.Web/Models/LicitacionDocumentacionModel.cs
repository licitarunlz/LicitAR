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

    public class LicitacionChecklistItemModel
    {
        public int IdLicitacionChecklistItem { get; set; }
        public int IdLicitacion { get; set; }
        public string DescripcionItem { get; set; }
        public bool DocumentoObligatorio { get; set; }

        public LicitacionChecklistItem GetLicitacionChecklistItem(AuditTable audit)
        {

            return new LicitacionChecklistItem
            {
                IdLicitacionChecklistItem = this.IdLicitacionChecklistItem,
                IdLicitacion = this.IdLicitacion,
                DescripcionItem = this.DescripcionItem,
                DocumentoObligatorio = this.DocumentoObligatorio,
                OfertasChecklistItems = null,
                Audit = audit,
            };
        }

    }

}
