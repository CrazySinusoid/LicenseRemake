namespace LicenseRemake.DTO.AdminPanel
{
    public record ChangePasswordRequest(
    Guid UserId,
    string NewPassword
    );
}
