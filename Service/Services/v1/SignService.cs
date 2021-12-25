global using Web.Base;
using Repository.Database;
using Service.Dtos.v1.Sign;
using Service.Services.v1._;
using System;
using System.Linq;

namespace Service.Services.v1
{
    public class SignService : BaseService, ISignService
    {
        /// <summary>
        /// 获取标记总数
        /// </summary>
        /// <param name="table"></param>
        /// <param name="tableId"></param>
        /// <param name="sign"></param>
        /// <returns></returns> 
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
        public bool AddSign(dtoSign addSign)
        {  
            var like = new TSign();
            like.Id = Guid.NewGuid();
            like.IsDelete = false;
            like.CreateTime = DateTime.Now;
            like.CreateUserId = CurrentUser.UserId;

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
        public bool DeleteSign(dtoSign deleteSign)
        { 
            var like = db.TSign.Where(t => t.IsDelete == false && t.CreateUserId == CurrentUser.UserId&& t.Table == deleteSign.Table && t.TableId == deleteSign.TableId && t.Sign == deleteSign.Sign).FirstOrDefault();

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
