using kevin.Domain.Share.Dtos.AI;
namespace kevin.Domain.Interfaces.IServices.AI
{
    /// <summary>
    /// SkillToolManagement
    /// </summary>
    public interface IAISkillToolManagementService : IBaseService
    {
        /// <summary>
        ///   获取所有Skill技能
        /// </summary> 
        /// <returns></returns>
        Task<List<AISkillToolManagementDto>> GetAllSkills();

        /// <summary>
        /// 根据Id获取Skill技能
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        Task<AISkillToolManagementDto> GetById(long id);
        /// <summary>
        ///   获取所有Tool工具
        /// </summary> 
        /// <returns></returns>
        Task<List<AISkillToolManagementDto>> GetAllTools();
        /// <summary>
        ///SkillToolManagement 分页获取
        /// </summary>
        /// <param name="dtoPagePar"></param>
        /// <returns></returns>
        Task<dtoPageData<AISkillToolManagementDto>> GetPageData(dtoPagePar<int> dtoPagePar);

        /// <summary>
        /// 新增编辑
        /// </summary>
        /// <param name="data"></param>
        /// <returns></returns>
        Task<bool> AddEdit(AISkillToolManagementDto data);

        /// <summary>
        /// 删除
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        Task<bool> Delete(long id);
    }
}
