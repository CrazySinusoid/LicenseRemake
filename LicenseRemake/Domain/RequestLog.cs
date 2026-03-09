namespace LicenseRemake.Domain;

public class RequestLog
{
    public Guid Id { get; set; } = Guid.NewGuid();

    public string Endpoint { get; set; } = null!;

    public string Method { get; set; } = null!;

    public string Body { get; set; } = null!;

    public string Ip { get; set; } = null!;

    public int StatusCode { get; set; }

    public DateTime CreatedAt { get; set; }
}