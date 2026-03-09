using LicenseRemake.Application.Interfaces;
using LicenseRemake.Domain;
using LicenseRemake.Domain.Errors;
using LicenseRemake.DTO.Auth;
using LicenseRemake.Infrastructure;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;

public class TokenService : ITokenService
{
    private readonly DataDbContext _db;
    private readonly IConfiguration _config;

    public TokenService(DataDbContext db, IConfiguration config)
    {
        _db = db;
        _config = config;
    }

    public async Task<TokenResponse> PasswordLoginAsync(
        PasswordLoginRequest request,
        string ip,
        CancellationToken ct)
    {
        var user = await _db.AppUsers
            .FirstOrDefaultAsync(x => x.Username == request.Username, ct);

        if (user == null)
            throw new ResponseException(
                ResponseErrorCode.InvalidCredentials);

        if (!BCrypt.Net.BCrypt.Verify(request.Password, user.PasswordHash))
            throw new ResponseException(
                ResponseErrorCode.InvalidCredentials);

        if (!user.IsActive)
            throw new ResponseException(
                ResponseErrorCode.UserIsBlocked,
                user.Username);

        var refreshRaw = Guid.NewGuid().ToString("N");

        var refresh = new RefreshToken
        {
            Token = BCrypt.Net.BCrypt.HashPassword(refreshRaw),
            UserId = user.Id,
            CreatedAt = DateTime.UtcNow,
            ExpiresAt = DateTime.UtcNow.AddDays(7)
        };

        _db.RefreshTokens.Add(refresh);
        await _db.SaveChangesAsync(ct);

        var access = GenerateJwt(user.Id, user.Username, user.Role);

        return new TokenResponse(
            access,
            refreshRaw,
            DateTime.UtcNow,
            DateTime.UtcNow.AddMinutes(30),
            user.Role,
            user.Username);
    }

    public async Task<string> EmitTerminalRefreshTokenAsync(
        EmitTerminalRefreshTokenRequest request,
        string ip,
        CancellationToken ct)
    {
        var user = await _db.AppUsers
            .FirstOrDefaultAsync(x => x.Id == request.UserGuid, ct);

        if (user == null)
            throw new ResponseException(
                ResponseErrorCode.UserNotFound,
                request.UserGuid.ToString());

        var refreshRaw = Guid.NewGuid().ToString("N");

        var refresh = new RefreshToken
        {
            Token = BCrypt.Net.BCrypt.HashPassword(refreshRaw),
            UserId = user.Id,
            CreatedAt = DateTime.UtcNow,
            ExpiresAt = DateTime.UtcNow.AddDays(7)
        };

        _db.RefreshTokens.Add(refresh);

        await _db.SaveChangesAsync(ct);

        return refreshRaw;
    }

    public async Task<TokenResponse> RefreshTokenLoginAsync(
        RefreshTokenLoginRequest request,
        string ip,
        CancellationToken ct)
    {
        var tokens = await _db.RefreshTokens
            .Include(x => x.User)
            .Where(x => x.ExpiresAt > DateTime.UtcNow && x.RevokedAt == null)
            .ToListAsync(ct);

        var token = tokens.FirstOrDefault(x =>
            BCrypt.Net.BCrypt.Verify(request.RefreshToken, x.Token));

        if (token == null)
            throw new ResponseException(
                ResponseErrorCode.InvalidCredentials,
                "Invalid refresh token");

        var access = GenerateJwt(
            token.User.Id,
            token.User.Username,
            "Global");

        return new TokenResponse(
            access,
            request.RefreshToken,
            DateTime.UtcNow,
            DateTime.UtcNow.AddMinutes(30),
            "Global",
            token.User.Username);
    }

    private string GenerateJwt(Guid userId, string username, string role)
    {
        var key = Encoding.ASCII.GetBytes(_config["Audience:Secret"]!);

        var claims = new[]
        {
            new Claim(ClaimTypes.NameIdentifier, userId.ToString()),
            new Claim(ClaimTypes.Name, username),
            new Claim(ClaimTypes.Role, role)
        };

        var token = new JwtSecurityToken(
            expires: DateTime.UtcNow.AddMinutes(30),
            claims: claims,
            signingCredentials: new SigningCredentials(
                new SymmetricSecurityKey(key),
                SecurityAlgorithms.HmacSha256)
        );

        return new JwtSecurityTokenHandler().WriteToken(token);
    }
}