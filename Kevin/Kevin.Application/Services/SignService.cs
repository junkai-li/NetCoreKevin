using kevin.Domain.Share.Dtos.Sign;

namespace kevin.Application
{
    public class SignService : BaseService, ISignService
    { 
        public ISignRp signRp { get; set; }
        public SignService(IHttpContextAccessor _httpContextAccessor, ISignRp _signRp) : base(_httpContextAccessor)
        {
            signRp = _signRp;
        }


        /// <summary>
        /// 获取标记总数
        /// </summary>
        /// <param name="table"></param>
        /// <param name="tableId"></param>
        /// <param name="sign"></param>
        /// <returns></returns> 
        public int GetSignCount(string table, string tableId, string sign)
        {

            var count = signRp.Query(isDataPer: true).Where(t => t.IsDelete == false && t.Table == table && t.TableId == tableId && t.Sign == sign).Count();

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
            like.Id = SnowflakeIdService.GetNextId();
            like.IsDelete = false;
            like.CreateTime = DateTime.Now;
            like.CreateUserId = CurrentUser.UserId;

            like.Table = addSign.Table;
            like.TableId = addSign.TableId;
            like.Sign = addSign.Sign;

            signRp.Add(like);
            signRp.SaveChanges();

            return true;
        }



        /// <summary>
        /// 删除标记
        /// </summary>
        /// <param name="deleteSign"></param>
        /// <returns></returns> 
        public bool DeleteSign(dtoSign deleteSign)
        {
            var like = signRp.Query(isDataPer: true).Where(t => t.IsDelete == false && t.CreateUserId == CurrentUser.UserId && t.Table == deleteSign.Table && t.TableId == deleteSign.TableId && t.Sign == deleteSign.Sign).FirstOrDefault();

            if (like != null)
            {
                like.IsDelete = true;
                like.DeleteTime = DateTime.Now;
                signRp.SaveChanges();
            }
            return true;
        }

    }
}
