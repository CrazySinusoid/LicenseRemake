namespace LicenseRemake.DTO.Licensing;

public record LicenseLogSearchRequest(
    string? SerialNumber,
    DateTime? DateFrom,
    DateTime? DateTo,
    int Page = 1,
    int PageSize = 50
);
