using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using GestorEncuestas_MVC.Data;
using GestorEncuestas_MVC.Models;
using System.Linq;
using System.Threading.Tasks;

namespace GestorEncuestas_MVC.Controllers.Api
{
    [ApiController]
    [Route("api/[controller]")]
    [Authorize]
    public class EncuestasController : ControllerBase
    {
        private readonly ApplicationDbContext _context;
        private readonly UserManager<Usuario> _userManager;

        public EncuestasController(ApplicationDbContext context, UserManager<Usuario> userManager)
        {
            _context = context;
            _userManager = userManager;
        }

        // GET: api/encuestas
        [HttpGet]
        public async Task<IActionResult> GetEncuestas()
        {
            var usuarioActual = await _userManager.GetUserAsync(User);
            if (usuarioActual == null)
            {
                return Unauthorized();
            }

            var encuestas = await _context.Encuestas
                .Where(e => e.AutorId == usuarioActual.Id)
                .Select(e => new EncuestaDto
                {
                    Id = e.Id,
                    Titulo = e.Titulo,
                    Descripcion = e.Descripcion,
                    Estado = e.Estado,
                    CierraEn = e.CierraEn,
                    CreadoEn = e.CreadoEn,
                    TotalRespuestas = e.Respuestas.Count
                })
                .ToListAsync();

            return Ok(encuestas);
        }

        // GET: api/encuestas/5
        [HttpGet("{id}")]
        public async Task<IActionResult> GetEncuesta(int id)
        {
            var usuarioActual = await _userManager.GetUserAsync(User);
            if (usuarioActual == null)
            {
                return Unauthorized();
            }

            var encuesta = await _context.Encuestas
                .Where(e => e.Id == id && e.AutorId == usuarioActual.Id)
                .Select(e => new EncuestaDto
                {
                    Id = e.Id,
                    Titulo = e.Titulo,
                    Descripcion = e.Descripcion,
                    Estado = e.Estado,
                    CierraEn = e.CierraEn,
                    CreadoEn = e.CreadoEn,
                    TotalRespuestas = e.Respuestas.Count
                })
                .FirstOrDefaultAsync();

            if (encuesta == null)
            {
                return NotFound();
            }

            return Ok(encuesta);
        }

        // POST: api/encuestas
        [HttpPost]
        public async Task<IActionResult> CreateEncuesta([FromBody] CreateEncuestaDto encuestaDto)
        {
            var usuarioActual = await _userManager.GetUserAsync(User);
            if (usuarioActual == null)
            {
                return Unauthorized();
            }

            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var encuesta = new Encuesta
            {
                Titulo = encuestaDto.Titulo,
                Descripcion = encuestaDto.Descripcion,
                Estado = encuestaDto.Estado,
                CierraEn = encuestaDto.CierraEn,
                CreadoEn = DateTime.Now,
                AutorId = usuarioActual.Id
            };

            _context.Encuestas.Add(encuesta);
            await _context.SaveChangesAsync();

            return Ok(new { 
                Success = true, 
                Message = "Encuesta creada exitosamente",
                Id = encuesta.Id 
            });
        }

        // PUT: api/encuestas/5
        [HttpPut("{id}")]
        public async Task<IActionResult> UpdateEncuesta(int id, [FromBody] UpdateEncuestaDto encuestaDto)
        {
            var usuarioActual = await _userManager.GetUserAsync(User);
            if (usuarioActual == null)
            {
                return Unauthorized();
            }

            var encuesta = await _context.Encuestas
                .FirstOrDefaultAsync(e => e.Id == id && e.AutorId == usuarioActual.Id);

            if (encuesta == null)
            {
                return NotFound();
            }

            encuesta.Titulo = encuestaDto.Titulo;
            encuesta.Descripcion = encuestaDto.Descripcion;
            encuesta.Estado = encuestaDto.Estado;
            encuesta.CierraEn = encuestaDto.CierraEn;

            await _context.SaveChangesAsync();

            return Ok(new { 
                Success = true, 
                Message = "Encuesta actualizada exitosamente" 
            });
        }

        // DELETE: api/encuestas/5
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteEncuesta(int id)
        {
            var usuarioActual = await _userManager.GetUserAsync(User);
            if (usuarioActual == null)
            {
                return Unauthorized();
            }

            var encuesta = await _context.Encuestas
                .FirstOrDefaultAsync(e => e.Id == id && e.AutorId == usuarioActual.Id);

            if (encuesta == null)
            {
                return NotFound();
            }

            _context.Encuestas.Remove(encuesta);
            await _context.SaveChangesAsync();

            return Ok(new { 
                Success = true, 
                Message = "Encuesta eliminada exitosamente" 
            });
        }
    }

    // DTOs para la API
    public class EncuestaDto
    {
        public int Id { get; set; }
        public string Titulo { get; set; } = string.Empty;
        public string Descripcion { get; set; } = string.Empty;
        public string Estado { get; set; } = string.Empty;
        public DateTime CierraEn { get; set; }
        public DateTime CreadoEn { get; set; }
        public int TotalRespuestas { get; set; }
    }

    public class CreateEncuestaDto
    {
        public string Titulo { get; set; } = string.Empty;
        public string Descripcion { get; set; } = string.Empty;
        public string Estado { get; set; } = "Activa";
        public DateTime CierraEn { get; set; }
    }

    public class UpdateEncuestaDto
    {
        public string Titulo { get; set; } = string.Empty;
        public string Descripcion { get; set; } = string.Empty;
        public string Estado { get; set; } = string.Empty;
        public DateTime CierraEn { get; set; }
    }
}