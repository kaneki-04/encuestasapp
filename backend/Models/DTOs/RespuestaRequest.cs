namespace GestorEncuestas_MVC.Models.DTOs
{
    public class RespuestaRequest
    {
        public int EncuestaId { get; set; }
        public int UsuarioId { get; set; }
        public List<RespuestaItem> Respuestas { get; set; } = new();
    }

    public class RespuestaItem
    {
        public int PreguntaId { get; set; }
        public string? Texto { get; set; }  // para respuestas abiertas
        public float? Numerica { get; set; } // para respuestas numéricas
        public int? SeleccionOpcionId { get; set; } // para preguntas de opción múltiple
    }
}
