using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace kevin.Permission.Permission.Enums
{
    public static class DataPermissionActionConst
    {
        public static Dictionary<string, string> DataPermissionActionConsts = new Dictionary<string, string> {
            { ALL, "所有数据" },
            { MyAndChildrenDepartment, "本部门及下属部门数据" },
            { MyDepartment, "本部门数据" },
            { My, "仅本人数据" }
        };
        /// <summary>
        /// 仅本人数据
        /// </summary>
        public const string My = "My";

        /// <summary>
        /// 本部门数据
        /// </summary>
        public const string MyDepartment = "MyDepartment";

        /// <summary>
        /// 本部门及下属部门数据
        /// </summary>
        public const string MyAndChildrenDepartment = "MyAndChildrenDepartment";

        /// <summary>
        /// 所有数据
        /// </summary>
        public const string ALL = "ALL";
    }
}
