namespace LicenseRemake.DTO.Auth
{
    public record TokenResponse(
        string AccessToken,
        string RefreshToken,
        DateTime Issued,
        DateTime Expires,
        string Role,
        string UserName
    );
}
