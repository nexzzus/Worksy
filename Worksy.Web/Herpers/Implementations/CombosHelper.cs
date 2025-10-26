using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using Worksy.Web.Data;
using Worksy.Web.Herpers.Abstractions;

namespace Worksy.Web.Herpers.Implementations;

public class CombosHelper: ICombosHelper
{
    private readonly DataContext _context;

    public CombosHelper(DataContext context)
    {
        _context = context;
    }

    public async Task<List<SelectListItem>> GetComboRoles()
    {
        return await _context.WorksyRoles.Select(r => new SelectListItem
        {
            Text = r.Name,
            Value = r.Id.ToString()
        }).ToListAsync();
    }
}