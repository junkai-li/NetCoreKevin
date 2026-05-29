namespace kevin.Domain.Interfaces.IServices
{
    /// <summary>
    /// ����ӿ�
    /// </summary>
    public interface IRegionTownService : IBaseService
    {
        /// <summary>
        /// ��ȡ��ҳ����
        /// </summary>
        /// <param name="dtoPagePar"></param>
        /// <returns></returns>
        Task<dtoPageData<TRegionTown>> GetPageData(dtoPagePar<string> dtoPagePar);

        /// <summary>
        /// ������༭
        /// </summary>
        /// <param name="data"></param>
        /// <returns></returns>
        Task<bool> AddEdit(TRegionTown data);

        /// <summary>
        /// ɾ��
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        Task<bool> Delete(long id);
    }
}
