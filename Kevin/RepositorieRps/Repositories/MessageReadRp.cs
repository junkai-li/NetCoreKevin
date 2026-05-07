namespace kevin.RepositorieRps.Repositories
{

    public class MessageReadRp : Repository<TMessageRead, long>, IMessageReadRp
    {
        public MessageReadRp(IServiceProvider serviceProvider) : base(serviceProvider)
        {
        }
    }
}
