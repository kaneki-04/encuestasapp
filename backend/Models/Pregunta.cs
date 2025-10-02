using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Mvc.ModelBinding.Validation;

namespace GestorEncuestas_MVC.Models
{
    public class Pregunta
    {
        public int Id { get; set; }
        
        [Required]
        public string Enunciado { get; set; } = string.Empty;
        
        [Required]
        public string TipoPregunta { get; set; } = string.Empty;
        
        public bool Obligatorio { get; set; }

        // FK
        public int EncuestaId { get; set; }
        [ValidateNever]
        public Encuesta Encuesta { get; set; } = null!;

        // Relaciones
        public ICollection<PreguntaOpcion> Opciones { get; set; } = new List<PreguntaOpcion>();
        public ICollection<Respuesta> Respuestas { get; set; } = new List<Respuesta>();
    }
}