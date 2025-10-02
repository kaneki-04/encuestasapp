// Services/ExcelExportService.cs
using ClosedXML.Excel;
using GestorEncuestas_MVC.DTOs;
using System;
using System.Collections.Generic;
using System.IO;

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
            using (var workbook = new XLWorkbook())
            {
                // Hoja de resumen
                var summarySheet = workbook.Worksheets.Add("Resumen");
                summarySheet.Cell(1, 1).Value = "Exportación de Encuesta";
                summarySheet.Cell(1, 1).Style.Font.Bold = true;
                summarySheet.Cell(1, 1).Style.Font.FontSize = 16;

                summarySheet.Cell(3, 1).Value = "Título:";
                summarySheet.Cell(3, 2).Value = encuesta.Titulo;

                summarySheet.Cell(4, 1).Value = "Descripción:";
                summarySheet.Cell(4, 2).Value = encuesta.Descripcion;

                summarySheet.Cell(5, 1).Value = "Estado:";
                summarySheet.Cell(5, 2).Value = encuesta.Estado;

                summarySheet.Cell(6, 1).Value = "Fecha de Cierre:";
                summarySheet.Cell(6, 2).Value = encuesta.CierraEn;

                summarySheet.Cell(7, 1).Value = "Fecha de Creación:";
                summarySheet.Cell(7, 2).Value = encuesta.CreadoEn;

                summarySheet.Cell(8, 1).Value = "Autor:";
                summarySheet.Cell(8, 2).Value = encuesta.Autor;

                summarySheet.Cell(9, 1).Value = "Total de Respuestas:";
                summarySheet.Cell(9, 2).Value = encuesta.TotalRespuestas;

                // Hoja de estadísticas por pregunta
                var statsSheet = workbook.Worksheets.Add("Estadísticas");
                statsSheet.Cell(1, 1).Value = "Pregunta";
                statsSheet.Cell(1, 2).Value = "Tipo";
                statsSheet.Cell(1, 3).Value = "Obligatoria";
                statsSheet.Cell(1, 4).Value = "Opciones";
                statsSheet.Cell(1, 5).Value = "Selecciones";
                statsSheet.Cell(1, 6).Value = "Porcentaje";

                var headerRange = statsSheet.Range(1, 1, 1, 6);
                headerRange.Style.Font.Bold = true;
                headerRange.Style.Fill.BackgroundColor = XLColor.LightGray;

                int row = 2;
                foreach (var pregunta in encuesta.Preguntas)
                {
                    foreach (var opcion in pregunta.Opciones)
                    {
                        statsSheet.Cell(row, 1).Value = pregunta.Enunciado;
                        statsSheet.Cell(row, 2).Value = pregunta.TipoPregunta;
                        statsSheet.Cell(row, 3).Value = pregunta.Obligatorio ? "Sí" : "No";
                        statsSheet.Cell(row, 4).Value = opcion.Label;
                        statsSheet.Cell(row, 5).Value = opcion.ConteoSelecciones;
                        
                        if (encuesta.TotalRespuestas > 0)
                        {
                            double porcentaje = (opcion.ConteoSelecciones * 100.0) / encuesta.TotalRespuestas;
                            statsSheet.Cell(row, 6).Value = porcentaje;
                            statsSheet.Cell(row, 6).Style.NumberFormat.Format = "0.00%";
                        }
                        
                        row++;
                    }
                }

                statsSheet.Columns().AdjustToContents();

                // Hoja de respuestas detalladas
                if (encuesta.Preguntas.Any(p => p.Respuestas != null && p.Respuestas.Any()))
                {
                    var detailsSheet = workbook.Worksheets.Add("Respuestas Detalladas");
                    detailsSheet.Cell(1, 1).Value = "Fecha Respuesta";
                    detailsSheet.Cell(1, 2).Value = "Usuario";
                    
                    int col = 3;
                    foreach (var pregunta in encuesta.Preguntas)
                    {
                        detailsSheet.Cell(1, col).Value = pregunta.Enunciado;
                        col++;
                    }

                    headerRange = detailsSheet.Range(1, 1, 1, col - 1);
                    headerRange.Style.Font.Bold = true;
                    headerRange.Style.Fill.BackgroundColor = XLColor.LightGray;

                    // Organizar respuestas por usuario y fecha
                    var respuestasAgrupadas = encuesta.Preguntas
                        .SelectMany(p => p.Respuestas)
                        .GroupBy(r => new { r.FechaRespuesta.Date, r.Usuario })
                        .OrderBy(g => g.Key.Date)
                        .ThenBy(g => g.Key.Usuario);

                    row = 2;
                    foreach (var grupo in respuestasAgrupadas)
                    {
                        detailsSheet.Cell(row, 1).Value = grupo.Key.Date;
                        detailsSheet.Cell(row, 1).Style.DateFormat.Format = "yyyy-mm-dd";
                        detailsSheet.Cell(row, 2).Value = grupo.Key.Usuario;
                        
                        col = 3;
                        foreach (var pregunta in encuesta.Preguntas)
                        {
                            var respuestasUsuario = grupo
                                .Where(r => pregunta.Respuestas.Any(pr => 
                                    pr.FechaRespuesta.Date == grupo.Key.Date && 
                                    pr.Usuario == grupo.Key.Usuario))
                                .Select(r => r.ValorRespuesta)
                                .ToList();

                            if (respuestasUsuario.Any())
                            {
                                detailsSheet.Cell(row, col).Value = string.Join(", ", respuestasUsuario);
                            }
                            
                            col++;
                        }
                        
                        row++;
                    }

                    detailsSheet.Columns().AdjustToContents();
                }

                using (var stream = new MemoryStream())
                {
                    workbook.SaveAs(stream);
                    return stream.ToArray();
                }
            }
        }

        public byte[] ExportMultipleEncuestasToExcel(List<EncuestaExportDTO> encuestas)
        {
            using (var workbook = new XLWorkbook())
            {
                var summarySheet = workbook.Worksheets.Add("Resumen Encuestas");
                
                // Encabezados
                summarySheet.Cell(1, 1).Value = "ID";
                summarySheet.Cell(1, 2).Value = "Título";
                summarySheet.Cell(1, 3).Value = "Estado";
                summarySheet.Cell(1, 4).Value = "Fecha Creación";
                summarySheet.Cell(1, 5).Value = "Fecha Cierre";
                summarySheet.Cell(1, 6).Value = "Autor";
                summarySheet.Cell(1, 7).Value = "Total Respuestas";
                
                var headerRange = summarySheet.Range(1, 1, 1, 7);
                headerRange.Style.Font.Bold = true;
                headerRange.Style.Fill.BackgroundColor = XLColor.LightGray;
                
                // Datos
                int row = 2;
                foreach (var encuesta in encuestas)
                {
                    summarySheet.Cell(row, 1).Value = encuesta.Id;
                    summarySheet.Cell(row, 2).Value = encuesta.Titulo;
                    summarySheet.Cell(row, 3).Value = encuesta.Estado;
                    summarySheet.Cell(row, 4).Value = encuesta.CreadoEn;
                    summarySheet.Cell(row, 5).Value = encuesta.CierraEn;
                    summarySheet.Cell(row, 6).Value = encuesta.Autor;
                    summarySheet.Cell(row, 7).Value = encuesta.TotalRespuestas;
                    row++;
                }
                
                summarySheet.Columns().AdjustToContents();
                
                using (var stream = new MemoryStream())
                {
                    workbook.SaveAs(stream);
                    return stream.ToArray();
                }
            }
        }
    }
}