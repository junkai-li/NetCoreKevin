using System.Collections.Generic;


namespace kevin.Permission.Permisson
{
    /// <summary>
    /// FrameworkArea
    /// </summary>
    public class SysArea
    {
        public string AreaName { get; set; }

        public string Prefix { get; set; }

        public List<SysCtrl> Modules { get; set; }

    }

}
