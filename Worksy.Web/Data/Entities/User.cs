using System.ComponentModel.DataAnnotations;
using Worksy.Web.Data.Abstractions;

namespace Worksy.Web.Data.Entities;

public class User: IId
{
    [Key]
    public Guid Id { get; set; }
    
    [MaxLength(32, ErrorMessage = "El campo '{0}' debe tener máximo {1} carácteres.")]
    [Required(ErrorMessage = "Este campo es requerido")]
    [Display(Name = "Nombres")]
    public required string FistName { get; set; }
    
    [MaxLength(32, ErrorMessage = "El campo '{0}' debe tener máximo {1} carácteres.")]
    [Required(ErrorMessage = "Este campo es requerido")]
    [Display(Name = "Apellidos")]
    public required string LastName { get; set; }
    
    [Required(ErrorMessage = "Este campo es requerido")]
    [EmailAddress(ErrorMessage = "Correo no válido")]
    [Display(Name = "Correo electrónico")]
    public required string Email { get; set; }

    [Required(ErrorMessage = "Este campo es requerido")]
    [DataType(DataType.Password)]
    [Display(Name = "Contraseña")]
    public required string Password { get; set;}
    
    [Phone(ErrorMessage = "Número de teléfono no válido")]
    [StringLength(15, ErrorMessage = "El campo '{0}' debe tener máximo {1} carácteres.")]
    [Display(Name = "Número de teléfono")]
    public int? PhoneNumber { get; set; }
    
    [MaxLength(256, ErrorMessage = "El campo '{0}' debe tener máximo {1} carácteres.")]
    [Required(ErrorMessage = "Este campo es requerido")]
    [Display(Name = "Dirección")]
    public required string Address { get; set; }
}