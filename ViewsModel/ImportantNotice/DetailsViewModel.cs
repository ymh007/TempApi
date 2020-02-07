using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using System.Web;

namespace Seagull2.YuanXin.AppApi.ViewsModel.ImportantNotice
{
    /// <summary>
    /// 重要发文详情
    /// </summary>
    public class DetailsViewModel
    {
        /// <summary>
        /// 标题
        /// </summary>
        public string Title { set; get; }
        /// <summary>
        /// 时间
        /// </summary>
        public string Created { set; get; }
        /// <summary>
        /// 发布人
        /// </summary>
        public string Author
        {
            set { author = value; }
            get
            {
                return Regex.Replace(author, @"[0-9]*;#", "");//替换字符串中的 ID;#（1073741823;#系统帐户）;
            }
        }
        private string author;
        /// <summary>
        /// 发布单位
        /// </summary>
        public string Organization { set; get; }
        /// <summary>
        /// 说明
        /// </summary>
        public string Body
        {
            set { body = value; }
            get
            {
                if (string.IsNullOrWhiteSpace(body))
                {
                    return "暂无";
                }
                var value = body.Trim();
                var regex = @"<(?!img|br|p|/p|a|/a).*?>|\sstyle=""([^"";]+;?)+""|class=\w+|align=\w+|border=\w+|<a[\s\S]*?>|</a>";
                value = Regex.Replace(value, regex, "", RegexOptions.IgnoreCase);//只保留img,br,p,a 不区分大小写
                value = Regex.Replace(value, @"\s", "");//去空格
                value = Regex.Replace(value, @"\r\n", "");//去回车、去换行
                value = Regex.Replace(value, @"<p></p>", "");//去空P标签
                return value;
            }
        }
        private string body;
        /// <summary>
        /// 附件列表
        /// </summary>
        public List<string> Attachments { set; get; }
    }
}