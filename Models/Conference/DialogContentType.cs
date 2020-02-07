using MCS.Library.Data.DataObjects;
using MCS.Library.Data.Mapping;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
namespace Seagull2.YuanXin.AppApi.Models
{
    /// <summary>
    /// 会话内容类型(现场服务，预定车辆)
    /// </summary>
    [ORTableMapping("office.DialogContentType")]
    public class DialogContentType
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
    /// 会话内容类型(现场服务，预定车辆)
    /// </summary>
    public class DialogContentTypeCollection : EditableDataObjectCollectionBase<DialogContentType>
    {

    }

    /// <summary>
    /// 会话内容类型适配器
    /// </summary>
    public class DialogContentTypeAdapter : BaseAdapter<DialogContentType, DialogContentTypeCollection>
    {
        /// <summary>
        /// 实例化
        /// </summary>
        public static DialogContentTypeAdapter Instance = new DialogContentTypeAdapter();

        private string ConnectionString = ConnectionNameDefine.YuanXinBusiness;
        /// <summary>
        /// 构造
        /// </summary>
        public DialogContentTypeAdapter()
        {
            BaseConnectionStr = this.ConnectionString;
        }
    }
}