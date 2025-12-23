using Kevin.EntityFrameworkCore._.Data;

namespace kevin.RepositorieRps.Repositories
{ 
    public class MessageRp : Repository<TMessage, long>, IMessageRp
    {
        public MessageRp(IServiceProvider serviceProvider) : base(serviceProvider)
        {
        }
    }
}
