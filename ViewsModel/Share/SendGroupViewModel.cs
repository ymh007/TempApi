using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Seagull2.YuanXin.AppApi.ViewsModel.Share
{
    /// <summary>
    /// 群组
    /// </summary>
    public class SendGroupFullViewModel
    {
        /// <summary>
        /// 群组信息
        /// </summary>
        public SendGroupViewModel Group { set; get; }
        /// <summary>
        /// 人员列表
        /// </summary>
        public List<SendGroupPersonViewModel> Persons { set; get; }
    }

    /// <summary>
    /// 群组信息
    /// </summary>
    public class SendGroupViewModel
    {
        /// <summary>
        /// 群组编码
        /// </summary>
        public string Code { set; get; }
        /// <summary>
        /// 群组名称
        /// </summary>
        public string Name { set; get; }
    }

    /// <summary>
    /// 群组人员信息
    /// </summary>
    public class SendGroupPersonViewModel
    {
        /// <summary>
        /// 人员编码
        /// </summary>
        public string Code { set; get; }
        /// <summary>
        /// 人员名称
        /// </summary>
        public string Name { set; get; }
    }
}