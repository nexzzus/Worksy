using System.ComponentModel.DataAnnotations;
using Worksy.Web.Data.Abstractions;

namespace Worksy.Web.Data.Entities;

public class Conversation: IId
{
    [Key]
    public Guid Id { get; set; }
    
    public Guid CustomerId { get; set; }
    public User? Customer { get; set; }
    
    public Guid ProviderId { get; set; }
    public User? Provider { get; set; }
    
    public Guid ServiceId { get; set; }
    public Service? Service { get; set; }
    
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

    public ICollection<Message> Messages { get; set; } = new List<Message>();
}