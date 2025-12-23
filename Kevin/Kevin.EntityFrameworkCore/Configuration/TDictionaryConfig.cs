using kevin.Domain.BaseDatas;
using kevin.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Kevin.EntityFrameworkCore.Configuration
{
    public class TDictionaryConfig : IEntityTypeConfiguration<TDictionary>
    {
        public void Configure(EntityTypeBuilder<TDictionary> builder)
        {
            builder.HasData(
                  TDictionaryBaseDatas.Data
                  );
        }
    }
}
