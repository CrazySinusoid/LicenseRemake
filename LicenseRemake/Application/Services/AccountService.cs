using LicenseRemake.Application.Interfaces;
using LicenseRemake.Domain;
using LicenseRemake.Domain.Errors;
using LicenseRemake.Infrastructure;
using Microsoft.EntityFrameworkCore;

namespace LicenseRemake.Application.Services;

public class AccountService : IAccountService
{
    private readonly DataDbContext _context;

    public AccountService(DataDbContext context)
    {
        _context = context;
    }

    public async Task CreateAdminAsync(CancellationToken ct)
    {
        if (await _context.AppUsers.AnyAsync(ct))
            throw new ResponseException(
                ResponseErrorCode.EntityAlreadyExists,
                "Admin already exists");

        var admin = new AppUser
        {
            Username = "admin",
            PasswordHash = BCrypt.Net.BCrypt.HashPassword("admin123"),
            Role = "Admin",
            IsActive = true,
            CreatedAt = DateTime.UtcNow
        };

        _context.AppUsers.Add(admin);
        await _context.SaveChangesAsync(ct);
    }

    public async Task CreateUserAsync(
        string userName,
        string password,
        int userTypeId,
        CancellationToken ct)
    {
        if (await _context.AppUsers.AnyAsync(x => x.Username == userName, ct))
            throw new ResponseException(
                ResponseErrorCode.EntityAlreadyExists,
                $"Username '{userName}' already exists");

        string role = userTypeId switch
        {
            1 => "Admin",
            2 => "User",
            _ => throw new ResponseException(
                ResponseErrorCode.EntityNotFound,
                $"Invalid UserTypeId: {userTypeId}")
        };

        var user = new AppUser
        {
            Username = userName,
            PasswordHash = BCrypt.Net.BCrypt.HashPassword(password),
            Role = role,
            IsActive = true,
            CreatedAt = DateTime.UtcNow
        };

        _context.AppUsers.Add(user);

        try
        {
            await _context.SaveChangesAsync(ct);
        }
        catch (Exception ex)
        {
            throw new ResponseException(
                ResponseErrorCode.DatabaseError,
                "Failed to create user",
                inner: ex);
        }
    }

    public async Task ChangePasswordAsync(Guid userId, string newPassword, CancellationToken ct)
    {
        var user = await _context.AppUsers
            .FirstOrDefaultAsync(x => x.Id == userId, ct);

        if (user == null)
            throw new ResponseException(
                ResponseErrorCode.UserNotFound,
                userId.ToString());

        user.PasswordHash = BCrypt.Net.BCrypt.HashPassword(newPassword);

        await _context.SaveChangesAsync(ct);
    }

    public async Task ChangeBlockStatusAsync(Guid userId, bool isActive, CancellationToken ct)
    {
        var user = await _context.AppUsers
            .FirstOrDefaultAsync(x => x.Id == userId, ct);

        if (user == null)
            throw new ResponseException(
                ResponseErrorCode.UserNotFound,
                userId.ToString());

        user.IsActive = isActive;

        await _context.SaveChangesAsync(ct);
    }

    public async Task DeleteUserAsync(Guid userId, CancellationToken ct)
    {
        var user = await _context.AppUsers
            .FirstOrDefaultAsync(x => x.Id == userId, ct);

        if (user == null)
            throw new ResponseException(
                ResponseErrorCode.UserNotFound,
                userId.ToString());

        _context.AppUsers.Remove(user);

        await _context.SaveChangesAsync(ct);
    }

    public async Task<IEnumerable<AppUser>> GetAllAsync(CancellationToken ct)
    {
        return await _context.AppUsers
            .AsNoTracking()
            .ToListAsync(ct);
    }
}