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
    public class PreguntasController : ControllerBase
    {
        private readonly ApplicationDbContext _context;
        private readonly UserManager<Usuario> _userManager;

        public PreguntasController(ApplicationDbContext context, UserManager<Usuario> userManager)
        {
            _context = context;
            _userManager = userManager;
        }

        // GET: api/preguntas/encuesta/5
        [HttpGet("encuesta/{encuestaId}")]
        public async Task<IActionResult> GetPreguntasByEncuesta(int encuestaId)
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

            var preguntas = await _context.Preguntas
                .Where(p => p.EncuestaId == encuestaId)
                .Include(p => p.Opciones)
                .Select(p => new PreguntaDto
                {
                    Id = p.Id,
                    Enunciado = p.Enunciado,
                    TipoPregunta = p.TipoPregunta,
                    Obligatorio = p.Obligatorio,
                    EncuestaId = p.EncuestaId,
                    Opciones = p.Opciones.Select(o => new OpcionDto
                    {
                        Id = o.Id,
                        Label = o.Label,
                        Value = o.Value
                    }).ToList()
                })
                .ToListAsync();

            return Ok(preguntas);
        }

        // AGREGAR ESTE MÉTODO AL PreguntasController.cs (en Controllers/Api/)
        [AllowAnonymous] // O [Authorize] sin verificación de autor
        [HttpGet("public/encuesta/{encuestaId}")]
        public async Task<IActionResult> GetPreguntasPublicasByEncuesta(int encuestaId)
        {
            try
            {
                // Verificar que la encuesta existe y está activa
                var encuesta = await _context.Encuestas
                    .FirstOrDefaultAsync(e => e.Id == encuestaId && e.Estado == "Activa");

                if (encuesta == null)
                {
                    return NotFound("Encuesta no encontrada o no está activa");
                }

                var preguntas = await _context.Preguntas
                    .Where(p => p.EncuestaId == encuestaId)
                    .Include(p => p.Opciones)
                    .Select(p => new PreguntaDto
                    {
                        Id = p.Id,
                        Enunciado = p.Enunciado,
                        TipoPregunta = p.TipoPregunta,
                        Obligatorio = p.Obligatorio,
                        EncuestaId = p.EncuestaId,
                        Opciones = p.Opciones.Select(o => new OpcionDto
                        {
                            Id = o.Id,
                            Label = o.Label,
                            Value = o.Value
                        }).ToList()
                    })
                    .ToListAsync();

                return Ok(preguntas);
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Error al cargar las preguntas: {ex.Message}");
            }
        }

        // POST: api/preguntas
        [HttpPost]
        public async Task<IActionResult> CreatePregunta([FromBody] CreatePreguntaDto preguntaDto)
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

            // Verificar que la encuesta pertenece al usuario
            var encuesta = await _context.Encuestas
                .FirstOrDefaultAsync(e => e.Id == preguntaDto.EncuestaId && e.AutorId == usuarioActual.Id);

            if (encuesta == null)
            {
                return NotFound("Encuesta no encontrada o no tienes permisos");
            }

            var pregunta = new Pregunta
            {
                Enunciado = preguntaDto.Enunciado,
                TipoPregunta = preguntaDto.TipoPregunta,
                Obligatorio = preguntaDto.Obligatorio,
                EncuestaId = preguntaDto.EncuestaId
            };

            // Si es una pregunta con opciones, crear las opciones básicas según el tipo
            if (preguntaDto.TipoPregunta == "SeleccionUnica" || preguntaDto.TipoPregunta == "OpcionMultiple")
            {
                pregunta.Opciones = CrearOpcionesPorDefecto(preguntaDto.TipoPregunta);
            }
            else if (preguntaDto.TipoPregunta == "Escala")
            {
                pregunta.Opciones = CrearOpcionesEscala();
            }

            _context.Preguntas.Add(pregunta);
            await _context.SaveChangesAsync();

            return Ok(new { 
                Success = true, 
                Message = "Pregunta creada exitosamente",
                Id = pregunta.Id 
            });
        }

        // DELETE: api/preguntas/5
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeletePregunta(int id)
        {
            var usuarioActual = await _userManager.GetUserAsync(User);
            if (usuarioActual == null)
            {
                return Unauthorized();
            }

            var pregunta = await _context.Preguntas
                .Include(p => p.Encuesta)
                .FirstOrDefaultAsync(p => p.Id == id && p.Encuesta.AutorId == usuarioActual.Id);

            if (pregunta == null)
            {
                return NotFound("Pregunta no encontrada o no tienes permisos");
            }

            _context.Preguntas.Remove(pregunta);
            await _context.SaveChangesAsync();

            return Ok(new { 
                Success = true, 
                Message = "Pregunta eliminada exitosamente" 
            });
        }

        private List<PreguntaOpcion> CrearOpcionesPorDefecto(string tipoPregunta)
        {
            var opciones = new List<PreguntaOpcion>();
            
            if (tipoPregunta == "SeleccionUnica")
            {
                opciones.AddRange(new[]
                {
                    new PreguntaOpcion { Position = 1, Label = "Opción 1", Value = "opcion_1" },
                    new PreguntaOpcion { Position = 2, Label = "Opción 2", Value = "opcion_2" },
                    new PreguntaOpcion { Position = 3, Label = "Opción 3", Value = "opcion_3" }
                });
            }
            else if (tipoPregunta == "OpcionMultiple")
            {
                opciones.AddRange(new[]
                {
                    new PreguntaOpcion { Position = 1, Label = "Opción A", Value = "opcion_a" },
                    new PreguntaOpcion { Position = 2, Label = "Opción B", Value = "opcion_b" },
                    new PreguntaOpcion { Position = 3, Label = "Opción C", Value = "opcion_c" },
                    new PreguntaOpcion { Position = 4, Label = "Opción D", Value = "opcion_d" }
                });
            }

            return opciones;
        }

        private List<PreguntaOpcion> CrearOpcionesEscala()
        {
            return new List<PreguntaOpcion>
            {
                new PreguntaOpcion { Position = 1, Label = "1 - Muy Malo", Value = "1" },
                new PreguntaOpcion { Position = 2, Label = "2 - Malo", Value = "2" },
                new PreguntaOpcion { Position = 3, Label = "3 - Regular", Value = "3" },
                new PreguntaOpcion { Position = 4, Label = "4 - Bueno", Value = "4" },
                new PreguntaOpcion { Position = 5, Label = "5 - Excelente", Value = "5" }
            };
        }
    }

    // DTOs para la API
    public class PreguntaDto
    {
        public int Id { get; set; }
        public string Enunciado { get; set; } = string.Empty;
        public string TipoPregunta { get; set; } = string.Empty;
        public bool Obligatorio { get; set; }
        public int EncuestaId { get; set; }
        public List<OpcionDto> Opciones { get; set; } = new List<OpcionDto>();
    }

    public class OpcionDto
    {
        public int Id { get; set; }
        public string Label { get; set; } = string.Empty;
        public string Value { get; set; } = string.Empty;
    }

    public class CreatePreguntaDto
    {
        [Required]
        public string Enunciado { get; set; } = string.Empty;
        
        [Required]
        public string TipoPregunta { get; set; } = string.Empty;
        
        public bool Obligatorio { get; set; }
        
        [Required]
        public int EncuestaId { get; set; }
    }
}