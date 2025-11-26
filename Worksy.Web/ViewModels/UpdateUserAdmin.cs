using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Mvc.Rendering;
using Worksy.Web.Data.Entities;

namespace Worksy.Web.ViewModels;

public class UpdateUserAdmin
{
    public Guid Id { get; set; }

    [Required(ErrorMessage = "El nombre es obligatorio.")]
    [Display(Name = "Nombres")]
    [StringLength(50, ErrorMessage = "El nombre no puede superar los 50 caracteres.")]
    public string FirstName { get; set; }

    [Required(ErrorMessage = "El apellido es obligatorio.")]
    [Display(Name = "Apellidos")]
    [StringLength(50, ErrorMessage = "El apellido no puede superar los 50 caracteres.")]
    public string LastName { get; set; }

    [Required(ErrorMessage = "El correo electrónico es obligatorio.")]
    [EmailAddress(ErrorMessage = "Ingrese un correo electrónico válido.")]
    [Display(Name = "Correo electrónico")]
    public string Email { get; set; }

    [Required(ErrorMessage = "El numero de telefono es obligatorio.")]
    [Phone(ErrorMessage = "Ingrese un número de teléfono válido.")]
    [Display(Name = "Número de teléfono")]
    public string? PhoneNumber { get; set; }

    [Required(ErrorMessage = "La dirección es obligatoria.")]
    [Display(Name = "Dirección")]
    [StringLength(100, ErrorMessage = "La dirección no puede superar los 100 caracteres.")]
    public string? Address { get; set; }

    [Display(Name = "Rol")] 
    [Required] 
    public Guid WorksyRoleId { get; set; }
    public WorksyRole? WorksyRole { get; set; }
    
    public List<SelectListItem>? Roles { get; set; }
}