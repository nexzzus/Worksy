using System.ComponentModel.DataAnnotations;

namespace Worksy.Web.ViewModels;

public class ChangePasswordViewModel
{
    [Required(ErrorMessage = "Este campo es requerido")]
    [DataType(DataType.Password)]
    [Display(Name = "Contraseña actual")]
    public string OldPassword { get; set; }
    
    [Required(ErrorMessage = "Este campo es requerido")]
    [DataType(DataType.Password)]
    [Display(Name = "Nueva contraseña")]

    public string NewPassword { get; set; }

    
    [Required(ErrorMessage = "Este campo es requerido")]
    [DataType(DataType.Password)]
    [Compare("NewPassword", ErrorMessage = "Las contraseñas no coinciden")]
    [Display(Name = "Confirmar contraseña")]

    public string ConfirmPassword { get; set; }
}