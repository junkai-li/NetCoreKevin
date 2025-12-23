using kevin.Domain.BaseDatas;
using kevin.Domain.Entities.Organizational;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

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
