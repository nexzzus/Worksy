using Microsoft.EntityFrameworkCore;
using Worksy.Web.Data.Entities;

namespace Worksy.Web.Data.Seeders;

public class CategorySeeder
{
    private readonly DataContext _context;

    public CategorySeeder(DataContext context)
    {
        _context = context;
    }

    public async Task SeedAsync()
    {
        List<Category> categories =
        [
            ..this.categories()
        ];

        foreach (var cat in categories)
        {
            bool exists = await _context.Categories.AnyAsync(c => c.Name == cat.Name);
            if (!exists)
            {
                await _context.Categories.AddAsync(cat);
            }
        }

        await _context.SaveChangesAsync();
    }

    private List<Category> categories()
    {
        return new()
        {
            new Category
            {
                Id = Guid.NewGuid(),
                Name = "Tecnología",
                Description = "Servicios relacionados con equipos, software, programación y soporte técnico."
            },
            new Category
            {
                Id = Guid.NewGuid(),
                Name = "Construcción",
                Description = "Servicios de obra civil, remodelación, mantenimiento y reparaciones."
            },
            new Category
            {
                Id = Guid.NewGuid(),
                Name = "Educación",
                Description = "Clases, tutorías y asesorías académicas."
            },
            new Category
            {
                Id = Guid.NewGuid(),
                Name = "Diseño",
                Description = "Diseño gráfico, diseño web, branding y artes digitales."
            },
            new Category
            {
                Id = Guid.NewGuid(),
                Name = "Hogar",
                Description = "Aseo, plomería, electricidad, jardinería y servicios del hogar."
            },
            new Category
            {
                Id = Guid.NewGuid(),
                Name = "Transporte",
                Description = "Mensajería, transporte de personas, encomiendas y mudanzas."
            }
        };
    }
}