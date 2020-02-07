using MCS.Library.Data.DataObjects;
using MCS.Library.Data.Mapping;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Seagull2.YuanXin.AppApi.Models.Conference
{

    /// <summary>
    /// 会议大屏互动实体
    /// </summary>
    [ORTableMapping("office.ConferenceBigScreen")]
    public class ConferenceBigScreenModel:BaseModel
    {


        /// <summary>
        /// 会议id
        /// </summary>
        [ORFieldMapping("ConfereenceId")]
        public string ConfereenceId { get; set; }


        /// <summary>
        /// 是否模板
        /// </summary>
        [ORFieldMapping("IsSystem")]
        public bool IsSystem { get; set; }

        /// <summary>
        /// 背景图片
        /// </summary>
        [ORFieldMapping("BgUrl")]
        public string BgUrl { get; set; }

        /// <summary>
        /// 文字位置
        /// </summary>
        [ORFieldMapping("FontLocation")]
        public int FontLocation { get; set; }


        /// <summary>
        /// 文字大小
        /// </summary>
        [ORFieldMapping("FontSize")]
        public int FontSize { get; set; }



        /// <summary>
        /// 效果
        /// </summary>
        [ORFieldMapping("EffectStatus")]
        public string EffectStatus { get; set; }

        /// <summary>
        /// 是否显示二维码
        /// </summary>
        [ORFieldMapping("QrShow")]
        public bool QrShow { get; set; }


        /// <summary>
        /// 效果类型
        /// </summary>
        [ORFieldMapping("ScreenType")]
        public int ScreenType { get; set; }


        /// <summary>
        /// 图片base64
        /// </summary>
        [NoMapping]
        public string Image { get; set; }

    }

    /// <summary>
    /// 会话集合
    /// </summary>
    public class ConferenceBigScreenModelCollection : EditableDataObjectCollectionBase<ConferenceBigScreenModel>
    {

    }
}