using LicenseRemake.Domain;
using LicenseRemake.DTO.Auth;

namespace LicenseRemake.Application.Interfaces;

public interface ITokenService
{
    Task<TokenResponse> PasswordLoginAsync(PasswordLoginRequest request, string ip, CancellationToken ct);

    Task<string> EmitTerminalRefreshTokenAsync(EmitTerminalRefreshTokenRequest request, string ip, CancellationToken ct);

    Task<TokenResponse> RefreshTokenLoginAsync(RefreshTokenLoginRequest request, string ip, CancellationToken ct);
}
