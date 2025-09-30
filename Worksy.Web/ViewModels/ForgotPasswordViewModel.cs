using System.ComponentModel.DataAnnotations;

namespace Worksy.Web.ViewModels;

public class ForgotPasswordViewModel
{
    [Required(ErrorMessage = "Este campo es requerido")]
    [EmailAddress(ErrorMessage = "Correo electrónico inválido")]
    [Display(Name = "Correo Electrónico")]
    public string Email { get; set; }
}