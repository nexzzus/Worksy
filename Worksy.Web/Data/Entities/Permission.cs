using System.ComponentModel.DataAnnotations;
using Worksy.Web.Data.Abstractions;

namespace Worksy.Web.Data.Entities;

public class Permission: IId
{
    [Key]
    public Guid Id { get; set; }
    
    [MaxLength(32)]
    [Required]
    public required string Name { get; set; }
    
    [MaxLength(128)]
    [Required]
    public required string Description { get; set; }
    
    [MaxLength(32)]
    [Required]
    public required string Module { get; set; }
    
    public ICollection<RolePermission>? RolePermissions { get; set; }
}