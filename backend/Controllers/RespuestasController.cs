// Controllers/RespuestasController.cs
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using GestorEncuestas_MVC.Data;
using GestorEncuestas_MVC.Models;
using System.Linq;
using System.Threading.Tasks;

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
                .Include(e => e.Preguntas)
                    .ThenInclude(p => p.Opciones)
                .FirstOrDefaultAsync(e => e.Id == id && e.Estado == "Activa");

            if (encuesta == null)
            {
                return NotFound();
            }

            // Verificar si el usuario ya respondió esta encuesta
            var usuarioActual = await _userManager.GetUserAsync(User);
            var yaRespondio = await _context.Respuestas
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
            {
                return View(model);
            }

            var usuarioActual = await _userManager.GetUserAsync(User);
            var fechaActual = DateTime.Now;

            // Verificar si ya respondió
            var yaRespondio = await _context.Respuestas
                .AnyAsync(r => r.EncuestaId == model.EncuestaId && r.UsuarioId == usuarioActual.Id);

            if (yaRespondio)
            {
                TempData["ErrorMessage"] = "Ya has respondido esta encuesta.";
                return RedirectToAction("Index", "Encuestas");
            }

            // Guardar respuestas
            foreach (var pregunta in model.Preguntas)
            {
                var respuesta = new Respuesta
                {
                    UsuarioId = usuarioActual.Id,
                    EncuestaId = model.EncuestaId,
                    PreguntaId = pregunta.PreguntaId,
                    FechaRespuesta = fechaActual
                };

                if (pregunta.TipoPregunta == "Texto")
                {
                    respuesta.Texto = pregunta.RespuestaTexto;
                }
                else if (pregunta.TipoPregunta == "Escala")
                {
                    respuesta.Numerica = float.Parse(pregunta.RespuestaTexto);
                }
                else if (pregunta.TipoPregunta == "SeleccionUnica" || pregunta.TipoPregunta == "OpcionMultiple")
                {
                    if (!string.IsNullOrEmpty(pregunta.RespuestaOpcionId))
                    {
                        respuesta.SeleccionOpcionId = int.Parse(pregunta.RespuestaOpcionId);
                        respuesta.Texto = pregunta.RespuestaTexto;
                    }
                }

                _context.Respuestas.Add(respuesta);
            }

            try
            {
                await _context.SaveChangesAsync();
                TempData["SuccessMessage"] = "¡Encuesta respondida exitosamente!";
                return RedirectToAction("Index", "Encuestas");
            }
            catch (Exception ex)
            {
                TempData["ErrorMessage"] = "Error al guardar las respuestas.";
                return View(model);
            }
        }

        // GET: Respuestas/MisRespuestas
        public async Task<IActionResult> MisRespuestas()
        {
            var usuarioActual = await _userManager.GetUserAsync(User);
            
            var respuestas = await _context.Respuestas
                .Include(r => r.Encuesta)
                .Include(r => r.Pregunta)
                .Where(r => r.UsuarioId == usuarioActual.Id)
                .OrderByDescending(r => r.FechaRespuesta)
                .ToListAsync();

            return View(respuestas);
        }
    }
}