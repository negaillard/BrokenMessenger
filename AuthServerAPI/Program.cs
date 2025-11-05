using AuthServerAPI.Logic;
using AuthServerAPI.Logic.Interfaces;
using AuthServerAPI.Models;
using AuthServerAPI.Storage;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

// это надо чтобы подставить значения из конфига(логин и пароль)
builder.Services.Configure<EmailSettings>(builder.Configuration.GetSection("EmailSettings"));
builder.Services.AddSingleton<IEmailService, EmailService>();
builder.Services.AddTransient<ICodeVerificationLogic, CodeVerificationLogic>();
builder.Services.AddTransient<IUserLogic, UserLogic>();

builder.Services.AddTransient<IUserStorage, UserStorage>();

var app = builder.Build();


// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
	app.UseSwagger();
	app.UseSwaggerUI();
}

app.UseHttpsRedirection();

app.Run();

