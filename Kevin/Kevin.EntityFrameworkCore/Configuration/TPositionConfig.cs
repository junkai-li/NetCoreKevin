using kevin.Domain.BaseDatas;
using kevin.Domain.Entities.Organizational;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Kevin.EntityFrameworkCore.Configuration
{
    internal class TPositionConfig : IEntityTypeConfiguration<TPosition>
    {
        public void Configure(EntityTypeBuilder<TPosition> builder)
        {
            builder.HasData(
                  TPositionBaseDatas.Data
                  );
        }
    }
}
