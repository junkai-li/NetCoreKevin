using kevin.Domain.BaseDatas;
using kevin.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Kevin.EntityFrameworkCore.Configuration
{
    public class TPermissionConfig : IEntityTypeConfiguration<TPermission>
    {
        public void Configure(EntityTypeBuilder<TPermission> builder)
        {
            builder.HasData(
                  TPermissionBaseDatas.Data
                  );
        }
    }
}
