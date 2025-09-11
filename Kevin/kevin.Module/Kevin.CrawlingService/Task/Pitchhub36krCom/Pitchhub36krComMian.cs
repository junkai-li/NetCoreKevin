using OpenQA.Selenium;
using OpenQA.Selenium.Edge;
using OpenQA.Selenium.Support.UI;
using System.Collections.ObjectModel;

namespace CrawlingService.Task.Pitchhub36krCom
{
    public static class Pitchhub36krComMian
    {

        public static void Run()
        {
            List<string> list = new List<string>
            {
                "https://pitchhub.36kr.com/project/2129653472549767",
                "https://pitchhub.36kr.com/project/2011445516141832",
            };
            var service = EdgeDriverService.CreateDefaultService(@".", "msedgedriver.exe");
            Parallel.ForEach(list, new ParallelOptions() { MaxDegreeOfParallelism = 3 }, dataitem =>
            {

                try
                {
                    Random r = new Random();
                    using (IWebDriver driver = new OpenQA.Selenium.Edge.EdgeDriver(service))
                    {
                        int number = r.Next(5000, 15000);
                        System.Threading.Thread.Sleep(number);
                        driver.Navigate().GoToUrl(dataitem);
                        ITimeouts timeouts = driver.Manage().Timeouts();
                        //设置查找元素最大超时时间 
                        timeouts.ImplicitWait = new TimeSpan(0, 0, 10);
                        //设置页面操作最大超时时间 
                        timeouts.PageLoad = new TimeSpan(0, 0, 10);
                        //设置脚本异步最大超时时间 
                        timeouts.AsynchronousJavaScript = new TimeSpan(0, 0, 10);
                        //等待页面元素加载完成
                        //默认等待20秒
                        WebDriverWait wait = new WebDriverWait(driver, TimeSpan.FromSeconds(20));
                        //等待页面上ID属性值加载完成
                        IWebElement myElement = wait.Until((d) =>
                        {
                            return d.MyFindElement(By.ClassName("website"));
                        });
                        MyCW(driver.MyFindElement(By.ClassName("website")).Text.ToString(), true);
                        MyCW(driver.MyFindElement(By.ClassName("css-1bazmro")).Text.ToString(), true);
                        //查看更多
                        var chakans = driver.MyFindElements(By.XPath("//div[@class='expand-more-box css-1fm5dcy']"));
                        foreach (var item in chakans)
                        {
                            item.Click();
                        }
                        //获取融资历史
                        MyCW("-------------融资历史------------", true);
                        var rongziDiv = driver.MyFindElement(By.XPath("//div[@class='css-wynwdi']"));
                        if (rongziDiv != default)
                        {
                            var trs = rongziDiv.MyFindElement(By.ClassName("project-tbody"))?.MyFindElements(By.TagName("tr")) ?? default;
                            if (trs != default)
                            {
                                foreach (var item in trs)
                                {
                                    MyCW("融资轮次：" + item.MyFindElement(By.ClassName("round")).Text);
                                    MyCW("融资时间：" + item.MyFindElement(By.ClassName("time")).Text);
                                    MyCW("融资金额：" + item.MyFindElement(By.ClassName("amount")).Text);
                                    MyCW("投资方：" + item.MyFindElement(By.ClassName("investor")).Text);
                                    Console.WriteLine();
                                }

                            }

                        }

                        MyCW("--------------工商信息------------", true);
                        //工商信息
                        var gongshangDiv = driver.MyFindElement(By.XPath("//div[@class='business-info']"));
                        if (gongshangDiv != default)
                        {
                            var tds = gongshangDiv.MyFindElements(By.TagName("td"));
                            for (int i = 0; i < tds.Count;)
                            {
                                MyCW(tds[i].Text + " ：" + tds[i + 1].Text);
                                Console.WriteLine();
                                i += 2;
                            }
                        }
                        //持股
                        MyCW("--------------持股------------", true);
                        var chigu = driver.MyFindElement(By.XPath("//div[@class='business-shareholder']"));
                        if (chigu != default)
                        {
                            var chigudivs = chigu.MyFindElements(By.XPath("//div[@class='business-table-content']"));
                            MyCW("---股东（发起人）-----------持股比例-------认缴出资额-----认缴出资额", true);
                        }
                        int number1 = r.Next(3000, 8000);
                        System.Threading.Thread.Sleep(number1);
                        driver.Close();
                        driver.Dispose();
                    }
                }
                catch (Exception ex)
                {
                    MyCW(ex.Message, true);
                }
                finally
                {
                    MyCW(dataitem, true, ConsoleColor.Yellow);
                }
            });
        }

        public static void MyCW(string text, bool isLine = false, ConsoleColor ConsoleColor = ConsoleColor.Red)
        {
            Console.ForegroundColor = ConsoleColor; //设置前景色，即字体颜色
            if (isLine)
            {
                Console.WriteLine(text);
            }
            else
            {
                Console.Write(text);
            }
            Console.ResetColor();
        }

        public static IWebElement MyFindElement(this ISearchContext webDriver, By by)
        {

            return webDriver.FindElement(by);

        }
        public static ReadOnlyCollection<IWebElement> MyFindElements(this ISearchContext webDriver, By by)
        {
            return webDriver.FindElements(by);
        }
    }

}
