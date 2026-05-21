using kevin.Domain.BaseDatas;
using kevin.Domain.Entities.AI;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Kevin.EntityFrameworkCore.Configuration
{
    public class TAIPromptsConfig : IEntityTypeConfiguration<TAIPrompts>
    {
        public void Configure(EntityTypeBuilder<TAIPrompts> builder)
        {
            builder.HasData(
                  TAIPromptsBaseDatas.Data
                  );
        }
    }
}
