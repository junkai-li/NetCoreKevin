using kevin.Domain.Entities.AI;
namespace kevin.Domain.Interfaces.IServices.AI
{
    /// <summary>
    /// IAIAppsBindIdService
    /// </summary>
    public interface IAIAppsBindIdService : IBaseService
    {
        Task<List<TAIAppsBindId>> GetListById(string Id);
        Task<bool> BatchAddIds(string Id, List<string> ids);
    }
}
