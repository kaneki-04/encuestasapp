using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Mvc.ModelBinding.Validation;

namespace GestorEncuestas_MVC.Models
{
    public class Respuesta
    {
        public int Id { get; set; }
        public string Texto { get; set; } = string.Empty;
        public float? Numerica { get; set; }
        public DateTime FechaRespuesta { get; set; }

        // FK
        public int UsuarioId { get; set; }
        public Usuario Usuario { get; set; } = null!;

        public int EncuestaId { get; set; }
        public Encuesta Encuesta { get; set; } = null!;

        public int PreguntaId { get; set; }
        public Pregunta Pregunta { get; set; } = null!;

        public int? SeleccionOpcionId { get; set; }
        public PreguntaOpcion? SeleccionOpcion { get; set; }

        // Relaciones N:M
        public ICollection<RespuestaOpcion> RespuestasOpciones { get; set; } = new List<RespuestaOpcion>();
    }
}