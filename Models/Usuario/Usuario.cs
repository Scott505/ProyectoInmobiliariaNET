using System.ComponentModel.DataAnnotations;

namespace _Net.Models
{
    public class Usuario
    {
        [Key]
        public int IdUsuario { get; set; }

        [Required]
        [EmailAddress]
        public string Email { get; set; } = null!;

        [Required]
        public string Password { get; set; } = null!;

        public string? Nombre { get; set; }

        [Required]
        public string Rol { get; set; } = "Empleado";

        public bool Activo { get; set; } = true;

        public int? IdPropietario { get; set; }
    }
}
