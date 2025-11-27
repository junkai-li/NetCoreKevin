using kevin.Domain.BaseDatas;
using kevin.Domain.Entities.Organizational;
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
