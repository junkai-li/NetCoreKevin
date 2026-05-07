namespace kevin.RepositorieRps.Repositories
{
    public class UserBindRoleRp : Repository<TUserBindRole, long>, IUserBindRoleRp
    {
        public UserBindRoleRp(IServiceProvider serviceProvider) : base(serviceProvider)
        {
        }
    }
}
