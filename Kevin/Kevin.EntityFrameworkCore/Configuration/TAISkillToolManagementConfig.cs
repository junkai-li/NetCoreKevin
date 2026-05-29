using kevin.Domain.BaseDatas;
using kevin.Domain.Entities.AI;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Kevin.EntityFrameworkCore.Configuration
{
    public class TAISkillToolManagementConfig : IEntityTypeConfiguration<TAISkillToolManagement>
    {
        public void Configure(EntityTypeBuilder<TAISkillToolManagement> builder)
        {
            builder.HasData(
                  TAISkillToolManagementBaseDatas.Data
                  );
        }
    }
}
