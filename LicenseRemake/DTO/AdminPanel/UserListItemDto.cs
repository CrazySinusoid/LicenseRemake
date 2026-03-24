namespace LicenseRemake.DTO.AdminPanel;

public record UserListItemDto(
    Guid Id,
    string Username,
    string Role,
    bool IsActive,
    DateTime CreatedAt
);