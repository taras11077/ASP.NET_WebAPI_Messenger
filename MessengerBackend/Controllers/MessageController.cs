using MessengerBackend.Core.Models;
using MessengerBackend.DTOs;
using MessengerBackend.Storage;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace MessengerBackend.Controllers;

[ApiController]
[Route("message")]
public class MessageController : Controller
{
    private readonly MessengerContext _context;

    public MessageController(MessengerContext context)
    {
        _context = context;
    }

    // створення чату
    [HttpPost("chat")]
    public async Task<ActionResult<PrivateChatDTO>> CreatePrivateChat(PrivateChatDTO privateChatDto)
    {
        var users = _context.Users.Where(x => privateChatDto.UsersIds.Contains(x.Id)).ToList();
        var privateChat = new PrivateChat()
        {
            Users = users,
            CreatedAt = DateTime.UtcNow
        };
        _context.Add(privateChat);
        await _context.SaveChangesAsync();

        return Ok(privateChatDto);
    }

    // відправлення повідомлення
    [HttpPost]
    public async Task<ActionResult<bool>> SendMessage(MessageDTO messageDto)
    {
        var sender = _context.Users.Single(x => x.Id == messageDto.SenderId);
        var chat = _context.PrivateChats.Single(x => x.Id == messageDto.ChatId);
        var message = new Message()
        {
            Content = messageDto.Text,
            Sender = sender,
            Chat = chat,
            SentAt = DateTime.UtcNow
        };

        _context.Add(message);
        await _context.SaveChangesAsync();
        return true;
    }

// отримання відправлених повідомлень за id користувача
    [HttpGet("user/{userId}/messages")]
    public async Task<ActionResult<IEnumerable<MessageDTO>>> GetMessagesByUserId(int userId)
    {
        var messages = await _context.Messages
            .Where(m => m.Sender.Id == userId)
            .Select(m => new MessageDTO
            {
                Text = m.Content,
                SenderId = m.Sender.Id,
                ChatId = m.Chat.Id,
            })
            .ToListAsync();

        if (messages == null || !messages.Any())
        {
            return NotFound($"No messages found for user with ID {userId}");
        }
        return Ok(messages);
    }
    
    // отримання отриманих повідомлень за id користувача
    
    
}
