namespace MessengerBackend.DTOs;

public class MessageDTO
{
    public int SenderId { get; set; }
    public int ChatId { get; set; }
    public string Text { get; set; }
}