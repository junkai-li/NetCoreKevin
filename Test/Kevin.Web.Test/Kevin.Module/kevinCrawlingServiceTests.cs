using kevin.CrawlingService;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static kevin.CrawlingService.SeleniumHelper;

namespace Kevin.Unit.Tests.Kevin.Module
{
    public class KevinCrawlingServiceTests
    {
        [Fact]
        public void TestSeleniumHelper()
        { 
            //TODO: Add your test code here
            using (var helper = new SeleniumHelper("msedgedriver.exe", BrowserType.Edge, false))
            {
                helper.OpenUrl("https://www.baidu.com");
                var searchBox = helper.FindById("kw");
                helper.InputText(searchBox, "Selenium测试");
                var searchBtn = helper.FindById("su");
                helper.Click(searchBtn);
                Thread.Sleep(2000); // 等待结果加载
                Console.WriteLine(helper.GetPageTitle());
            }
        }
    }
}
