using LicenseRemake.Domain;
using LicenseRemake.DTO.AdminPanel;

public interface IAccountService
{
    Task CreateAdminAsync(CancellationToken ct);

    Task<Guid> CreateUserAsync(string userName, string password, int userTypeId, CancellationToken ct);

    Task ChangePasswordAsync(Guid userId, string newPassword, CancellationToken ct);

    Task ChangeBlockStatusAsync(Guid userId, bool isActive, CancellationToken ct);

    Task DeleteUserAsync(Guid userId, CancellationToken ct);

    Task<IEnumerable<UserListItemDto>> GetAllAsync(CancellationToken ct);
}