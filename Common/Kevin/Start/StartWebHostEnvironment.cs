using Microsoft.AspNetCore.Hosting;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Kevin.Common.Kevin.Start
{
    public class StartWebHostEnvironment
    {
        public static IWebHostEnvironment webHostEnvironment { get; set; } 

        public StartWebHostEnvironment(IWebHostEnvironment env)
        {
            webHostEnvironment = env;
        }
    }
}
