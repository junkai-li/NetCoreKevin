using OpenQA.Selenium.Edge;
using OpenQA.Selenium.Interactions;
using OpenQA.Selenium.Support.UI;
using OpenQA.Selenium;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace kevin.CrawlingService
{
    public class SeleniumHelper
    {
        public IWebDriver driver;

        public SeleniumHelper()
        {
            // 配置 Edge 驱动选项
            var options = new EdgeOptions();
            options.AddArgument("-inprivate");
            options.AddArgument("--disable-blink-features=AutomationControlled");// 禁用Blink自动化特征


            // 启用无痕浏览 
            // options.AddArgument("--headless"); // 可选，启用无头模式（去掉这一行会看到浏览器界面）
            // 禁用自动化特征检测
            options.AddExcludedArgument("enable-automation");
            options.AddAdditionalOption("useAutomationExtension", false);
            // 伪装用户代理
            options.AddArgument("--user-agent=Mozilla/5.0 (Windows NT 10.0; Win64; x64) AppleWebKit/537.36...");
            // 禁用GPU加速（提升无头模式稳定性）
            options.AddArgument("--disable-gpu");
            driver = new EdgeDriver(options);     // 启动 Edge 浏览器
            driver.Manage().Timeouts().ImplicitWait = TimeSpan.FromSeconds(10);
            // 设置标准窗口尺寸（避免分辨率问题）
            //driver.Manage().Window.Size = new Size(1280, 1024);
            // 或最大化窗口
            driver.Manage().Window.Maximize();
        }

        public void OpenPage(string url)
        {
            driver.Navigate().GoToUrl(url);
        }

        public void Close()
        {
            driver.Quit();
        }

        public IWebElement WaitForElement(By by, TimeSpan timeout)
        {
            var wait = new WebDriverWait(driver, timeout);
            return wait.Until(drv => drv.FindElement(by));
        }

        public void Login(string username, string password)
        {
            // 打开登录页面
            OpenPage("https://login.1688.com/member/signin.htm");
            Thread.Sleep(3000);
            // 执行点击（带随机延迟）
            new Actions(driver)
                .Pause(TimeSpan.FromMilliseconds(new Random().Next(100, 500)))
                .Click()
                .Perform();
            //  driver.FindElement(By.Name("fm-login-id")).SendKeys(username);

            // 执行点击（带随机延迟）
            new Actions(driver)
                .Pause(TimeSpan.FromMilliseconds(new Random().Next(100, 500)))
                .Click()
                .Perform();
            // driver.FindElement(By.Name("fm-login-password")).SendKeys(password);
            // 处理可能的验证码（需要人工干预或第三方服务）
            Thread.Sleep(30000); // 留出时间手动完成验证
                                 // 提交登录表单
                                 // driver.FindElement(By.CssSelector("button.btn-login")).Click();

            // 等待验证码处理，用户可以手动完成
            Console.WriteLine("请完成验证码并按任意键继续...");
        }
    }
}
