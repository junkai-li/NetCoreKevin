using kevin.Domain.BaseDatas;
using kevin.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Kevin.EntityFrameworkCore.Configuration
{
    public class TTenantConfiguration : IEntityTypeConfiguration<TTenant>
    {
        public void Configure(EntityTypeBuilder<TTenant> builder)
        {
            builder.HasData(
               TTenantBaseData.TTenants
               );
        }
    }
}
