using EmploymentSystem.Application.Services;
using EmploymentSystem.Domain.Interfaces;
using EmploymentSystem.Infrastructure.Authentication;
using EmploymentSystem.Infrastructure.Data;
using EmploymentSystem.Infrastructure.MappingProfiles;
using EmploymentSystem.Infrastructure.Repositories;
using EmploymentSystem.Infrastructure.Services;
using Hangfire;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using Serilog;
using System.Text;
using Serilog;
var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddControllers();
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();
builder.Services.AddScoped<IUserRepository, UserRepository>();
builder.Services.AddScoped<IVacancyRepository, VacancyRepository>();
builder.Services.AddScoped<IUserService, UserService>();
builder.Services.AddScoped<IVacancyService, VacancyService>();
builder.Services.AddAutoMapper(typeof(VacancyProfile).Assembly, typeof(ApplicationProfile).Assembly);

builder.Services.AddDbContext<EmploymentDbContext>(cfg => cfg.UseSqlServer(builder.Configuration["ConnectionStrings:DefaultConnection"]));

builder.Services.AddHangfire(config => config
    .UseSqlServerStorage(builder.Configuration.GetConnectionString("DefaultConnection")));

builder.Services.AddHangfireServer();

var jwtOptions = builder.Configuration.GetSection("JWT").Get<JwtOptions>();

builder.Services.AddSingleton(jwtOptions);

var configuration = new ConfigurationBuilder()
    .AddJsonFile("appsettings.json")
    .Build();


Log.Logger = new LoggerConfiguration()
          .ReadFrom.Configuration(configuration)
          .Enrich.FromLogContext()
          .WriteTo.Console()
          .CreateLogger();

builder.Host.UseSerilog();
builder.Services.AddMemoryCache();
builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowAngular", policy =>
    {
        policy.WithOrigins("http://localhost:4200") // Allow Angular frontend
              .AllowAnyHeader()
              .AllowAnyMethod()
              .AllowCredentials();
    });
});

builder.Services.AddAuthentication()

    .AddJwtBearer(JwtBearerDefaults.AuthenticationScheme, options =>
    {
        options.SaveToken = true;
        options.TokenValidationParameters = new TokenValidationParameters
        {
            ValidateIssuer = true,
            ValidIssuer = jwtOptions.Issuer,
            ValidateAudience = true,
            ValidateLifetime = true,
            ValidAudience = jwtOptions.Audience,
            ValidateIssuerSigningKey = true,
            IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(jwtOptions.SignKey))

        };

        // Enable logging for authentication failures
        options.Events = new JwtBearerEvents
        {
            OnAuthenticationFailed = context =>
            {
                Console.WriteLine("Authentication failed:", context.Exception.Message);
                return Task.CompletedTask;
            }
        };

    });

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseCors("AllowAngular");
app.UseHttpsRedirection();
app.UseHangfireDashboard("/dashboard");
app.UseAuthorization();
app.MapControllers();
RecurringJob.AddOrUpdate<VacancyService>(
    "archive-expired-vacancies",
    service => service.ArchiveExpiredVacanciesAsync(),
    Cron.MinuteInterval(5));

app.Run();

