using System;
using System.Collections.Generic;
using MCS.Library.OGUPermission;
using Newtonsoft.Json;
using Seagull2.YuanXin.AppApi.Adapter.ActivityNew;

namespace Seagull2.YuanXin.AppApi.ViewsModel.ActivityNew
{
    #region 活动发布/编辑 ViewModel
    /// <summary>
    /// 活动发布/编辑 ViewModel
    /// </summary>
    public class ActivitySaveViewModel
    {
        /// <summary>
        /// 活动编码
        /// </summary>
        public string Code { set; get; }
        /// <summary>
        /// 活动主题
        /// </summary>
        public string Title { set; get; }
        /// <summary>
        /// 封面图片
        /// </summary>
        public string Cover { set; get; }
        /// <summary>
        /// 活动开始时间
        /// </summary>
        public DateTime StartTime { set; get; }
        /// <summary>
        /// 活动结束时间
        /// </summary>
        public DateTime EndTime { set; get; }
        /// <summary>
        /// 是否为线下活动
        /// </summary>
        public bool IsOffline { set; get; }
        /// <summary>
        /// 线下活动详细地址
        /// </summary>
        public string OfflineAddress { set; get; }
        /// <summary>
        /// 活动详情描述
        /// </summary>
        public string Description { set; get; }
        /// <summary>
        /// 咨询电话
        /// </summary>
        public string Contact { get; set; }
        /// <summary>
        /// 结束报名时间
        /// </summary>
        public DateTime ApplyEndTime { set; get; }
        /// <summary>
        /// 活动分类编码列表
        /// </summary>
        public List<string> CategoryCodeList { set; get; }
        /// <summary>
        /// 是否全部人员可以报名
        /// </summary>
        public bool IsApplyAll { get; set; }
        /// <summary>
        /// 活动报名范围设置
        /// </summary>
        public List<ActivityApplySetOrigSaveViewModel> ApplySetList { get; set; }
    }
    #endregion

    #region 活动信息基类
    /// <summary>
    /// 活动信息基类
    /// </summary>
    public abstract class ActivityInfoBaseViewModel
    {
        /// <summary>
        /// 活动编码
        /// </summary>
        public string Code { set; get; }
        /// <summary>
        /// 主题
        /// </summary>
        public string Title { set; get; }
        /// <summary>
        /// 封面图
        /// </summary>
        public string Cover { set; get; }
        /// <summary>
        /// 开始时间 JsonIgnore
        /// </summary>
        [JsonIgnore]
        public DateTime StartTime { set; get; }
        /// <summary>
        /// 开始时间字符串格式
        /// </summary>
        public string StartTimeSort { get { return StartTime.ToString("yyyy-MM-dd HH:mm"); } }
        /// <summary>
        /// 结束时间 JsonIgnore
        /// </summary>
        [JsonIgnore]
        public DateTime EndTime { set; get; }
        /// <summary>
        /// 结束时间字符串格式
        /// </summary>
        public string EndTimeSort { get { return EndTime.ToString("yyyy-MM-dd HH:mm"); } }
        /// <summary>
        /// 活动状态
        /// </summary>
        public string State
        {
            get
            {
                if (DateTime.Now < StartTime)
                {
                    return "未开始";
                }
                else if (StartTime < DateTime.Now && DateTime.Now < EndTime)
                {
                    return "进行中";
                }
                else
                {
                    return "已结束";
                }
            }
        }
        /// <summary>
        /// 是否线下活动
        /// </summary>
        public bool IsOffline { set; get; }
        /// <summary>
        /// 线下活动地址
        /// </summary>
        public string OfflineAddress
        {
            set { _OfflineAddress = value; }
            get
            {
                if (IsOffline)
                {
                    return _OfflineAddress;
                }
                else
                {
                    return string.Empty;
                }
            }
        }
        string _OfflineAddress;
        /// <summary>
        /// 创建时间 JsonIgnore
        /// </summary>
        [JsonIgnore]
        public DateTime CreateTime { get; set; }
        /// <summary>
        /// 创建时间
        /// </summary>
        public string CreateTimeStr
        {
            get
            {
                return this.CreateTime.ToString("yyyy-MM-dd HH:mm:ss");
            }
        }
    }
    #endregion

    #region 全部活动列表 ViewModel
    /// <summary>
    /// 全部活动列表 ViewModel
    /// </summary>
    public class AppActivityListForAllViewModel : ActivityInfoBaseViewModel
    {
        /// <summary>
        /// 是否关注
        /// </summary>
        public bool IsFollow
        {
            get
            {
                DateTime dt;
                return DateTime.TryParse(FollowTime, out dt);
            }
        }
        /// <summary>
        /// 关注时间
        /// </summary>
        [JsonIgnore]
        public string FollowTime { set; get; }
        /// <summary>
        /// 报名人数
        /// </summary>
        public int ApplyCount
        {
            get
            {
                return ActivityApplyInfoAdapter.Instance.GetApplyCountByActivityCode(base.Code);
            }
        }
    }
    #endregion

    #region 我参与的活动列表 ViewModel
    /// <summary>
    /// 我参与的活动列表 ViewModel
    /// </summary>
    public class AppActivityListForApplyViewModel : ActivityInfoBaseViewModel
    {
        /// <summary>
        /// 创建人编码
        /// </summary>
        public string CreatorCode { set; get; }
        /// <summary>
        /// 创建人姓名
        /// </summary>
        public string CreatorName
        {
            get
            {
                try
                {
                    OguObjectCollection<IUser> users = OguMechanismFactory.GetMechanism().GetObjects<IUser>(SearchOUIDType.Guid, this.CreatorCode);
                    return users[0].DisplayName;
                }
                catch
                {
                    return "未知用户";
                }
            }
        }
        /// <summary>
        /// 联系电话
        /// </summary>
        public string Contact { set; get; }
    }
    #endregion

    #region 我发布的活动列表 ViewModel
    /// <summary>
    /// 我发布的活动列表 ViewModel
    /// </summary>
    public class AppActivityListForMyViewModel : ActivityInfoBaseViewModel
    {

    }
    #endregion

    #region 活动详细信息 ViewModel
    /// <summary>
    /// 活动详细信息 ViewModel
    /// </summary>
    public class AppActivityInfoViewModel : ActivityInfoBaseViewModel
    {
        string CurrentUserCode;
        /// <summary>
        /// 活动详细信息 ViewModel
        /// </summary>
        /// <param name="currentUserCode">当前用户Code</param>
        public AppActivityInfoViewModel(string currentUserCode)
        {
            CurrentUserCode = currentUserCode;
        }
        /// <summary>
        /// 详情描述
        /// </summary>
        public string Description { set; get; }
        /// <summary>
        /// 创建人编码
        /// </summary>
        public string CreatorCode { set; get; }
        /// <summary>
        /// 创建人姓名
        /// </summary>
        public string CreatorName
        {
            get
            {
                try
                {
                    OguObjectCollection<IUser> users = OguMechanismFactory.GetMechanism().GetObjects<IUser>(SearchOUIDType.Guid, this.CreatorCode);
                    return users[0].DisplayName;
                }
                catch
                {
                    return "未知用户";
                }
            }
        }
        /// <summary>
        /// 联系电话
        /// </summary>
        public string Contact { set; get; }
        /// <summary>
        /// 浏览次数
        /// </summary>
        public int ViewCount
        {
            get
            {
                return ActivityRecordAdapter.Instance.GetCount(base.Code, 0);
            }
        }
        /// <summary>
        /// 是否关注
        /// </summary>
        public bool IsFollow
        {
            get
            {
                return ActivityRecordAdapter.Instance.Exists(w =>
                {
                    w.AppendItem("ActivityCode", this.Code);
                    w.AppendItem("UserCode", this.CurrentUserCode);
                    w.AppendItem("Type", (int)Models.ActivityNew.ActivityRecordType.Follow);
                });
            }
        }
        /// <summary>
        /// 是否报名
        /// </summary>
        public bool IsApply
        {
            get
            {
                return ActivityApplyInfoAdapter.Instance.Exists(w =>
                {
                    w.AppendItem("ActivityCode", this.Code);
                    w.AppendItem("UserCode", this.CurrentUserCode);
                });
            }
        }
        /// <summary>
        /// 结束报名时间
        /// </summary>
        [JsonIgnore]
        public DateTime ApplyEndTime { set; get; }
        /// <summary>
        /// 是否结束报名
        /// </summary>
        public bool IsApplyEnd
        {
            get
            {
                if (this.ApplyEndTime > DateTime.Now)
                {
                    return false;
                }
                return true;
            }
        }
        /// <summary>
        /// 活动分类列表
        /// </summary>
        public List<ActivityCategoryForAppViewModel> CategoryList
        {
            get
            {
                var table = ActivityCategoryAdapter.Instance.GetCategoryByActivityCode(this.Code);
                return DataConvertHelper<ActivityCategoryForAppViewModel>.ConvertToList(table);
            }
        }
        /// <summary>
        /// 活动报名人员列表
        /// </summary>
        public List<ActivityApplyInfoViewModel.UserInfoViewModel> ApplyUserList
        {
            get
            {
                return ActivityApplyInfoViewModel.ToList(this.Code);
            }
        }
        /// <summary>
        /// 报名范围设置列表
        /// </summary>
        public List<ActivityApplySetOrigShowViewModel> ApplySetList
        {
            get
            {
                var view = new List<ActivityApplySetOrigShowViewModel>();
                ActivityApplySetOrigAdapter.Instance.Load(w => w.AppendItem("ActivityCode", this.Code)).ForEach(item =>
                {
                    view.Add(new ActivityApplySetOrigShowViewModel()
                    {
                        Code = item.SelectCode,
                        Type = item.SelectType
                    });
                });
                return view;
            }
        }
    }
    #endregion

    #region 后端管理 ViewModel
    /// <summary>
    /// 后端管理 ViewModel
    /// </summary>
    public class ActivityListForPC : ActivityInfoBaseViewModel
    {
        /// <summary>
        /// 封面图片
        /// </summary>
        public new string Cover
        {
            set { _Conver = value; }
            get
            {
                return FileService.DownloadFile(this._Conver);
            }

        }
        private string _Conver;
        /// <summary>
        /// 发布人编码
        /// </summary>
        public string CreatorCode { set; get; }
        /// <summary>
        /// 发布人名称
        /// </summary>
        public string CreatorName
        {
            get
            {
                try
                {
                    OguObjectCollection<IUser> users = OguMechanismFactory.GetMechanism().GetObjects<IUser>(SearchOUIDType.Guid, this.CreatorCode);
                    return users[0].DisplayName;
                }
                catch
                {
                    return "未知用户";
                }
            }
        }
    }
    #endregion

    #region 活动统计信息及报名人员列表
    /// <summary>
    /// 活动统计信息及报名人员列表
    /// </summary>
    public class AppActivityInfoStatisticsViewModel
    {
        /// <summary>
        /// 活动编码
        /// </summary>
        public string ActivityCode { set; get; }
        /// <summary>
        /// 报名人数
        /// </summary>
        public int ApplyCount
        {
            get
            {
                return this.ApplyUserList.Count;
            }
        }
        /// <summary>
        /// 浏览次数
        /// </summary>
        public int ViewCount
        {
            get
            {
                return ActivityRecordAdapter.Instance.GetCount(this.ActivityCode, Models.ActivityNew.ActivityRecordType.View);
            }
        }
        /// <summary>
        /// 关注人数
        /// </summary>
        public int FollowCount
        {
            get
            {
                return ActivityRecordAdapter.Instance.GetCount(this.ActivityCode, Models.ActivityNew.ActivityRecordType.Follow);
            }
        }
        /// <summary>
        /// 报名人员列表
        /// </summary>
        public List<ActivityApplyInfoViewModel.UserInfoViewModel> ApplyUserList
        {
            get
            {
                return ActivityApplyInfoViewModel.ToList(this.ActivityCode);
            }
        }
    }
    #endregion
}