using LicenseRemake.Domain;
using Microsoft.EntityFrameworkCore;

namespace LicenseRemake.Infrastructure;

public static class DataSeeder
{
    public static async Task SeedAsync(DataDbContext context)
    {
        if (!await context.AppUsers.AnyAsync())
        {
            var admin = new AppUser
            {
                Username = "admin",
                PasswordHash = BCrypt.Net.BCrypt.HashPassword("admin123"),
                Role = "Admin",
                IsActive = true,
                CreatedAt = DateTime.UtcNow
            };

            context.AppUsers.Add(admin);
            await context.SaveChangesAsync();

            // создаём тестовый refresh token
            var rawToken = Guid.NewGuid().ToString("N");

            var refreshToken = new RefreshToken
            {
                Token = BCrypt.Net.BCrypt.HashPassword(rawToken),
                UserId = admin.Id,
                CreatedAt = DateTime.UtcNow,
                ExpiresAt = DateTime.UtcNow.AddDays(7),
                RevokedAt = null
            };

            context.RefreshTokens.Add(refreshToken);

            Console.WriteLine("====================================");
            Console.WriteLine($"TEST REFRESH TOKEN: {rawToken}");
            Console.WriteLine("====================================");
        }

        if (!await context.CashRegisters.AnyAsync())
        {
            context.CashRegisters.Add(new CashRegister
            {
                FcNumber = "1991070005FN",
                IsBlocked = false,
                CurrentLicenseExpiration = DateTime.UtcNow.AddMonths(1),
                CreatedAt = DateTime.UtcNow,
                UpdatedAt = DateTime.UtcNow
            });
        }

        await context.SaveChangesAsync();
    }
}
