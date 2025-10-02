// Models/ResponderEncuestaViewModel.cs
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace GestorEncuestas_MVC.Models
{
    public class ResponderEncuestaViewModel
    {
        public int EncuestaId { get; set; }
        public string Titulo { get; set; } = string.Empty;
        public string Descripcion { get; set; } = string.Empty;
        
        [Required]
        public List<PreguntaRespuestaViewModel> Preguntas { get; set; } = new List<PreguntaRespuestaViewModel>();
    }

    public class PreguntaRespuestaViewModel
    {
        public int PreguntaId { get; set; }
        public string Enunciado { get; set; } = string.Empty;
        public string TipoPregunta { get; set; } = string.Empty;
        public bool Obligatorio { get; set; }
        
        // Para preguntas de texto y numéricas
        [RequiredIfObligatorio]
        public string? RespuestaTexto { get; set; }
        
        // Para preguntas de opción única/múltiple
        public string? RespuestaOpcionId { get; set; }
        
        public List<OpcionViewModel> Opciones { get; set; } = new List<OpcionViewModel>();
    }

    public class OpcionViewModel
    {
        public int OpcionId { get; set; }
        public string Label { get; set; } = string.Empty;
        public string Value { get; set; } = string.Empty;
    }

    // Atributo de validación personalizado
    public class RequiredIfObligatorioAttribute : ValidationAttribute
    {
        protected override ValidationResult IsValid(object value, ValidationContext validationContext)
        {
            var pregunta = (PreguntaRespuestaViewModel)validationContext.ObjectInstance;
            
            if (pregunta.Obligatorio && string.IsNullOrEmpty(value?.ToString()))
            {
                return new ValidationResult($"La pregunta '{pregunta.Enunciado}' es obligatoria.");
            }
            
            return ValidationResult.Success;
        }
    }
}