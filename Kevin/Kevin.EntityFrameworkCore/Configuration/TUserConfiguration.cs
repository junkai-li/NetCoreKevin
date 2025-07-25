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
