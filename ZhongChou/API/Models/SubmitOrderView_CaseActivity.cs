using MCS.Library.OGUPermission;
using Seagull2.YuanXin.AppApi.Models.AddressBook;
using Seagull2.YuanXin.AppApi.ZhongChouData.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Seagull2.YuanXin.AppApi.ZhongChouApi.Models
{
    public class SubmitOrderView_CaseActivity
    {
        public SubmitOrderView_CaseActivity(string projectCode, string userCode)
        {
            var project = ProjectAdapter.Instance.LoadByCode(projectCode);
            this.ActivityEvents = ActivityEventAdapter.Instance.LoadByProjectCode(projectCode);
            //this.UserInfo = UserInfoAdapter.Instance.LoadByCode(userCode);
            if (project.SubItemJoinLimit == 1)
            {
                SubmitLimit = true;
                for (int i = 0; i < ActivityEvents.Count;i++ )
                {
                    var orders = OrderAdapter.Instance.LoadCaseOrderBySubProject(userCode, ActivityEvents[i].Code);
                    if (orders != null)
                    {
                        ActivityEvents[i].IsApply = true;
                    }
                    else {
                        ActivityEvents[i].IsApply = false;
                    }
                }
            }
            else
            {
                SubmitLimit = false;
            }
        }

        /// <summary>
        /// 场次
        /// </summary>
        public ActivityEventCollection ActivityEvents { get; set; }

        /// <summary>
        /// 用户信息--编码，姓名，电话？？
        /// </summary>
        public ContactsModel UserInfo { get; set; }
        /// <summary>
        /// 是否限制报名
        /// </summary>
        public bool SubmitLimit { get; set; }
    }
}