using LicenseRemake.Domain;
using Microsoft.EntityFrameworkCore;

namespace LicenseRemake.Infrastructure.Configurations
{
    public class RefreshTokenConfiguration : IDbTableRegistration
    {
        public void FillModel(ModelBuilder modelBuilder)
        {
            // RefreshToken
            modelBuilder.Entity<RefreshToken>(entity =>
            {
                entity.HasKey(x => x.Id);
                entity.Property(x => x.Token).IsRequired();
                entity.Property(x => x.Id).HasColumnType("uuid");
                entity.HasOne(x => x.User)
                      .WithMany(x => x.RefreshTokens)
                      .HasForeignKey(x => x.UserId)
                      .OnDelete(DeleteBehavior.Cascade);
            });
        }
    }
}
