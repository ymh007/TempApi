using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using System.Web;

namespace Seagull2.YuanXin.AppApi.ViewsModel.Moss
{
    #region 通知纪要基类
    /// <summary>
    /// 通知纪要基类
    /// </summary>
    public class NoticeBase
    {
        /// <summary>
        /// WebId（如：A8F5690C-24D1-4A28-AF6A-B1A634E0AE50）
        /// </summary>
        public string WebId { set; get; }
        /// <summary>
        /// ListId（如：FBB85F14-D478-4F56-B120-B3F163710DD9）
        /// </summary>
        public string ListId { set; get; }
        /// <summary>
        /// 编号（如：2189）
        /// </summary>
        public int ID { set; get; }
        /// <summary>
        /// 标题
        /// </summary>
        public string Title { set; get; }
        /// <summary>
        /// 创建时间（如：2017-07-03 11:18:05）
        /// </summary>
        public string Created { set; get; }
        /// <summary>
        /// 来源
        /// </summary>
        public string Source { set; get; }
    }
    #endregion

    #region 通知纪要列表
    /// <summary>
    /// 通知纪要列表
    /// </summary>
    public class NoticeListViewModel : NoticeBase
    {

    }
    #endregion

    #region 重要发文列表
    /// <summary>
    /// 重要发文列表
    /// </summary>
    public class ImportantNoticeListViewModel : NoticeBase
    {

    }
    #endregion

    #region 通知纪要详情
    /// <summary>
    /// 通知纪要详情
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
        /// 执行人
        /// </summary>
        public string Author { set; get; }
        /// <summary>
        /// 执行人单位
        /// </summary>
        public string Organization { set; get; }
        /// <summary>
        /// 正文
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
        /// ResourceId
        /// </summary>
        public string ResourceId { set; get; }
        /// <summary>
        /// 是否收藏
        /// </summary>
        public bool IsFav { set; get; }
        /// <summary>
        /// 附件列表
        /// </summary>
        public List<Attachment> Attachments { set; get; }
    }
    /// <summary>
    /// 通知纪要详情附件
    /// </summary>
    public class Attachment
    {
        /// <summary>
        /// 附件名称
        /// </summary>
        public string Name { set; get; }
        /// <summary>
        /// 附件预览地址
        /// </summary>
        public string Url { set; get; }
        /// <summary>
        /// 附件原始地址
        /// </summary>
        public string OriginalUrl { set; get; }
    }
    #endregion
}