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
    public class TUserBindRoleConfig : IEntityTypeConfiguration<TUserBindRole>
    {
        public void Configure(EntityTypeBuilder<TUserBindRole> builder)
        {
            builder.HasData(
                TUserBindRoleBaseData.GetUserBindRoles()
                );

        }
    }
}
