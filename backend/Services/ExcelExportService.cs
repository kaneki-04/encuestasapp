// Services/ExcelExportService.cs
using ClosedXML.Excel;
using GestorEncuestas_MVC.DTOs;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace GestorEncuestas_MVC.Services
{
    public interface IExcelExportService
    {
        byte[] ExportEncuestaToExcel(EncuestaExportDTO encuesta);
        byte[] ExportMultipleEncuestasToExcel(List<EncuestaExportDTO> encuestas);
    }

    public class ExcelExportService : IExcelExportService
    {
        public byte[] ExportEncuestaToExcel(EncuestaExportDTO encuesta)
        {
            if (encuesta == null) throw new ArgumentNullException(nameof(encuesta));

            using (var workbook = new XLWorkbook())
            {
                AddResumenSheet(workbook, encuesta);
                AddEstadisticasSheet(workbook, encuesta);

                if (encuesta.Preguntas != null && encuesta.Preguntas.Any(p => p.Respuestas != null && p.Respuestas.Any()))
                    AddRespuestasSheet(workbook, encuesta);

                using (var stream = new MemoryStream())
                {
                    workbook.SaveAs(stream);
                    stream.Position = 0; // 🔹 Crucial para que Excel abra el archivo correctamente
                    return stream.ToArray();
                }
            }
        }

        public byte[] ExportMultipleEncuestasToExcel(List<EncuestaExportDTO> encuestas)
        {
            if (encuestas == null) throw new ArgumentNullException(nameof(encuestas));

            using (var workbook = new XLWorkbook())
            {
                var sheet = workbook.Worksheets.Add("Resumen Encuestas");

                string[] headers = { "ID", "Título", "Estado", "Fecha Creación", "Fecha Cierre", "Autor", "Total Respuestas" };
                for (int i = 0; i < headers.Length; i++)
                    sheet.Cell(1, i + 1).Value = headers[i];

                var headerRange = sheet.Range(1, 1, 1, headers.Length);
                headerRange.Style.Font.Bold = true;
                headerRange.Style.Fill.BackgroundColor = XLColor.LightGray;
                headerRange.Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Center;
                headerRange.Style.Border.OutsideBorder = XLBorderStyleValues.Thin;

                int row = 2;
                foreach (var e in encuestas)
                {
                    sheet.Cell(row, 1).Value = e.Id;
                    sheet.Cell(row, 2).Value = e.Titulo;
                    sheet.Cell(row, 3).Value = e.Estado;
                    sheet.Cell(row, 4).Value = e.CreadoEn;
                    sheet.Cell(row, 4).Style.DateFormat.Format = "yyyy-MM-dd";
                    sheet.Cell(row, 5).Value = e.CierraEn;
                    sheet.Cell(row, 5).Style.DateFormat.Format = "yyyy-MM-dd";
                    sheet.Cell(row, 6).Value = e.Autor;
                    sheet.Cell(row, 7).Value = e.TotalRespuestas;
                    row++;
                }

                sheet.Columns().AdjustToContents();

                using (var stream = new MemoryStream())
                {
                    workbook.SaveAs(stream);
                    stream.Position = 0;
                    return stream.ToArray();
                }
            }
        }

        #region Private Methods

        private void AddResumenSheet(XLWorkbook workbook, EncuestaExportDTO encuesta)
        {
            var sheet = workbook.Worksheets.Add(SanitizeSheetName("Resumen"));
            sheet.Cell(1, 1).Value = "Exportación de Encuesta";
            sheet.Cell(1, 1).Style.Font.Bold = true;
            sheet.Cell(1, 1).Style.Font.FontSize = 16;

            sheet.Cell(3, 1).Value = "Título:";
            sheet.Cell(3, 2).Value = encuesta.Titulo;

            sheet.Cell(4, 1).Value = "Descripción:";
            sheet.Cell(4, 2).Value = encuesta.Descripcion;

            sheet.Cell(5, 1).Value = "Estado:";
            sheet.Cell(5, 2).Value = encuesta.Estado;

            sheet.Cell(6, 1).Value = "Fecha de Cierre:";
            sheet.Cell(6, 2).Value = encuesta.CierraEn;
            sheet.Cell(6, 2).Style.DateFormat.Format = "yyyy-MM-dd";

            sheet.Cell(7, 1).Value = "Fecha de Creación:";
            sheet.Cell(7, 2).Value = encuesta.CreadoEn;
            sheet.Cell(7, 2).Style.DateFormat.Format = "yyyy-MM-dd";

            sheet.Cell(8, 1).Value = "Autor:";
            sheet.Cell(8, 2).Value = encuesta.Autor;

            sheet.Cell(9, 1).Value = "Total de Respuestas:";
            sheet.Cell(9, 2).Value = encuesta.TotalRespuestas;

            sheet.Columns().AdjustToContents();
        }

        private void AddEstadisticasSheet(XLWorkbook workbook, EncuestaExportDTO encuesta)
        {
            if (encuesta.Preguntas == null) return;

            var sheet = workbook.Worksheets.Add(SanitizeSheetName("Estadísticas"));

            string[] headers = { "Pregunta", "Tipo", "Obligatoria", "Opciones", "Selecciones", "Porcentaje" };
            for (int i = 0; i < headers.Length; i++)
                sheet.Cell(1, i + 1).Value = headers[i];

            var headerRange = sheet.Range(1, 1, 1, headers.Length);
            headerRange.Style.Font.Bold = true;
            headerRange.Style.Fill.BackgroundColor = XLColor.LightGray;
            headerRange.Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Center;

            int row = 2;
            foreach (var pregunta in encuesta.Preguntas)
            {
                if (pregunta.Opciones == null) continue;

                foreach (var opcion in pregunta.Opciones)
                {
                    sheet.Cell(row, 1).Value = pregunta.Enunciado;
                    sheet.Cell(row, 2).Value = pregunta.TipoPregunta;
                    sheet.Cell(row, 3).Value = pregunta.Obligatorio ? "Sí" : "No";
                    sheet.Cell(row, 4).Value = opcion.Label;
                    sheet.Cell(row, 5).Value = opcion.ConteoSelecciones;

                    if (encuesta.TotalRespuestas > 0)
                    {
                        double porcentaje = opcion.ConteoSelecciones / (double)encuesta.TotalRespuestas;
                        sheet.Cell(row, 6).Value = porcentaje;
                        sheet.Cell(row, 6).Style.NumberFormat.Format = "0.00%";
                    }
                    row++;
                }
            }

            sheet.Columns().AdjustToContents();
        }

        private void AddRespuestasSheet(XLWorkbook workbook, EncuestaExportDTO encuesta)
        {
            var sheet = workbook.Worksheets.Add(SanitizeSheetName("Respuestas Detalladas"));

            sheet.Cell(1, 1).Value = "Fecha Respuesta";
            sheet.Cell(1, 2).Value = "Usuario";

            int col = 3;
            foreach (var pregunta in encuesta.Preguntas)
                sheet.Cell(1, col++).Value = pregunta.Enunciado;

            var headerRange = sheet.Range(1, 1, 1, col - 1);
            headerRange.Style.Font.Bold = true;
            headerRange.Style.Fill.BackgroundColor = XLColor.LightGray;
            headerRange.Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Center;

            var respuestasAgrupadas = encuesta.Preguntas
                .SelectMany(p => p.Respuestas)
                .GroupBy(r => new { r.FechaRespuesta.Date, r.Usuario })
                .OrderBy(g => g.Key.Date)
                .ThenBy(g => g.Key.Usuario);

            int row = 2;
            foreach (var grupo in respuestasAgrupadas)
            {
                sheet.Cell(row, 1).Value = grupo.Key.Date;
                sheet.Cell(row, 1).Style.DateFormat.Format = "yyyy-MM-dd";
                sheet.Cell(row, 2).Value = grupo.Key.Usuario;

                col = 3;
                foreach (var pregunta in encuesta.Preguntas)
                {
                    var respuestasUsuario = grupo
                        .Where(r => r.PreguntaId == pregunta.Id)
                        .Select(r => r.ValorRespuesta)
                        .Where(v => !string.IsNullOrEmpty(v))
                        .ToList();

                    if (respuestasUsuario.Any())
                        sheet.Cell(row, col).Value = string.Join(", ", respuestasUsuario);

                    col++;
                }
                row++;
            }

            sheet.Columns().AdjustToContents();
        }

        private string SanitizeSheetName(string name)
        {
            string clean = new string(name.Where(ch => !Path.GetInvalidFileNameChars().Contains(ch)).ToArray());
            return clean.Length > 31 ? clean.Substring(0, 31) : clean;
        }

        #endregion
    }
}
