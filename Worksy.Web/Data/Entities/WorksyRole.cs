using System.ComponentModel.DataAnnotations;
using Worksy.Web.Data.Abstractions;

namespace Worksy.Web.Data.Entities;

public class WorksyRole: IId
{
    [Key]
    public Guid Id { get; set; }
    
    [MaxLength(32)]
    [Required]
    public required string Name { get; set; }
    
    public ICollection<RolePermission>? RolePermissions { get; set; }
}