using ClosedXML.Excel;
using DocumentFormat.OpenXml.Spreadsheet;
using LicitAR.Core.Business.Licitaciones;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LicitAR.Core.Business.Reportes
{
    public interface IReportesManager
    {
        Task<byte[]> ExportarExcelLicitaciones();
        Task<byte[]> ExportarExcelEntidadesLicitantes();
        Task<byte[]> ExportarExcelOferentes();
        Task<byte[]> ExportarExcelOfertas();
    }

    public class ReportesManager : IReportesManager
    {
        private IEvaluacionManager _evaluacionManager;
        private ILicitacionManager _licitacionManager;
        private IEntidadLicitanteManager _entidadLicitanteManager;
        private IOfertaManager _ofertaManager;
        private IPersonaManager _personaManager;

        public ReportesManager(ILicitacionManager licitacionManager,
                               IEntidadLicitanteManager entidadLicitanteManager,
                               IOfertaManager ofertaManager,
                               IPersonaManager personaManager,
                               IEvaluacionManager evaluacionManager
                               )
        {
            _licitacionManager = licitacionManager;
            _entidadLicitanteManager = entidadLicitanteManager;
            _ofertaManager = ofertaManager;
            _personaManager = personaManager;
            _evaluacionManager = evaluacionManager;

        }

        public async Task<byte[]> ExportarExcelLicitaciones()
        {
            using var workbook = new XLWorkbook();
            var worksheet = workbook.Worksheets.Add("Licitaciones");


            worksheet.Cell(1, 1).Value = "Codigo";
            worksheet.Cell(1, 2).Value = "Titulo";
            worksheet.Cell(1, 3).Value = "Descripción";
            worksheet.Cell(1, 4).Value = "Rubro Licitación";
            worksheet.Cell(1, 5).Value = "Entidad Licitante Cuit";
            worksheet.Cell(1, 6).Value = "Entidad Licitante Razón Social";
            worksheet.Cell(1, 7).Value = "Fecha Publicación";
            worksheet.Cell(1, 8).Value = "Fecha de Cierre de ofertas";
            worksheet.Cell(1, 9).Value = "Estado Licitación";
            worksheet.Cell(1, 10).Value = "Categoría Licitación";
            
            var licitaciones = await _licitacionManager.GetAllActiveLicitacionesAsync();
            for (int i = 0; i < licitaciones.Count; i++)
            {
                worksheet.Cell(i + 2, 1).Value = licitaciones[i].CodigoLicitacion;
                worksheet.Cell(i + 2, 2).Value = licitaciones[i].Titulo;
                worksheet.Cell(i + 2, 3).Value = licitaciones[i].Descripcion;
                worksheet.Cell(i + 2, 4).Value = licitaciones[i].Rubro.Descripcion;
                worksheet.Cell(i + 2, 5).Value = licitaciones[i].EntidadLicitante.Cuit;
                worksheet.Cell(i + 2, 6).Value = licitaciones[i].EntidadLicitante.RazonSocial;
                worksheet.Cell(i + 2, 7).Value = licitaciones[i].FechaPublicacion;
                worksheet.Cell(i + 2, 8).Value = licitaciones[i].FechaCierre;
                worksheet.Cell(i + 2, 9).Value = licitaciones[i].EstadoLicitacion.Descripcion;
                worksheet.Cell(i + 2, 10).Value = licitaciones[i].CategoriaLicitacion.Descripcion;
                 
            }

            var headerRange = worksheet.Range("A1", $"J1"); // Ajustá las columnas según el caso
            headerRange.Style.Font.Bold = true;
            headerRange.Style.Font.FontColor= XLColor.White;

            headerRange.Style.Fill.BackgroundColor = XLColor.FromArgb(123, 152, 195);

            headerRange.Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Left;
            worksheet.Columns().AdjustToContents();

            using var stream = new MemoryStream();
            workbook.SaveAs(stream);

            return stream.ToArray();

        }


        public async Task<byte[]> ExportarExcelEntidadesLicitantes()
        {
            using var workbook = new XLWorkbook();
            var worksheet = workbook.Worksheets.Add("Entidades Licitantes");
            worksheet.Cell(1, 1).Value = "Cuit";
            worksheet.Cell(1, 2).Value = "Razón Social";
            worksheet.Cell(1, 3).Value = "Dirección Provincia";
            worksheet.Cell(1, 4).Value = "Dirección Localidad";
            worksheet.Cell(1, 5).Value = "Dirección Barrio";
            worksheet.Cell(1, 6).Value = "Dirección Calle";
            worksheet.Cell(1, 7).Value = "Dirección Número";
            worksheet.Cell(1, 8).Value = "Dirección Piso";
            worksheet.Cell(1, 9).Value = "Dirección Depto";
            worksheet.Cell(1, 10).Value = "Dirección C.P.A.";
            var entidades = await _entidadLicitanteManager.GetAllEntidadesLicitantesAsync();
            for (int i = 0; i < entidades.Count; i++)
            {
                worksheet.Cell(i + 2, 1).Value = entidades[i].Cuit;

                worksheet.Cell(i + 2, 2).Value = entidades[i].RazonSocial;
                worksheet.Cell(i + 2, 3).Value = entidades[i].Provincia.Descripcion;
                worksheet.Cell(i + 2, 4).Value = entidades[i].Localidad.Descripcion;
                worksheet.Cell(i + 2, 5).Value = entidades[i].DireccionBarrio;
                worksheet.Cell(i + 2, 6).Value = entidades[i].DireccionCalle;
                worksheet.Cell(i + 2, 7).Value = entidades[i].DireccionNumero;
                worksheet.Cell(i + 2, 8).Value = entidades[i].DireccionPiso;
                worksheet.Cell(i + 2, 9).Value = entidades[i].DireccionDepto;
                worksheet.Cell(i + 2, 10).Value = entidades[i].DireccionCodigoPostal;

            }

            var headerRange = worksheet.Range("A1", $"J1"); // Ajustá las columnas según el caso
            headerRange.Style.Font.Bold = true;
            headerRange.Style.Font.FontColor = XLColor.White;

            headerRange.Style.Fill.BackgroundColor =  XLColor.FromArgb(123, 152, 195); ;

            headerRange.Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Left;
            worksheet.Columns().AdjustToContents();

            using var stream = new MemoryStream();
            workbook.SaveAs(stream);

            return stream.ToArray();

        }
 
        public async Task<byte[]> ExportarExcelOferentes()
        {

            using var workbook = new XLWorkbook();
            var worksheet = workbook.Worksheets.Add("Oferentes");

            // Ejemplo básico
            worksheet.Cell(1, 1).Value = "Cuit";
            worksheet.Cell(1, 2).Value = "Razón Social";
            worksheet.Cell(1, 3).Value = "Tipo Persona";
            worksheet.Cell(1, 4).Value = "Email";
            worksheet.Cell(1, 5).Value = "Teléfono"; 
            worksheet.Cell(1, 6).Value = "Rubro";
            worksheet.Cell(1, 7).Value = "Dirección Provincia";
            worksheet.Cell(1, 8).Value = "Dirección Localidad";
            worksheet.Cell(1, 9).Value = "Dirección Barrio";
            worksheet.Cell(1, 10).Value = "Dirección Calle";
            worksheet.Cell(1, 11).Value = "Dirección Número"; 
            worksheet.Cell(1, 12).Value = "Dirección Piso"; 
            worksheet.Cell(1, 13).Value = "Dirección Depto"; 
            worksheet.Cell(1, 14).Value = "Dirección C.P.A.";

            var oferentes = (await _personaManager.GetAllPersonasAsync()).ToList();
            for (int i = 0; i < oferentes.Count; i++)
            {
                worksheet.Cell(i + 2, 1).Value = oferentes[i].Cuit;
                worksheet.Cell(i + 2, 2).Value = oferentes[i].RazonSocial;
                worksheet.Cell(i + 2, 3).Value = oferentes[i].TipoPersona.Descripcion;
                worksheet.Cell(i + 2, 4).Value = oferentes[i].Email;
                worksheet.Cell(i + 2, 5).Value = oferentes[i].Telefono; 
                worksheet.Cell(i + 2, 6).Value = oferentes[i].Rubro.Descripcion;
                worksheet.Cell(i + 2, 7).Value = oferentes[i].Provincia.Descripcion;
                worksheet.Cell(i + 2, 8).Value = oferentes[i].Localidad.Descripcion;
                worksheet.Cell(i + 2, 9).Value = oferentes[i].DireccionBarrio;
                worksheet.Cell(i + 2, 10).Value = oferentes[i].DireccionCalle;
                worksheet.Cell(i + 2, 11).Value = oferentes[i].DireccionNumero;
                worksheet.Cell(i + 2, 12).Value = oferentes[i].DireccionPiso;
                worksheet.Cell(i + 2, 13).Value = oferentes[i].DireccionDepto;
                worksheet.Cell(i + 2, 14).Value = oferentes[i].DireccionCodigoPostal;
            }

            var headerRange = worksheet.Range("A1", $"N1"); // Ajustá las columnas según el caso
            headerRange.Style.Font.Bold = true;
            headerRange.Style.Font.FontColor = XLColor.White;

            headerRange.Style.Fill.BackgroundColor = XLColor.FromArgb(123, 152, 195); 

            headerRange.Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Left;
            worksheet.Columns().AdjustToContents();

            using var stream = new MemoryStream();
            workbook.SaveAs(stream);

            return stream.ToArray();
        }

        public async Task<byte[]> ExportarExcelOfertas()
        {
            using var workbook = new XLWorkbook();
            var worksheet = workbook.Worksheets.Add("Ofertas");

            worksheet.Cell(1, 1).Value = "Código Oferta";
            worksheet.Cell(1, 2).Value = "Oferente Cuit";
            worksheet.Cell(1, 3).Value = "Oferente Razón Social";
            worksheet.Cell(1, 4).Value = "Subtotal Oferta";
            worksheet.Cell(1, 5).Value = "Estado Oferta";
            worksheet.Cell(1, 6).Value = "Fecha Alta Oferta";

            worksheet.Cell(1, 7).Value = "Código Licitación";
            worksheet.Cell(1, 8).Value = "Fecha Publicación Licitación";


            var ofertas = await _ofertaManager.GetAllOfertasAsync();
            for (int i = 0; i < ofertas.Count; i++)
            {
                worksheet.Cell(i + 2, 1).Value = ofertas[i].CodigoOfertaLicitacion;
                worksheet.Cell(i + 2, 2).Value = ofertas[i].Persona.Cuit;
                worksheet.Cell(i + 2, 3).Value = ofertas[i].Persona.RazonSocial;
                worksheet.Cell(i + 2, 4).Value = ofertas[i].Items.Sum(x => x.ImporteSubtotal);
                worksheet.Cell(i + 2, 5).Value = ofertas[i].EstadoOferta.Descripcion;
                worksheet.Cell(i + 2, 6).Value = ofertas[i].FechaOferta;
                worksheet.Cell(i + 2, 7).Value = ofertas[i].Licitacion.CodigoLicitacion;
                worksheet.Cell(i + 2, 8).Value = ofertas[i].Licitacion.FechaPublicacion;
            }


            var headerRange = worksheet.Range("A1", $"H1"); // Ajustá las columnas según el caso
            headerRange.Style.Font.Bold = true;
            headerRange.Style.Font.FontColor = XLColor.White;

            headerRange.Style.Fill.BackgroundColor = XLColor.FromArgb(123, 152, 195);

            headerRange.Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Left;
            worksheet.Columns().AdjustToContents();

            using var stream = new MemoryStream();
            workbook.SaveAs(stream);

            return stream.ToArray();

        }
    }
}
