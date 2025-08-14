using kevin.Domain.BaseDatas;
using kevin.Domain.Configuration;
using kevin.Domain.Entities;
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
