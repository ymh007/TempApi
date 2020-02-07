using System;
using System.Linq;
using System.Web;
using System.Collections.Generic;
using MCS.Library.Core;
using MCS.Library.Data;
using MCS.Library.Data.Mapping;
using MCS.Library.Data.DataObjects;
using MobileBusiness.Common.Data;
using Seagull2.YuanXin.AppApi.ZhongChouData.Common;

namespace Seagull2.YuanXin.AppApi.ZhongChouData.Models
{
    /// <summary>
    /// 活动场次
    /// </summary>
    [Serializable]
    [ORTableMapping("zc.ActivityEvent")]
    public class ActivityEvent
    {
        /// <summary>
        /// 编码
        /// </summary>
        [ORFieldMapping("Code", PrimaryKey = true)]
        public string Code { get; set; }

        /// <summary>
        /// 众筹项目编码
        /// </summary>
        [ORFieldMapping("ProjectCode")]
        public string ProjectCode { get; set; }

        /// <summary>
        /// 开始时间
        /// </summary>
        [ORFieldMapping("StartTime")]
        public DateTime StartTime { get; set; }

        /// <summary>
        /// 场次时间（yyyy-MM-dd 星期几 hh:mm）
        /// </summary>
        [NoMapping]
        public string ActivityEventTimeFormart
        {
            get
            {
                return StartTime.ToString("yyyy-MM-dd") + " " + StartTime.ToString("dddd", new System.Globalization.CultureInfo("zh-cn")) + " " + StartTime.ToShortTimeString().ToString();
            }
        }

        [NoMapping]
        public string StartTimeFormart
        {
            get
            {
                return StartTime.ToString("yyyy-MM-dd HH:mm");
            }
        }
        /// <summary>
        /// 结束时间
        /// </summary>
        [ORFieldMapping("EndTime")]
        public DateTime EndTime { get; set; }
        [NoMapping]
        public string EndTimeFormart
        {
            get
            {
                if (EndTime.Year > 1000)
                {
                    return EndTime.ToString("yyyy-MM-dd HH:mm");
                }
                else
                {
                    return StartTime.AddHours(Hours).ToString("yyyy-MM-dd HH:mm");
                }
            }
        }

        /// <summary>
        /// 限制人数
        /// </summary>
        [ORFieldMapping("LimitNo")]
        public int LimitNo { get; set; }

        /// <summary>
        /// 限制人数
        /// </summary>
        [NoMapping]
        public string LimitNoFormat
        {
            get
            {
                if (LimitNo == 0)
                {
                    return "不限制";
                }
                else
                {
                    return "限制" + LimitNo + "人";
                }
            }
        }
        /// <summary>
        /// 预计时长
        ///用户界面显示时长，去掉结束时间；结束时间由开始时间+时长算出
        /// </summary>
        [ORFieldMapping("Hours")]
        public int Hours { get; set; }
        /// <summary>
        /// 票价
        /// </summary>
        [ORFieldMapping("Price")]
        public float Price { get; set; }

        [NoMapping]
        public string PriceFormat
        {
            get
            {
                return Price != 0 ? "￥" + Price.ToString("0.00") : "免费";
            }
        }
        /// <summary>
        /// 活动说明
        /// </summary>
        [ORFieldMapping("Description")]
        public string Description { get; set; }

        /// <summary>
        /// 报名人数
        /// </summary>
        [ORFieldMapping("EnrollNo")]
        public int EnrollNo { get; set; }

        /// <summary>
        /// 排序号
        /// </summary>
        [ORFieldMapping("SortNo")]
        public int SortNo { get; set; }

        /// <summary>
        /// 对应用户是否参加该场次
        /// </summary>
        [NoMapping]
        public bool IsApply { get; set; }

        /// <summary>
        /// 报名是否截止
        /// </summary>
        [NoMapping]
        public bool IsEnrollDeadline
        {
            get
            {
                return DateTime.Now > this.StartTime.AddHours(this.Hours) ? true : false;
            }
        }
        /// <summary>
        /// 是否达到限制人数
        /// </summary>
        [NoMapping]
        public bool IsReachLimit
        {
            get
            {
                if (LimitNo == 0)
                    return false;

                if (EnrollNo >= LimitNo)
                    return true;
                else
                    return false;
            }
        }

        /// <summary>
        /// 活动是否开始
        /// </summary>
        [NoMapping]
        public bool IsStart
        {
            get
            {
                return DateTime.Now >= this.StartTime ? true : false;
            }
        }

        /// <summary>
        /// 活动是否结束
        /// </summary>
        [NoMapping]
        public bool IsEnd
        {
            get
            {
                return DateTime.Now > this.StartTime.AddHours(this.Hours) ? true : false;
            }
        }
        /// <summary>
        /// 用户是否可以报名
        /// </summary>
        [NoMapping]
        public bool IsCanEnroll
        {
            get
            {
                if (IsEnrollDeadline || IsReachLimit || IsEnd)
                    return false;

                return true;
            }
        }

        /// <summary>
        /// 场次状态
        /// </summary>
        [NoMapping]
        public string Status
        {
            get
            {
                if (IsApply) return "已报名";
                if (IsEnrollDeadline) return "已截止";
                if (IsReachLimit) return "已报满";
                if (this.IsEnd) return "已结束";

                return "报名";
            }
        }

    }

    /// <summary>
    /// 活动场次集合
    /// </summary>
    [Serializable]
    public class ActivityEventCollection : EditableDataObjectCollectionBase<ActivityEvent>
    {
    }

    /// <summary>
    /// 活动场次操作类
    /// </summary>
    public class ActivityEventAdapter : UpdatableAndLoadableAdapterBase<ActivityEvent, ActivityEventCollection>
    {
        public static readonly ActivityEventAdapter Instance = new ActivityEventAdapter();

        private ActivityEventAdapter()
        {
        }

        protected override string GetConnectionName()
        {
            return CommonHelper.GetConnectionName();
        }

        public ActivityEvent LoadByCode(string code)
        {
            return this.Load(where =>
            {
                where.AppendItem("Code", code);
            }).FirstOrDefault();
        }

        public ActivityEventCollection LoadByProjectCode(string projectCode)
        {
            return this.Load(where =>
            {
                where.AppendItem("ProjectCode", projectCode);
            });
        }

        /// <summary>
        /// 报名人数+ number
        /// </summary>
        /// <param name="activityEventCode">场次编码</param>
        /// <param name="number">人数</param>
        public void SetIncEnrollNo(string activityEventCode, int number)
        {
            this.SetInc("EnrollNo", number, where =>
            {
                where.AppendItem("Code", activityEventCode);
            }, this.GetConnectionName());
        }
        public void DeleteByCode(string code, bool trueDelete = false)
        {
            this.Delete(where => where.AppendItem("Code", code));
        }

        public ActivityEventCollection LoadAll()
        {
            return this.Load(where => where.AppendItem("1", 1));
        }

        public ActivityEventCollection LoadAll(bool isValid)
        {
            return this.Load(where => where.AppendItem("IsValid", isValid));
        }
        /// <summary>
        /// 获得最近的项目
        /// </summary>
        /// <returns></returns>
        /// <param name="minutes">发送间隔</param>
        public ActivityEventCollection LoadNearActivity(int minutes = 10)
        {
            return this.Load(where =>
            {
                where.AppendItem("StartTime", DateTime.Now.AddDays(1).AddMinutes(-minutes), ">=");
                where.AppendItem("StartTime", DateTime.Now.AddDays(1), "<");
            });
        }
    }

    public class ActivityEventComparer : EqualityComparer<ActivityEvent>
    {
        public override bool Equals(ActivityEvent x, ActivityEvent y)
        {
            if (x == null && y == null)
            {
                return false;
            }
            else
            {
                return x.StartTime == y.StartTime;
            }
        }

        public override int GetHashCode(ActivityEvent obj)
        {
            return obj.StartTime.GetHashCode();
        }
    }

}

