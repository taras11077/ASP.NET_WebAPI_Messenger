namespace MessengerBackend.Core.Models;

public class User
{
    public int Id { get; set; }
    public string Nickname { get; set; }
    public string Password { get; set; }
    public DateTime CreatedAt { get; set; }
    public DateTime LastSeenOnline { get; set; }
    
    public ICollection<Chat> Chats { get; set; }
    
}