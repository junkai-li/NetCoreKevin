using Microsoft.AspNetCore.Mvc;
using Repository.Database;  
using Models.Extension;
using Web.Extension.Autofac;

namespace WebApi.Controllers.Bases
{
    public class PubilcControllerBase: ControllerBase
    {
        [Autowired]
        public  dbContext db; 

    }
}
