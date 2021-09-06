using Microsoft.AspNetCore.Mvc;
using Repository.Database;  
using Models.Extension;
using Web.Extension.Autofac;
using Web.Global.User;
using Service.Services.v1;

namespace WebApi.Controllers.Bases
{
    public class PubilcControllerBase: ControllerBase
    {
        [Autowired]
        public dbContext db { get; set; }

        [Autowired]
        public ICurrentUser CurrentUser{ get; set; } 
    }
}
