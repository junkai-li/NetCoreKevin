using System;
using System.Net;
using System.Net.Sockets;

namespace Common
{
    public class DateTimeHelper
    {


        /// <summary>
        /// unix时间戳转DateTime
        /// </summary>
        /// <param name="unix">unix时间戳</param>
        /// <param name="startYear">startYear，默认为 1970</param>
        /// <returns></returns>
        public static DateTime UnixToTime(long unix, int startYear = 1970)
        {
            var startTime = TimeZoneInfo.ConvertTimeToUtc((new DateTime(startYear, 1, 1)).ToLocalTime()).ToLocalTime(); // 当地时区
            return startTime.AddSeconds(unix);
        }



        /// <summary>
        /// DateTime转unix时间戳
        /// </summary>
        /// <param name="time">待转换的时间</param>
        /// <param name="startYear">startYear，默认为 1970</param>
        /// <returns></returns>
        public static long TimeToUnix(DateTime time, int startYear = 1970)
        {
            var startTime = TimeZoneInfo.ConvertTimeToUtc((new DateTime(startYear, 1, 1)).ToLocalTime()).ToLocalTime(); // 当地时区
            return (long)(time - startTime).TotalSeconds; // 相差秒数
        }



        /// <summary>
        /// JavaScript时间戳转DateTime
        /// </summary>
        /// <param name="js">js时间戳</param>
        /// <param name="startYear">startYear，默认为 1970</param>
        /// <returns></returns>
        public static DateTime JsToTime(long js, int startYear = 1970)
        {

            var startTime = TimeZoneInfo.ConvertTimeToUtc((new DateTime(startYear, 1, 1)).ToLocalTime()).ToLocalTime(); // 当地时区
            return startTime.AddMilliseconds(js);
        }



        /// <summary>
        /// DateTime转Js时间戳
        /// </summary>
        /// <param name="time">待转换的时间</param>
        /// <param name="startYear">startYear，默认为 1970</param>
        /// <returns></returns>
        public static long TimeToJs(DateTime time, int startYear = 1970)
        {
            var startTime = TimeZoneInfo.ConvertTimeToUtc((new DateTime(startYear, 1, 1)).ToLocalTime()).ToLocalTime(); // 当地时区
            return (long)(time - startTime).TotalMilliseconds;
        }



        /// <summary>
        /// 获取某个日期，本周第一天
        /// </summary>
        /// <param name="time"></param>
        /// <returns></returns>
        public static DateTime GetWeekOne(DateTime time)
        {
            switch (time.DayOfWeek.ToString())
            {

                case "Monday":
                    {
                        return Convert.ToDateTime(time.AddDays(0).ToLongDateString());
                    }

                case "Tuesday":
                    {
                        return Convert.ToDateTime(time.AddDays(-1).ToLongDateString());
                    }

                case "Wednesday":
                    {
                        return Convert.ToDateTime(time.AddDays(-2).ToLongDateString());
                    }

                case "Thursday":
                    {
                        return Convert.ToDateTime(time.AddDays(-3).ToLongDateString());
                    }

                case "Friday":
                    {
                        return Convert.ToDateTime(time.AddDays(-4).ToLongDateString());
                    }

                case "Saturday":
                    {
                        return Convert.ToDateTime(time.AddDays(-5).ToLongDateString());
                    }

                case "Sunday":
                    {
                        return Convert.ToDateTime(time.AddDays(-6).ToLongDateString());
                    }
            }

            return default;
        }



        /// <summary>
        /// 获取某个日期，本季度第一天
        /// </summary>
        /// <param name="time"></param>
        /// <returns></returns>
        public static DateTime GetQuarterlyOne(DateTime time)
        {
            var month = time.Month;

            var startTime = Convert.ToDateTime(time.Year + "-01-01");

            if (month < 4)
            {
                return startTime;
            }
            else if (month > 3 && month < 7)
            {
                return startTime.AddMonths(3);
            }
            else if (month > 6 && month < 10)
            {
                return startTime.AddMonths(6);
            }
            else if (month > 9)
            {
                return startTime.AddMonths(9);
            }

            return default;
        }



        /// <summary>
        /// 获取一个时间是第几季度
        /// </summary>
        /// <param name="time"></param>
        /// <returns></returns>
        public static int GetQuarterly(DateTime time)
        {

            var month = time.Month;

            int quarterly = 0;

            if (month <= 3)
            {
                quarterly = 1;
            }
            else if (month >= 4 & month <= 6)
            {
                quarterly = 2;
            }
            else if (month >= 7 & month <= 9)
            {
                quarterly = 3;
            }
            else if (month >= 10 & month <= 12)
            {
                quarterly = 4;
            }

            return quarterly;
        }



        /// <summary>
        /// 获取NTC时间
        /// </summary>
        /// <returns></returns>
        public static DateTime GetNetworkTime()
        {

            string ntpServer = "ntp.onekib.com";

            var ntpData = new byte[48];

            ntpData[0] = 0x1B;

            var addresses = Dns.GetHostEntry(ntpServer).AddressList;

            var ipEndPoint = new IPEndPoint(addresses[0], 123);
            var socket = new Socket(AddressFamily.InterNetwork, SocketType.Dgram, ProtocolType.Udp);

            socket.Connect(ipEndPoint);

            socket.ReceiveTimeout = 3000;

            socket.Send(ntpData);
            socket.Receive(ntpData);
            socket.Close();

            const byte serverReplyTime = 40;

            ulong intPart = BitConverter.ToUInt32(ntpData, serverReplyTime);

            ulong fractPart = BitConverter.ToUInt32(ntpData, serverReplyTime + 4);

            intPart = (uint)(((intPart & 0x000000ff) << 24) + ((intPart & 0x0000ff00) << 8) + ((intPart & 0x00ff0000) >> 8) + ((intPart & 0xff000000) >> 24));
            fractPart = (uint)(((fractPart & 0x000000ff) << 24) + ((fractPart & 0x0000ff00) << 8) + ((fractPart & 0x00ff0000) >> 8) + ((fractPart & 0xff000000) >> 24));

            var milliseconds = (intPart * 1000) + ((fractPart * 1000) / 0x100000000L);

            var networkDateTime = (new DateTime(1900, 1, 1, 0, 0, 0, DateTimeKind.Utc)).AddMilliseconds((long)milliseconds);

            return networkDateTime.ToLocalTime();
        }


    }
}
