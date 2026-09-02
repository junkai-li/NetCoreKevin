using Aop.Api.Domain;
using kevin.Domain.Entities.AI;
using kevin.Domain.Interfaces.IRepositories.AI;
using kevin.Domain.Interfaces.IServices.AI;
using kevin.Domain.Share.Dtos.AI;
using kevin.Domain.Share.Enums;
using kevin.FileStorage;
using kevin.RepositorieRps.Repositories;
using Kevin.Common;
using Kevin.Common.Helper;
using System.Text.RegularExpressions;

namespace kevin.Application.Services.AI
{
    /// <summary>
    /// AISkillToolManagement服务接口
    /// </summary>
    public class AISkillToolManagementService : BaseService, IAISkillToolManagementService
    {
        public readonly IAISkillToolManagementRp AISkillToolManagementRp;
        public readonly IFileRp _FileRp;

        public readonly IFileStorage _FileStorage;
        public AISkillToolManagementService(IHttpContextAccessor _httpContextAccessor, IAISkillToolManagementRp _AISkillToolManagementRp, IFileRp _IFileRp, IFileStorage _IFileStorage) : base(_httpContextAccessor)
        {
            this.AISkillToolManagementRp = _AISkillToolManagementRp;
            this._FileRp = _IFileRp;
            this._FileStorage = _IFileStorage;
        }

        public async Task<dtoPageData<AISkillToolManagementDto>> GetPageData(dtoPagePar<int> dtoPagePar)
        {
            int skip = dtoPagePar.GetSkip();
            var result = new dtoPageData<AISkillToolManagementDto>();
            var data = AISkillToolManagementRp.Query(isDataPer: true).Where(t => t.IsDelete == false);
            if ((AISkillToolTypeEnums)dtoPagePar.Parameter == AISkillToolTypeEnums.Tool)
            {
                data = AISkillToolManagementRp.Query(isDataPer: false).Where(t => t.IsDelete == false);
            }
            if (!string.IsNullOrEmpty(dtoPagePar.searchKey))
            {
                data = data.Where(t => (t.Name ?? "").Contains(dtoPagePar.searchKey) || (t.Description ?? "").Contains(dtoPagePar.searchKey));
            }
            if (dtoPagePar.Parameter != null && dtoPagePar.Parameter > 0)
            {
                data = data.Where(t => t.SkillToolType == (AISkillToolTypeEnums)dtoPagePar.Parameter);
            }
            result.total = await data.CountAsync();
            var dbdata = await data.OrderByDescending(x => x.CreateTime).Skip(skip).Take(dtoPagePar.pageSize).Include(t => t.CreateUser).Include(t => t.UpdateUser).OrderByDescending(t => t.CreateTime).ToListAsync();
            result.data = dbdata.MapToList<TAISkillToolManagement, AISkillToolManagementDto>();
            var flieData = _FileRp.Query().Where(t => t.IsDelete == false && t.Table == "AISkillToolManagement" && t.Sign == "SkillZip" && result.data.Select(a => a.Id.ToString()).ToList().Contains(t.TableId)).ToList().MapToList<TFile, FileDto>().ToList();
            foreach (var item in result.data)
            {
                item.SkillFile = flieData.Where(t => t.TableId == item.Id.ToString()).OrderByDescending(t => t.CreateTime).FirstOrDefault();
                item.CreateUser = dbdata.FirstOrDefault(d => d.Id == item.Id)?.CreateUser?.Name;
                item.UpdateUser = dbdata.FirstOrDefault(d => d.Id == item.Id)?.UpdateUser?.Name;
            }
            result.pageSize = dtoPagePar.pageSize;
            result.pageNum = dtoPagePar.pageNum;
            return result;
        }
        /// <summary>
        /// 根据id获取数据
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public async Task<AISkillToolManagementDto> GetById(long id)
        {
            var data = await AISkillToolManagementRp.Query(isDataPer: true).Where(t => t.IsDelete == false && t.Id == id).Include(t => t.CreateUser).Include(t => t.UpdateUser).FirstOrDefaultAsync();
            var result = data.MapTo<AISkillToolManagementDto>();
            var flieData = _FileRp.Query().Where(t => t.IsDelete == false && t.Table == "AISkillToolManagement" && t.Sign == "SkillZip" && id.ToString() == t.TableId).ToList().MapToList<TFile, FileDto>().ToList();
            result.SkillFile = flieData.OrderByDescending(t => t.CreateTime).FirstOrDefault();
            result.CreateUser = data?.CreateUser?.Name;
            result.UpdateUser = data?.UpdateUser?.Name;
            return result;
        }

        public async Task<bool> AddEdit(AISkillToolManagementDto data)
        {
            var isAdd = data.Id == default;
            data.Check();
            if (!isAdd)
            {
                var msg = AISkillToolManagementRp.Query().Where(t => t.IsDelete == false && t.Id == data.Id).FirstOrDefault();
                if (msg == default)
                {
                    isAdd = true;
                }
            }
            var NameData = AISkillToolManagementRp.Query(isTenant: false).Where(t => t.Name == data.Name && t.IsDelete == false && t.SkillToolType == data.SkillToolType).FirstOrDefault();
            if (isAdd)
            {
                //验证工具名称唯一不允许添加
                if (NameData != default)
                {
                    throw new UserFriendlyException($"{data.Name}:工具技能名称已存在");
                }
                var add = data.MapTo<TAISkillToolManagement>();
                add.Id = data.Id == default ? SnowflakeIdService.GetNextId() : data.Id;
                add.IsDelete = false;
                add.IsSystem = false;
                add.CreateTime = DateTime.Now;
                add.CreateUserId = CurrentUser.UserId;
                add.TenantId = CurrentUser.TenantId;
                AISkillToolManagementRp.Add(add);
            }
            else
            {
                //验证工具名称唯一不允许添加
                if (NameData != default && data.Id != NameData.Id)
                {
                    throw new UserFriendlyException($"{data.Name}:工具技能名称已存在");
                }
                var upData = AISkillToolManagementRp.Query().Where(t => t.IsDelete == false && t.Id == data.Id).FirstOrDefault();
                if (upData != default)
                {
                    if (upData.IsSystem)
                    {
                        throw new UserFriendlyException("系统内置工具不允许修改");
                    }
                    upData.Name = data.Name;
                    upData.SkillToolType = data.SkillToolType;
                    upData.ActiveStatus = data.ActiveStatus;
                    upData.ClassMethod = data.ClassMethod;
                    upData.Description = data.Description;
                    upData.McpUrl = data.McpUrl;
                    upData.McpType = data.McpType;
                    upData.McpHeaders = data.McpHeaders;
                    upData.McpCommand = data.McpCommand;
                    upData.McpArguments = data.McpArguments;
                    upData.McpEnvironment = data.McpEnvironment;
                    upData.UpdateTime = DateTime.Now;
                    upData.UpdateUserId = CurrentUser.UserId;
                    upData.TenantId = CurrentUser.TenantId;
                    upData.IsDelete = false;
                    upData.IsSystem = false;

                }
                else
                {
                    throw new UserFriendlyException("数据不存在或已删除");
                }
            }
            if (data.SkillToolType == AISkillToolTypeEnums.Skill)
            {

                //处理skill技能附件包
                var flieData = _FileRp.Query().Where(t => t.IsDelete == false && t.Table == "AISkillToolManagement" && t.Sign == "SkillZip" && data.Id.ToString() == t.TableId).OrderByDescending(t => t.CreateTime).FirstOrDefault(); ;
                if (flieData != default && !string.IsNullOrEmpty(flieData.Url))
                {
                    //拼接路径
                    var path = Path.Combine(AppContext.BaseDirectory, "Skills", data.Name, data.Name);

                    //如果目录存在则删除目录下的所有文件  
                    if (Directory.Exists(path))
                    {
                        Directory.Delete(path, true);
                    }
                    Directory.CreateDirectory(path);
                    _FileStorage.FileDownload(flieData.Url, path + flieData.Name);
                    //将zip文件流解压到写入磁盘
                    using (var fileStream = File.OpenRead(path + flieData.Name))
                    {
                        FileZipHelper.ExtractZipStreamToDirectory(fileStream, path);
                    }
                    File.Delete(path + flieData.Name);

                    //校验技能包脚本(.py/.ps1/.sh)中提取到的http(s)地址是否都在授权前缀白名单内
                    //存在非授权域名地址时，先清理已解压文件再抛出异常，避免非授权脚本残留磁盘
                    try
                    {
                        ValidateSkillScriptUrls(path);
                    }
                    catch
                    {
                        if (Directory.Exists(path))
                        {
                            Directory.Delete(path, true);
                        }
                        throw;
                    }
                }
                else
                {
                    throw new UserFriendlyException("请上传skill技能附件包");
                }
            }

            await AISkillToolManagementRp.SaveChangesAsync();
            return true;


        }

        public async Task<bool> Delete(long id)
        {
            var data = await AISkillToolManagementRp.Query(isDataPer: true).Where(t => t.IsDelete == false && t.Id == id).FirstOrDefaultAsync();
            if (data != default)
            {
                if (data.IsSystem)
                {
                    throw new UserFriendlyException("系统内置工具不允许删除");
                }
                data.IsDelete = true;
                data.DeleteTime = DateTime.Now;
                AISkillToolManagementRp.SaveChangesWithSaveLog();
                //删除skill技能附件包
                var flieData = _FileRp.Query().Where(t => t.IsDelete == false && t.Table == "AISkillToolManagement" && t.Sign == "SkillZip" && data.Id.ToString() == t.TableId).OrderByDescending(t => t.CreateTime).FirstOrDefault();
                if (flieData != default && !string.IsNullOrEmpty(flieData.Url))
                {
                    var path = Path.Combine(AppContext.BaseDirectory, "Skills", data.Name, data.Name);
                    if (Directory.Exists(path))
                    {
                        Directory.Delete(path, true);
                    }
                }
            }
            else
            {
                throw new UserFriendlyException("数据不存在或已删除");
            }
            return true;
        }

        public async Task<List<AISkillToolManagementDto>> GetAllSkills()
        {
            return (await AISkillToolManagementRp.Query(isDataPer: true).Where(t => t.IsDelete == false && t.SkillToolType == AISkillToolTypeEnums.Skill && t.ActiveStatus == InActiveStatusEnums.Active).OrderByDescending(t => t.CreateTime).ToListAsync()).MapToList<TAISkillToolManagement, AISkillToolManagementDto>();

        }

        public async Task<List<AISkillToolManagementDto>> GetAllTools()
        {
            return (await AISkillToolManagementRp.Query(isDataPer: false).Where(t => t.IsDelete == false && t.SkillToolType == AISkillToolTypeEnums.Tool && t.ActiveStatus == InActiveStatusEnums.Active).OrderByDescending(t => t.CreateTime).ToListAsync()).MapToList<TAISkillToolManagement, AISkillToolManagementDto>();

        }
        /// <summary>
        /// 不受数据权限控制的技能工具列表（主要用于系统管理员等特殊角色使用）
        /// </summary>
        /// <returns></returns>
        public async Task<List<AISkillToolManagementDto>> GetNotDataPerAllSkills()
        {
            return (await AISkillToolManagementRp.Query().Where(t => t.IsDelete == false && t.SkillToolType == AISkillToolTypeEnums.Skill && t.ActiveStatus == InActiveStatusEnums.Active).OrderByDescending(t => t.CreateTime).ToListAsync()).MapToList<TAISkillToolManagement, AISkillToolManagementDto>();

        }
        /// <summary>
        /// 不受数据权限控制的技能工具列表（主要用于系统管理员等特殊角色使用）
        /// </summary>
        /// <returns></returns>
        public async Task<List<AISkillToolManagementDto>> GetNotDataPerAllTools()
        {
            return (await AISkillToolManagementRp.Query().Where(t => t.IsDelete == false && t.SkillToolType == AISkillToolTypeEnums.Tool && t.ActiveStatus == InActiveStatusEnums.Active).OrderByDescending(t => t.CreateTime).ToListAsync()).MapToList<TAISkillToolManagement, AISkillToolManagementDto>();

        }

        public async Task<List<AISkillToolManagementDto>> GetAllMcps()
        {
            return (await AISkillToolManagementRp.Query(isDataPer: true).Where(t => t.IsDelete == false && t.SkillToolType == AISkillToolTypeEnums.Mcp && t.ActiveStatus == InActiveStatusEnums.Active).OrderByDescending(t => t.CreateTime).ToListAsync()).MapToList<TAISkillToolManagement, AISkillToolManagementDto>();
        }

        /// <summary>
        /// 不受数据权限控制的技能工具列表（主要用于系统管理员等特殊角色使用）
        /// </summary>
        /// <returns></returns>
        public async Task<List<AISkillToolManagementDto>> GetNotDataPerAllMcps()
        {
            return (await AISkillToolManagementRp.Query().Where(t => t.IsDelete == false && t.SkillToolType == AISkillToolTypeEnums.Mcp && t.ActiveStatus == InActiveStatusEnums.Active).OrderByDescending(t => t.CreateTime).ToListAsync()).MapToList<TAISkillToolManagement, AISkillToolManagementDto>();

        }

        /// <summary>
        /// 校验skill技能包脚本(.py/.ps1/.sh)中提取到的http(s)地址是否都在授权前缀白名单内。
        /// 白名单读取自配置节点 SkillToolSecuritySetting:AllowedUrlPrefixes；未配置时不做限制。
        /// 存在非授权域名地址时抛出异常，并指明具体文件，提示联系开发人员。
        /// </summary>
        /// <param name="skillDirectory">技能包解压后的根目录</param>
        private void ValidateSkillScriptUrls(string skillDirectory)
        {
            if (string.IsNullOrEmpty(skillDirectory) || !Directory.Exists(skillDirectory))
            {
                return;
            } 
            if (ConfigHelper.Configuration["SkillToolSecuritySetting:IsOpenAllowedUrlPrefixes"].ToBoolean() == false)
            {
                return;
            }
            //读取授权地址前缀白名单（appsettings 节点：SkillToolSecuritySetting:AllowedUrlPrefixes）
            var allowedPrefixes = ConfigHelper.GetSection<List<string>>("SkillToolSecuritySetting:AllowedUrlPrefixes")?
                .Where(t => !string.IsNullOrWhiteSpace(t))
                .Select(t => t.Trim())
                .ToList();

            //未配置白名单时不做限制
            if (allowedPrefixes == null || allowedPrefixes.Count == 0)
            {
                return;
            }

            //需要扫描的脚本文件后缀
            var scanExtensions = new[] { ".py", ".ps1", ".sh" };
            //匹配 http:// 或 https:// 地址，遇到空白、单双引号、尖括号等即结束
            var urlRegex = new Regex(@"https?://[^\s'""<>]+", RegexOptions.IgnoreCase);

            var violations = new List<string>();
            var scriptFiles = Directory.EnumerateFiles(skillDirectory, "*.*", SearchOption.AllDirectories)
                .Where(f => scanExtensions.Contains(Path.GetExtension(f).ToLowerInvariant()));

            foreach (var file in scriptFiles)
            {
                string content;
                try
                {
                    content = File.ReadAllText(file);
                }
                catch
                {
                    //无法读取的文件（如编码异常）跳过
                    continue;
                }

                foreach (Match match in urlRegex.Matches(content))
                {
                    //去除地址末尾可能粘连的标点符号
                    var url = match.Value.TrimEnd('.', ',', ';', ':', ')', ']', '}');
                    //只要命中任意一个授权前缀即视为合法（忽略大小写）
                    var isAllowed = allowedPrefixes.Any(prefix => url.StartsWith(prefix, StringComparison.OrdinalIgnoreCase));
                    if (!isAllowed)
                    {
                        //使用相对技能包根目录的路径，便于定位存在非授权地址的文件
                        var relativePath = Path.GetRelativePath(skillDirectory, file);
                        violations.Add($"文件[{relativePath}]存在非授权域名地址：{url}");
                    }
                }
            }

            if (violations.Count > 0)
            {
                //去重，避免同一地址重复出现导致提示信息冗长
                var distinctViolations = violations.Distinct().ToList();
                throw new UserFriendlyException(
                    $"skill技能包脚本中存在非授权域名地址，请联系开发人员！授权前缀白名单：{string.Join("、", allowedPrefixes)}。{Environment.NewLine}{string.Join(Environment.NewLine, distinctViolations)}");
            }
        }
    }
}
