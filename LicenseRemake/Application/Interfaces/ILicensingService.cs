using LicenseRemake.DTO.Licensing;

namespace LicenseRemake.Application.Interfaces;

public interface ILicensingService
{
    Task<LicenseResponse> RefreshLicenseAsync(
        string serialNumber,
        string hostIp,
        CancellationToken cancellationToken);
}
