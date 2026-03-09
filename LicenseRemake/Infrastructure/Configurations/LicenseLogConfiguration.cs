using LicenseRemake.Domain;
using Microsoft.EntityFrameworkCore;

namespace LicenseRemake.Infrastructure.Configurations
{
    public class LicenseLogConfiguration : IDbTableRegistration
    {
        public void FillModel(ModelBuilder modelBuilder)
        {
            // CashRegister
            modelBuilder.Entity<LicenseLog>(entity =>
            {
                entity.HasKey(x => x.Id);
                entity.Property(x => x.FcNumber).IsRequired();
                entity.Property(x => x.Host).IsRequired();
                entity.Property(x => x.Signature).IsRequired();
                entity.Property(x => x.Id).HasColumnType("uuid");
            });
        }
    }
}
