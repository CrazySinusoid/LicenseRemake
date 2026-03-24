using AutoMapper;
using AutoMapper.QueryableExtensions;
using LicenseRemake.Application.Interfaces;
using LicenseRemake.Domain;
using LicenseRemake.Domain.Errors;
using LicenseRemake.Domain.Helpers;
using LicenseRemake.DTO.AdminPanel;
using LicenseRemake.Infrastructure;
using Microsoft.EntityFrameworkCore;
using System.Reflection.Metadata.Ecma335;

namespace LicenseRemake.Application.Services;

public class AccountService : IAccountService
{
    private readonly DataDbContext _context;
    private readonly IMapper _mapper;

    public AccountService(DataDbContext context, IMapper mapper)
    {
        _context = context;
        _mapper = mapper;
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

    public async Task<Guid> CreateUserAsync(string userName, string password, int userTypeId, CancellationToken ct)
    {
        if (await _context.AppUsers.AnyAsync(x => x.Username == userName, ct))
            throw new ResponseException(
                ResponseErrorCode.EntityAlreadyExists,
                userName);

        string role = userTypeId switch
        {
            1 => "Admin",
            2 => "User",
            _ => throw new ResponseException(
                ResponseErrorCode.EntityNotFound,
                userTypeId.ToString())
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
        await _context.SaveChangesAsync(ct);

        return user.Id;
    }

    public async Task<UserDto> ChangePasswordAsync(Guid userId, string newPassword, CancellationToken ct)
    {
        var user = await _context.AppUsers.FirstOrDefaultAsync(x => x.Id == userId, ct);

        if (user == null)
            throw new ResponseException(ResponseErrorCode.UserNotFound, userId.ToString());

        user.PasswordHash = BCrypt.Net.BCrypt.HashPassword(newPassword);
        await _context.SaveChangesAsync(ct);

        return Map(user);
    }

    public async Task<UserDto> ChangeBlockStatusAsync(Guid userId, bool isActive, CancellationToken ct)
    {
        var user = await _context.AppUsers.FirstOrDefaultAsync(x => x.Id == userId, ct);

        if (user == null)
            throw new ResponseException(ResponseErrorCode.UserNotFound, userId.ToString());

        user.IsActive = isActive;
        await _context.SaveChangesAsync(ct);

        return Map(user);
    }

    public async Task<UserDto> DeleteUserAsync(Guid userId, CancellationToken ct)
    {
        var user = await _context.AppUsers.FirstOrDefaultAsync(x => x.Id == userId, ct);

        if (user == null)
            throw new ResponseException(ResponseErrorCode.UserNotFound, userId.ToString());

        _context.AppUsers.Remove(user);
        await _context.SaveChangesAsync(ct);

        return Map(user);
    }

    public async Task<IEnumerable<UserDto>> GetAllAsync(CancellationToken ct)
    {
        var list = await _context.AppUsers
            .AsNoTracking()
            .ProjectTo<UserDto>(_mapper.ConfigurationProvider)
            .ToListAsync(ct);

        return list.Select(x => x with
        {
            CreatedAt = DateUtils.ToKyiv(x.CreatedAt)
        });
    }

    private UserDto Map(AppUser user)
    {
        var dto = _mapper.Map<UserDto>(user);

        return dto with
        {
            CreatedAt = DateUtils.ToKyiv(dto.CreatedAt)
        };
    }
}