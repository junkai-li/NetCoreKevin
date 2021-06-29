using Microsoft.AspNetCore.Mvc;
using Repository.Database;  
using Models.Extension;

namespace WebApi.Controllers.Bases
{
    public class PubilcControllerBase: ControllerBase
    {
        public  dbContext db ; 

    }
}
