// Controllers/RespuestasController.cs
using System;
using System.Globalization;
using System.Linq;
using System.Threading.Tasks;
using GestorEncuestas_MVC.Data;
using GestorEncuestas_MVC.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace GestorEncuestas_MVC.Controllers
{
    [Authorize]
    public class RespuestasController : Controller
    {
        private readonly ApplicationDbContext _context;
        private readonly UserManager<Usuario> _userManager;

        public RespuestasController(ApplicationDbContext context, UserManager<Usuario> userManager)
        {
            _context = context;
            _userManager = userManager;
        }

        // GET: Respuestas/Responder/5
        [HttpGet]
        public async Task<IActionResult> Responder(int id)
        {
            var encuesta = await _context.Encuestas
                .AsNoTracking()
                .Include(e => e.Preguntas)
                    .ThenInclude(p => p.Opciones)
                .FirstOrDefaultAsync(e => e.Id == id && e.Estado == "Activa");

            if (encuesta == null)
                return NotFound();

            var usuarioActual = await _userManager.GetUserAsync(User);
            if (usuarioActual is null)
                return Challenge();

            var yaRespondio = await _context.Respuestas
                .AsNoTracking()
                .AnyAsync(r => r.EncuestaId == id && r.UsuarioId == usuarioActual.Id);

            if (yaRespondio)
            {
                TempData["InfoMessage"] = "Ya has respondido esta encuesta.";
                return RedirectToAction("Index", "Encuestas");
            }

            var model = new ResponderEncuestaViewModel
            {
                EncuestaId = encuesta.Id,
                Titulo = encuesta.Titulo,
                Descripcion = encuesta.Descripcion,
                Preguntas = encuesta.Preguntas.Select(p => new PreguntaRespuestaViewModel
                {
                    PreguntaId = p.Id,
                    Enunciado = p.Enunciado,
                    TipoPregunta = p.TipoPregunta,
                    Obligatorio = p.Obligatorio,
                    Opciones = p.Opciones.Select(o => new OpcionViewModel
                    {
                        OpcionId = o.Id,
                        Label = o.Label,
                        Value = o.Value
                    }).ToList()
                }).ToList()
            };

            return View(model);
        }

        // POST: Respuestas/Responder
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Responder(ResponderEncuestaViewModel model)
        {
            if (!ModelState.IsValid)
                return View(model);

            var usuarioActual = await _userManager.GetUserAsync(User);
            if (usuarioActual is null)
                return Challenge();

            var yaRespondio = await _context.Respuestas
                .AsNoTracking()
                .AnyAsync(r => r.EncuestaId == model.EncuestaId && r.UsuarioId == usuarioActual.Id);

            if (yaRespondio)
            {
                TempData["ErrorMessage"] = "Ya has respondido esta encuesta.";
                return RedirectToAction("Index", "Encuestas");
            }

            var fechaActual = DateTime.Now;

            foreach (var pregunta in model.Preguntas)
            {
                var respuesta = new Respuesta
                {
                    UsuarioId = usuarioActual.Id,
                    EncuestaId = model.EncuestaId,
                    PreguntaId = pregunta.PreguntaId,
                    FechaRespuesta = fechaActual
                };

                switch (pregunta.TipoPregunta)
                {
                    case "Texto":
                        // No asignamos null: usamos string.Empty si viene vacío
                        respuesta.Texto = SanitizeText(pregunta.RespuestaTexto);
                        break;

                    case "Escala":
                        if (TryParseFloat(pregunta.RespuestaTexto, out var numero))
                        {
                            respuesta.Numerica = numero;
                        }
                        else
                        {
                            if (pregunta.Obligatorio)
                                ModelState.AddModelError(string.Empty, $"La pregunta '{pregunta.Enunciado}' requiere un número válido.");
                        }
                        break;

                    case "SeleccionUnica":
                    case "OpcionMultiple":
                        if (!string.IsNullOrWhiteSpace(pregunta.RespuestaOpcionId))
                        {
                            if (int.TryParse(pregunta.RespuestaOpcionId, out var opcionId))
                            {
                                respuesta.SeleccionOpcionId = opcionId;
                                // Texto adicional (comentario) sin nulls
                                respuesta.Texto = SanitizeText(pregunta.RespuestaTexto);
                            }
                            else
                            {
                                ModelState.AddModelError(string.Empty, $"La selección de la pregunta '{pregunta.Enunciado}' no es válida.");
                            }
                        }
                        else if (pregunta.Obligatorio)
                        {
                            ModelState.AddModelError(string.Empty, $"Debes seleccionar una opción en '{pregunta.Enunciado}'.");
                        }
                        break;

                    default:
                        ModelState.AddModelError(string.Empty, $"Tipo de pregunta desconocido: {pregunta.TipoPregunta}");
                        break;
                }

                _context.Respuestas.Add(respuesta);
            }

            if (!ModelState.IsValid)
                return View(model);

            try
            {
                await _context.SaveChangesAsync();
                TempData["SuccessMessage"] = "¡Encuesta respondida exitosamente!";
                return RedirectToAction("Index", "Encuestas");
            }
            catch (Exception)
            {
                TempData["ErrorMessage"] = "Error al guardar las respuestas.";
                return View(model);
            }
        }

        // GET: Respuestas/MisRespuestas
        public async Task<IActionResult> MisRespuestas()
        {
            var usuarioActual = await _userManager.GetUserAsync(User);
            if (usuarioActual is null)
                return Challenge();

            var respuestas = await _context.Respuestas
                .AsNoTracking()
                .Include(r => r.Encuesta)
                .Include(r => r.Pregunta)
                .Where(r => r.UsuarioId == usuarioActual.Id)
                .OrderByDescending(r => r.FechaRespuesta)
                .ToListAsync();

            return View(respuestas);
        }

        // ---- Helpers ----

        private static bool TryParseFloat(string? input, out float value)
        {
            value = default;
            if (string.IsNullOrWhiteSpace(input)) return false;

            return float.TryParse(input, NumberStyles.Float, CultureInfo.CurrentCulture, out value)
                || float.TryParse(input, NumberStyles.Float, CultureInfo.InvariantCulture, out value);
        }

        private static string SanitizeText(string? text)
            => string.IsNullOrWhiteSpace(text) ? string.Empty : text;
    }
}
