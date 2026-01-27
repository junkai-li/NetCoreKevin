using System.Reflection;

namespace kevin.Permission.Permisson
{
    /// <summary>
    /// 应用全局缓存
    /// </summary>
    public class GlobalData
    {
        /// <summary>
        /// Controllers
        /// </summary>
        private List<Type> _allControllers = new List<Type>();
        public List<Type> AllControllers
        {
            get
            {
                if (_allControllers == null)
                {
                    _allControllers = FrameworkServiceExtension.GetAllControllers(AllAssembly);
                }
                return _allControllers;
            }
        }
        /// <summary>
        /// 程序集
        /// </summary>
        private List<Assembly> _allAssembly = new List<Assembly>();
        public List<Assembly> AllAssembly
        {
            get
            {
                if (_allAssembly == null)
                {
                    _allAssembly = Utils.GetAllAssembly();
                }
                return _allAssembly;
            }
        }


        /// <summary>
        /// 可访问的url地址
        /// </summary>

        private List<string> _allAccessUrls = new List<string>();
        public List<string> AllAccessUrls
        {
            get
            {
                if (_allAccessUrls == null)
                {
                    _allAccessUrls = FrameworkServiceExtension.GetAllAccessUrls(AllControllers);
                }
                return _allAccessUrls;
            }
        }

        /// <summary>
        /// 模块
        /// </summary>
        private List<SysCtrl> _allModules = new List<SysCtrl>();
        public List<SysCtrl> AllModule
        {
            get
            {
                if (_allModules == null)
                {
                    _allModules = FrameworkServiceExtension.GetAllModules(AllControllers);
                }
                return _allModules;
            }
        }
        /// <summary>
        /// 菜单
        /// </summary>
        private List<PermissionDto> _allModule = new List<PermissionDto>();
        public List<PermissionDto> AllModules
        {
            get
            {
                if (_allModule == null)
                {
                    _allModule = FrameworkServiceExtension.GetAllMenus(AllModule);
                }
                return _allModule;
            }
        }
    }
}
