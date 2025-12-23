using kevin.Domain.BaseDatas;
using kevin.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Kevin.EntityFrameworkCore.Configuration
{
    internal class TRoleConfiguration : IEntityTypeConfiguration<TRole>
    {
        public void Configure(EntityTypeBuilder<TRole> builder)
        {
            builder.HasData(
                  TRoleBaseData.TRoles
                  );
        }
    }
}
