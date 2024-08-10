using System.ComponentModel.DataAnnotations.Schema;

namespace MessengerBackend.Core.Models;

[Table("GroupChat")]
public class GroupChat : Chat
{
    public string Title { get; set; }
    public string Description { get; set; }
}
