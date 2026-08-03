using ApiLanchonete.Data;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using ApiLanchonete.Features.Clients;

namespace ApiLanchonete.Authentication;

public class AuthService(
    AppDbContext context,
    IPasswordHasher<User> passwordHasher,
    JwtService jwtService)
{
    public async Task<TokenResponseDto?> Login(LoginDto dto)
    {
        var user = await context.Users
            .FirstOrDefaultAsync(u => u.Email == dto.Email);

        if (user is null)
            return null;

        if (!user.Active)
            return null;

        var result = passwordHasher.VerifyHashedPassword(
            user,
            user.PasswordHash,
            dto.Password);

        if (result == PasswordVerificationResult.Failed)
            return null;

        var token = jwtService.GenerateToken(user);

        return new TokenResponseDto
        {
            Token = token,
            ExpiresAt = DateTime.UtcNow.AddHours(2)
        };
    }

    public async Task<TokenResponseDto?> Register(RegisterDto dto)
    {
        var exists = await context.Users
            .AnyAsync(u => u.Email == dto.Email);

        if (exists)
            return null;

        var companyExists = await context.Companies.AnyAsync(company => company.Id == dto.CompanyId);
        if (!companyExists)
            return null;

        var user = new User
        {
            Id = Guid.NewGuid(),
            Email = dto.Email,
            Role = UserRole.Customer,
            Active = true,
            CreatedAt = DateTime.UtcNow,
            CreatedBy = "System"
        };

        user.PasswordHash = passwordHasher.HashPassword(user, dto.Password);

        var client = new Client
        {
            Id = Guid.NewGuid(),
            CompanyId = dto.CompanyId,
            UserId = user.Id,

            Name = dto.Name,
            Address = dto.Address,
            City = dto.City,
            State = dto.State,
            CEP = dto.CEP,
            Country = dto.Country,
            Phone = dto.Phone,

            CreatedAt = DateTime.UtcNow,
            CreatedBy = "System"
        };

        context.Users.Add(user);
        context.Clients.Add(client);

        await context.SaveChangesAsync();

        var token = jwtService.GenerateToken(user);

        return new TokenResponseDto
        {
            Token = token,
            ExpiresAt = DateTime.UtcNow.AddHours(2)
        };
    }
}
