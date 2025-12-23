using kevin.Domain.Configuration;
using kevin.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Kevin.EntityFrameworkCore.Configuration
{
    public class TUserConfiguration : IEntityTypeConfiguration<TUser>
    {
        public void Configure(EntityTypeBuilder<TUser> builder)
        { 
            builder.HasData(
                TUserBaseData.TUsers
                );

        }
    }
}
