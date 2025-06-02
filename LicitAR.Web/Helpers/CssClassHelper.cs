using LicitAR.Core.Data.Models.Parametros;

namespace LicitAR.Web.Helpers
{
    public static class CssClassHelper
    {
        public static string GetLicitacionBagdeCssClassByIdEstado(int idEstadoLicitacion)
        {
            string result = "";

            switch (idEstadoLicitacion)
            {
                case 1: //Planificación
                    result = "badge bg-label-secondary";
                    break;
                case 3: //Publicado
                    result = "badge bg-label-primary";
                    break;
                case 6: //Impugnada
                    result = "badge bg-label-danger";
                    break;
                case 7: //Evaluacion
                    result = "badge bg-label-dark";
                    break;
                case 8: //Desistida
                    result = "badge bg-label-danger";
                    break;
                case 9: //Adjudicacion
                    result = "badge bg-label-success";
                    break;
                case 13: //Desierta
                    result = "badge bg-label-warning";
                    break;
                case 14: //Cancelada
                    result = "badge bg-label-danger";
                    break;
                default:
                    result = "badge bg-label-secondary";
                    break;
            }

            return result;
        }
        public static string GetLicitacionAlertCssClassByIdEstado(int idEstadoLicitacion)
        {
            string result = "";
            switch (idEstadoLicitacion)
            {
                case 1: //Planificación
                    result = "alert alert-secondary";
                    break;
                case 3: //Publicado
                    result = "alert alert-primary";
                    break;
                case 6: //Impugnada
                    result = "alert alert-danger";
                    break;
                case 7: //Evaluacion
                    result = "alert alert-dark";
                    break;
                case 8: //Desistida
                    result = "alert alert-danger";
                    break;
                case 9: //Adjudicacion
                    result = "alert alert-success";
                    break;
                case 13: //Desierta
                    result = "alert alert-warning";
                    break;
                case 14: //Cancelada
                    result = "alert alert-danger";
                    break;
                default:
                    result = "alert alert-secondary";
                    break;
            }

            return result;

        }

        public static string GetOfertaAlertCssClassByIdEstado(int idEstadoOferta)
        {
            string result = "";
            switch (idEstadoOferta)
            {
                 
                case 1: //Borrador
                    result = "badge bg-label-secondary";
                    break;
                case 2: //Publicada
                    result = "badge bg-label-success";
                    break;
                case 3: //Cancelada
                    result = "badge bg-label-danger";
                    break;
                
                default:
                    result = "badge bg-label-secondary";
                    break;
            
        }

            return result;

        }
        public static string GetOfertaBagdeCssClassByIdEstado(int idEstadoOferta)
        {
            string result = "";
            switch (idEstadoOferta)
            {

                case 1: //Borrador
                    result = "alert alert-secondary";
                    break;
                case 2: //Publicado
                    result = "alert alert-success";
                    break;
                case 3: //Cancelado
                    result = "alert alert-danger";
                    break;

                default:
                    result = "alert alert-secondary";
                    break;
            }

            return result;

        }
        public static string GetEvaluacionAlertCssClassByIdEstado(int idEstadoEvaluacion)
        {
            string result = "";
            switch (idEstadoEvaluacion)
            {
                case 1:
                    result = "";
                    break;
                default:
                    result = "";
                    break;
            }
            return result;

        }
        public static string GetEvaluacionBagdeCssClassByIdEstado(int idEstadoEvaluacion)
        {
            string result = "";
            switch (idEstadoEvaluacion)
            {
                case 1:
                    result = "";
                    break;
                default:
                    result = "";
                    break;
            }
            return result;
        }
    }
}
