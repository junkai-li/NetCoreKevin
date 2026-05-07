namespace kevin.RepositorieRps.Repositories
{
    public class RoleRp : Repository<TRole, long>, IRoleRp
    {
        public RoleRp(IServiceProvider serviceProvider) : base(serviceProvider)
        {
        }
    }
}
