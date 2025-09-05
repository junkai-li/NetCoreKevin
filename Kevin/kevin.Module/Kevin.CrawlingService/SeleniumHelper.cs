
using OpenQA.Selenium;
using OpenQA.Selenium.Chrome;
using OpenQA.Selenium.Edge;
using OpenQA.Selenium.Firefox;
using OpenQA.Selenium.Support.UI;

namespace kevin.CrawlingService
{
     
    public class SeleniumHelper : IDisposable
    {
        private IWebDriver _driver;
        private WebDriverWait _wait;
        private const int DefaultWaitSeconds = 30;
        public enum BrowserType
        {
            Chrome,
            Firefox,
            Edge
        }
        // 初始化浏览器驱动
        public SeleniumHelper(string driverPath, BrowserType browserType, bool headless = false)
        {
            switch (browserType)
            {
                case BrowserType.Firefox:
                    var firefoxOptions = new FirefoxOptions();
                    if (headless) firefoxOptions.AddArgument("--headless");
                    _driver = new FirefoxDriver(driverPath, firefoxOptions);
                    break;
                case BrowserType.Edge:
                    var edgeOptions = new EdgeOptions();
                    if (headless) edgeOptions.AddArgument("--headless");
                    _driver = new EdgeDriver(driverPath, edgeOptions);
                    break;
                default:
                    var chromeOptions = new ChromeOptions();
                    if (headless) chromeOptions.AddArgument("--headless");
                    _driver = new ChromeDriver(driverPath, chromeOptions);
                    break;
            }
            _wait = new WebDriverWait(_driver, TimeSpan.FromSeconds(DefaultWaitSeconds));
        }
        public bool IsElementPresent(By locator)
        {
            try
            {
                _driver.FindElement(locator);
                return true;
            }
            catch (NoSuchElementException)
            {
                return false;
            }
        }
        public bool IsElementVisible(By locator)
        {
            try
            {
                return _driver.FindElement(locator).Displayed;
            }
            catch (NoSuchElementException)
            {
                return false;
            }
        }
        public IWebElement WaitForElement(By locator, int seconds = 30)
        {
            var wait = new WebDriverWait(_driver, TimeSpan.FromSeconds(seconds));
            return wait.Until(d => d.FindElement(locator));
        }

        // 打开网页
        public void OpenUrl(string url)
        {
            _driver.Navigate().GoToUrl(url);
        }

        // 通过ID查找元素
        public IWebElement FindById(string id)
        {
            return _wait.Until(d => d.FindElement(By.Id(id)));
        }

        // 通过XPath查找元素
        public IWebElement FindByXPath(string xpath)
        {
            return _wait.Until(d => d.FindElement(By.XPath(xpath)));
        }

        // 输入文本
        public void InputText(IWebElement element, string text)
        {
            element.Clear();
            element.SendKeys(text);
        }

        // 点击元素
        public void Click(IWebElement element)
        {
            _wait.Until(d => element.Displayed && element.Enabled);
            element.Click();
        }

        // 等待元素可见
        public void WaitForVisible(By locator)
        {
            _wait.Until(d => d.FindElement(locator).Displayed);
        }

        // 获取页面标题
        public string GetPageTitle()
        {
            return _driver.Title;
        }

        // 执行JavaScript
        public object ExecuteScript(string script)
        {
            return ((IJavaScriptExecutor)_driver).ExecuteScript(script);
        }

        // 关闭浏览器
        public void Close()
        {
            _driver.Quit();
        }

        // 实现IDisposable接口
        public void Dispose()
        {
            Close();
        }
    }

}
