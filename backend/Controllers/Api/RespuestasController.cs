using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using GestorEncuestas_MVC.Data;
using GestorEncuestas_MVC.Models;
using System.Linq;
using System.Threading.Tasks;
using System.ComponentModel.DataAnnotations;

namespace GestorEncuestas_MVC.Controllers.Api
{
    [ApiController]
    [Route("api/[controller]")]
    [Authorize]
    public class RespuestasController : ControllerBase
    {
        private readonly ApplicationDbContext _context;
        private readonly UserManager<Usuario> _userManager;

        public RespuestasController(ApplicationDbContext context, UserManager<Usuario> userManager)
        {
            _context = context;
            _userManager = userManager;
        }

        // POST: api/respuestas/responder
        [HttpPost("responder")]
        public async Task<IActionResult> ResponderEncuesta([FromBody] ResponderEncuestaRequest request)
        {
            var usuarioActual = await _userManager.GetUserAsync(User);
            if (usuarioActual == null)
            {
                return Unauthorized();
            }

            // Verificar si el usuario ya respondió esta encuesta
            var yaRespondio = await _context.Respuestas
                .AnyAsync(r => r.EncuestaId == request.EncuestaId && r.UsuarioId == usuarioActual.Id);

            if (yaRespondio)
            {
                return BadRequest(new { Message = "Ya has respondido esta encuesta" });
            }

            // Verificar que la encuesta existe y está activa
            var encuesta = await _context.Encuestas
                .FirstOrDefaultAsync(e => e.Id == request.EncuestaId && e.Estado == "Activa");

            if (encuesta == null)
            {
                return NotFound("Encuesta no encontrada o no está activa");
            }

            var fechaActual = DateTime.Now;
            var respuestas = new List<Respuesta>();

            foreach (var respuestaRequest in request.Respuestas)
            {
                var respuesta = new Respuesta
                {
                    UsuarioId = usuarioActual.Id,
                    EncuestaId = request.EncuestaId,
                    PreguntaId = respuestaRequest.PreguntaId,
                    FechaRespuesta = fechaActual
                };

                // Asignar la respuesta según el tipo
                if (!string.IsNullOrEmpty(respuestaRequest.RespuestaTexto))
                {
                    respuesta.Texto = respuestaRequest.RespuestaTexto;
                }

                if (!string.IsNullOrEmpty(respuestaRequest.RespuestaOpcionId))
                {
                    if (int.TryParse(respuestaRequest.RespuestaOpcionId, out int opcionId))
                    {
                        respuesta.SeleccionOpcionId = opcionId;
                    }
                }

                respuestas.Add(respuesta);
            }

            _context.Respuestas.AddRange(respuestas);
            await _context.SaveChangesAsync();

            return Ok(new { 
                Success = true, 
                Message = "Encuesta respondida exitosamente" 
            });
        }

        // GET: api/respuestas/mis-respuestas
        [HttpGet("mis-respuestas")]
        public async Task<IActionResult> GetMisRespuestas()
        {
            var usuarioActual = await _userManager.GetUserAsync(User);
            if (usuarioActual == null)
            {
                return Unauthorized();
            }

            var respuestas = await _context.Respuestas
                .Where(r => r.UsuarioId == usuarioActual.Id)
                .Include(r => r.Encuesta)
                .Include(r => r.Pregunta)
                .Include(r => r.SeleccionOpcion)
                .OrderByDescending(r => r.FechaRespuesta)
                .Select(r => new RespuestaInfoDto
                {
                    Id = r.Id,
                    Texto = !string.IsNullOrEmpty(r.Texto) ? r.Texto : 
                           r.SeleccionOpcion != null ? r.SeleccionOpcion.Label : "Sin respuesta",
                    FechaRespuesta = r.FechaRespuesta,
                    Encuesta = new EncuestaInfoDto { 
                        Id = r.Encuesta.Id, 
                        Titulo = r.Encuesta.Titulo 
                    },
                    Pregunta = new PreguntaInfoDto { 
                        Id = r.Pregunta.Id, 
                        Enunciado = r.Pregunta.Enunciado 
                    }
                })
                .ToListAsync();

            return Ok(respuestas);
        }

        // GET: api/respuestas/encuesta/5
        [HttpGet("encuesta/{encuestaId}")]
        public async Task<IActionResult> GetRespuestasByEncuesta(int encuestaId)
        {
            var usuarioActual = await _userManager.GetUserAsync(User);
            if (usuarioActual == null)
            {
                return Unauthorized();
            }

            // Verificar que la encuesta pertenece al usuario
            var encuesta = await _context.Encuestas
                .FirstOrDefaultAsync(e => e.Id == encuestaId && e.AutorId == usuarioActual.Id);

            if (encuesta == null)
            {
                return NotFound("Encuesta no encontrada o no tienes permisos");
            }

            var respuestasAgrupadas = await _context.Respuestas
                .Where(r => r.EncuestaId == encuestaId)
                .Include(r => r.Usuario)
                .Include(r => r.Pregunta)
                .Include(r => r.SeleccionOpcion)
                .GroupBy(r => new { r.UsuarioId, r.Usuario.UserName, r.FechaRespuesta })
                .Select(g => new RespuestasUsuarioDto
                {
                    Usuario = g.Key.UserName ?? "Usuario Anónimo",
                    Fecha = g.Key.FechaRespuesta,
                    Respuestas = g.Select(r => new RespuestaDetalleDto
                    {
                        Pregunta = r.Pregunta.Enunciado,
                        Respuesta = !string.IsNullOrEmpty(r.Texto) ? r.Texto : 
                                   r.SeleccionOpcion != null ? r.SeleccionOpcion.Label : 
                                   r.Numerica.HasValue ? r.Numerica.Value.ToString() : "Sin respuesta"
                    }).ToList()
                })
                .ToListAsync();

            var resultado = new
            {
                Encuesta = new { 
                    Id = encuesta.Id, 
                    Titulo = encuesta.Titulo, 
                    Descripcion = encuesta.Descripcion 
                },
                Respuestas = respuestasAgrupadas
            };

            return Ok(resultado);
        }
    }

    // DTOs para la API - NOMBRES CORREGIDOS
    public class ResponderEncuestaRequest
    {
        [Required]
        public int EncuestaId { get; set; }
        
        [Required]
        public List<RespuestaRequest> Respuestas { get; set; } = new List<RespuestaRequest>();
    }

    public class RespuestaRequest
    {
        [Required]
        public int PreguntaId { get; set; }
        public string? RespuestaTexto { get; set; }
        public string? RespuestaOpcionId { get; set; }
    }

    public class RespuestaInfoDto
    {
        public int Id { get; set; }
        public string Texto { get; set; } = string.Empty;
        public DateTime FechaRespuesta { get; set; }
        public EncuestaInfoDto Encuesta { get; set; } = new EncuestaInfoDto();
        public PreguntaInfoDto Pregunta { get; set; } = new PreguntaInfoDto();
    }

    public class RespuestaDetalleDto
    {
        public string Pregunta { get; set; } = string.Empty;
        public string Respuesta { get; set; } = string.Empty;
    }

    public class RespuestasUsuarioDto
    {
        public string Usuario { get; set; } = string.Empty;
        public DateTime Fecha { get; set; }
        public List<RespuestaDetalleDto> Respuestas { get; set; } = new List<RespuestaDetalleDto>();
    }

    public class EncuestaInfoDto
    {
        public int Id { get; set; }
        public string Titulo { get; set; } = string.Empty;
    }

    public class PreguntaInfoDto
    {
        public int Id { get; set; }
        public string Enunciado { get; set; } = string.Empty;
    }
}