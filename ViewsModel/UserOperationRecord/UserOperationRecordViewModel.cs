using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Seagull2.YuanXin.AppApi.ViewsModel.UserOperationRecord
{
    /// <summary>
    /// 添加用户操作记录 ViewModel
    /// </summary>
    public class AddUserOperationRecordViewModel
    {
        /// <summary>
        /// 模块标识
        /// </summary>
        public UserOperationRecordModule Module { set; get; }
    }

    /// <summary>
    /// Module 枚举
    /// </summary>
    public enum UserOperationRecordModule
    {
        /// <summary>
        /// 打卡提醒设置
        /// </summary>
        PunchRemindSetting
    }
}