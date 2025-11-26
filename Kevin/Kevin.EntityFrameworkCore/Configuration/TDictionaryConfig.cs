using kevin.Domain.BaseDatas;
using kevin.Domain.Kevin;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

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
