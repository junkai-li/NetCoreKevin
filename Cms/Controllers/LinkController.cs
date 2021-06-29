using Cms.Filters;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Repository.Database;
using System;
using System.Collections.Generic;
using System.Linq;


namespace Web.Areas.Admin.Controllers
{


    [IsLogin]
    public class LinkController : Controller
    {


        private readonly dbContext db;

        public LinkController(dbContext context)
        {
            db = context;
        }


        public IActionResult LinkIndex()
        {
            return View();
        }


        public JsonResult GetLinkList()
        {


            var list = db.TLink.Where(t => t.IsDelete == false).OrderBy(t => t.Sort).ToList();

            return Json(new { data = list });
        }



        public IActionResult LinkEdit(Guid id)
        {

            if (id == default)
            {
                var link = new TLink();
                link.Id = Guid.NewGuid();

                ViewData["LinkInfo"] = link;

                ViewData["coverList"] = new List<TFile>();
                return View();
            }
            else
            {

                var Link = db.TLink.Where(t => t.Id == id).FirstOrDefault();

                ViewData["LinkInfo"] = Link;

                var coverList = db.TFile.Where(t => t.IsDelete == false && t.Sign == "cover" & t.Table == "TLink" & t.TableId == id).OrderBy(t => t.Sort).ToList();
                ViewData["coverList"] = coverList;


                return View();
            }
        }


        public bool LinkSave(TLink Link)
        {
            try
            {

                var nid = db.TLink.Where(t => t.Id == Link.Id).Select(t => t.Id).FirstOrDefault();

                if (nid == default)
                {
                    //执行添加

                    var userId = Guid.Parse(HttpContext.Session.GetString("userId"));

                    Link.Id = Guid.NewGuid();
                    Link.CreateTime = DateTime.Now;
                    Link.CreateUserId = userId;
                    Link.IsDelete = false;

                    db.TLink.Add(Link);
                }
                else
                {
                    //执行修改
                    var dbLink = db.TLink.Where(t => t.Id == Link.Id).FirstOrDefault();

                    dbLink.Name = Link.Name;
                    dbLink.Url = Link.Url;
                    dbLink.Remarks = Link.Remarks;
                    dbLink.Sort = Link.Sort;
                }

                db.SaveChanges();

                return true;
            }
            catch
            {
                return false;
            }
        }


        public JsonResult LinkDelete(Guid id)
        {
            var userId = Guid.Parse(HttpContext.Session.GetString("userId"));

            var Link = db.TLink.Where(t => t.Id == id).FirstOrDefault();
            Link.IsDelete = true;
            Link.DeleteTime = DateTime.Now;
            Link.DeleteUserId = userId;

            db.SaveChanges();

            var data = new { status = true, msg = "删除成功！" };
            return Json(data);

        }
    }
}
