using NUnit.Framework;

namespace Kevin.Test
{
  
    public class UnitTest1: SetEnvironment
    {
        [SetUp]
        public void Setup()
        {
        }

        [Test]
        public void TsetDynamicExpresso()
        {
            Console.WriteLine(  "TestDynamicExpresso");
            Assert.Pass();
        }

        //[IocProperty]
        //public IUserService _userService { get; set; }

        //[IocProperty]
        //public ICacheService _cacheService { get; set; }
        //[IocProperty]
        //public ITestService _TestService { get; set; }

        [Test]
        public void TestCacheServiceOk()
        {
            Console.WriteLine("TestCacheServiceOk");
            Assert.Pass();

        }

    }
}