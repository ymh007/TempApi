using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Seagull2.YuanXin.AppApi.Models
{
    public class BannerConfigModel
    {

        /// <summary>
        /// 来源编码
        /// </summary>            
        public string SourceId { get; set; }
        /// <summary>
        /// 图片路径
        /// </summary>            
        public string ImageUrl { get; set; }
        /// <summary>
        /// 轮播类型
        /// </summary>            
        public int Type { get; set; }
        /// <summary>
        /// 排序号
        /// </summary>           
        public int SortNo { get; set; }

        public string ImageType { get; set; }




    }
}