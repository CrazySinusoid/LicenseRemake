namespace LicenseRemake.Domain;

public class LicenseLog
{
    public Guid Id { get; set; } = Guid.NewGuid();

    public string FcNumber { get; set; } = null!;

    public string Host { get; set; } = null!;

    public DateTime DateIssue { get; set; }

    public DateTime DateExpired { get; set; }

    public string Signature { get; set; } = null!;

    public DateTime CreatedAt { get; set; }
}