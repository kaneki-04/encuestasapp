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
    [Authorize] // Protege todo el controlador
    public class EncuestasController : Controller
    {
        private readonly ApplicationDbContext _context;
        private readonly UserManager<Usuario> _userManager;

        public EncuestasController(ApplicationDbContext context, UserManager<Usuario> userManager)
        {
            _context = context;
            _userManager = userManager;
        }

        // GET: Encuestas
        public async Task<IActionResult> Index()
        {
            var usuarioActual = await _userManager.GetUserAsync(User);
            if (usuarioActual == null)
            {
                return Challenge();
            }
            
            var encuestas = await _context.Encuestas
                .Include(e => e.Autor)
                .Where(e => e.AutorId == usuarioActual.Id) // Solo las encuestas del usuario actual
                .ToListAsync();
                
            return View(encuestas);
        }

        // GET: Encuestas/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var usuarioActual = await _userManager.GetUserAsync(User);
            if (usuarioActual == null)
            {
                return Challenge();
            }
            
            var encuesta = await _context.Encuestas
                .Include(e => e.Autor)
                .Include(e => e.Preguntas)
                    .ThenInclude(p => p.Opciones)
                .FirstOrDefaultAsync(m => m.Id == id && m.AutorId == usuarioActual.Id); // Solo si es del usuario
            
            if (encuesta == null)
            {
                return NotFound();
            }

            return View(encuesta);
        }

        // GET: Encuestas/Create
        public IActionResult Create()
        {
            return View();
        }

        // POST: Encuestas/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> CreateForm([Bind("Titulo,Descripcion,Estado,CierraEn")] Encuesta encuesta)
        {
            if (ModelState.IsValid)
            {
                var usuarioActual = await _userManager.GetUserAsync(User);
                if (usuarioActual == null)
                {
                    return Challenge();
                }
                
                encuesta.CreadoEn = DateTime.Now;
                encuesta.AutorId = usuarioActual.Id; // Asignar el usuario autenticado
                
                _context.Add(encuesta);
                await _context.SaveChangesAsync();
                
                TempData["SuccessMessage"] = "Encuesta creada exitosamente.";
                return RedirectToAction(nameof(Index));
            }
            return View(encuesta);
        }

        // GET: Encuestas/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var usuarioActual = await _userManager.GetUserAsync(User);
            if (usuarioActual == null)
            {
                return Challenge();
            }
            
            var encuesta = await _context.Encuestas
                .FirstOrDefaultAsync(e => e.Id == id && e.AutorId == usuarioActual.Id); // Solo si es del usuario
                
            if (encuesta == null)
            {
                return NotFound();
            }
            
            return View(encuesta);
        }

        // POST: Encuestas/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("Id,Titulo,Descripcion,Estado,CierraEn,CreadoEn,AutorId")] Encuesta encuesta)
        {
            if (id != encuesta.Id)
            {
                return NotFound();
            }

            var usuarioActual = await _userManager.GetUserAsync(User);
            if (usuarioActual == null)
            {
                return Challenge();
            }
            
            // Verificar que la encuesta pertenece al usuario actual
            var encuestaExistente = await _context.Encuestas
                .AsNoTracking()
                .FirstOrDefaultAsync(e => e.Id == id && e.AutorId == usuarioActual.Id);
                
            if (encuestaExistente == null)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    // Mantener los valores originales que no se editan
                    encuesta.AutorId = usuarioActual.Id;
                    encuesta.CreadoEn = encuestaExistente.CreadoEn;

                    _context.Update(encuesta);
                    await _context.SaveChangesAsync();

                    TempData["SuccessMessage"] = "Encuesta actualizada exitosamente.";
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!EncuestaExists(encuesta.Id))
                    {
                        return NotFound();
                    }
                    else
                    {
                        throw;
                    }
                }
                return RedirectToAction(nameof(Index));
            }
            else
            {
                Console.WriteLine("[DEBUG] ModelState inválido. Errores:");
                foreach (var entry in ModelState)
                {
                    if (entry.Value.Errors.Count > 0)
                    {
                        Console.WriteLine($" - {entry.Key}: {string.Join(", ", entry.Value.Errors.Select(e => e.ErrorMessage))}");
                    }
                }
                return View(encuesta);
            }

            return View(encuesta);
        }

        // GET: Encuestas/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var usuarioActual = await _userManager.GetUserAsync(User);
            if (usuarioActual == null)
            {
                return Challenge();
            }
            
            var encuesta = await _context.Encuestas
                .Include(e => e.Autor)
                .FirstOrDefaultAsync(m => m.Id == id && m.AutorId == usuarioActual.Id); // Solo si es del usuario
                
            if (encuesta == null)
            {
                return NotFound();
            }

           return View("Create", encuesta);
        }

        // POST: Encuestas/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var usuarioActual = await _userManager.GetUserAsync(User);
            if (usuarioActual == null)
            {
                return Challenge();
            }
            
            var encuesta = await _context.Encuestas
                .FirstOrDefaultAsync(e => e.Id == id && e.AutorId == usuarioActual.Id); // Solo si es del usuario
                
            if (encuesta == null)
            {
                return NotFound();
            }

            _context.Encuestas.Remove(encuesta);
            await _context.SaveChangesAsync();
            
            TempData["SuccessMessage"] = "Encuesta eliminada exitosamente.";
            return RedirectToAction(nameof(Index));
        }

        // GET: Encuestas/CreatePregunta/5
        public async Task<IActionResult> CreatePregunta(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var usuarioActual = await _userManager.GetUserAsync(User);
            if (usuarioActual == null)
            {
                return Challenge();
            }
            
            var encuesta = await _context.Encuestas
                .FirstOrDefaultAsync(e => e.Id == id && e.AutorId == usuarioActual.Id); // Solo si es del usuario
                
            if (encuesta == null)
            {
                return NotFound();
            }

            ViewData["EncuestaId"] = id.Value;
            ViewData["EncuestaTitulo"] = encuesta.Titulo;
            return View();
        }

        // POST: Encuestas/CreatePregunta
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> CreatePregunta(int encuestaId, [Bind("Enunciado,TipoPregunta,Obligatorio")] Pregunta pregunta)
        {
            var usuarioActual = await _userManager.GetUserAsync(User);
            if (usuarioActual == null)
            {
                return Challenge();
            }
            
            // Verificar que la encuesta pertenece al usuario actual
            var encuesta = await _context.Encuestas
                .FirstOrDefaultAsync(e => e.Id == encuestaId && e.AutorId == usuarioActual.Id);
                
            if (encuesta == null)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                pregunta.EncuestaId = encuestaId;
                _context.Add(pregunta);
                await _context.SaveChangesAsync();

                TempData["SuccessMessage"] = "Pregunta agregada exitosamente.";
                return RedirectToAction(nameof(Details), new { id = encuestaId });
            }
            else
            {
                foreach (var kvp in ModelState)
                {
                    if (kvp.Value.Errors.Count > 0)
                    {
                        Console.WriteLine($"{kvp.Key}: {string.Join(", ", kvp.Value.Errors.Select(e => e.ErrorMessage))}");
                    }
                }
            }
            
            ViewData["EncuestaId"] = encuestaId;
            ViewData["EncuestaTitulo"] = encuesta.Titulo ?? string.Empty;
            return View(pregunta);
        }

        private bool EncuestaExists(int id)
        {
            return _context.Encuestas.Any(e => e.Id == id);
        }
    }
}