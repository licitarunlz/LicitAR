using System.Collections.Generic;
using LicitAR.Core.Data.Models; // Para Persona

namespace LicitAR.Web.Models
{
    public class AssignPersonaToLicitacionViewModel
    {
        public int IdLicitacion { get; set; }
        public string CodigoLicitacion { get; set; }
        public string TituloLicitacion { get; set; }
        public List<Persona> AvailablePersonas { get; set; }
        public List<Persona> AssignedPersonas { get; set; }
    }
}