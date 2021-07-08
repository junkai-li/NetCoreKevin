using Cms.Filters;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using Repository.Database;

namespace Cms.Controllers
{

    [AuthenticationFilter]
    public class ArticleController : Controller
    {

        private readonly dbContext db;

        public ArticleController(dbContext context)
        {
            db = context;
        }


        public IActionResult ChannelIndex()
        {
            return View();
        }


        public JsonResult GetChannelList()
        {
            var list = db.TChannel.Where(t => t.IsDelete == false).OrderBy(t => t.Sort).ToList();

            return Json(new { data = list });
        }


        public IActionResult ChannelEdit(Guid id)
        {

            if (id == default)
            {
                return View(new TChannel());
            }
            else
            {

                var Channel = db.TChannel.Where(t => t.Id == id).FirstOrDefault();
                return View(Channel);
            }

        }


        public bool ChannelSave(TChannel Channel)
        {


            if (Channel.Id == default)
            {
                //执行添加
                Channel.Id = Guid.NewGuid();

                var userid = Guid.Parse(HttpContext.Session.GetString("userId"));

                Channel.CreateTime = DateTime.Now;
                Channel.CreateUserId = userid;
                Channel.IsDelete = false;

                db.TChannel.Add(Channel);
            }
            else
            {
                //执行修改
                var dbChannel = db.TChannel.Where(t => t.Id == Channel.Id).FirstOrDefault();

                dbChannel.Name = Channel.Name;
                dbChannel.Remarks = Channel.Remarks;
                dbChannel.Sort = Channel.Sort;
            }

            db.SaveChanges();

            return true;
        }


        public JsonResult ChannelDelete(Guid id)
        {

            var Channel = db.TChannel.Where(t => t.Id == id).FirstOrDefault();
            Channel.IsDelete = true;
            Channel.DeleteTime = DateTime.Now;
            Channel.DeleteUserId = Guid.Parse(HttpContext.Session.GetString("userId"));

            db.SaveChanges();

            var data = new { status = true, msg = "删除成功！" };
            return Json(data);

        }


        public IActionResult CategoryIndex(string ChannelId)
        {
            ViewBag.ChannelId = ChannelId;
            return View();
        }


        public JsonResult GetCategoryList(Guid ChannelId)
        {


            var list = db.TCategory.Where(t => t.ChannelId == ChannelId && t.IsDelete == false).Select(t => new { t.Id, t.ChannelId, t.Name, t.Remarks, ParentName = t.Parent.Name, t.Sort, t.CreateTime }).ToList();

            return Json(new { data = list });
        }


        public IActionResult CategoryEdit(Guid channelid, Guid id)
        {


            IDictionary<string, object> list = new Dictionary<string, object>();

            var categoryList = db.TCategory.Where(t => t.IsDelete == false && t.ChannelId == channelid).OrderBy(t => t.Sort).ToList();

            list.Add("categoryList", categoryList);


            if (id == default)
            {
                var category = new TCategory();
                category.ChannelId = channelid;
                list.Add("categoryInfo", category);
            }
            else
            {
                var Category = db.TCategory.Where(t => t.Id == id).FirstOrDefault();
                list.Add("categoryInfo", Category);
            }

            return View(list);

        }


        public bool CategorySave(TCategory Category)
        {

            if (Category.ParentId == default)
            {
                Category.ParentId = null;
            }

            var userId = Guid.Parse(HttpContext.Session.GetString("userId"));



            if (Category.Id == default)
            {
                //执行添加

                Category.Id = Guid.NewGuid();

                Category.CreateTime = DateTime.Now;
                Category.CreateUserId = userId;
                Category.IsDelete = false;

                db.TCategory.Add(Category);
            }
            else
            {
                //执行修改
                var dbCategory = db.TCategory.Where(t => t.Id == Category.Id).FirstOrDefault();

                dbCategory.ParentId = Category.ParentId;
                dbCategory.Name = Category.Name;
                dbCategory.Remarks = Category.Remarks;
                dbCategory.Sort = Category.Sort;

            }

            db.SaveChanges();

            return true;
        }


        public JsonResult CategoryDelete(Guid id)
        {


            var subCategoryCount = db.TCategory.Where(t => t.ParentId == id && t.IsDelete == false).Count();

            if (subCategoryCount == 0)
            {
                var userId = Guid.Parse(HttpContext.Session.GetString("userId"));

                var Category = db.TCategory.Where(t => t.Id == id).FirstOrDefault();

                Category.IsDelete = true;
                Category.DeleteUserId = userId;
                Category.DeleteTime = DateTime.Now;

                var articleList = db.TArticle.Where(t => t.CategoryId == id).ToList();

                foreach (var article in articleList)
                {
                    article.IsDelete = true;
                    article.DeleteUserId = userId;
                    article.DeleteTime = DateTime.Now;
                }


                db.SaveChanges();
                var data = new { status = true, msg = "删除成功！" };
                return Json(data);
            }
            else
            {
                var data = new { status = false, msg = "该类别下存在子级分类无法直接删除，请先删除子级分类！" };
                return Json(data);
            }


        }


        public IActionResult ArticleIndex(Guid ChannelId)
        {
            ViewBag.ChannelId = ChannelId;
            return View();
        }


        public JsonResult GetArticleList(Guid ChannelId)
        {


            var list = db.TArticle.Where(t => t.Category.ChannelId == ChannelId && t.IsDelete == false).Select(t => new { t.Id, t.Title, CategoryName = t.Category.Name, t.Category.ChannelId, t.IsDisplay, t.IsRecommend, t.ClickCount, t.CreateTime }).ToList();

            return Json(new { data = list });
        }


        public IActionResult ArticleEdit(Guid channelid, Guid id)
        {



            ViewData["Tenantid"] = HttpContext.Session.GetString("tenantid");


            var categoryList = db.TCategory.Where(t => t.IsDelete == false && t.ChannelId == channelid).OrderBy(t => t.Sort).ToList();

            ViewData["categoryList"] = categoryList;


            if (id == default)
            {
                var article = new TArticle();

                article.Id = Guid.NewGuid();
                article.IsDisplay = true;

                ViewData["article"] = article;
                ViewData["coverList"] = new List<TFile>();
            }
            else
            {
                var article = db.TArticle.Where(t => t.Id == id).FirstOrDefault();
                ViewData["article"] = article;

                var coverList = db.TFile.Where(t => t.IsDelete == false && t.Sign == "cover" & t.Table == "TArticle" & t.TableId == id).OrderBy(t => t.Sort).ToList();
                ViewData["coverList"] = coverList;

            }

            ViewData["channelid"] = channelid;

            return View();

        }


        public bool ArticleSave(TArticle article)
        {
            try
            {
                var userId = Guid.Parse(HttpContext.Session.GetString("userId"));



                if (db.TArticle.Where(t => t.Id == article.Id).FirstOrDefault() == null)
                {
                    //执行添加

                    article.CreateTime = DateTime.Now;
                    article.CreateUserId = userId;
                    article.IsDelete = false;

                    db.TArticle.Add(article);
                }
                else
                {
                    //执行修改
                    var dbArticle = db.TArticle.Where(t => t.Id == article.Id).FirstOrDefault();

                    dbArticle.CategoryId = article.CategoryId;
                    dbArticle.Title = article.Title;
                    dbArticle.Abstract = article.Abstract;
                    dbArticle.Content = article.Content;
                    dbArticle.IsDisplay = article.IsDisplay;
                    dbArticle.IsRecommend = article.IsRecommend;
                    dbArticle.ClickCount = article.ClickCount;
                    dbArticle.Sort = article.Sort;

                }

                db.SaveChanges();



                return true;
            }
            catch
            {
                return false;
            }
        }


        public JsonResult ArticleDelete(Guid id)
        {

            var userId = Guid.Parse(HttpContext.Session.GetString("userId"));

            var article = db.TArticle.Where(t => t.Id == id).FirstOrDefault();

            article.IsDelete = true;
            article.DeleteUserId = userId;
            article.DeleteTime = DateTime.Now;


            db.SaveChanges();

            var data = new { status = true, msg = "删除成功！" };
            return Json(data);

        }

    }
}
