using kevin.Domain.BaseDatas;
using kevin.Domain.Entities.AI;
using kevin.Domain.Entities.Organizational;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

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
