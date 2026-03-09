using LicenseRemake.Domain;

public interface IAccountService
{
    Task CreateAdminAsync(CancellationToken ct);

    Task CreateUserAsync(string userName, string password, int userTypeId, CancellationToken ct);

    Task ChangePasswordAsync(Guid userId, string newPassword, CancellationToken ct);

    Task ChangeBlockStatusAsync(Guid userId, bool isActive, CancellationToken ct);

    Task DeleteUserAsync(Guid userId, CancellationToken ct);

    Task<IEnumerable<AppUser>> GetAllAsync(CancellationToken ct);
}