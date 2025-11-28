using System.ComponentModel.DataAnnotations;
using Worksy.Web.Data.Abstractions;

namespace Worksy.Web.Data.Entities;

public class Message: IId
{
    [Key]
    public Guid Id { get; set; }
    
    public Guid ConversationId { get; set; }
    public Conversation? Conversation { get; set; }
    
    public Guid SenderId { get; set; }
    public User? Sender { get; set; }
    public string Content { get; set; }
    
    public DateTime SentAt { get; set; } = DateTime.UtcNow;
}