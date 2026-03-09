namespace LicenseRemake.DTO.Licensing;

public record LicenseLogSearchResult(
    int Total,
    int Page,
    int PageSize,
    IReadOnlyList<LicenseLogDto> Data
);

public record LicenseLogDto(
    string FcNumber,
    string Host,
    DateTime DateIssue,
    DateTime DateExpired,
    string Signature
);
