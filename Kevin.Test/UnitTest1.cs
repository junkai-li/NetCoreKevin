using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Routing;

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

    }
}