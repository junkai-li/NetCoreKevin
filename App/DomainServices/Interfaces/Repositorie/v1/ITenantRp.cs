using kevin.Domain.Entities;
using kevin.Domain.Interface;
using kevin.Domain.Kevin;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace App.Domain.Interfaces.Repositorie.v1
{
    public interface ITenantRp : IRepository<TTenant, Guid>
    {

    }
}
