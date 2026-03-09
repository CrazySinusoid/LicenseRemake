namespace LicenseRemake.DTO.Licensing;

public record LicenseResponse(
    DateTime DateExpired,
    string Signature
);