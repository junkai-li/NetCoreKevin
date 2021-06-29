using Common;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.DependencyInjection;
using Models.Dtos;
using Repository.Database;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace WebApi.Controllers
{
    /// <summary>
    /// 系统基础方法控制器
    /// </summary>
    [ApiVersionNeutral]
    [Route("api/[controller]")]
    [ApiController]
    public class BaseController : ControllerBase
    {

        private readonly dbContext db;

        public BaseController(dbContext context)
        {
            db = context;
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
                list = db.TRegionProvince.Select(t => new dtoKeyValue { Key = t.Id, Value = t.Province }).ToList();
            }

            if (provinceId != 0)
            {
                list = db.TRegionCity.Where(t => t.ProvinceId == provinceId).Select(t => new dtoKeyValue { Key = t.Id, Value = t.City }).ToList();
            }

            if (cityId != 0)
            {
                list = db.TRegionArea.Where(t => t.CityId == cityId).Select(t => new dtoKeyValue { Key = t.Id, Value = t.Area }).ToList();
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

            var list = db.TRegionProvince.Select(t => new dtoKeyValueChild
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
        /// 自定义二维码生成方法
        /// </summary>
        /// <param name="text">数据内容</param>
        /// <returns></returns>
        [HttpGet("GetQrCode")]
        public FileResult GetQrCode(string text)
        {
            var image = ImgHelper.GetQrCode(text);
            MemoryStream ms = new MemoryStream();
            image.Save(ms, System.Drawing.Imaging.ImageFormat.Png);
            return File(ms.ToArray(), "image/png");
        }



        /// <summary>
        /// 获取指定Key的可选值
        /// </summary>
        /// <param name="key"></param>
        /// <returns></returns>
        [HttpGet("GetSelectValue")]
        public List<dtoKeyValue> GetSelectValue(string key)
        {

            var list = db.TDictionary.Where(t => t.IsDelete == false).OrderBy(t => t.Sort).Select(t => new dtoKeyValue
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
            return HttpContext.RequestServices.GetService<Common.SnowflakeHelper>().GetId();
        }

    }
}