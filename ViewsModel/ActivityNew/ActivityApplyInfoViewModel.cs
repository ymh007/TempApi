using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Seagull2.YuanXin.AppApi.ViewsModel.ActivityNew
{
    /// <summary>
    /// 活动报名 ViewModel
    /// </summary>
    public class ActivityApplyInfoViewModel
    {
        #region 报名 ViewModel
        /// <summary>
        /// 报名 ViewModel
        /// </summary>
        public class ApplyViewModel
        {
            /// <summary>
            /// 活动编码
            /// </summary>
            public string ActivityCode { get; set; }

            /// <summary>
            /// 用户信息ViewModel
            /// </summary>
            public List<ApplyUserInfoViewModel> UserList { get; set; }
        }
        #endregion

        #region 报名用户信息 ViewModel
        /// <summary>
        /// 报名用户信息 ViewModel
        /// </summary>
        public class ApplyUserInfoViewModel
        {
            /// <summary>
            /// 用户姓名
            /// </summary>
            public string UserName { get; set; }

            /// <summary>
            /// 电话号码
            /// </summary>
            public string PhoneNumber { get; set; }
        }
        #endregion

        #region 用户信息 ViewModel
        /// <summary>
        /// 用户信息 ViewModel
        /// </summary>
        public class UserInfoViewModel
        {
            /// <summary>
            /// 报名编码
            /// </summary>
            public string Code { set; get; }
            /// <summary>
            /// 用户编码
            /// </summary>
            public string UserCode { get; set; }

            /// <summary>
            /// 用户姓名
            /// </summary>
            public string UserName { get; set; }

            /// <summary>
            /// 电话号码
            /// </summary>
            public string PhoneNumber { get; set; }
        }
        #endregion

        #region 取消报名 ViewModel
        /// <summary>
        /// 取消报名（用户自己取消）ViewModel
        /// </summary>
        public class CancelByUserViewModel
        {
            /// <summary>
            /// 活动编码
            /// </summary>
            public string ActivityCode { get; set; }
        }

        /// <summary>
        /// 取消报名（活动发起者取消）ViewModel
        /// </summary>
        public class CancelByActivityCreatorViewModel
        {
            /// <summary>
            /// 报名编码
            /// </summary>
            public string ApplyCode { get; set; }
        }
        #endregion

        #region 返回活动的报名人员列表 Method
        /// <summary>
        /// 返回活动的报名人员列表 Method
        /// </summary>
        public static List<UserInfoViewModel> ToList(string activityCode)
        {
            var view = new List<UserInfoViewModel>();
            var data = Adapter.ActivityNew.ActivityApplyInfoAdapter.Instance.Load(
                w => { w.AppendItem("ActivityCode", activityCode); },
                o => { o.AppendItem("CreateTime", MCS.Library.Data.Builder.FieldSortDirection.Ascending); });
            data.ForEach(m =>
            {
                view.Add(new ActivityApplyInfoViewModel.UserInfoViewModel()
                {
                    Code = m.Code,
                    UserCode = m.UserCode,
                    UserName = m.UserName,
                    PhoneNumber = m.PhoneNumber
                });
            });
            return view;
        }
        #endregion
    }
}