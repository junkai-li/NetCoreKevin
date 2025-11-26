using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace kevin.Domain.Share.Enums
{
    /// <summary>
    /// 员工状态 -1:离职 1:在职 2:休假 3:停职 4:退休 5:实习
    /// </summary>
    public enum EmployeeStatus
    {
        Resignation=-1,
        OnDuty=1,
        Vacation=2,
        Suspension=3,
        Retire=4, 
        Internship=5 
    }
}
