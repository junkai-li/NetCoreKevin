namespace kevin.RepositorieRps.Repositories
{
    public class UserBindPositionRp : Repository<TUserBindPosition, long>, IUserBindPositionRp
    {
        public UserBindPositionRp(IServiceProvider serviceProvider) : base(serviceProvider)
        {
        }
    }
}
