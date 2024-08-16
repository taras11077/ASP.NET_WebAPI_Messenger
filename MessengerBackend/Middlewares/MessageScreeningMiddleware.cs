using System.Text;
using System.Text.RegularExpressions;
using MessengerBackend.DTOs;
using Newtonsoft.Json;

namespace MessengerBackend.Middlewares;

public class MessageScreeningMiddleware
{
    private readonly RequestDelegate _next;
    private readonly List<string> _forbiddenWords = new() { "russia", "war", "putin"};

    public MessageScreeningMiddleware(RequestDelegate next)
    {
        _next = next;
    }

    public async Task Invoke(HttpContext ctx)
    {
        // Зчитування тіла запиту
        ctx.Request.EnableBuffering(); // дозволяє зчитувати тіло запиту декілька разів
        using var sr = new StreamReader(ctx.Request.Body, leaveOpen: true);
        var body = await sr.ReadToEndAsync();
        ctx.Request.Body.Position = 0;
    
        // десеріалізація повідомлення
        var messageDto = JsonConvert.DeserializeObject<MessageDTO>(body);
    
        if (messageDto != null && !string.IsNullOrWhiteSpace(messageDto.Text))
        {
            // заміна заборонених слів
            messageDto.Text = VerifyText(messageDto.Text);

            // створення нового тіла запиту з відредагованим текстом
            var updatedBody = JsonConvert.SerializeObject(messageDto);
            var bytes = Encoding.UTF8.GetBytes(updatedBody);
            ctx.Request.Body = new MemoryStream(bytes);
        }
        
        await _next(ctx);
    }
    
    private string VerifyText(string text)
    {
        foreach (var word in _forbiddenWords)
        {
            var regex = new Regex(word, RegexOptions.IgnoreCase);
            text = regex.Replace(text, "***");
        }
        return text;
    }
}