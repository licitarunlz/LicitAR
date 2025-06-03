namespace LicitAR.Web.Helpers
{
    public static class StringFormatHelper
    {

        public static string FormatearCuitSeguro(string cuit, string razonSocial)
        {
            if (string.IsNullOrWhiteSpace(cuit) && string.IsNullOrWhiteSpace(razonSocial))
                return "[S/D] - S/D";

            string cuitFormateado;
            if (string.IsNullOrWhiteSpace(cuit))
            {
                cuitFormateado = "S/D";
            }
            else if (cuit.Length == 11 && long.TryParse(cuit, out _))
            {
                cuitFormateado = $"{cuit.Substring(0, 2)}-{cuit.Substring(2, 8)}-{cuit.Substring(10, 1)}";
            }
            else
            {
                cuitFormateado = cuit;
            }

            string razon = string.IsNullOrWhiteSpace(razonSocial) ? "S/D" : razonSocial;

            return $"[{cuitFormateado}] - {razon}";
        }

    }
}
