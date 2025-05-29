using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Repository.Database;
using Service.Dtos.v1.Sign;
using Service.Services.v1._;
using System.Linq;
using WebApi.Controllers.Bases;

namespace WebApi.Controllers.v1
{

    /// <summary>
    /// 标记相关控制器
    /// </summary>
    [ApiVersion("1")]
    [Route("api/[controller]")]
    [Authorize]
    [AllowAnonymous]
    public class SignController : ApiControllerBase
    {
        private ISignService _signService { get; set; }

        public SignController(ISignService signService)
        {
            this._signService = signService;
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
            return _signService.GetSignCount(table, tableId, sign);
        }


        /// <summary>
        /// 新增标记
        /// </summary>
        /// <param name="addSign"></param>
        /// <returns></returns>
        [HttpPost("AddSign")]
        public bool AddSign([FromBody] dtoSign addSign)
        {
            return _signService.AddSign(addSign);
        }



        /// <summary>
        /// 删除标记
        /// </summary>
        /// <param name="deleteSign"></param>
        /// <returns></returns>
        [HttpDelete("DeleteSign")]
        public bool DeleteSign(dtoSign deleteSign)
        {
            return _signService.DeleteSign(deleteSign); 
        }

    }
}
