using NatyHi.Application.Interfaces;
using NatyHi.Application.Services;
using NatyHi.Domain.Interfaces;
using NatyHi.Infrastructure.Repositories;
using NatyHi.Infrastructure.Services;
using Supabase;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

// Clean Architecture Dependencies
builder.Services.AddScoped<IUserRepository, SupabaseUserRepository>();
builder.Services.AddScoped<IPasswordHasher, PasswordHasher>();
builder.Services.AddScoped<ITokenService, JwtTokenService>();
builder.Services.AddScoped<IAuthService, AuthService>();

// Supabase Configuration
// Nota: Reemplazar con valores reales o usar User Secrets
var url = builder.Configuration["Supabase:Url"] ?? "https://xyz.supabase.co";
var key = builder.Configuration["Supabase:Key"] ?? "public-anon-key";
builder.Services.AddScoped<Client>(provider => new Client(url, key));

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

// app.UseAuthentication(); // Descomentar cuando habilitemos middleware JWT
app.UseAuthorization();

app.MapControllers();

app.Run();
