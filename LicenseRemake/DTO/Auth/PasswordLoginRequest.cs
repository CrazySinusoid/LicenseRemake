namespace LicenseRemake.DTO.Auth
{
    public record PasswordLoginRequest(
        string Username,
        string Password
    );
}
