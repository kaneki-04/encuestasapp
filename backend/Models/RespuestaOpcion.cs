using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace GestorEncuestas_MVC.Models
{
    [Table("respuestas_opciones")]
    public class RespuestaOpcion
    {
        public int RespuestaId { get; set; }
        public Respuesta Respuesta { get; set; } = null!;

        public int OpcionId { get; set; }
        public PreguntaOpcion Opcion { get; set; } = null!;
    }
}