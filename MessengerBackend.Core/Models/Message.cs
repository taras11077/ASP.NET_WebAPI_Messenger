namespace MessengerBackend.Core.Models;

public class Message
{
    public int Id { get; set; }
    public virtual User Sender { get; set; }
    public Chat Chat { get; set; }
    public DateTime SentAt { get; set; }
    public string Content { get; set; }
    
    public virtual ICollection<Attachment> Attachments { get; set; }
}