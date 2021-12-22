using Service.Services.Bases._;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Service.Services.v1._
{
    public interface  IUserService: IBaseService
    {
        public int GetId();
    }
}
