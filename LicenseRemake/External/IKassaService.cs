namespace LicenseRemake.External;

public interface IKassaService
{
    Task<DateTime?> RelevantLicenseInfo(
        string serialNumber,
        CancellationToken cancellationToken);

    Task LicenseInfoLog(
        string fcNumber,
        string host,
        DateTime dateIssue,
        DateTime expired,
        string signature,
        CancellationToken cancellationToken);
}
