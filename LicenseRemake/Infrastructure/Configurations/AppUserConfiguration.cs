using LicenseRemake.Domain;
using Microsoft.EntityFrameworkCore;

namespace LicenseRemake.Infrastructure.Configurations
{
    public class AppUserConfiguration : IDbTableRegistration
    {
        public void FillModel(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<AppUser>(entity =>
            {
                entity.HasKey(x => x.Id);
                entity.Property(x => x.Username).IsRequired();
                entity.Property(x => x.PasswordHash).IsRequired();
                entity.Property(x => x.Role).IsRequired();
                entity.HasIndex(x => x.Username).IsUnique();
                entity.Property(x => x.Id).HasColumnType("uuid");
            });
        }
    }

    public interface IDbTableRegistration {
        public void FillModel(ModelBuilder modelBuilder);
    };


}
