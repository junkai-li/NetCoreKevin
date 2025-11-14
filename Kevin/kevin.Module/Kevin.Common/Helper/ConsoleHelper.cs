using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace Kevin.Common.Helper
{
    /// <summary>
    /// 好看的输出控制台帮助类
    /// </summary>
    public static class ConsoleHelper
    {
        public  enum Style
        {
            Gradient,
            Boxed,
            Modern,
            Typewriter
        }

        /// <summary>
        /// 综合风格输出
        /// </summary>
        /// <param name="frameworkName"></param>
        /// <param name="style"></param>
        public static void Print(string frameworkName, Style style = Style.Gradient)
        {
            switch (style)
            {
                case Style.Gradient:
                    PrintGradient(frameworkName);
                    break;
                case Style.Boxed:
                    PrintBoxed(frameworkName);
                    break;
                case Style.Modern:
                    PrintModern(frameworkName);
                    break;
                case Style.Typewriter:
                    PrintTypewriter(frameworkName);
                    break;
            }
        }

        private static void PrintGradient(string text)
        {
            ConsoleColor[] gradient = {
            ConsoleColor.DarkRed,
            ConsoleColor.Red,
            ConsoleColor.Yellow,
            ConsoleColor.Green,
            ConsoleColor.Cyan,
            ConsoleColor.Blue
        };

            for (int i = 0; i < text.Length; i++)
            {
                Console.ForegroundColor = gradient[i % gradient.Length];
                Console.Write(text[i]);
            }
            Console.WriteLine();
            Console.ResetColor();
        }

        private static void PrintBoxed(string text)
        {
            int padding = 4;
            int totalWidth = text.Length + padding * 2;

            Console.ForegroundColor = ConsoleColor.Magenta;
            Console.WriteLine("╔" + new string('═', totalWidth) + "╗");
            Console.WriteLine("║" + new string(' ', totalWidth) + "║");
            Console.Write("║" + new string(' ', padding));
            Console.ForegroundColor = ConsoleColor.White;
            Console.Write(text);
            Console.ForegroundColor = ConsoleColor.Magenta;
            Console.WriteLine(new string(' ', padding) + "║");
            Console.WriteLine("║" + new string(' ', totalWidth) + "║");
            Console.WriteLine("╚" + new string('═', totalWidth) + "╝");
            Console.ResetColor();
        }

        private static void PrintModern(string text)
        {
            Console.ForegroundColor = ConsoleColor.DarkCyan;
            Console.Write("✦ ");
            Console.ForegroundColor = ConsoleColor.White;
            Console.Write(text);
            Console.ForegroundColor = ConsoleColor.DarkCyan;
            Console.WriteLine(" ✦");
            Console.ResetColor();
        }

        private static void PrintTypewriter(string text)
        {
            Console.ForegroundColor = ConsoleColor.Gray;
            Console.Write("➤ ");
            Console.ForegroundColor = ConsoleColor.Yellow;

            foreach (char c in text)
            {
                Console.Write(c);
                Thread.Sleep(50);
            }
            Console.WriteLine();
            Console.ResetColor();
        }


        /// <summary>
        /// 彩色渐变风格
        /// </summary>
        /// <param name="frameworkName"></param>
        public static void PrintFrameworkName(string frameworkName)
        {
            Console.ForegroundColor = ConsoleColor.White;
            Console.WriteLine("\n" + new string('═', frameworkName.Length + 4));

            Console.Write("║ ");
            for (int i = 0; i < frameworkName.Length; i++)
            {
                Console.ForegroundColor = GetRainbowColor(i);
                Console.Write(frameworkName[i]);
            }
            Console.ForegroundColor = ConsoleColor.White;
            Console.WriteLine(" ║");

            Console.WriteLine(new string('═', frameworkName.Length + 4));
            Console.ResetColor();
        }

        private static ConsoleColor GetRainbowColor(int index)
        {
            ConsoleColor[] colors = {
            ConsoleColor.Red,
            ConsoleColor.Yellow,
            ConsoleColor.Green,
            ConsoleColor.Cyan,
            ConsoleColor.Blue,
            ConsoleColor.Magenta
        };
            return colors[index % colors.Length];
        }

        /// <summary>
        /// 艺术字风格
        /// </summary>
        /// <param name="frameworkName"></param>
        public static void PrintPrintAsciiArt(string frameworkName)
        {
            Console.ForegroundColor = ConsoleColor.Cyan;
            Console.WriteLine(@"
    ╔═══════════════════════════════════╗
    ║                                   ║
    ║                                   ║
    ║                                   ║
    ║           " + frameworkName.PadRight(15).PadLeft(20) + @"            ║
    ║                                   ║
    ╚═══════════════════════════════════╝
        ");
            Console.ResetColor();
        }

        /// <summary>
        /// 现代简约风格
        /// </summary>
        /// <param name="frameworkName"></param>

        public static void PrintModernStyle(string frameworkName)
        {
            Console.OutputEncoding = System.Text.Encoding.UTF8;

            // 顶部装饰
            Console.ForegroundColor = ConsoleColor.DarkGray;
            Console.WriteLine("┌" + new string('─', frameworkName.Length + 2) + "┐");

            // 框架名称
            Console.ForegroundColor = ConsoleColor.Blue;
            Console.Write("│ ");
            Console.ForegroundColor = ConsoleColor.White;
            Console.Write(frameworkName);
            Console.ForegroundColor = ConsoleColor.Blue;
            Console.WriteLine(" │");

            // 底部装饰
            Console.ForegroundColor = ConsoleColor.DarkGray;
            Console.WriteLine("└" + new string('─', frameworkName.Length + 2) + "┘");

            Console.ResetColor();
        }

        /// <summary>
        /// 动态打字机效果
        /// </summary>
        /// <param name="frameworkName"></param>

        public static void PrintWithTypewriterEffect(string frameworkName)
        { 
            // 播放提示音
            Console.ForegroundColor = ConsoleColor.Green;

            Console.Write("");
            Console.ForegroundColor = ConsoleColor.Yellow;

            // 打字机效果
            foreach (char c in frameworkName)
            {
                Console.Write(c);
                Thread.Sleep(50); // 控制打字速度
            }

            Console.WriteLine();
            Console.ResetColor();
        }



    }
}
