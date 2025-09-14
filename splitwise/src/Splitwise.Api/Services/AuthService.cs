using Microsoft.EntityFrameworkCore;
using Splitwise.Api.Data;
using Splitwise.Api.DTOs;
using Splitwise.Api.Entities;
using Splitwise.Api.Services.Interfaces;

namespace Splitwise.Api.Services;

public class AuthService : IAuthService
{
    private readonly AppDbContext _db;
    private readonly IConfiguration _cfg;
    public AuthService(AppDbContext db, IConfiguration cfg) { _db = db; _cfg = cfg; }

    public async Task<AuthResponse> RegisterAsync(RegisterRequest req)
    {
        if (await _db.Users.AnyAsync(u => u.Email == req.Email))
            throw new InvalidOperationException("Email already exists");
        var user = new User {
            Id = Guid.NewGuid(),
            Name = req.Name,
            Email = req.Email,
            // If your User entity doesn't have PasswordHash, add it or use Identity in real apps
            PasswordHash = BCrypt.Net.BCrypt.HashPassword(req.Password)
        };
        _db.Users.Add(user);
        await _db.SaveChangesAsync();
        var token = JwtHelper.IssueToken(user, _cfg);
        return new AuthResponse(token, user.Id, user.Name, user.Email);
    }

    public async Task<AuthResponse> LoginAsync(LoginRequest req)
    {
        var user = await _db.Users.FirstOrDefaultAsync(u => u.Email == req.Email)
                   ?? throw new UnauthorizedAccessException("Invalid credentials");
        if (!BCrypt.Net.BCrypt.Verify(req.Password, user.PasswordHash))
            throw new UnauthorizedAccessException("Invalid credentials");
        var token = JwtHelper.IssueToken(user, _cfg);
        return new AuthResponse(token, user.Id, user.Name, user.Email);
    }
}

public static class JwtHelper
{
    public static string IssueToken(User user, IConfiguration cfg)
    {
        var issuer = cfg["Jwt:Issuer"]!;
        var audience = cfg["Jwt:Audience"]!;
        var key = cfg["Jwt:Key"]!;

        var securityKey = new Microsoft.IdentityModel.Tokens.SymmetricSecurityKey(System.Text.Encoding.UTF8.GetBytes(key));
        var creds = new Microsoft.IdentityModel.Tokens.SigningCredentials(securityKey, Microsoft.IdentityModel.Tokens.SecurityAlgorithms.HmacSha256);

        var claims = new[]
        {
            new System.Security.Claims.Claim(System.Security.Claims.ClaimTypes.NameIdentifier, user.Id.ToString()),
            new System.Security.Claims.Claim(System.Security.Claims.ClaimTypes.Name, user.Name),
            new System.Security.Claims.Claim(System.Security.Claims.ClaimTypes.Email, user.Email)
        };

        var token = new System.IdentityModel.Tokens.Jwt.JwtSecurityToken(
            issuer, audience, claims, expires: DateTime.UtcNow.AddHours(8), signingCredentials: creds);

        return new System.IdentityModel.Tokens.Jwt.JwtSecurityTokenHandler().WriteToken(token);
    }
}
