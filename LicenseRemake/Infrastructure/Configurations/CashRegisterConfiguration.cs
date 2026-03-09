using LicenseRemake.Domain;
using Microsoft.EntityFrameworkCore;

namespace LicenseRemake.Infrastructure.Configurations
{
    public class CashRegisterConfiguration : IDbTableRegistration
    {
        public void FillModel(ModelBuilder modelBuilder)
        {
            // CashRegister
            modelBuilder.Entity<CashRegister>(entity =>
            {
                entity.HasKey(x => x.Id);
                entity.Property(x => x.FcNumber).IsRequired();
                entity.HasIndex(x => x.FcNumber).IsUnique();
                entity.Property(x => x.Id).HasColumnType("uuid");
            });
        }
    }
}
