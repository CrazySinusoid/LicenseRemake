using LicenseRemake.Domain;
using LicenseRemake.External;
using LicenseRemake.Infrastructure.Configurations;
using Microsoft.EntityFrameworkCore;

namespace LicenseRemake.Infrastructure;

public class DataDbContext : DbContext
{

    private readonly IServiceProvider _provider;
    public DataDbContext(DbContextOptions<DataDbContext> options, IServiceProvider provider)
        : base(options)
    {
        _provider = provider;
    }

    public DbSet<AppUser> AppUsers => Set<AppUser>();
    public DbSet<RefreshToken> RefreshTokens => Set<RefreshToken>();
    public DbSet<CashRegister> CashRegisters => Set<CashRegister>();
    public DbSet<LicenseLog> LicenseLogs => Set<LicenseLog>();
    public DbSet<RequestLog> RequestLogs => Set<RequestLog>();

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        var collectionService = _provider.GetServices<IDbTableRegistration>();
        foreach (var coll in collectionService)
            coll.FillModel(modelBuilder);
    }
}