using NatyHi.Domain.Entities;
using NatyHi.Domain.Interfaces;
using Supabase;

namespace NatyHi.Infrastructure.Repositories;

public class SupabaseUserRepository : IUserRepository
{
    private readonly Client _supabaseClient;

    public SupabaseUserRepository(Client supabaseClient)
    {
        _supabaseClient = supabaseClient;
    }

    public async Task AddAsync(User user)
    {
        // Supabase guarda objetos directamente si coinciden con la tabla.
        // Aquí asumimos que la clase User tiene atributos [Table] y [Column] 
        // o usamos un modelo de datos específico de infra y mapeamos.
        // Por simplicidad para el tutorial, usaremos insert directo asumiendo el mapeo.
        
        // NOTA IMPORTANTE: La librería 'Supabase-csharp' usa atributos Postgrest.Attributes para mapear.
        // En una Clean Architecture estricta, NO debemos ensuciar la entidad de Dominio con atributos de librería externa.
        // Lo correcto es crear un 'UserModel' aquí en Infra, mapear User -> UserModel y guardar UserModel.
        
        var model = new UserModel 
        { 
            Id = user.Id, 
            Email = user.Email, 
            PasswordHash = user.PasswordHash,
            RefreshToken = user.RefreshToken,
            RefreshTokenExpiryTime = user.RefreshTokenExpiryTime
        };

        await _supabaseClient.From<UserModel>().Insert(model);
    }

    public async Task<bool> ExistsByEmailAsync(string email)
    {
        var response = await _supabaseClient
            .From<UserModel>()
            .Select("id") // Solo traemos ID para optimizar
            .Filter("email", Supabase.Postgrest.Constants.Operator.Equals, email)
            .Get();

        return response.Models.Any();
    }

    public async Task<User?> GetByEmailAsync(string email)
    {
        var response = await _supabaseClient
            .From<UserModel>()
            .Filter("email", Supabase.Postgrest.Constants.Operator.Equals, email)
            .Single(); // Devuelve null si no encuentra o excepción según config

        if (response == null) return null;

        // Reconstruimos la entidad de Dominio (usando reflexión o constructor público si lo abrimos)
        // Para este ejemplo usaremos el constructor público que creamos
        var user = new User(response.Email, response.PasswordHash);
        
        // Hack: Como el ID es privado set, necesitamos reflexión o un método factory en User 
        // para restaurar el estado completo. Para simplicidad del tutorial, usaremos reflexión rápida.
        typeof(User).GetProperty("Id")?.SetValue(user, response.Id);
        if (response.RefreshToken != null && response.RefreshTokenExpiryTime != null)
        {
            user.UpdateRefreshToken(response.RefreshToken, response.RefreshTokenExpiryTime.Value);
        }

        return user;
    }

    public async Task<User?> GetByIdAsync(Guid id)
    {
        var response = await _supabaseClient
            .From<UserModel>()
            .Filter("id", Supabase.Postgrest.Constants.Operator.Equals, id.ToString())
            .Single();

        if (response == null) return null;

        var user = new User(response.Email, response.PasswordHash);
        typeof(User).GetProperty("Id")?.SetValue(user, response.Id);
         if (response.RefreshToken != null && response.RefreshTokenExpiryTime != null)
        {
            user.UpdateRefreshToken(response.RefreshToken, response.RefreshTokenExpiryTime.Value);
        }
        return user;
    }

    public async Task UpdateAsync(User user)
    {
        var model = new UserModel 
        { 
            Id = user.Id, 
            Email = user.Email, 
            PasswordHash = user.PasswordHash,
            RefreshToken = user.RefreshToken,
            RefreshTokenExpiryTime = user.RefreshTokenExpiryTime
        };
        
        await _supabaseClient
            .From<UserModel>()
            .Update(model);
    }
}

// Modelo interno de Infraestructura para Supabase (para no ensuciar el Dominio)
[Supabase.Postgrest.Attributes.Table("users")]
public class UserModel : Supabase.Postgrest.Models.BaseModel
{
    [Supabase.Postgrest.Attributes.PrimaryKey("id")]
    public Guid Id { get; set; }

    [Supabase.Postgrest.Attributes.Column("email")]
    public string Email { get; set; }

    [Supabase.Postgrest.Attributes.Column("password_hash")]
    public string PasswordHash { get; set; }

    [Supabase.Postgrest.Attributes.Column("refresh_token")]
    public string? RefreshToken { get; set; }

    [Supabase.Postgrest.Attributes.Column("refresh_token_expiry")]
    public DateTime? RefreshTokenExpiryTime { get; set; }
}
