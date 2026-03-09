using LicenseRemake.DTO.Licensing;

namespace LicenseRemake.Application.Interfaces;

public interface ILicenseLogService
{
    Task<LicenseLogSearchResult> SearchAsync(LicenseLogSearchRequest request, CancellationToken cancellationToken);
}
