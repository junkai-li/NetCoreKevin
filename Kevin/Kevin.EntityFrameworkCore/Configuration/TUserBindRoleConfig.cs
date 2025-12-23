using kevin.Domain.BaseDatas;
using kevin.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Kevin.EntityFrameworkCore.Configuration
{
    public class TUserBindRoleConfig : IEntityTypeConfiguration<TUserBindRole>
    {
        public void Configure(EntityTypeBuilder<TUserBindRole> builder)
        {
            builder.HasData(
                TUserBindRoleBaseData.GetUserBindRoles()
                );

        }
    }
}
