using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Seagull2.YuanXin.AppApi.Models
{
    public class OguUserInfo
    {
        /// <summary>
        /// 对象的ID
        /// </summary>
        public string ID { get; set; }

        /// <summary>
        /// 对象的名称
        /// </summary>
        public string Name { get; set; }

        /// <summary>
        /// 对象的显示名称
        /// </summary>
        public string DisplayName { get; set; }

        /// <summary>
        /// 全路径
        /// </summary>
        public string FullPath { get; set; }

        /// <summary>
        /// 对象的类型
        /// </summary>
        public string ObjectType { get; set; }

        /// <summary>
        /// 是否已展开
        /// </summary>
        public bool IsSpread { get; set; }

        /// <summary>
        /// 是否有子集
        /// </summary>
        public bool IsHasChild { get; set; }

        /// <summary>
        /// 是否有用户
        /// </summary>
        public bool IsHasUser{ get; set; }
        /// <summary>
        /// 用户数量
        /// </summary>
        public int  UserCount { get; set; }
    }
}