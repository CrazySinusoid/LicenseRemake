using LicenseRemake.Domain;
using Microsoft.EntityFrameworkCore;

namespace LicenseRemake.Infrastructure.Configurations
{
    public class RequestLogConfiguration : IDbTableRegistration
    {
        public void FillModel(ModelBuilder modelBuilder)
        {

            // RequestLog
            modelBuilder.Entity<RequestLog>(entity =>
            {
                entity.HasKey(x => x.Id);
                entity.Property(x => x.Endpoint).IsRequired();
                entity.Property(x => x.Method).IsRequired();
                entity.Property(x => x.Ip).IsRequired();
                entity.Property(x => x.Id).HasColumnType("uuid");
            });
        }
    }
}
