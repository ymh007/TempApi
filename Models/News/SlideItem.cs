using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Seagull2.YuanXin.AppApi.Models
{
    /// <summary>
    /// 推荐图片新闻
    /// </summary>
    public class SlideItem
    {
        string _webId;
        string _userName;
        /// <summary>
        /// 构造
        /// </summary>
        public SlideItem(string webId, string userName)
        {
            _webId = webId;
            _userName = userName;
        }
        /// <summary>
        /// 标题
        /// </summary>
        public string Name { get; set; }
        /// <summary>
        /// 图片Url
        /// </summary>
        public string ImgUrl { get; set; }
        /// <summary>
        /// 跳转Url
        /// </summary>
        public string RedirectUrl { get; set; }
        /// <summary>
        /// 是否置顶
        /// </summary>
        public string IsTop { get; set; }
        /// <summary>
        /// 是否视频
        /// </summary>
        public string IsVideo { get; set; }
        /// <summary>
        /// 是否外部Url
        /// </summary>
        public bool OutSideUrl { get; set; }
        /// <summary>
        /// 发布时间
        /// </summary>
        public string DateFull { set; get; }
        /// <summary>
        /// 来源
        /// </summary>
        public string Source
        {
            get
            {
                MossNewService ms = new MossNewService("News.xml");
                var web = ms.GetWebModel(_webId, _userName);
                if (web != null)
                {
                    return web.Title;
                }
                return string.Empty;
            }
        }
    }
}