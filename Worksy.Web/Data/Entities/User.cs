using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Identity;
using Worksy.Web.Data.Abstractions;

namespace Worksy.Web.Data.Entities;

public class User: IdentityUser<Guid>
{
    [MaxLength(32, ErrorMessage = "El campo '{0}' debe tener máximo {1} carácteres.")]
    [Required(ErrorMessage = "Este campo es requerido")]
    [Display(Name = "Nombres")]
    public required string FirstName { get; set; }
    
    [MaxLength(32, ErrorMessage = "El campo '{0}' debe tener máximo {1} carácteres.")]
    [Required(ErrorMessage = "Este campo es requerido")]
    [Display(Name = "Apellidos")]
    public required string LastName { get; set; }

    [Phone(ErrorMessage = "Número de teléfono inválido")]
    public override string? PhoneNumber { get; set; }
    
    [MaxLength(256, ErrorMessage = "El campo '{0}' debe tener máximo {1} carácteres.")]
    [Required(ErrorMessage = "Este campo es requerido")]
    [Display(Name = "Dirección")]
    public required string Address { get; set; }

    [MaxLength(512, ErrorMessage = "El campo '{0}' debe tener máximo {1} carácteres.")]
    [Display(Name = "Biografía")]
    public string? Biography { get; set; }
}