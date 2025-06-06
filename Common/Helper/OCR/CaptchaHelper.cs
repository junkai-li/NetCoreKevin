using System;
using System.Linq;
using System.Runtime.InteropServices;
using Tesseract;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;
using Aop.Api.Domain;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace Kevin.Common.Helper.Captcha
{
    #region 使用方法
    //var imgpng = CaptchaRecognizer.RecognizeFromFile("1.jpeg");
    //var imgpng2 = CaptchaRecognizer.RecognizeFromFile("2.png");
    //var imgpng3 = CaptchaRecognizer.RecognizeFromFile("captcha_temp(1).png");
    //Console.WriteLine(imgpng);
    //        Console.WriteLine(imgpng2);
    //        Console.WriteLine(imgpng3);
    //        // 1. 生成验证码
    //        var(realCode, captchaImage) = CaptchaRecognizer.GenerateCaptcha();
    //        Console.WriteLine($"真实验证码: {realCode}");

    //        // 2. 显示验证码
    //        CaptchaRecognizer.ShowCaptcha(captchaImage);

    //        // 3. 识别验证码
    //        var recognizedCode = CaptchaRecognizer.RecognizeCaptcha(captchaImage);
    //Console.WriteLine($"识别结果: {recognizedCode}");

    //        // 4. 对比结果
    //        Console.WriteLine($"识别{(realCode == recognizedCode ? "成功" : "失败")}");
    #endregion
    /// <summary>
    /// 验证码帮助类
    /// </summary>
    public class CaptchaHelper
    {

        /// <summary>
        /// 创建跨平台OCR引擎
        /// </summary>
        private static TesseractEngine CreateOcrEngine()
        {
            // 获取程序运行目录
            string baseDir = AppDomain.CurrentDomain.BaseDirectory;

            // 根据操作系统选择路径
            string tessdataPath = RuntimeInformation.IsOSPlatform(OSPlatform.Windows)
                ? Path.Combine(baseDir, "tessdata")
                : Path.Combine(baseDir, "Contents", "tessdata");

            // 检查目录是否存在
            if (!Directory.Exists(tessdataPath))
            {
                Directory.CreateDirectory(tessdataPath);
                throw new DirectoryNotFoundException($"tessdata目录不存在，已自动创建: {tessdataPath}");
            }  
            // 替换路径分隔符（Windows兼容）
            tessdataPath = tessdataPath.Replace('\\', '/');

            try
            {
                // 初始化引擎（使用英文）
                return new TesseractEngine(tessdataPath, "eng", EngineMode.Default);
            }
            catch (Exception ex)
            {
                throw new Exception($"Tesseract初始化失败: {ex.Message}");
            }
        }

        /// <summary>
        /// 从文件识别验证码
        /// </summary>
        public static string RecognizeFromFile(string imagePath)
        {
            try
            {
                using (var engine = CreateOcrEngine())
                using (var img = Pix.LoadFromFile(imagePath))
                using (var page = engine.Process(img))
                {
                    return CleanResult(page.GetText());
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"识别失败: {ex.Message}");
                return string.Empty;
            }
        }

        // 识别验证码（使用Tesseract）
        public static string RecognizeCaptcha(Bitmap captchaImage)
        {
            try
            {
                using (var engine = CreateOcrEngine())
                {
                    using (var ms = new MemoryStream())
                    {
                        captchaImage.Save(ms, System.Drawing.Imaging.ImageFormat.Bmp);
                        using (var page = engine.Process(Pix.LoadFromMemory(ms.ToArray())))
                        {
                            return CleanResult(page.GetText());
                        }
                    }  
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"识别失败: {ex.Message}");
                return string.Empty;
            }
        }

        /// <summary>
        /// 从字节数组识别验证码
        /// </summary>
        public static string RecognizeFromBytes(byte[] imageData)
        {
            try
            {
                using (var engine = CreateOcrEngine())
                using (var img = Pix.LoadFromMemory(imageData))
                using (var page = engine.Process(img))
                {
                    return CleanResult(page.GetText());
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"识别失败: {ex.Message}");
                return string.Empty;
            }
        } 
        /// <summary>
        /// 清理识别结果
        /// </summary>
        private static string CleanResult(string rawText)
        {
            return (rawText ?? "").Replace(" ", "").Replace("\n", "").Replace("\r", "").Trim();
        }

        // 生成随机验证码
        public static (string code, Bitmap image) GenerateCaptcha(int width = 120, int height = 40)
        {
            // 1. 创建画布
            var bmp = new Bitmap(width, height);
            using (var g = Graphics.FromImage(bmp))
            {
                // 2. 填充背景
                g.Clear(Color.White);

                // 3. 生成随机字符（4位数字字母混合）
                var random = new Random();
                const string chars = "ABCDEFGHJKLMNPQRSTUVWXYZabcdefghjkmnpqrstuvwxyz23456789";
                var code = new char[4];
                for (int i = 0; i < 4; i++)
                {
                    code[i] = chars[random.Next(chars.Length)];
                }
                var captchaCode = new string(code);

                // 4. 绘制干扰线
                for (int i = 0; i < 5; i++)
                {
                    g.DrawLine(
                        new Pen(Color.FromArgb(random.Next(256), random.Next(256), random.Next(256))),
                        new Point(0, random.Next(height)),
                        new Point(width, random.Next(height))
                    );
                }

                // 5. 绘制验证码文字
                for (int i = 0; i < 4; i++)
                {
                    g.DrawString(
                        captchaCode[i].ToString(),
                        new Font("Arial", 18, FontStyle.Bold),
                        new SolidBrush(Color.FromArgb(random.Next(256), random.Next(256), random.Next(256))),
                        new PointF(i * 25 + 10, random.Next(5, 15))
                    );
                }

                return (captchaCode, bmp);
            }
        }

        // 显示验证码（控制台应用程序示例）
        public static void ShowCaptcha(Bitmap captchaImage)
        {
            // 临时保存图片
            string tempPath = Path.Combine(Path.GetTempPath(), "captcha_temp.png");
            captchaImage.Save(tempPath, System.Drawing.Imaging.ImageFormat.Png);

            // 调用系统默认图片查看器打开
            System.Diagnostics.Process.Start(new System.Diagnostics.ProcessStartInfo()
            {
                FileName = tempPath,
                UseShellExecute = true
            });
        }

    }
}
