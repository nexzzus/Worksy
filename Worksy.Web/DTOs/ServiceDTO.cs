using System.ComponentModel.DataAnnotations;

namespace Worksy.Web.DTOs
{
    public class ServiceDTO
    {
        public Guid ServiceId { get; set; }
        [Required(ErrorMessage = "El título es obligatorio")]
        public string Title { get; set; }
        [Required(ErrorMessage = "La descripción es obligatoria")]
        public string Description { get; set; }
        [Required(ErrorMessage = "El precio es obligatorio")]
        [Range(0.01, double.MaxValue, ErrorMessage = "El precio debe ser mayor a 0")]
        public decimal Price { get; set; }
        public List<CategoryDTO>? Categories { get; set; }
    }
}
