namespace LicenseRemake.External;

public class MockKassaService : IKassaService
{
    public Task<DateTime?> RelevantLicenseInfo(
        string serialNumber,
        CancellationToken cancellationToken)
    {
        if (serialNumber == "INVALID")
            return Task.FromResult<DateTime?>(null);

        return Task.FromResult<DateTime?>(DateTime.UtcNow.AddMonths(1));
    }

    public Task LicenseInfoLog(
        string fcNumber,
        string host,
        DateTime dateIssue,
        DateTime expired,
        string signature,
        CancellationToken cancellationToken)
    {
        return Task.CompletedTask;
    }
}
