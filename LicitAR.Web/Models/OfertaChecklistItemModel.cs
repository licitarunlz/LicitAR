using LicitAR.Core.Data.Models.Helpers;
using LicitAR.Core.Data.Models;
using LicitAR.Core.Utils;
using System.ComponentModel.DataAnnotations;

namespace LicitAR.Web.Models
{
    public class OfertaChecklistItemModel
    { 
        public int IdOfertaChecklistItem { get; set; }
         
        public int IdOferta { get; set; }
         
        public int IdLicitacionChecklistItem { get; set; }
         
        public string? BlobUri { get; set; }
        public IFormFile archivo { get; set; }
        public OfertaChecklistItem GetOfertaChecklistItem(AuditTable audit)
        {
            return new OfertaChecklistItem
            {
                Audit = audit,
                BlobUri = this.BlobUri,
                IdLicitacionChecklistItem = this.IdLicitacionChecklistItem,
                IdOferta = this.IdOferta,
                IdOfertaChecklistItem = this.IdOfertaChecklistItem,
                LicitacionChecklistItem = null,
                Oferta = null
            };
        }
 
    }
}
