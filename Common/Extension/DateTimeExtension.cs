using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace System
{
    public  static class DateTimeExtension
    {
        /// <summary>
        /// 获取日期中的 年月日 组成新日期
        /// </summary>
        /// <param name="day"></param>
        /// <returns></returns>
        public static DateTime GetDayDate(this DateTime day)
        {
            return new DateTime(day.Year, day.Month, day.Day);
        }

        /// <summary>  
        /// 日期是当月的第几周  
        /// </summary>  
        /// <param name="date">要判断的日期</param>  
        /// <param name="sundayStart">是否周日为一周的开始</param>  
        /// <returns></returns>  
        public static int GetWeekOfMonth(this DateTime date, bool sundayStart = true)
        {
            //如果要判断的日期为1号，则肯定是第一周了  
            if (date.Day == 1)
                return 1;
            else
            {
                //得到本月第一天  
                DateTime dtStart = new(date.Year, date.Month, 1);
                //得到本月第一天是周几  
                int dayofweek = (int)dtStart.DayOfWeek;

                //如果不是以周日开始，需要重新计算一下dayofweek，详细风DayOfWeek枚举的定义  
                if (!sundayStart)
                {
                    dayofweek--;

                    if (dayofweek < 0)
                        dayofweek = 7;
                }

                //得到本月的第一周一共有几天  
                int startWeekDays = 7 - dayofweek;

                //如果要判断的日期在第一周范围内，返回1  
                if (date.Day <= startWeekDays)
                    return 1;
                else
                {
                    int aday = date.Day + 7 - startWeekDays;
                    return aday / 7 + (aday % 7 > 0 ? 1 : 0);
                }
            }
        }


        /// <summary>
        /// 获取指定日期，在为一年中为第几周
        /// </summary>
        /// <param name="day">指定时间</param>
        /// <reutrn>返回第几周</reutrn>
        public static int GetWeekOfYear(this DateTime day)
        {
            GregorianCalendar gc = new();
            int weekOfYear = gc.GetWeekOfYear(day, CalendarWeekRule.FirstDay, DayOfWeek.Monday);
            return weekOfYear;
        }
        
    }
}
