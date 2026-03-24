using LicenseRemake.Domain;
using LicenseRemake.DTO.AdminPanel;

public interface IAccountService
{
    Task CreateAdminAsync(CancellationToken ct);

    Task<Guid> CreateUserAsync(string userName, string password, int userTypeId, CancellationToken ct);

    Task<UserDto> ChangePasswordAsync(Guid userId, string newPassword, CancellationToken ct);

    Task<UserDto> ChangeBlockStatusAsync(Guid userId, bool isActive, CancellationToken ct);

    Task<UserDto> DeleteUserAsync(Guid userId, CancellationToken ct);

    Task<IEnumerable<UserDto>> GetAllAsync(CancellationToken ct);
}