using LicitAR.Core.Data.Models;
using LicitAR.Core.Data.Models.Helpers;
using LicitAR.Core.Utils;
using LicitAR.Web.Helpers;
using Microsoft.EntityFrameworkCore;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;

namespace LicitAR.Web.Models
{

    [PrimaryKey("IdEntidadLicitante")]
    public class EntidadLicitanteModel : WithDireccion
    {
        [Required(ErrorMessage = ErrorMessages.REQUIRED)]
        public int IdEntidadLicitante { get; set; }

        [Required(ErrorMessage = ErrorMessages.REQUIRED)]
        [MaxLength(30, ErrorMessage = ErrorMessages.MAXLENGTH)]
        public string Cuit { get; set; }

        [Required(ErrorMessage = ErrorMessages.REQUIRED)]
        [MaxLength(200, ErrorMessage = ErrorMessages.MAXLENGTH)]

        [DisplayName("Razón Social")]
        public string RazonSocial { get; set; }

        public string? Provincia { get; set; }
        public string? Localidad { get; set; }



        public EntidadLicitante GetEntidadLicitante(AuditTable audit)
        {
            return new EntidadLicitante
            {
                Cuit = this.Cuit,
                Audit = audit,
                RazonSocial = this.RazonSocial,
                DireccionBarrio = this.DireccionBarrio,
                DireccionCalle = this.DireccionCalle,
                DireccionCodigoPostal = this.DireccionCodigoPostal,
                DireccionDepto = this.DireccionDepto,
                DireccionNumero = this.DireccionNumero,
                DireccionPiso = this.DireccionPiso,
                IdLocalidad = this.IdLocalidad,
                IdProvincia = this.IdProvincia
            };
        }

        public void SetEntidadLicitanteData(EntidadLicitante entidadLicitante)
        {
            this.IdEntidadLicitante = entidadLicitante.IdEntidadLicitante;
            this.Cuit = entidadLicitante.Cuit;
            this.RazonSocial = entidadLicitante.RazonSocial;
            this.DireccionBarrio = entidadLicitante.DireccionBarrio;
            this.DireccionCalle = entidadLicitante.DireccionCalle;
            this.DireccionCodigoPostal = entidadLicitante.DireccionCodigoPostal;
            this.DireccionDepto = entidadLicitante.DireccionDepto;
            this.DireccionNumero = entidadLicitante.DireccionNumero;
            this.DireccionPiso = entidadLicitante.DireccionPiso;
            this.IdLocalidad = entidadLicitante.IdLocalidad;
            this.IdProvincia = entidadLicitante.IdProvincia;

        }
    }

}
