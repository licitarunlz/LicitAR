using LicitAR.Core.Data.Models.Helpers;
using LicitAR.Core.Data.Models;
using LicitAR.Core.Utils;
using Microsoft.EntityFrameworkCore;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel;

namespace LicitAR.Web.Models
{
    [PrimaryKey("IdPersona")]
    public class PersonaModel : WithDireccion
    {
        [Required(ErrorMessage = ErrorMessages.REQUIRED)]
        public int IdPersona { get; set; }

        [Required(ErrorMessage = ErrorMessages.REQUIRED)]
        [MaxLength(30, ErrorMessage = ErrorMessages.MAXLENGTH)]
        public string? Cuit { get; set; }

        [Required(ErrorMessage = ErrorMessages.REQUIRED)]
        [DisplayName("Tipo Persona")]
        public int IdTipoPersona { get; set; }

        [Required(ErrorMessage = ErrorMessages.REQUIRED)]
        [DisplayName("Rubro")]
        public int? IdRubro { get; set; }

        [Required(ErrorMessage = ErrorMessages.REQUIRED)]
        [MaxLength(200, ErrorMessage = ErrorMessages.MAXLENGTH)]

        [DisplayName("Razón Social")]
        public string? RazonSocial { get; set; }


        [EmailAddress(ErrorMessage = ErrorMessages.VALIDEMAIL)]
        [Required(ErrorMessage = ErrorMessages.REQUIRED)]
        [MaxLength(30, ErrorMessage = ErrorMessages.MAXLENGTH)]
        public string? Email { get; set; }

        [Required(ErrorMessage = ErrorMessages.REQUIRED)]
        [MaxLength(30, ErrorMessage = ErrorMessages.MAXLENGTH)]
        public string? Telefono { get; set; }


        public string? Rubro { get; set; }
        public string? TipoPersona { get; set; }
        public string? Provincia { get; set; }
        public string? Localidad { get; set; }



        public Persona GetPersona(AuditTable audit)
        {
            return new Persona
            {
                Email = this.Email,
                IdPersona = this.IdPersona,
                IdTipoPersona = this.IdTipoPersona,
                Telefono = this.Telefono,
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
                IdProvincia = this.IdProvincia,
                IdRubro = this.IdRubro
            };
        }

        public void SetPersonaData(Persona persona)
        {
            this.IdPersona = persona.IdPersona;
            this.Cuit = persona.Cuit;
            this.RazonSocial = persona.RazonSocial;
            this.DireccionBarrio = persona.DireccionBarrio;
            this.DireccionCalle = persona.DireccionCalle;
            this.DireccionCodigoPostal = persona.DireccionCodigoPostal;
            this.DireccionDepto = persona.DireccionDepto;
            this.DireccionNumero = persona.DireccionNumero;
            this.DireccionPiso = persona.DireccionPiso;
            this.IdLocalidad = persona.IdLocalidad;
            this.IdProvincia = persona.IdProvincia;
            this.Email = persona.Email;
            this.IdPersona = persona.IdPersona;
            this.IdTipoPersona = persona.IdTipoPersona;
            this.Telefono = persona.Telefono;
            this.Provincia = persona.Provincia.Descripcion;
            this.Localidad = persona.Localidad.Descripcion;
            this.TipoPersona = persona.TipoPersona.Descripcion;
            this.IdRubro = persona.IdRubro;
            this.Rubro = persona.Rubro.Descripcion;
        }
    }
}

