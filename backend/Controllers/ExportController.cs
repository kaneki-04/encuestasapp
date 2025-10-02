// Controllers/ExportController.cs
using ClosedXML.Excel;
using GestorEncuestas_MVC.Data;
using GestorEncuestas_MVC.DTOs;
using GestorEncuestas_MVC.Models;
using GestorEncuestas_MVC.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace GestorEncuestas_MVC.Controllers
{
    [ApiController]
    [Route("api/export")]
    [Authorize]
    public class ExportController : ControllerBase
    {
        private readonly ApplicationDbContext _context;
        private readonly IExcelExportService _excelExportService;

        public ExportController(ApplicationDbContext context, IExcelExportService excelExportService)
        {
            _context = context;
            _excelExportService = excelExportService;
        }

        // GET: api/export/encuesta/5/excel
        [HttpGet("encuesta/{id}/excel")]
        public async Task<IActionResult> ExportEncuestaToExcel(int id)
        {
            try
            {
                var encuesta = await _context.Encuestas
                    .Include(e => e.Autor)
                    .Include(e => e.Preguntas)
                        .ThenInclude(p => p.Opciones)
                    .Include(e => e.Respuestas)
                        .ThenInclude(r => r.SeleccionOpcion)
                    .Include(e => e.Respuestas)
                        .ThenInclude(r => r.Usuario)
                    .Include(e => e.Respuestas)
                        .ThenInclude(r => r.RespuestasOpciones)
                            .ThenInclude(ro => ro.Opcion)
                    .FirstOrDefaultAsync(e => e.Id == id);

                if (encuesta == null)
                {
                    return NotFound($"Encuesta con ID {id} no encontrada.");
                }

                // Mapear a DTO de exportación
                var encuestaExport = MapToExportDTO(encuesta);

                // Generar Excel
                var excelData = _excelExportService.ExportEncuestaToExcel(encuestaExport);

                // Devolver como archivo descargable
                return File(excelData, 
                           "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet", 
                           $"Encuesta_{encuesta.Titulo}_{DateTime.Now:yyyyMMddHHmmss}.xlsx");
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Error al exportar la encuesta: {ex.Message}");
            }
        }

        // POST: api/export/encuestas/excel
        [HttpPost("encuestas/excel")]
        public async Task<IActionResult> ExportEncuestasToExcel([FromBody] List<int> encuestaIds)
        {
            try
            {
                var encuestas = await _context.Encuestas
                    .Include(e => e.Autor)
                    .Where(e => encuestaIds.Contains(e.Id))
                    .Select(e => new EncuestaExportDTO
                    {
                        Id = e.Id,
                        Titulo = e.Titulo,
                        Descripcion = e.Descripcion,
                        Estado = e.Estado,
                        CierraEn = e.CierraEn,
                        CreadoEn = e.CreadoEn,
                        Autor = e.Autor.UserName,
                        TotalRespuestas = e.Respuestas.Count
                    })
                    .ToListAsync();

                if (!encuestas.Any())
                {
                    return NotFound("No se encontraron encuestas con los IDs proporcionados.");
                }

                var excelData = _excelExportService.ExportMultipleEncuestasToExcel(encuestas);

                return File(excelData, 
                           "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet", 
                           $"Resumen_Encuestas_{DateTime.Now:yyyyMMddHHmmss}.xlsx");
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Error al exportar las encuestas: {ex.Message}");
            }
        }

        // POST: api/export/encuesta/json-to-excel
        [HttpPost("json-to-excel")]
        public IActionResult ConvertJsonToExcel([FromBody] EncuestaExportDTO encuestaData)
        {
            try
            {
                if (encuestaData == null)
                {
                    return BadRequest("Datos de encuesta no válidos.");
                }

                var excelData = _excelExportService.ExportEncuestaToExcel(encuestaData);

                return File(excelData, 
                           "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet", 
                           $"Encuesta_Desde_JSON_{DateTime.Now:yyyyMMddHHmmss}.xlsx");
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Error al convertir JSON a Excel: {ex.Message}");
            }
        }

        private EncuestaExportDTO MapToExportDTO(Encuesta encuesta)
        {
            var exportDTO = new EncuestaExportDTO
            {
                Id = encuesta.Id,
                Titulo = encuesta.Titulo,
                Descripcion = encuesta.Descripcion,
                Estado = encuesta.Estado,
                CierraEn = encuesta.CierraEn,
                CreadoEn = encuesta.CreadoEn,
                Autor = encuesta.Autor?.UserName ?? "Desconocido",
                TotalRespuestas = encuesta.Respuestas.Count,
                Preguntas = new List<PreguntaExportDTO>()
            };

            foreach (var pregunta in encuesta.Preguntas)
            {
                var preguntaDTO = new PreguntaExportDTO
                {
                    Id = pregunta.Id,
                    Enunciado = pregunta.Enunciado,
                    TipoPregunta = pregunta.TipoPregunta,
                    Obligatorio = pregunta.Obligatorio,
                    Opciones = new List<OpcionExportDTO>(),
                    Respuestas = new List<RespuestaExportDTO>()
                };

                // Mapear opciones
                foreach (var opcion in pregunta.Opciones)
                {
                    var conteoSelecciones = encuesta.Respuestas
                        .Count(r => r.PreguntaId == pregunta.Id && 
                                   (r.SeleccionOpcionId == opcion.Id || 
                                    r.RespuestasOpciones.Any(ro => ro.OpcionId == opcion.Id)));

                    preguntaDTO.Opciones.Add(new OpcionExportDTO
                    {
                        Id = opcion.Id,
                        Label = opcion.Label,
                        Value = opcion.Value,
                        ConteoSelecciones = conteoSelecciones
                    });
                }

                // Mapear respuestas
                foreach (var respuesta in encuesta.Respuestas.Where(r => r.PreguntaId == pregunta.Id))
                {
                    string valorRespuesta = "";

                    if (respuesta.SeleccionOpcionId.HasValue)
                    {
                        var opcion = pregunta.Opciones.FirstOrDefault(o => o.Id == respuesta.SeleccionOpcionId.Value);
                        valorRespuesta = opcion?.Label ?? "Opción no encontrada";
                    }
                    else if (respuesta.RespuestasOpciones.Any())
                    {
                        var opcionesSeleccionadas = respuesta.RespuestasOpciones
                            .Select(ro => pregunta.Opciones.FirstOrDefault(o => o.Id == ro.OpcionId)?.Label)
                            .Where(label => label != null)
                            .ToList();

                        valorRespuesta = string.Join(", ", opcionesSeleccionadas);
                    }
                    else if (!string.IsNullOrEmpty(respuesta.Texto))
                    {
                        valorRespuesta = respuesta.Texto;
                    }
                    else if (respuesta.Numerica.HasValue)
                    {
                        valorRespuesta = respuesta.Numerica.Value.ToString();
                    }

                    preguntaDTO.Respuestas.Add(new RespuestaExportDTO
                    {
                        FechaRespuesta = respuesta.FechaRespuesta,
                        Usuario = respuesta.Usuario?.UserName ?? "Anónimo",
                        ValorRespuesta = valorRespuesta
                    });
                }

                exportDTO.Preguntas.Add(preguntaDTO);
            }

            return exportDTO;
        }
    }
}