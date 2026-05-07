namespace kevin.RepositorieRps.Repositories
{
    public class TenantRp : Repository<TTenant, long>, ITenantRp
    {
        public TenantRp(IServiceProvider serviceProvider) : base(serviceProvider)
        {
        }
    }
}
