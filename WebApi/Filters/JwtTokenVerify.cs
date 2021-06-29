using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using Repository.Database;
using System;
using System.Linq;
using System.Security.Claims;

namespace WebApi.Filters
{


    /// <summary>
    /// JWT 访问鉴权 Token 验证方法
    /// </summary>
    public class JwtTokenVerify : Attribute, IActionFilter
    {

        /// <summary>
        /// 是否跳过Token验证，可用于控制器下单个接口指定跳过Token验证
        /// </summary>
        public bool IsSkip { get; set; }



        void IActionFilter.OnActionExecuting(ActionExecutingContext context)
        {

            var filter = (JwtTokenVerify)context.Filters.Where(t => t.ToString() == (typeof(JwtTokenVerify).Assembly.GetName().Name + ".Filters.JwtTokenVerify")).ToList().LastOrDefault();

            if (!filter.IsSkip)
            {

                var exp = Convert.ToInt64(Libraries.Verify.JwtToken.GetClaims("exp"));

                var exptime = Common.DateTimeHelper.UnixToTime(exp);

                if (exptime < DateTime.Now)
                {
                    var tokenId = Guid.Parse(Libraries.Verify.JwtToken.GetClaims("tokenId"));

                    using (var db = new dbContext())
                    {

                        var endtime = DateTime.Now.AddMinutes(-3);

                        var tokeninfo = db.TUserToken.Where(t => t.Id == tokenId || (t.LastId == tokenId & t.CreateTime > endtime)).FirstOrDefault();

                        if (tokeninfo == null)
                        {
                            context.HttpContext.Response.StatusCode = 401;

                            context.Result = new JsonResult(new { errMsg = "非法 Token ！" });
                        }
                    }
                }

            }
        }


        void IActionFilter.OnActionExecuted(ActionExecutedContext context)
        {

            var filter = (JwtTokenVerify)context.Filters.Where(t => t.ToString() == "WebApi.Filters.JwtTokenVerify").ToList().LastOrDefault();

            if (!filter.IsSkip)
            {


                var exp = Convert.ToInt64(Libraries.Verify.JwtToken.GetClaims("exp"));

                var exptime = Common.DateTimeHelper.UnixToTime(exp);

                if (exptime < DateTime.Now)
                {

                    var tokenId = Guid.Parse(Libraries.Verify.JwtToken.GetClaims("tokenId"));
                    var userId = Guid.Parse(Libraries.Verify.JwtToken.GetClaims("userId"));

                    using (var db = new dbContext())
                    {

                        var endtime = DateTime.Now.AddMinutes(-3);

                        var newtoken = db.TUserToken.Where(t => t.LastId == tokenId & t.CreateTime > endtime).FirstOrDefault();

                        if (newtoken == null)
                        {

                            var tokeninfo = db.TUserToken.Where(t => t.Id == tokenId).FirstOrDefault();

                            if (tokeninfo != null)
                            {

                                TUserToken userToken = new TUserToken();
                                userToken.Id = Guid.NewGuid();
                                userToken.UserId = userId;
                                userToken.LastId = tokenId;
                                userToken.CreateTime = DateTime.Now;


                                var claim = new Claim[]{
                                    new Claim("tokenId",userToken.Id.ToString()),
                                    new Claim("userId",userId.ToString())
                                };

                                var token = Libraries.Verify.JwtToken.GetToken(claim);
                                context.HttpContext.Response.Headers.Add("NewToken", token);

                                //解决 Ionic 取不到 Header中的信息问题
                                context.HttpContext.Response.Headers.Add("Access-Control-Expose-Headers", "NewToken");


                                userToken.Token = token;

                                db.TUserToken.Add(userToken);


                                db.TUserToken.Remove(tokeninfo);
                                db.SaveChanges();

                                var oldtime = DateTime.Now.AddDays(-7);
                                var oldlist = db.TUserToken.Where(t => t.CreateTime < oldtime).ToList();
                                db.TUserToken.RemoveRange(oldlist);

                                db.SaveChanges();
                            }
                        }
                        else
                        {
                            context.HttpContext.Response.Headers.Add("NewToken", newtoken.Token);
                        }
                    }
                }

            }

        }
    }
}
