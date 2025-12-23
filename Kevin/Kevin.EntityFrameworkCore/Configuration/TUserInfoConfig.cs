using kevin.Domain.BaseDatas;
using kevin.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Kevin.EntityFrameworkCore.Configuration
{
    public class TUserInfoConfig : IEntityTypeConfiguration<TUserInfo>
    {
        public void Configure(EntityTypeBuilder<TUserInfo> builder)
        {
            builder.HasData(
                TUserInfoBaseDatas.GetData()
                );

        }
    }
}
