using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Repository.Database;
using System;
using System.Linq;
using WebApi.Filters;
using WebApi.Models.v1.Sign;

namespace WebApi.Controllers.v1
{

    /// <summary>
    /// 标记相关控制器
    /// </summary>
    [ApiVersion("1")]
    [Route("api/[controller]")]
    [Authorize]
    [JwtTokenVerify]
    public class SignController : ControllerBase
    {

        private readonly dbContext db;

        public SignController(dbContext context)
        {
            db = context;
        }


        /// <summary>
        /// 获取标记总数
        /// </summary>
        /// <param name="table"></param>
        /// <param name="tableId"></param>
        /// <param name="sign"></param>
        /// <returns></returns>
        [HttpGet("GetSignCount")]
        public int GetSignCount(string table, Guid tableId, string sign)
        {

            var count = db.TSign.Where(t => t.IsDelete == false && t.Table == table && t.TableId == tableId && t.Sign == sign).Count();

            return count;
        }


        /// <summary>
        /// 新增标记
        /// </summary>
        /// <param name="addSign"></param>
        /// <returns></returns>
        [HttpPost("AddSign")]
        public bool AddSign([FromBody] dtoSign addSign)
        {
            var userId = Guid.Parse(Libraries.Verify.JwtToken.GetClaims("userId"));


            var like = new TSign();
            like.Id = Guid.NewGuid();
            like.IsDelete = false;
            like.CreateTime = DateTime.Now;
            like.CreateUserId = userId;

            like.Table = addSign.Table;
            like.TableId = addSign.TableId;
            like.Sign = addSign.Sign;

            db.TSign.Add(like);
            db.SaveChanges();

            return true;
        }



        /// <summary>
        /// 删除标记
        /// </summary>
        /// <param name="deleteSign"></param>
        /// <returns></returns>
        [HttpDelete("DeleteSign")]
        public bool DeleteSign(dtoSign deleteSign)
        {
            var userId = Guid.Parse(Libraries.Verify.JwtToken.GetClaims("userId"));


            var like = db.TSign.Where(t => t.IsDelete == false && t.CreateUserId == userId && t.Table == deleteSign.Table && t.TableId == deleteSign.TableId && t.Sign == deleteSign.Sign).FirstOrDefault();

            if (like != null)
            {
                like.IsDelete = true;
                like.DeleteTime = DateTime.Now;
                db.SaveChanges();
            }
            return true;
        }

    }
}
