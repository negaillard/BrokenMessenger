using AuthServerAPI.Logic;
using AuthServerAPI.Logic.Interfaces;
using AuthServerAPI.Models;
using AuthServerAPI.Storage;
using Microsoft.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle


// Entity Framework если используешь БД
//builder.Services.AddDbContext<Context>(options =>
//	options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection")));

// Добавляем Redis
// по сути мы здесь указываем, что Redis реализует IDistributedCache, хотя явно мы это не указали,
// но это тот же AddTransient
builder.Services.AddStackExchangeRedisCache(options =>
{
	//берем значение из блока ConnectionStrings со значением Redis (из appsetttings.json)
	options.Configuration = builder.Configuration.GetConnectionString("Redis");
	//берем значение из блока Redis со значением InstanceName (из appsetttings.json)
	options.InstanceName = builder.Configuration["Redis:InstanceName"];
});


// Регистрируем настройки Redis
// ASP.NET Core автоматически находит раздел "Redis" в appsettings.json
//Создает экземпляр RedisSettings
//Заполняет свойство VerificationCodeExpirationMinutes значением из конфига
//Регистрирует это в DI-контейнере как IOptions<RedisSettings>
builder.Services.Configure<RedisSettings>(builder.Configuration.GetSection("Redis"));

// это надо чтобы подставить значения из конфига(логин и пароль)
builder.Services.Configure<EmailSettings>(builder.Configuration.GetSection("EmailSettings"));

// AddScoped - один экземпляр класса на один http запрос
builder.Services.AddScoped<IEmailService, EmailService>();
builder.Services.AddScoped<ICodeVerificationLogic, CodeVerificationLogic>();
builder.Services.AddScoped<IUserLogic, UserLogic>();
builder.Services.AddScoped<ISessionService, SessionService>();

builder.Services.AddScoped<IUserStorage, UserStorage>();

builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

var app = builder.Build();


// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
	app.UseSwagger();
	app.UseSwaggerUI();
}

app.UseHttpsRedirection();
app.MapControllers();

app.Run();

