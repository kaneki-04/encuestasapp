using Microsoft.AspNetCore.Identity;
using System.ComponentModel.DataAnnotations;

namespace GestorEncuestas_MVC.Models
{
    public class Rol : IdentityRole<int>
    {
        [Required]
        [MaxLength(50)]
        [Display(Name = "Nombre del rol")]
        public string DisplayRolNombre { get; set; } = string.Empty;

        // Relación 1:N con Usuarios
        public ICollection<Usuario> Usuarios { get; set; } = new List<Usuario>();
    }
}