using Microsoft.AspNetCore.Builder;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Kevin.Cors
{
    public static class ApplicationBuilderExtensions
    {
        public static void UseKevinCors(this IApplicationBuilder app)
        {
            //注册跨域信息
            app.UseCors();
        }
    }
}
