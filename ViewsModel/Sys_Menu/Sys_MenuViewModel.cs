using Seagull2.YuanXin.AppApi.Adapter.Feedback;
using Seagull2.YuanXin.AppApi.Adapter.UserHeadPhoto;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Seagull2.YuanXin.AppApi.ViewsModel.Sys_Menu_
{
    /// <summary>
    /// 系统菜单
    /// </summary>
    public class Sys_MenuViewModel
    {
        /// <summary>
        ///  系统code
        /// </summary>
        public string SystemCode { set; get; }

        /// <summary>
        ///  系统名称
        /// </summary>
        public string SystemName { set; get; }

        /// <summary>
        ///  Key
        /// </summary>
        public string Key { set; get; }
        /// <summary>
        ///  Icon
        /// </summary>
        public string Icon { set; get; }
        /// <summary>
        ///  标题
        /// </summary>
        public string Title { set; get; }
        /// <summary>
        ///  路由
        /// </summary>
        public string Path { set; get; }

        /// <summary>
        ///  排序
        /// </summary>
        public int Sort { set; get; }
        /// <summary>
        ///  父级code
        /// </summary>
        public string ParentCode { set; get; }

        /// <summary>
        /// 子列表
        /// </summary>
        public List<SystemChild> Child { get; set; }
        /// <summary>
        /// 子菜单
        /// </summary>
        public class SystemChild
        {
            /// <summary>
            ///  系统code
            /// </summary>
            public string SystemCode { set; get; }

            /// <summary>
            ///  系统名称
            /// </summary>
            public string SystemName { set; get; }

            /// <summary>
            ///  Key
            /// </summary>
            public string Key { set; get; }
            /// <summary>
            ///  Icon
            /// </summary>
            public string Icon { set; get; }
            /// <summary>
            ///  标题
            /// </summary>
            public string Title { set; get; }
            /// <summary>
            ///  路由
            /// </summary>
            public string Path { set; get; }
            /// <summary>
            ///  父级key
            /// </summary>
            public string ParentKey { set; get; }

            /// <summary>
            ///  排序
            /// </summary>
            public int Sort { set; get; }
            /// <summary>
            ///  父级code
            /// </summary>
            public string ParentCode { set; get; }

            /// <summary>
            /// 待处理的任务数量 角标显示数字
            /// </summary>
            public int MsgCount
            {
                get
                {
                    int TotalMsg = 0;
                    if (Key == "avatarReview")
                    {
                        TotalMsg= UserHeadPhotoAdapter.Instance.GetListForPC(0, "");
                    }
                    if (Key == "feedback")
                    {
                        TotalMsg = FeedbackAdapter.Instance.GetNoReplayCount(1);
                    }
                    return TotalMsg;
                }

            }
        }
    }
}