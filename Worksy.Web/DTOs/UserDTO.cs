using System.ComponentModel.DataAnnotations;

namespace Worksy.Web.DTOs;

public class UserDTO
{
    [MaxLength(32, ErrorMessage = "El campo '{0}' debe tener máximo {1} caracteres.")]
    [Required(ErrorMessage = "Este campo es requerido")]
    [Display(Name = "Nombres")]
    public required string FirstName { get; set; }
    
    [MaxLength(32, ErrorMessage = "El campo '{0}' debe tener máximo {1} caracteres.")]
    [Required(ErrorMessage = "Este campo es requerido")]
    [Display(Name = "Apellidos")]
    public required string LastName { get; set; }
    
    [Display(Name = "Nombre completo")]
    public string FullName => $"{FirstName} {LastName}";
    
    [Required(ErrorMessage = "Este campo es requerido")]
    [EmailAddress(ErrorMessage = "Correo no válido")]
    [Display(Name = "Correo electrónico")]
    public required string Email { get; set; }

    [Required(ErrorMessage = "Este campo es requerido")]
    [DataType(DataType.Password)]
    [Display(Name = "Contraseña")]
    public required string Password { get; set; }
    
    [Phone(ErrorMessage = "Número de teléfono no válido")]
    [StringLength(15, ErrorMessage = "El campo '{0}' debe tener máximo {1} caracteres.")]
    [Display(Name = "Número de teléfono")]
    public string? PhoneNumber { get; set; }
    
    [MaxLength(256, ErrorMessage = "El campo '{0}' debe tener máximo {1} caracteres.")]
    [Required(ErrorMessage = "Este campo es requerido")]
    [Display(Name = "Dirección")]
    public required string Address { get; set; }
    
    [MaxLength(512, ErrorMessage = "El campo '{0}' debe tener máximo {1} carácteres.")]
    [Display(Name = "Biografía")]
    public string? Biography { get; set; }
}