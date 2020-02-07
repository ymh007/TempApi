using MCS.Library.Data.DataObjects;
using MCS.Library.Data.Mapping;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
namespace Seagull2.YuanXin.AppApi.Models
{
    /// <summary>
    /// 会话类型(IM,WeiXin)
    /// </summary>
    [ORTableMapping("office.DialogType")]
    public class DialogType
    {
        /// <summary>
        /// 编码
        /// </summary>
        [ORFieldMapping("ID", PrimaryKey = true)]
        public string ID { get; set; }
        /// <summary>
        /// 名称
        /// </summary>
        [ORFieldMapping("Name")]
        public string Name { get; set; }
    }

    /// <summary>
    /// 会话类型(IM,WeiXin)
    /// </summary>
    public class DialogTypeCollection : EditableDataObjectCollectionBase<DialogType>
    {

    }

    /// <summary>
    /// 会话类型适配器
    /// </summary>
    public class DialogTypeAdapter : BaseAdapter<DialogType, DialogTypeCollection>
    {
        /// <summary>
        /// 实例化
        /// </summary>
        public static DialogTypeAdapter Instance = new DialogTypeAdapter();

        private string ConnectionString = ConnectionNameDefine.YuanXinBusiness;

        /// <summary>
        /// 构造
        /// </summary>
        public DialogTypeAdapter()
        {
            BaseConnectionStr = this.ConnectionString;
        }
    }
}