using System.ComponentModel.DataAnnotations;

namespace Worksy.Web.Models
{
    public class TodoItem
    {
        public int Id { get; set; }

        [Required]
        public string Title { get; set; } = string.Empty;

        public bool IsDone { get; set; }
    }
}