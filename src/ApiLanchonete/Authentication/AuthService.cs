using ApiLanchonete.Auth;
using ApiLanchonete.Clients;
using ApiLanchonete.Data;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;

namespace ApiLanchonete.Authentication;

public class AuthService(
    AppDbContext context,
    IPasswordHasher<Client> passwordHasher,
    JwtService jwtService)
{
    public async Task<TokenResponseDto?> Login(LoginDto dto)
    {
        var client = await context.Clients
            .FirstOrDefaultAsync(c => c.Email == dto.Email);

        if (client is null)
            return null;

        var result = passwordHasher.VerifyHashedPassword(
            client,
            client.PasswordHash,
            dto.Password);

        if (result == PasswordVerificationResult.Failed)
            return null;

        var token = jwtService.GenerateToken(client);

        return new TokenResponseDto
        {
            Token = token,
            ExpiresAt = DateTime.UtcNow.AddHours(2)
        };
    }
}