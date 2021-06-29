using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Repository.Database;

namespace TaskAdmin.Controllers
{


    public class UserController : Controller
    {


        private readonly dbContext db;

        public UserController(dbContext context)
        {
            db = context;
        }


        public IActionResult Login()
        {
            return View();
        }


        public JsonResult LoginAction(string name, string pwd)
        {
            var Data = new { status = true };


            if (name == "admin" && pwd == "123456")
            {
                HttpContext.Session.SetString("userId", "admin");
            }
            else
            {
                Data = new { status = false };
            }


            return Json(Data);
        }

    }
}
