using Microsoft.EntityFrameworkCore;
using MessengerBackend.Core.Models;

namespace MessengerBackend.Storage;

public class MessengerContext : DbContext
{
    public MessengerContext(DbContextOptions<MessengerContext> options) : base(options)
    {
        
    } 
    
   //  protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
   // {
   //     optionsBuilder.UseSqlServer(
   //         "Data Source=(localdb)\\MSSQLLocalDB;Initial Catalog=MessengerDB;Integrated Security=True;Connect Timeout=30;Encrypt=False;Trust Server Certificate=False;Application Intent=ReadWrite;Multi Subnet Failover=False");
   // }
    
    public DbSet<User> Users { get; set; }
    
    public DbSet<Message> Messages { get; set; }
    
    public DbSet<Attachment> Attachments { get; set; }
    public DbSet<PrivateChat> PrivateChats { get; set; }
    public DbSet<GroupChat> GroupChats { get; set; }
}