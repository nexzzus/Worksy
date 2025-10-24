namespace Worksy.Web.Data.Entities;

public class RolePermission
{
    public required Guid WorksyRoleId { get; set; }
    public required Guid PermissionId { get; set; }
    public WorksyRole WorksyRole { get; set; }
    public Permission Permission { get; set; }
}