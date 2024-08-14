using MessengerBackend.Core.Interfaces;
using MessengerBackend.Core.Services;
using MessengerBackend.Storage;
using Microsoft.EntityFrameworkCore;
using MessengerBackend.Filters;
using MessengerBackend.Middlewares;

var builder = WebApplication.CreateBuilder(args);

var connectionString = builder.Configuration.GetConnectionString("Local");
builder.Services.AddDbContext<MessengerContext>(options => options.UseSqlServer(connectionString));

builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

builder.Services.AddAutoMapper(typeof(Program));

builder.Services.AddScoped<IRepository, Repository>();
builder.Services.AddTransient<IUserService, UserService>();
builder.Services.AddTransient<IUserStatisticService, UserStatisticService>();

builder.Services.AddDistributedMemoryCache();
builder.Services.AddSession(options =>
{
    options.IdleTimeout = TimeSpan.FromSeconds((int)builder.Configuration.GetValue(typeof(int), "SessionTimeout"));
    options.Cookie.HttpOnly = true;
    options.Cookie.IsEssential = true;
});

//builder.Services.AddControllers();
builder.Services.AddControllers(options =>
{
    options.Filters.Add(new SessionCheckAttribute());
});

var app = builder.Build();

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseMiddleware<UserStatisticMiddleware>();

//app.UseMiddleware<InfoMiddleware>();

app.UseHttpsRedirection();

app.UseAuthorization();

app.UseSession(); 

app.MapControllers();

app.Run();