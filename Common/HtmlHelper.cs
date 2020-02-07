using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text.RegularExpressions;
using System.Web;
using Seagull2.YuanXin.AppApi.Extension;

namespace Seagull2.YuanXin.AppApi
{
    /// <summary>
    /// Html标签帮助类
    /// </summary>
    public class HtmlHelper
    {
        private static readonly string _fileService = System.Configuration.ConfigurationManager.AppSettings["AppFileService"];

        /// <summary>
        /// 剔除标签,只保留img和p
        /// </summary>
        public static string Replace(string content)
        {
            var str = "";
            if (!string.IsNullOrEmpty(content))
            {
                string regexstr = @"<(?!img|br|p|/p|a|/a).*?>|\sstyle=""([^"";]+;?)+""|class=\w+|align=\w+|border=\w+|<a[\s\S]*?>|</a>";
                //去除所有标签，只剩img,br,p,去除剩余表内的style,class，align，border
                str = Regex.Replace(content.StringManipulation(regexstr), @"\s", "");
                var r = new Regex(@"<img[\s\S]*?>", RegexOptions.IgnoreCase);
                var col = r.Matches(str);
                var bigImage = System.Configuration.ConfigurationManager.AppSettings["BigImage"];
                if (col.Count != 0)
                {
                    var index = str.IndexOf("<p>" + col[0] + "</p>", StringComparison.Ordinal);
                    if (index < 0)
                    {
                        str = str.Replace("<p>", "").Replace("</p>", "");
                    }
                    for (int i = 0; i < col.Count; i++)
                    {
                        var item = col[i].ToString();
                        var src = item.Substring(item.IndexOf("/", StringComparison.Ordinal), item.IndexOf('>') - (item.IndexOf("/", StringComparison.Ordinal) + 1));
                        var oldstr = index < 0 ? item : "<p>" + item + "</p>";
                        var imgName = src.Split('/').LastOrDefault();
                        var imgdata = "data:image/Jpeg;base64," + Convert.ToBase64String(ImageProcessing(_fileService + src, 0, 0));
                        // var imgdata = _fileService + src;
                        var newstr = string.Format("<img src='{0}' title='查看大图' jumpurl='{1}'></br>", imgdata, bigImage + src);
                        str = str.Replace(oldstr, newstr);
                    }
                }
            }
            return str;
        }


        /// <summary>
        /// 富文本框图片base 64 转换为 url
        /// </summary>
        public static string ReplaceImgUrl(string content)
        {
            if (!string.IsNullOrEmpty(content))
            {
                Regex regImg = new Regex(@"<img\b[^<>]*?\bsrc[\s\t\r\n]*=[\s\t\r\n]*[""']?[\s\t\r\n]*(?<imgUrl>[^\s\t\r\n""'<>]*)[^<>]*?/?[\s\t\r\n]*>", RegexOptions.IgnoreCase);
                MatchCollection imgs = regImg.Matches(content);
                if (imgs.Count != 0)
                {
                    foreach (Match img in imgs)
                    {
                        string fullUrl = FileService.DownloadFile(img.Groups["imgUrl"].Value);
                        content = content.Replace(img.Groups["imgUrl"].Value, fullUrl);
                    }
                }
            }
            return content;
        }
        /// <summary>
        /// 富文本框图片base 64 转换为 url
        /// </summary>
        /// <param name="content"></param>
        /// <param name="isView">是否查看</param>
        /// <returns></returns>
        public static string ReplaceRow(string content)
        {
            if (!string.IsNullOrEmpty(content))
            {
                content = Regex.Replace(content, @"[\n\r]", "");//去回车、去换行
            }
            return content;
        }

        /// <summary>
        /// 图片剪裁
        /// </summary>
        /// <param name="mossPath"></param>
        /// <param name="width"></param>
        /// <param name="height"></param>
        /// <returns></returns>
        private static byte[] ImageProcessing(string mossPath, double width, double height)
        {

            var user = System.Configuration.ConfigurationManager.AppSettings["User"];
            var password = System.Configuration.ConfigurationManager.AppSettings["password"];
            var domain = System.Configuration.ConfigurationManager.AppSettings["Domain"];
            var bytes = PictureProcessing.MakeThumbnail(mossPath, width, height, user, password, domain);
            if (bytes.Length != 0) return bytes;
            var stream = Assembly.GetExecutingAssembly().GetManifestResourceStream("Seagull2.YuanXin.AppApi.Resources.404.jpg");
            System.Drawing.Image image = System.Drawing.Image.FromStream(stream);
            bytes = PictureProcessing.MakeThumbnail(image, width, height);

            return bytes;
        }
    }
}