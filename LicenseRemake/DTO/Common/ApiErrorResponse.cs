using System.Text.Json.Serialization;

namespace LicenseRemake.DTO.Common;

public class ApiErrorResponse
{
    public int err_code { get; set; }

    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
    public string? err_code_string { get; set; }

    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
    public string? err_descr { get; set; }

    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
    public string? param { get; set; }

    public DateTime time_stamp { get; set; } = DateTime.UtcNow;
}
