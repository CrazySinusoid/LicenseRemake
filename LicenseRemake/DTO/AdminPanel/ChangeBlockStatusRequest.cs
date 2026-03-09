namespace LicenseRemake.DTO.AdminPanel
{
    public record ChangeBlockStatusRequest(
    Guid UserId,
    bool IsActive
    );
}
