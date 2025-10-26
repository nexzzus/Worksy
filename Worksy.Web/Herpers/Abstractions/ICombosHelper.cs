using Microsoft.AspNetCore.Mvc.Rendering;

namespace Worksy.Web.Herpers.Abstractions;

public interface ICombosHelper
{
    public Task<List<SelectListItem>> GetComboRoles();
}