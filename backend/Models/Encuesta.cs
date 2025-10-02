using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Mvc.ModelBinding.Validation;

namespace GestorEncuestas_MVC.Models
{
    public class Encuesta
    {
        public int Id { get; set; }
        
        [Required]
        public string Titulo { get; set; } = string.Empty;
        
        public string Descripcion { get; set; } = string.Empty;
        
        [Required]
        public string Estado { get; set; } = string.Empty;
        
        public DateTime CierraEn { get; set; }
        public DateTime CreadoEn { get; set; }

        // FK
        public int AutorId { get; set; }
        [ValidateNever]
        public Usuario Autor { get; set; } = null!;

        // Relaciones
        [ValidateNever]
        public ICollection<Pregunta> Preguntas { get; set; } = new List<Pregunta>();
        [ValidateNever]
        public ICollection<Respuesta> Respuestas { get; set; } = new List<Respuesta>();
    }
}