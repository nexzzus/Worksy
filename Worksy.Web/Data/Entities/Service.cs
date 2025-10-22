using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Worksy.Web.Data.Entities
{
    public class Service
    {
        [Key]
        public Guid ServiceId { get; set; }

        [MaxLength(32, ErrorMessage = "El campo '{0}' debe tener maximo {1} caracteres.")]
        [Required(ErrorMessage = "El campo '{0}' es obligatorio.")]
        public required string Title { get; set; }

        [MaxLength(32, ErrorMessage = "El campo '{0}' debe tener maximo {1} caracteres.")]
        [Required(ErrorMessage = "El campo '{0}' es obligatorio.")]
        public string Description { get; set; }
        
        [Column(TypeName = "decimal(10,2)")]
        public decimal Price { get; set; }

        public string? Location { get; set; }
        
        public Guid? UserId { get; set; }
        
        public User? User { get; set; }
        
        public DateTime? PublicationDate { get; set; }
        public ICollection<Category>? Categories { get; set; } = new List<Category>();
    }
}
