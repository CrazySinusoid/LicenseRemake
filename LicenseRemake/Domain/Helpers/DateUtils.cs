namespace LicenseRemake.Domain.Helpers;

public static class DateUtils
{
    private static readonly TimeZoneInfo KyivTz =
        TimeZoneInfo.FindSystemTimeZoneById(
            OperatingSystem.IsWindows()
                ? "FLE Standard Time"
                : "Europe/Kyiv");

    public static DateTime ToKyiv(DateTime utc)
    {
        if (utc.Kind != DateTimeKind.Utc)
            utc = DateTime.SpecifyKind(utc, DateTimeKind.Utc);

        return TimeZoneInfo.ConvertTimeFromUtc(utc, KyivTz);
    }

    public static DateTime ToUtc(DateTime date)
    {
        return date.Kind switch
        {
            DateTimeKind.Utc => date,
            DateTimeKind.Local => date.ToUniversalTime(),
            DateTimeKind.Unspecified => TimeZoneInfo.ConvertTimeToUtc(date, KyivTz),
            _ => date
        };
    }
}