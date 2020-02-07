using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Drawing.Imaging;
using System.IO;
using System.Linq;
using System.Net;
using System.Web;

namespace Seagull2.YuanXin.AppApi
{
    public class PictureProcessing
    {
        /// <summary>
        /// 创建缩略图
        /// </summary>
        /// <param name="image"></param>
        /// <param name="width">缩略图宽度</param>
        /// <param name="height">缩略图高度</param>
        /// <returns>字节数组</returns>
        public static byte[] MakeThumbnail(System.Drawing.Image image, double width, double height)
        {
            double ratio = 1;
            double w = image.Width;
            double h = image.Height;
            double newWidth = width / w;
            double newHeight = height / h;
            if (width == 0 && height == 0)
            {
                ratio = 1;
            }
            else if (width == 0)
            { //
                if (newHeight < 1)
                    ratio = newHeight;
            }
            else if (height == 0)
            {
                if (newWidth < 1)
                    ratio = newWidth;
            }
            else if (newWidth < 1 || newHeight < 1)
            {
                ratio = (newWidth <= newHeight ? newWidth : newHeight);
            }
            if (ratio < 1)
            {
                w = w * ratio;
                h = h * ratio;
            }

            //取得图片大小
            Size size = new Size((int)w, (int)h);
            //新建一个bmp图片
            System.Drawing.Image bitmap = new Bitmap(size.Width, size.Height);
            //新建一个画板
            Graphics g = Graphics.FromImage(bitmap);
            //设置高质量插值法
            g.InterpolationMode = InterpolationMode.High;
            //设置高质量,低速度呈现平滑程度
            g.SmoothingMode = SmoothingMode.HighQuality;
            //清空一下画布
            g.Clear(Color.White);
            //在指定位置画图
            g.DrawImage(image, new Rectangle(0, 0, bitmap.Width, bitmap.Height), new Rectangle(0, 0, image.Width, image.Height), GraphicsUnit.Pixel);
            //保存高清晰度的缩略图
            MemoryStream stream = new MemoryStream();
            bitmap.Save(stream, ImageFormat.Jpeg);
            byte[] buffer = stream.GetBuffer();
            g.Dispose();
            image.Dispose();
            bitmap.Dispose();
            return buffer;
        }

        /// <summary>
        /// 创建缩略图
        /// </summary>
        /// <param name="src">来源页面可以是相对地址或者绝对地址 </param>
        /// <param name="width">缩略图宽度</param>
        /// <param name="height">缩略图高度</param>
        /// <param name="user"></param>
        /// <param name="password"></param>
        /// <param name="domain"></param>
        /// <returns>字节数组</returns>
        public static byte[] MakeThumbnail(string src, double width, double height, string user, string password, string domain)
        {
            System.Drawing.Image image;
            // 相对路径从本机直接读取
            if (src.ToLower().IndexOf("http://", System.StringComparison.Ordinal) == -1)
            {
                try
                {
                    src = HttpContext.Current.Server.MapPath(src);
                    image = System.Drawing.Image.FromFile(src, true);
                }
                catch (Exception)
                {
                    return new byte[0];
                }
            }
            else // 绝对路径从 Http 读取
            {
                try
                {
                    var myWebClient = new WebClient { Credentials = new NetworkCredential(user, password, domain) };
                    Stream receiveStream = myWebClient.OpenRead(src);
                    image = System.Drawing.Image.FromStream(receiveStream);
                    receiveStream.Close();
                }
                catch (Exception)
                {
                    return new byte[0];
                }
            }
            return MakeThumbnail(image, width, height);
        }
    }
}