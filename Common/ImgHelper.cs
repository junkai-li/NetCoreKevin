using QRCoder;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Imaging;
using System.Text;
using Encoder = System.Drawing.Imaging.Encoder;

namespace Common
{
    public static class ImgHelper
    {

        /// <summary>
        /// string生成二维码
        /// </summary>
        /// <param name="text"></param>
        /// <returns></returns>
        public static Bitmap GetQrCode(string text)
        {
            QRCodeGenerator qrGenerator = new QRCodeGenerator();

            QRCodeData qrCodeData = qrGenerator.CreateQrCode(text, QRCodeGenerator.ECCLevel.L);

            QRCode qrCode = new QRCode(qrCodeData);

            Bitmap qrCodeImage = qrCode.GetGraphic(15);

            Encoder myEncoder = Encoder.Quality;

            var myEncoderParameters = new EncoderParameters(5);

            var myEncoderParameter = new EncoderParameter(myEncoder, 25L);

            myEncoderParameters.Param[0] = myEncoderParameter;

            return qrCodeImage;
        }



        /// <summary>
        /// 从指定的图片截取指定坐标的部分下来
        /// </summary>
        /// <param name="fromImagePath">源图路径</param>
        /// <param name="offsetX">距上</param>
        /// <param name="offsetY">距左</param>
        /// <param name="toImagePath">保存路径</param>
        /// <param name="width">宽度</param>
        /// <param name="height">高度</param>
        /// <returns></returns>
        public static bool Screenshot(string fromImagePath, int offsetX, int offsetY, string toImagePath, int width, int height)
        {
            try
            {
                //原图片文件
                Image fromImage = Image.FromFile(fromImagePath);
                //创建新图位图
                Bitmap bitmap = new Bitmap(width, height);
                //创建作图区域
                Graphics graphic = Graphics.FromImage(bitmap);
                //截取原图相应区域写入作图区
                graphic.DrawImage(fromImage, 0, 0, new Rectangle(offsetX, offsetY, width, height), GraphicsUnit.Pixel);
                //从作图区生成新图
                Image saveImage = Image.FromHbitmap(bitmap.GetHbitmap());
                //保存图片
                saveImage.Save(toImagePath, ImageFormat.Png);
                //释放资源   
                saveImage.Dispose();
                graphic.Dispose();
                bitmap.Dispose();

                return true;
            }
            catch
            {
                return false;
            }
        }




        /// <summary>
        /// 获取图像数字验证码
        /// </summary>
        /// <param name="width"></param>
        /// <param name="height"></param>
        /// <returns></returns>
        public static Image GetVerificationCode(int width, int height)
        {
            Bitmap image = new Bitmap(width, height);
            Graphics g = Graphics.FromImage(image);
            g.Clear(Color.White);
            var randString = "0123456789";
            Random random = new Random();

            var RandCharString = "";

            do
            {
                //使用DateTime.Now.Millisecond作为生成随机数的参数，增加随机性
                randString += RandCharString.Substring(random.Next(DateTime.Now.Millisecond) % RandCharString.Length, 1);
            }

            while (randString.Length < 6);//此处的4表示二维码的包含几个数字
            float emSize = (float)width / randString.Length;
            Font font = new Font("Arial", emSize, (FontStyle.Bold | FontStyle.Italic));
            Pen pen = new Pen(Color.Silver);
            #region 画图片的背景噪音线
            int x1, y1, x2, y2;

            for (int i = 0; i < 100; i++)
            {
                x1 = random.Next(image.Width);
                y1 = random.Next(image.Height);
                x2 = random.Next(image.Width);
                y2 = random.Next(image.Height);
                g.DrawLine(pen, x1, y1, x2, y2);
            }
            #endregion

            #region 画图片的前景噪音点
            for (int i = 0; i < 800; i++)
            {
                x1 = random.Next(image.Width);
                y1 = random.Next(image.Height);
                image.SetPixel(x1, y1, Color.FromArgb(random.Next(Int32.MaxValue)));
            }
            #endregion

            g.DrawString(randString, font, Brushes.Red, 2, 2);
            g.Dispose();
            return image;
        }
    }
}
