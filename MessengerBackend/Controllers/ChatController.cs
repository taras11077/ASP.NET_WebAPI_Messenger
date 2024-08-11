using AutoMapper;
using MessengerBackend.Core.Models;
using MessengerBackend.DTOs;
using MessengerBackend.Storage;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace MessengerBackend.Controllers;

[Route("api/chat")]
[ApiController]
public class ChatController : ControllerBase
{
    private readonly MessengerContext _context;
    private readonly IMapper _mapper;

    public ChatController(MessengerContext context, IMapper mapper)
    {
        _context = context;
        _mapper = mapper;
    }
    
    [HttpGet]
    public async Task<ActionResult<IEnumerable<PrivateChatDTO>>> GetPrivateChats()
    {
        var chats = _context.PrivateChats.Include(x => x.Users).Cast<Chat>().ToList();
        chats.AddRange(_context.GroupChats);
    
        return chats.Select(x=> new PrivateChatDTO()
        {
            UsersIds = x.Users.Select(x => x.Id).ToList()
        }).ToList();
    }
    
    [HttpPost("createPrivateChat")]
    public async Task<ActionResult<PrivateChat>> AddPrivateChat(PrivateChatDTO chat)
    {
        var privateChat = new PrivateChat()
        {
            Users = _context.Users.Where(x => chat.UsersIds.Contains(x.Id)).ToList(),
            CreatedAt = DateTime.Now
        };
        _context.PrivateChats.Add(privateChat);
        await _context.SaveChangesAsync();
        return Created("chat", chat);
    }

}

