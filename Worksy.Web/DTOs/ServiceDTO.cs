namespace Worksy.Web.DTOs
{
    public class ServiceDTO
    {
        public Guid ServiceId { get; set; }
        public string Title { get; set; }
        public string Description { get; set; }
        public List<CategoryDTO>? Categories { get; set; }
    }
}
