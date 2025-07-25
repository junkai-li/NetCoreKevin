using kevin.Domain.BaseDatas;
using kevin.Domain.Configuration;
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
