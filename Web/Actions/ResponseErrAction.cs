/*
 * *******************************************************************
 * 
 * 版权：GEJI
 * 
 * *******************************************************************
 * 
 * 创建人：lijunkai
 * 创建时间：2021-07-16
 * 
 * *******************************************************************
 * 
 * 功能描述：Response处理
 * 
 * *******************************************************************
 * 变更记录： 
 * 
 * *******************************************************************
 */ 
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Web.Libraries.Http;

namespace Web.Actions
{
    public static class ResponseErrAction
    {
        public static void ExceptionMessage(string Message) 
        {
            HttpContext.Current().Response.StatusCode = 400;
            HttpContext.Current().Items.Add("errMsg", Message); 
        } 
    }
}
