using System.ComponentModel.DataAnnotations;
using Worksy.Web.Data.Abstractions;

namespace Worksy.Web.Data.Entities
{
    public class Category: IId
    {
        [Key]
        public Guid Id { get; set; }
        public required string Name { get; set; }
        public required string Description { get; set; }

        public ICollection<Service> Services { get; set; } = new List<Service>();
    }
}
