using System.ComponentModel.DataAnnotations;

namespace Worksy.Web.DTOs
{
    public class CategoryDTO
    {
        [Key]
        public Guid CategoryId { get; set; }
        
        [MaxLength(32, ErrorMessage = "El campo '{0}' debe tener minimo {1} caracteres.")]
        [Required(ErrorMessage = "El campo '{0}' es obligatorio.")]
        public string Name { get; set; }
        
        [MaxLength(256, ErrorMessage = "El campo '{0}' debe tener minimo {1} caracteres.")]
        [Required(ErrorMessage = "El campo '{0}' es obligatorio.")]
        public string Description { get; set; }
        public List<ServiceDTO>? Services { get; set; }
    }
}
