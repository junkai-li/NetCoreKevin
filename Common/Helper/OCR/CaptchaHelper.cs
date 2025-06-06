using Emgu.CV;
using Emgu.CV.CvEnum;
using Emgu.CV.Structure;
using System;
using System.Drawing;
using System.IO;
using System.Runtime.InteropServices;
using Tesseract;

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

        // 图像预处理，灰度化和二值化
        public static Image<Gray, byte> PreprocessImage(string imagePath)
        {
            return ImgPreprocess(new Image<Bgr, byte>(imagePath));
        }
        public static Image<Gray, byte> PreprocessImage(Bitmap bitmap)
        {
            return ImgPreprocess(bitmap.ToImage<Bgr, byte>());
        }

        public static Image<Gray, byte> PreprocessImage(byte[] bitmap)
        {
            using (MemoryStream ms = new MemoryStream(bitmap))
            {
                // 从流中加载图像
                Image image = Image.FromStream(ms);

                // 将 Image 转换为 Bitmap
                return ImgPreprocess(new Bitmap(image).ToImage<Bgr, byte>());

            }
        }

        /// <summary>
        /// 图像预处理，灰度化和二值化
        /// </summary>
        /// <param name="img"></param>
        /// <returns></returns>
        public static Image<Gray, byte> ImgPreprocess(Image<Bgr, byte> img, bool issave = true)
        {
            // 转为灰度图
            Image<Gray, byte> grayImage = img.Convert<Gray, byte>();

            //自适应阈值
            //// 二值化图像（高阈值设置）
            Image<Gray, byte> binarizedImage = grayImage.ThresholdAdaptive(new Gray(255), AdaptiveThresholdType.GaussianC, ThresholdType.Binary, 11, new Gray(2));

            if (issave)
            {
                grayImage.Save("gray_" + Guid.NewGuid().ToString() + ".png");
            }
            return binarizedImage;
        }

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
                var captchaImage = PreprocessImage(imagePath).ToBitmap();
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
        /// Bitmap识别
        /// </summary>
        /// <param name="captchaImage"></param>
        /// <returns></returns>
        public static string RecognizeCaptcha(Bitmap captchaImage)
        {
            try
            {
                captchaImage = PreprocessImage(captchaImage).ToBitmap();
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
                var captchaImage = PreprocessImage(imageData).ToBitmap();
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
                        new System.Drawing.Font("Arial", 18, FontStyle.Bold),
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
            string tempPath = Path.Combine(Path.GetTempPath(), Guid.NewGuid() + "captcha_temp.png");
            captchaImage.Save(tempPath, System.Drawing.Imaging.ImageFormat.Png);

            // 调用系统默认图片查看器打开
            System.Diagnostics.Process.Start(new System.Diagnostics.ProcessStartInfo()
            {
                FileName = tempPath,
                UseShellExecute = true
            });
        }

        public static void SavePng(string code, Bitmap captchaImage)
        {
            // 临时保存图片
            string tempPath = Path.Combine(Environment.CurrentDirectory, "CodeImg\\" + code);
            // 检查目录是否存在
            if (!Directory.Exists(tempPath))
            {
                Directory.CreateDirectory(tempPath);
            }
            captchaImage.Save(tempPath + "\\" + code + ".png", System.Drawing.Imaging.ImageFormat.Png);
        }

        /// <summary>
        /// 图片区域裁剪
        /// </summary>
        /// <param name="sourcePath">源图路径</param>
        /// <param name="x">裁剪起始坐标x</param>
        /// <param name="y">裁剪起始坐标y</param>
        /// <param name="width">裁剪区域长度</param>
        /// <param name="height">裁剪区域高度</param>
        /// <returns></returns>
        public static Bitmap RegionCropping(string sourcePath, int x, int y, int width, int height)
        {
            Bitmap result = null;
            //从文件加载原图
            using (Image originImage = Image.FromFile(sourcePath))
            {
                //创建矩形对象表示原图上裁剪的矩形区域，这里相当于划定原图上坐标为(10, 10)处，50x50大小的矩形区域为裁剪区域
                Rectangle cropRegion = new Rectangle(x, y, width, height);
                //创建空白画布，大小为裁剪区域大小
                result = new Bitmap(cropRegion.Width, cropRegion.Height);
                //创建Graphics对象，并指定要在result（目标图片画布）上绘制图像
                Graphics graphics = Graphics.FromImage(result);
                //使用Graphics对象把原图指定区域图像裁剪下来并填充进刚刚创建的空白画布
                graphics.DrawImage(originImage, new Rectangle(0, 0, cropRegion.Width, cropRegion.Height), cropRegion, GraphicsUnit.Pixel);
            }
            return result;
        }

    }
}
