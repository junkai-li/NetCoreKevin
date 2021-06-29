using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;

namespace Common
{
    public static class SystemHelper
    {


        /// <summary>
        /// 获取本机全部IP
        /// </summary>
        /// <returns></returns>
        public static List<string> GetAllIpAddress()
        {
            var allIp = System.Net.Dns.GetHostEntry(System.Net.Dns.GetHostName()).AddressList.Select(t => t.ToString()).ToList();

            return allIp;
        }



        /// <summary>
        /// 获取本机 IPV4 地址
        /// </summary>
        /// <returns></returns>
        public static string GetIpv4Address()
        {
            var ipv4 = System.Net.Dns.GetHostEntry(System.Net.Dns.GetHostName()).AddressList.FirstOrDefault(address => address.AddressFamily == System.Net.Sockets.AddressFamily.InterNetwork)?.ToString();

            return ipv4;
        }



        /// <summary>
        /// 获取本机 IPV6 地址
        /// </summary>
        /// <returns></returns>
        public static string GetIpv6Address()
        {
            var ipv6 = System.Net.Dns.GetHostEntry(System.Net.Dns.GetHostName()).AddressList.FirstOrDefault(address => address.AddressFamily == System.Net.Sockets.AddressFamily.InterNetworkV6)?.ToString();

            return ipv6;
        }



        /// <summary>
        /// 运行 shell 脚本
        /// </summary>
        /// <param name="shell"></param>
        /// <returns></returns>
        public static string LinuxShell(string shell)
        {

            string output = "";

            //创建一个ProcessStartInfo对象 使用系统shell 指定命令和参数 设置标准输出
            var psi = new ProcessStartInfo("/bin/bash", "-c \"" + shell + "\"") { RedirectStandardOutput = true };


            //启动
            using (var proc = Process.Start(psi))
            {

                if (proc == null)
                {
                    Console.WriteLine("Can not exec.");
                }
                else
                {
                    Console.WriteLine("Shell执行：\n" + shell);

                    output = proc.StandardOutput.ReadToEnd();

                    if (!proc.HasExited)
                    {
                        proc.Kill();

                        Console.WriteLine("Shell已结束,进程已关闭");
                    }
                }

            }


            Console.WriteLine("Shell结果：\n" + output);

            return output;
        }

    }
}
