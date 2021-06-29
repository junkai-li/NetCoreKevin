using Microsoft.AspNetCore.Hosting;
using System;
using System.Collections.Generic;
using System.Text;

namespace Cms.Libraries.Start
{
    public class StartHostingEnvironment
    {
        public static IWebHostEnvironment webHostEnvironment;

        public static void Add(IWebHostEnvironment in_webHostEnvironment)
        {
            webHostEnvironment = in_webHostEnvironment;
        }

    }
}
