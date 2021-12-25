using Service.Dtos.v1.Sign;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Service.Services.v1._
{
    public interface ISignService
    {
        int GetSignCount(string table, Guid tableId, string sign);

        bool AddSign(dtoSign addSign);

        bool DeleteSign(dtoSign deleteSign);
    }
}
