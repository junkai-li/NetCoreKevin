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
    public class TDepartmentConfig : IEntityTypeConfiguration<TDepartment>
    {
        public void Configure(EntityTypeBuilder<TDepartment> builder)
        {
            builder.HasData(
                  TDepartmentBaseDatas.Data
                  );
        }
    }
}
