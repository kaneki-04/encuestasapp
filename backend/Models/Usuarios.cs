using Microsoft.AspNetCore.Identity;
using System.ComponentModel.DataAnnotations;

namespace GestorEncuestas_MVC.Models
{
    public class Usuario : IdentityUser<int>
    {
        // FK
        public int RolId { get; set; }
        public Rol Rol { get; set; } = null!;

        // Relaciones
        public ICollection<Encuesta> EncuestasCreadas { get; set; } = new List<Encuesta>();
        public ICollection<Respuesta> Respuestas { get; set; } = new List<Respuesta>();
    }
}