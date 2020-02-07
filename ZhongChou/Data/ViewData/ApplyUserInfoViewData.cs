using MCS.Library.Data.DataObjects;
using MCS.Library.Data.Mapping;
using MobileBusiness.Common.Data;
using Seagull2.YuanXin.AppApi.Adapter.AddressBook;
using Seagull2.YuanXin.AppApi.Models.AddressBook;
using Seagull2.YuanXin.AppApi.ZhongChouData.Common;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Linq;
using System.Text;

namespace Seagull2.YuanXin.AppApi.ZhongChouData.ViewData
{
    /// <summary>
    /// 
    /// </summary>
    public class ApplyUserInfoViewData
    {
        /// <summary>
        /// 
        /// </summary>
        public string UserID { get; set; }

        private ContactsModel UserInfo
        {
            get
            {
                return ContactsAdapter.Instance.LoadByCode(this.UserID);
            }
        }
        /// <summary>
        /// 
        /// </summary>
        public string UserName
        {
            get
            {
                if (UserInfo != null)
                {
                    return UserInfo.DisplayName;
                }
                return "";
            }
        }

        /// <summary>
        /// --用户昵称
        /// </summary>
        public string NickName
        {
            get
            {
                if (UserInfo != null)
                {
                    return UserInfo.DisplayName;
                }
                return "";
            }
        }

        /// <summary>
        /// 用户头像
        /// </summary>
        public string HeadImage
        {
            get
            {
                if (UserInfo != null)
                {
                    return UserHeadPhotoService.GetUserHeadPhoto(UserInfo.ObjectID); 
                }
                return "";
            }
        }

        /// <summary>
        ///  用户手机
        /// </summary>
        public string Phone
        {
            get
            {
                if (UserInfo != null)
                {
                    return UserInfo.MP;
                }
                return "";
            }
        }

        /// <summary>
        /// --场次时间
        /// </summary>
        public DateTime EventStartTime { get; set; }

        /// <summary>
        /// --场次时间格式
        /// </summary>
        [NoMapping]
        public string EventStartTimeFormate
        {
            get
            {
                return EventStartTime.Month.ToString() + "月" + EventStartTime.Day.ToString() + "日";
            }
        }

        /// <summary>
        /// 报名时间
        /// </summary>
        public DateTime ApplyTime { get; set; }

        /// <summary>
        /// 报名用户总数量
        /// </summary>
        public int ApplyUserCount { get; set; }

        /// <summary>
        /// 
        /// </summary>
        [NoMapping]
        public string ApplyTimeFormate
        {
            get
            {
                return CommonHelper.APPDateFormateDiff(ApplyTime, DateTime.Now);
            }
        }

        /// <summary>
        /// 商品分数
        /// </summary>
        public int GoodsCount { get; set; }
    }

    /// <summary>
    /// 
    /// </summary>
    public class ApplyUserInfoViewDataCollection : EditableDataObjectCollectionBase<ApplyUserInfoViewData>
    {
        /// <summary>
        /// 转化为ListDataView
        /// </summary>
        /// <param name="pageIndex">当前页码</param>
        /// <param name="pageSize">页大小</param>
        /// <param name="totalCount">总行数</param>
        /// <returns></returns>
        public ListDataView ToListDataView(int pageIndex, int pageSize, int totalCount)
        {
            var result = new ListDataView
            {
                PageIndex = pageIndex,
                PageCount = totalCount % pageSize > 0 ? totalCount / pageSize + 1 : totalCount / pageSize,
                TotalCount = totalCount,
                ListData = this
            };

            return result;
        }
    }

}
