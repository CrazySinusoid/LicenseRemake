using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace LicenseRemake.Domain;


public class CashRegister
{
    public Guid Id { get; set; } = Guid.NewGuid();

    public string FcNumber { get; set; } = null!;

    public bool IsBlocked { get; set; }

    public DateTime? CurrentLicenseExpiration { get; set; }

    public DateTime CreatedAt { get; set; }

    public DateTime UpdatedAt { get; set; }
}