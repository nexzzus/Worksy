using System.ComponentModel.DataAnnotations;

namespace Worksy.Web.DTOs;

public class ChangePasswordDTO
{
    [Required(ErrorMessage = "Este campo es requerido")]
    [DataType(DataType.Password)]
    public string OldPassword { get; set; }
    
    [Required(ErrorMessage = "Este campo es requerido")]
    [DataType(DataType.Password)]
    public string NewPassword { get; set; }

    
    [Required(ErrorMessage = "Este campo es requerido")]
    [DataType(DataType.Password)]
    [Compare("NewPassword", ErrorMessage = "Las contraseñas no coinciden")]
    public string ConfirmPassword { get; set; }
}