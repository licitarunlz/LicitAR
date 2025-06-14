namespace LicitAR.Web.ViewModels.Dashboard
{
    public class AdminDashboardViewModel
    {
        public int TotalLicitaciones { get; set; }
        public int TotalLicitacionesActivas { get; set; }
        public int TotalAdjudicaciones { get; set; }
        public int LicitacionesActivas { get; set; }
        public int Adjudicaciones { get; set; }

        public decimal PorcentajeLicitacionesActivasVsMesAnterior { get; set; }
        public decimal PorcentajeAdjudicacionesVsMesAnterior { get; set; }

        // Método utilitario para obtener icono y color según rubro
        public static (string icon, string color) GetIconAndColorForRubro(string? rubro)
        {
            switch (rubro)
            {
                case "Bienes - Equipamiento Informático":
                    return ("bx-laptop", "bg-label-info");
                case "Bienes - Vehículos":
                    return ("bx-car", "bg-label-primary");
                case "Bienes - Mobiliario":
                    return ("bx-chair", "bg-label-warning");
                case "Bienes - Medicamentos":
                    return ("bx-capsule", "bg-label-success");
                case "Bienes - Materiales de Construcción":
                    return ("bx-building-house", "bg-label-secondary");
                case "Bienes - Textiles / Indumentaria":
                    return ("bx-user", "bg-label-danger");
                case "Bienes - Insumos Médicos y Hospitalarios":
                    return ("bx-first-aid", "bg-label-success");
                case "Bienes - Material Didáctico / Escolar":
                    return ("bx-book-open", "bg-label-info");
                case "Bienes - Equipos de Laboratorio":
                    return ("bx-test-tube", "bg-label-secondary");
                case "Bienes - Herramientas y Equipos Menores":
                    return ("bx-wrench", "bg-label-warning");
                case "Servicios - Limpieza":
                    return ("bx-trash", "bg-label-success");
                case "Servicios - Seguridad":
                    return ("bx-shield", "bg-label-danger");
                case "Servicios - Capacitación":
                    return ("bx-book", "bg-label-info");
                case "Servicios - Desarrollo de Software":
                    return ("bx-code-alt", "bg-label-primary");
                case "Servicios - Consultoría Técnica":
                    return ("bx-user-voice", "bg-label-warning");
                case "Servicios - Mantenimiento General":
                    return ("bx-wrench", "bg-label-secondary");
                case "Servicios - Alquiler de Equipos":
                    return ("bx-cube", "bg-label-info");
                case "Servicios - Atención Médica / Profesional":
                    return ("bx-capsule", "bg-label-success");
                case "Servicios - Emergencias / Ambulancias":
                    return ("bx-ambulance", "bg-label-danger");
                case "Servicios - Esterilización / Laboratorio":
                    return ("bx-droplet", "bg-label-info");
                case "Servicios - Editoriales / Impresiones":
                    return ("bx-printer", "bg-label-secondary");
                case "Servicios - Actividades Culturales / Recreativas":
                    return ("bx-music", "bg-label-warning");
                case "Servicios - Recolección / Gestión de Residuos":
                    return ("bx-trash", "bg-label-success");
                case "Servicios - Control Ambiental / Monitoreo":
                    return ("bx-leaf", "bg-label-success");
                case "Servicios - Investigación / Desarrollo":
                    return ("bx-search-alt", "bg-label-info");
                case "Servicios - Recursos Humanos":
                    return ("bx-group", "bg-label-primary");
                case "Servicios - Eventos y Protocolo":
                    return ("bx-calendar-event", "bg-label-warning");
                case "Servicios - Traducción / Interpretación":
                    return ("bx-globe", "bg-label-info");
                case "Servicios - Publicidad":
                    return ("bx-calendar", "bg-label-danger");
                case "Obras - Viales":
                    return ("bx-traffic-cone", "bg-label-warning");
                case "Obras - Edilicias":
                    return ("bx-building", "bg-label-primary");
                case "Obras - Hidráulicas":
                    return ("bx-water", "bg-label-info");
                case "Obras - Infraestructura Escolar":
                    return ("bx-building", "bg-label-success");
                case "Obras - Energía y Electricidad":
                    return ("bx-bolt-circle", "bg-label-warning");
                case "Alimentos - No Perecederos":
                    return ("bx-basket", "bg-label-success");
                case "Alimentos - Catering / Comedores":
                    return ("bx-restaurant", "bg-label-danger");
                case "Papelería - Útiles y Oficina":
                    return ("bx-pencil", "bg-label-info");
                case "Transporte - Logística y Envíos":
                    return ("bx-truck", "bg-label-primary");
                case "Tecnología - Comunicaciones y Redes":
                    return ("bx-wifi", "bg-label-info");
                case "Arrendamientos - Inmuebles":
                    return ("bx-home", "bg-label-secondary");
                default:
                    return ("bx-folder", "bg-label-dark"); // Por defecto
            }
        }

        public class Licitacion
        {
            public string? Rubro { get; set; }
            public string? Titulo { get; set; }
            public decimal MontoEstimado { get; set; }
            public string? Nombre { get; set; }       

            public string IconClass => AdminDashboardViewModel.GetIconAndColorForRubro(Rubro).icon;
            public string ColorClass => AdminDashboardViewModel.GetIconAndColorForRubro(Rubro).color;
        }

        public List<Licitacion> UltimasLicitaciones { get; set; } = new List<Licitacion>();

        public class LicitacionPorSector
        {
            public string? Rubro { get; set; }
            public string? Descripcion { get; set; }
            public int Cantidad { get; set; }
            public string? Nombre { get; set; }      

            public string IconClass => AdminDashboardViewModel.GetIconAndColorForRubro(Rubro).icon;
            public string ColorClass => AdminDashboardViewModel.GetIconAndColorForRubro(Rubro).color;
        }

        public List<LicitacionPorSector> LicitacionesPorSector { get; set; } = new List<LicitacionPorSector>();

        public class LicitacionProximaACerrar
        {
            public string? CodigoLicitacion { get; set; }
            public string? Titulo { get; set; }
            public decimal MontoEstimado { get; set; }
            public DateTime FechaCierre { get; set; }
            public double DiferenciaHoras { get; set; } 
            public string? Rubro { get; set; }
            public string? Nombre { get; set; }      

            public string IconClass => AdminDashboardViewModel.GetIconAndColorForRubro(Rubro).icon;
            public string ColorClass => AdminDashboardViewModel.GetIconAndColorForRubro(Rubro).color;
        }

        public List<LicitacionProximaACerrar> LicitacionesProximasACerrar { get; set; } = new List<LicitacionProximaACerrar>();
    }
}
