namespace LicenseRemake.DTO.AdminPanel
{
    public record UserDto(
        Guid Id,
        string Username,
        string Role,
        bool IsActive,
        DateTime CreatedAt
    );
}
