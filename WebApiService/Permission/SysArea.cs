using System.Collections.Generic;


namespace WebApiService.Permisson
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
