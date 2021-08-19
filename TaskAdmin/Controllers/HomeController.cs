using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Repository.Database;
using TaskAdmin.Filters;

namespace TaskAdmin.Controllers
{

    [Authorize]
    public class HomeController : Controller
    {

        private readonly dbContext db;

        public HomeController(dbContext context)
        {
            db = context;
        }


        public void Index()
        {
            //进行任务注册
            Tasks.Main.Run();

            Response.Redirect("hangfire");
        }

    }
}
