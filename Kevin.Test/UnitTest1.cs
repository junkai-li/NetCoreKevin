using AppServices.Services.v1._;
using DynamicExpresso;
using kevin.Domain.Share.Interfaces;
using kevin.Ioc.IocAttrributes;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Routing;
using Microsoft.AspNetCore.TestHost;
using Service.Services.v1._;

namespace Kevin.Test
{
    [TestClass]
    public class UnitTest1
    {

        [TestMethod]
        [DataRow(" 1==1 ")]
        public void TsetDynamicExpresso(string str)
        {
            var interpreter = new DynamicExpresso.Interpreter();
            Assert.IsFalse(!interpreter.Eval<bool>(str), $"{str} TsetDynamicExpresso");
        }

        [IocProperty]
        public IUserService _userService { get; set; }

        [IocProperty]
        public ICacheService _cacheService { get; set; }
        [IocProperty]
        public ITestService _TestService { get; set; }

        [TestMethod]
        [DataRow(" 1==1 ")]
        [DataRow(" _cacheService")]
        public void TestCacheServiceOk(string data)
        {
            _cacheService.SetString("1", data);
            Assert.IsFalse(string.IsNullOrEmpty(_cacheService.GetString("1")), $"{data} TestCacheServiceOk");

        }

    }
}