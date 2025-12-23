using Common;
using kevin.Domain.Entities;
using kevin.Domain.Interfaces.IServices;
using Kevin.SnowflakeId.Service;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.DependencyInjection;
using System.Linq;

namespace WebApi.Controllers
{
    /// <summary>
    /// 系统基础方法控制器
    /// </summary>
    [ApiVersionNeutral]
    [Route("api/[controller]")]
    [AllowAnonymous]
    public class BaseController : ApiControllerBase
    {
        private IUserService _userService { get; set; }
        public BaseController(IUserService userService)
        {
            this._userService = userService;
        }


        /// <summary>
        /// 获取微信小程序OpenId
        /// </summary>
        /// <param name="weixinkeyid">微信配置密钥ID</param>
        /// <param name="code">微信临时code</param>
        /// <returns>openid,userid</returns>
        /// <remarks>传入租户ID和微信临时 code 获取 openid，如果 openid 在系统有中对应用户，则一并返回用户的ID值，否则用户ID值为空</remarks>
        [HttpGet("GetWeiXinMiniAppOpenId")]
        public string GetWeiXinMiniAppOpenId(long weixinkeyid, string code)
        {
            return _userService.GetWeiXinMiniAppOpenId(weixinkeyid, code);
        } 
        
        /// <summary>
        /// 获取省市级联地址数据
        /// </summary>
        /// <param name="provinceId">省份ID</param>
        /// <param name="cityId">城市ID</param>
        /// <returns></returns>
        /// <remarks>不传递任何参数返回省份数据，传入省份ID返回城市数据，传入城市ID返回区域数据</remarks>
        [HttpGet("GetRegion")]
        public List<dtoKeyValue> GetRegion(int provinceId, int cityId)
        {
            var list = new List<dtoKeyValue>(); 
            if (provinceId == 0 && cityId == 0)
            {
                list = db.Set<TRegionProvince>().Select(t => new dtoKeyValue { Key = t.Id, Value = t.Province }).ToList();
            } 
            if (provinceId != 0)
            {
                list = db.Set<TRegionCity>().Where(t => t.ProvinceId == provinceId).Select(t => new dtoKeyValue { Key = t.Id, Value = t.City }).ToList();
            } 
            if (cityId != 0)
            {
                list = db.Set<TRegionArea>().Where(t => t.CityId == cityId).Select(t => new dtoKeyValue { Key = t.Id, Value = t.Area }).ToList();
            } 
            return list;
        }



        /// <summary>
        /// 获取全部省市级联地址数据
        /// </summary>
        /// <returns></returns>
        [HttpGet("GetRegionAll")]
        public List<dtoKeyValueChild> GetRegionAll()
        {

            var list = db.Set<TRegionProvince>().Select(t => new dtoKeyValueChild
            {
                Key = t.Id,
                Value = t.Province,
                ChildList = t.TRegionCity.Select(c => new dtoKeyValueChild
                {
                    Key = c.Id,
                    Value = c.City,
                    ChildList = c.TRegionArea.Select(a => new dtoKeyValueChild
                    {
                        Key = a.Id,
                        Value = a.Area
                    }).ToList()
                }).ToList()
            }).ToList();

            return list;
        }



        /// <summary>
        /// 二维码生成
        /// </summary>
        /// <param name="text">数据内容</param>
        /// <returns></returns>
        [HttpGet("GetQrCode")]
        public FileResult GetQrCode(string text)
        {
            var image = ImgHelper.GetQrCode(text);
            return File(image, "image/png");
        }



        /// <summary>
        /// 获取指定Key的可选值
        /// </summary>
        /// <param name="key"></param>
        /// <returns></returns>
        [HttpGet("GetSelectValue")]
        public List<dtoKeyValue> GetSelectValue(string key)
        {

            var list = db.Set<TDictionary>().Where(t => t.IsDelete == false).OrderBy(t => t.Sort).Select(t => new dtoKeyValue
            {
                Key = t.Value,
                Value = t.Id
            }).ToList();

            return list;
        } 
        /// <summary>
        /// 获取一个雪花ID
        /// </summary>
        /// <returns></returns>
        [HttpGet("GetSnowflakeId")]
        public long GetSnowflakeId()
        {
            return HttpContext.RequestServices.GetService<ISnowflakeIdService>().GetNextId();
        }

        /// <summary>
        /// 获取一个雪花ID
        /// </summary>
        /// <returns></returns>
        [HttpGet("GetGuId")]
        public Guid GetNewGuid()
        {
            return Guid.NewGuid();
        }
    }
}