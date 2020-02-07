using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Reflection;
using System.Web.Http;
using log4net;
using Newtonsoft.Json;
using System.Drawing;
using System.IO;
using System.Drawing.Imaging;
using System.Web;
using System.Configuration;

namespace Seagull2.YuanXin.AppApi.Controllers
{
    /// <summary>
    /// 图片处理Controller
    /// </summary>
    public class ImageController : ApiController
    {
        ILog log = LogManager.GetLogger(MethodBase.GetCurrentMethod().DeclaringType);

        /// <summary>
        /// 将根据图片URL返回图片
        /// </summary>
        /// <param name="url">图片URL（如：http://km.sinooceanland.com/sites/NewsCenter/groupnews/PublishingImages/高效高质量交付提升客户体验/1.jpg）</param>
        [HttpGet, AllowAnonymous]
        public HttpResponseMessage GetImage(string url)
        {
            try
            {
                Uri uri = new Uri(url);

                var webClient = new WebClient()
                {
                    Credentials = new NetworkCredential(ConfigAppSetting.User, ConfigAppSetting.Password, ConfigAppSetting.Domain)
                };
                byte[] bytes = webClient.DownloadData(uri);
                webClient.Dispose();

                // 把字节数组转成图片
                Image img = ByteArrayToImage(bytes);

                // 压缩图片像素宽度和高度
                Bitmap map = PercentImage(img);
                Image ysimg = map;

                //对图片进行质量压缩
                bytes = GetPicThumbnail(ysimg, 50);

                var response = new HttpResponseMessage(HttpStatusCode.OK)
                {
                    Content = new ByteArrayContent(bytes)
                };
                response.Content.Headers.ContentType = new System.Net.Http.Headers.MediaTypeHeaderValue("image/jpg");

                return response;
            }
            catch (Exception e)
            {
                log.Error("获取Moss图片异常：" + JsonConvert.SerializeObject(e));

                var stream = Assembly.GetExecutingAssembly().GetManifestResourceStream("Seagull2.YuanXin.AppApi.Resources.404.jpg");
                byte[] bytes = new byte[stream.Length];
                stream.Read(bytes, 0, bytes.Length);

                var response = new HttpResponseMessage(HttpStatusCode.OK)
                {
                    Content = new ByteArrayContent(bytes)
                };
                response.Content.Headers.ContentType = new System.Net.Http.Headers.MediaTypeHeaderValue("image/jpg");

                return response;
            }
        }
        
        /// <summary>
        /// 对原始图片的宽高进行压缩
        /// </summary>
        /// <param name="srcImage">原始图片</param>
        /// <returns></returns>
        public Bitmap PercentImage(Image srcImage)
        {
            int newW = srcImage.Width < 800 ? srcImage.Width : 800;
            int newH = int.Parse(Math.Round(srcImage.Height * (double)newW / srcImage.Width).ToString());

            try
            {
                Bitmap b = new Bitmap(newW, newH);
                Graphics g = Graphics.FromImage(b);

                g.InterpolationMode = System.Drawing.Drawing2D.InterpolationMode.Default;
                g.DrawImage(srcImage, new Rectangle(0, 0, newW, newH), new Rectangle(0, 0, srcImage.Width, srcImage.Height), GraphicsUnit.Pixel);
                g.Dispose();
                return b;
            }
            catch (Exception)
            {
                return null;
            }
        }
        
        /// <summary>
        /// 按照比例压缩图片并返回对应的字节数组
        /// </summary>
        /// <param name="iSource">原始图片</param>       
        /// <param name="flag">压缩比1-100</param>
        /// <returns></returns>
        private byte[] GetPicThumbnail(Image iSource, int flag)
        {
            ImageFormat tFormat = iSource.RawFormat;
            //以下代码为保存图片时，设置压缩质量 
            EncoderParameters ep = new EncoderParameters();
            long[] qy = new long[1];
            qy[0] = flag;//设置压缩的比例1-100 
            EncoderParameter eParam = new EncoderParameter(Encoder.Quality, qy);
            ep.Param[0] = eParam;
            string baseUrl = HttpContext.Current.Server.MapPath(ConfigurationManager.AppSettings["ConferenceImageUploadRootPath"]);//图片保存路径
            string newFileName = Guid.NewGuid().ToString("N") + ".jpg";
            string outPath = baseUrl + "\\" + newFileName;
            try
            {
                ImageCodecInfo[] arrayICI = ImageCodecInfo.GetImageEncoders();
                ImageCodecInfo jpegICIinfo = null;
                for (int x = 0; x < arrayICI.Length; x++)
                {
                    if (arrayICI[x].FormatDescription.Equals("JPEG"))
                    {
                        jpegICIinfo = arrayICI[x];
                        break;
                    }
                }

                //先把压缩后的图片临时保存至Images文件夹中
                if (jpegICIinfo != null)
                {
                    iSource.Save(outPath, jpegICIinfo, ep);
                }
                else
                {
                    iSource.Save(outPath, tFormat);
                }

                //把压缩后的图片转换成字节数组              
                Image img = Image.FromFile(outPath);
                byte[] bytes = ImageToByte(img);
                img.Dispose();
                //然后再把临时保存的图片删除              
                if (System.IO.File.Exists(outPath))
                {
                    System.IO.File.Delete(outPath);
                }
                return bytes;
            }
            catch (Exception e)
            {
                log.Error("压缩图片异常：" + JsonConvert.SerializeObject(e));

                var stream = Assembly.GetExecutingAssembly().GetManifestResourceStream("Seagull2.YuanXin.AppApi.Resources.404.jpg");
                byte[] bytes = new byte[stream.Length];
                stream.Read(bytes, 0, bytes.Length);
                return bytes;
            }
            finally
            {
                iSource.Dispose();
                iSource.Dispose();
            }
        }

        /// <summary>
        /// 字节数组生成图片
        /// </summary>
        /// <param name="Bytes">字节数组</param>
        /// <returns>图片</returns>
        Image ByteArrayToImage(byte[] Bytes)
        {
            MemoryStream ms = new MemoryStream(Bytes);
            return Image.FromStream(ms, true);
        }

        /// <summary>
        /// 图片转为字节数组
        /// </summary>
        /// <param name="image"></param>
        /// <returns>byte[]</returns>
        byte[] ImageToByte(Image image)
        {
            ImageFormat format = image.RawFormat;
            using (MemoryStream ms = new MemoryStream())
            {
                //if (format.Equals(ImageFormat.Jpeg))
                //{
                //    image.Save(ms, ImageFormat.Jpeg);
                //}
                //else if (format.Equals(ImageFormat.Png))
                //{
                //    image.Save(ms, ImageFormat.Png);
                //}
                //else if (format.Equals(ImageFormat.Bmp))
                //{
                //    image.Save(ms, ImageFormat.Bmp);
                //}
                //else if (format.Equals(ImageFormat.Gif))
                //{
                //    image.Save(ms, ImageFormat.Gif);
                //}
                //else if (format.Equals(ImageFormat.Icon))
                //{
                //    image.Save(ms, ImageFormat.Icon);
                //}
                image.Save(ms, ImageFormat.Jpeg);
                byte[] buffer = new byte[ms.Length];
                //Image.Save()会改变MemoryStream的Position，需要重新Seek到Begin
                ms.Seek(0, SeekOrigin.Begin);
                ms.Read(buffer, 0, buffer.Length);
                return buffer;
            }
        }
    }
}