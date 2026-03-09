using System.Security.Cryptography;
using System.Text;
using LicenseRemake.Application.Interfaces;
using LicenseRemake.Domain;
using LicenseRemake.DTO.Licensing;
using LicenseRemake.External;
using LicenseRemake.Infrastructure;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;

namespace LicenseRemake.Application.Services;

public class LicensingService : ILicensingService
{
    private readonly IKassaService _kassaService;
    private readonly DataDbContext _context;
    private readonly IConfiguration _configuration;

    public LicensingService(
        IKassaService kassaService,
        DataDbContext context,
        IConfiguration configuration)
    {
        _kassaService = kassaService;
        _context = context;
        _configuration = configuration;
    }

    public async Task<LicenseResponse> RefreshLicenseAsync(
        string serialNumber,
        string hostIp,
        CancellationToken cancellationToken)
    {
        var expirationDate = await _kassaService
            .RelevantLicenseInfo(serialNumber, cancellationToken);

        if (expirationDate == null)
            throw new Exception("Cash register not registered or license not valid");

        var signature = GenerateSignature(serialNumber, expirationDate.Value);

        var now = DateTime.UtcNow;

        var cashRegister = await _context.CashRegisters
            .FirstOrDefaultAsync(x => x.FcNumber == serialNumber, cancellationToken);

        if (cashRegister == null)
        {
            cashRegister = new CashRegister
            {
                FcNumber = serialNumber,
                IsBlocked = false,
                CurrentLicenseExpiration = expirationDate,
                CreatedAt = now,
                UpdatedAt = now
            };

            _context.CashRegisters.Add(cashRegister);
        }
        else
        {
            cashRegister.CurrentLicenseExpiration = expirationDate;
            cashRegister.UpdatedAt = now;
        }

        var log = new LicenseLog
        {
            FcNumber = serialNumber,
            Host = hostIp,
            DateIssue = now,
            DateExpired = expirationDate.Value,
            Signature = signature,
            CreatedAt = now
        };

        _context.LicenseLogs.Add(log);

        await _context.SaveChangesAsync(cancellationToken);

        await _kassaService.LicenseInfoLog(
            serialNumber,
            hostIp,
            now,
            expirationDate.Value,
            signature,
            cancellationToken);

        return new LicenseResponse(
            expirationDate.Value,
            signature);
    }

    private string GenerateSignature(string serialNumber, DateTime expirationDate)
    {
        var secret = _configuration.GetSection("Audience")["Secret"]!;
        var raw = $"{serialNumber}|{expirationDate:O}|{secret}";

        using var sha256 = SHA256.Create();
        var bytes = sha256.ComputeHash(Encoding.UTF8.GetBytes(raw));
        return Convert.ToBase64String(bytes);
    }
}