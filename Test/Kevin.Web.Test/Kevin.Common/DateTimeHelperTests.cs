using Common;

namespace Kevin.Unit.Test.Kevin.Common
{ 
 
        public class DateTimeHelperTests
        {
            [Fact]
            public void UnixToTime_ShouldConvertCorrectly()
            {
                // 测试1970年基准
                var result = DateTimeHelper.UnixToTime(0);
                Assert.Equal(new DateTime(1970, 1, 1), new DateTime(result.Year, result.Month, result.Day));

                // 测试自定义基准年
                result = DateTimeHelper.UnixToTime(0, 2000);
                Assert.Equal(new DateTime(2000, 1, 1), new DateTime(result.Year, result.Month, result.Day));

                // 测试秒数转换
                result = DateTimeHelper.UnixToTime(86400);
                Assert.Equal(new DateTime(1970, 1, 2), new DateTime(result.Year, result.Month, result.Day));
            }
         

            [Fact]
            public void JsToTime_ShouldConvertCorrectly()
            {
                // 测试1970年基准
                var result = DateTimeHelper.JsToTime(0);
                Assert.Equal(new DateTime(1970, 1, 1),new DateTime(result.Year, result.Month, result.Day));

                // 测试自定义基准年
                result = DateTimeHelper.JsToTime(0, 2000);
                Assert.Equal(new DateTime(2000, 1, 1), new DateTime(result.Year, result.Month, result.Day));

                // 测试毫秒转换
                result = DateTimeHelper.JsToTime(86400000);
                Assert.Equal(new DateTime(1970, 1, 2), new DateTime(result.Year, result.Month, result.Day));
            }

            
            [Fact]
            public void GetWeekOne_ShouldReturnMonday()
            {
                // 测试周一
                var result = DateTimeHelper.GetWeekOne(new DateTime(2023, 1, 2)); // 周一
                Assert.Equal(new DateTime(2023, 1, 2), result);

                // 测试周日
                result = DateTimeHelper.GetWeekOne(new DateTime(2023, 1, 8)); // 周日
                Assert.Equal(new DateTime(2023, 1, 2), result);
            }

            [Fact]
            public void GetQuarterlyOne_ShouldReturnFirstDayOfQuarter()
            {
                // Q1测试
                var result = DateTimeHelper.GetQuarterlyOne(new DateTime(2023, 2, 15));
                Assert.Equal(new DateTime(2023, 1, 1), result);

                // Q4测试
                result = DateTimeHelper.GetQuarterlyOne(new DateTime(2023, 11, 20));
                Assert.Equal(new DateTime(2023, 10, 1), result);
            }

            [Fact]
            public void GetQuarterly_ShouldReturnCorrectQuarter()
            {
                // Q1测试
                var result = DateTimeHelper.GetQuarterly(new DateTime(2023, 1, 1));
                Assert.Equal(1, result);

                // Q4测试
                result = DateTimeHelper.GetQuarterly(new DateTime(2023, 12, 31));
                Assert.Equal(4, result);
            }

            [Fact]
            public void GetNetworkTime_ShouldReturnValidDateTime()
            {
                var result = DateTimeHelper.GetNetworkTime();
                Assert.True(result > new DateTime(2000, 1, 1)); // 简单验证返回时间是否合理
            }
        } 
}
