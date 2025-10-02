using System.ComponentModel.DataAnnotations;

namespace GestorEncuestas_MVC.Models
{
    public class PerfilViewModel
    {
        [Display(Name = "Nombre de usuario")]
        public string Username { get; set; } = string.Empty;
    }
}