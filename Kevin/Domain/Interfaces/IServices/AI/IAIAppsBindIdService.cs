using kevin.Domain.Entities.AI;
namespace kevin.Domain.Interfaces.IServices.AI
{
    /// <summary>
    /// IAIAppsBindIdService
    /// </summary>
    public interface IAIAppsBindIdService : IBaseService
    {
        Task<List<TAIAppsBindId>> GetListByBindId(string aIAppsId);
        Task<List<TAIAppsBindId>> GetListById(List<string> bindIds);
        Task<bool> BatchAddIds(string Id, List<string> ids);
    }
}
