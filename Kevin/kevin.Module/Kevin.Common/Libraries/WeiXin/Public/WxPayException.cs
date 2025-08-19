using System;

namespace Web.Libraries.WeiXin.Public
{
    public class WxPayException : Exception 
    {
        public WxPayException(string msg) : base(msg)
        {

        }
    }
}
