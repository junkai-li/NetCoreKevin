using Cms.Filters;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Repository.Database;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace Cms.Controllers
{

    [AuthenticationFilter]
    public class FileController : Controller
    {


        private readonly dbContext db;

        public FileController(dbContext context)
        {
            db = context;
        }



        public JsonResult SetFileSort(Guid id, int sort)
        {


            var file = db.TFile.Where(t => t.Id == id).FirstOrDefault();

            if (file != null)
            {
                file.Sort = sort;

                db.SaveChanges();
            }

            var data = new { status = true, msg = "调整成功！" };
            return Json(data);
        }



        [HttpPost]
        [DisableRequestSizeLimit]
        public bool UploadFile(string business, Guid key, string sign)
        {
            try
            {
                var userId = Guid.Parse(HttpContext.Session.GetString("userId"));

                var ReqFiles = Request.Form.Files;

                List<IFormFile> Attachments = new List<IFormFile>();
                for (int i = 0; i < ReqFiles.Count; i++)
                {
                    Attachments.Add(ReqFiles[i]);
                }

                var url = string.Empty;
                var fileName = string.Empty;
                var fileExtension = string.Empty;
                var fullFileName = string.Empty;

                string filepath = Libraries.IO.Path.WebRootPath() + "/Upload/" + DateTime.Now.ToString("yyyy/MM/dd");


                foreach (var file in Attachments)
                {
                    if (file != null)
                    {
                        Directory.CreateDirectory(filepath);

                        fileName = Guid.NewGuid().ToString();
                        fileExtension = Path.GetExtension(file.FileName).ToLowerInvariant();
                        fullFileName = string.Format("{0}{1}", fileName, fileExtension);
                        if (file != null && file.Length > 0)
                        {
                            string path = filepath + "/" + fullFileName;

                            using (FileStream fs = System.IO.File.Create(path))
                            {
                                file.CopyTo(fs);
                                fs.Flush();
                            }


                            var upRemote = false;

                            if (upRemote)
                            {

                                var oss = new Common.AliYun.OssHelper();

                                var upload = oss.FileUpload(path, "Files/" + DateTime.Now.ToString("yyyy/MM/dd"), file.FileName);

                                if (upload)
                                {
                                    Common.IO.IOHelper.DeleteFile(path);

                                    path = "/Files/" + DateTime.Now.ToString("yyyy/MM/dd") + "/" + fullFileName;
                                }
                            }
                            else
                            {
                                path = path.Replace(Libraries.IO.Path.WebRootPath(), "");
                            }




                            TFile fi = new TFile();
                            fi.Id = Guid.NewGuid();
                            fi.CreateUserId = userId;
                            fi.CreateTime = DateTime.Now;
                            fi.IsDelete = false;
                            fi.Name = file.FileName;
                            fi.Table = business;
                            fi.TableId = key;
                            fi.Sign = sign;
                            fi.Path = path;

                            db.TFile.Add(fi);

                            db.SaveChanges();
                        }

                    }

                }

                return true;

            }
            catch
            {
                return false;
            }
        }



        public bool DeleteFile(Guid id)
        {

            var userid = HttpContext.Session.GetString("userId");

            var file = db.TFile.Where(t => t.Id == id).FirstOrDefault();

            if (file != null)
            {
                file.IsDelete = true;

                db.SaveChanges();
            }

            return true;
        }
    }
}
