namespace MessengerBackend.DTOs;

public class UserDTO
{
    public int Id { get; set; }
    public string Nickname { get; set; }
    public DateTime LastSeenOnline { get; set; }
}