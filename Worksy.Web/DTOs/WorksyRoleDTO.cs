using System.ComponentModel.DataAnnotations;

namespace Worksy.Web.DTOs;

public class WorksyRoleDTO
{
    public Guid Id { get; set; }
    
    [Display(Name = "Role")]
    [Required(ErrorMessage = "El campo {0} es requerido")]
    [MaxLength(64,ErrorMessage = "El campo {0} debe tener máximo {1} catácteres")]
    public string Name { get; set; }
    
    public string? PermissionsIds {get; set;}
    
    public List<PermissionsForRoleDTO> Permissions { get; set; }
}

public class PermissionsForRoleDTO: PermissionDTO
{
    public bool Selected { get; set; }
}