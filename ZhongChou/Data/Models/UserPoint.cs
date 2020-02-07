using System;
using System.Linq;
using System.Web;
using System.Collections.Generic;
using MCS.Library.Core;
using MCS.Library.Data;
using MCS.Library.Data.Mapping;
using MCS.Library.Data.DataObjects;
using MobileBusiness.Common.Data;
using System.Transactions;
using Seagull2.YuanXin.AppApi.ZhongChouData.Enums;
using Seagull2.YuanXin.AppApi.ZhongChouData.Common;

namespace Seagull2.YuanXin.AppApi.ZhongChouData.Models
{
    /// <summary>
    /// 用户积分
    /// </summary>
    [ORTableMapping("zc.UserPoint")]
    public class UserPoint
    {
        /// <summary>
        /// 编码
        /// </summary>
        [ORFieldMapping("Code", PrimaryKey = true)]
        public string Code { get; set; }

        /// <summary>
        /// 积分来源
        /// </summary>
        [ORFieldMapping("Source")]
        [SqlBehavior(EnumUsage = EnumUsageTypes.UseEnumValue)]
        public SourceType Source { get; set; }

        [NoMapping]
        public string SourceName { get { return EnumItemDescriptionAttribute.GetDescription(Source); } }
        /// <summary>
        /// 积分变化
        /// </summary>
        [ORFieldMapping("ChangeNo")]
        public int ChangeNo { get; set; }

        /// <summary>
        /// 备注
        /// </summary>
        [ORFieldMapping("Remarks")]
        public string Remarks { get; set; }

        /// <summary>
        /// 创建人
        /// </summary>
        [ORFieldMapping("Creator")]
        public string Creator { get; set; }

        /// <summary>
        /// 创建时间
        /// </summary>
        [ORFieldMapping("CreateTime")]
        public DateTime CreateTime { get; set; }

        /// <summary>
        /// 来源编码
        /// </summary>
        [ORFieldMapping("ResourceID")]
        public string ResourceID { get; set; }

    }

    /// <summary>
    /// 用户积分集合
    /// </summary>
    public class UserPointCollection : EditableDataObjectCollectionBase<UserPoint>
    {
    }

    /// <summary>
    /// 用户积分操作类
    /// </summary>
    public class UserPointAdapter : UpdatableAndLoadableAdapterBase<UserPoint, UserPointCollection>
    {
        public static readonly UserPointAdapter Instance = new UserPointAdapter();

        private UserPointAdapter() { }

        protected override string GetConnectionName()
        {
            return CommonHelper.GetConnectionName();
        }

        public UserPoint LoadByCode(string code)
        {
            return this.Load(p =>
            {
                p.AppendItem("Code", code);
            }).FirstOrDefault();
        }
        public UserPointCollection LoadByUserCodeAndSourceType(string userCode, SourceType source)
        {
            return this.Load(p =>
            {
                p.AppendItem("Creator", userCode);
                p.AppendItem("Source", source.GetHashCode());
            });
        }
        public UserPointCollection LoadByUserCode(string userCode)
        {
            return this.Load(p =>
            {
                p.AppendItem("Creator", userCode);
            });
        }

        public bool IsCompleteRepeatTaskByResourceID(string userCode,string resourceID,SourceType type)
        {
            return this.Load(o => o
               .AppendItem("Creator", userCode)
               .AppendItem("Source", type.GetHashCode())
               .AppendItem("ResourceID", resourceID)
               ).Count > 0;
        }

        public bool IsCompleteTodayTaks(string userCode, SourceType type)
        {
            return this.Load(o => o
                .AppendItem("Creator", userCode)
                .AppendItem("Source", type.GetHashCode())
                .AppendItem("Year(CreateTime)", DateTime.Now.Year.ToString(), "=", true)
                .AppendItem("Month(CreateTime)", DateTime.Now.Month.ToString(), "=", true)
                .AppendItem("Day(CreateTime)", DateTime.Now.Day.ToString(), "=", true)
                ).Count > 0;
        }

        public bool IsCompleteNewUserTaks(string userCode, SourceType type)
        {
            return this.Load(o => o
                .AppendItem("Creator", userCode)
                .AppendItem("Source", type.GetHashCode())
                ).Count > 0;
        }

        /// <summary>
        /// 根据code删除内容
        /// </summary>
        /// <param name="Code"></param>
        public void DeleteByCode(string code)
        {
            this.Delete(p =>
                {
                    p.AppendItem("Code", code);
                });
        }
        /// <summary>
        /// 设置积分
        /// </summary>
        /// <param name="usercode">用户编号</param>
        /// <param name="Source">来源</param>
        /// <param name="ChangeNo">变化值</param>
        /// <param name="Remarks">备注</param>
        /// <param name="validStatus"></param>
        /// <returns></returns>
        public UserPoint SetPoint(string usercode, SourceType Source, string Remarks, string validStatus = "")
        {
            return SetPoint(usercode, Source, Remarks, "", "");
        }
        public UserPoint SetPoint(string usercode, SourceType Source, string Remarks, string ResourceID, string validStatus = "")
        {
            string ChangeNo = "";
            var settings = SysSettingHelper.GetGroup(CommonHelper.GetSysSettingPath(), "Point");
            switch (Source)
            {
                //case SourceType.Sign: ChangeNo = UserPointValue(usercode, Source, settings["Sign"].Value); break;
                case SourceType.Sign: ChangeNo = settings["Sign"].Value; break;
                case SourceType.Share: ChangeNo = settings["Share"].Value; break;
                case SourceType.Discuss: ChangeNo = settings["Discuss"].Value; break;
                case SourceType.Perfect: ChangeNo = settings["Perfect"].Value; break;
                case SourceType.AvtivityIn: ChangeNo = settings["AvtivityIn"].Value; break;
                case SourceType.AssessAvtivity: ChangeNo = settings["AssessAvtivity"].Value; break;
                case SourceType.CollectIn: ChangeNo = settings["CollectIn"].Value; break;
                case SourceType.Gravatar: ChangeNo = settings["Gravatar"].Value; break;
                case SourceType.Nickname: ChangeNo = settings["Nickname"].Value; break;
                case SourceType.Praise: ChangeNo = settings["Praise"].Value; break;
                case SourceType.FollowCrowdfund: ChangeNo = settings["FollowCrowdfund"].Value; break;
            }
            UserPoint p = new UserPoint()
            {
                Code = Guid.NewGuid().ToString(),
                Source = Source,
                ChangeNo = int.Parse(ChangeNo),
                Remarks = Remarks,
                Creator = usercode,
                CreateTime = DateTime.Now,
                ResourceID = ResourceID
            };
            UserInfo u = UserInfoAdapter.Instance.LoadByCode(usercode);
            if (u != null)
            {
                u.PointTotal += p.ChangeNo;
                using (TransactionScope scope = TransactionScopeFactory.Create())
                {
                    UserInfoAdapter.Instance.Update(u);
                    UserPointAdapter.Instance.Update(p);
                    scope.Complete();
                }
            }
            return p;
        }

        /// <summary>
        /// 签到积分计算
        /// </summary>
        /// <param name="usercode">用户</param>
        /// <param name="Source">积分来源类型</param>
        /// <param name="signValue">积分累加值</param>
        /// <returns></returns>
        private string UserPointValue(string usercode, SourceType Source, string signValue)
        {
            string changeNo = signValue;

            var userPointList = UserPointAdapter.Instance.LoadByUserCodeAndSourceType(usercode, Source);

            var dateNow = DateTime.Now.Date;

            var point = userPointList.Where(w => w.CreateTime.Date == dateNow.AddDays(-1));

            if (point != null && point.Count() > 0)
            {
                var c = point.FirstOrDefault().ChangeNo;

                if (point.FirstOrDefault().ChangeNo < 7)
                {
                    changeNo = (c + int.Parse(signValue)).ToString();
                }
                else
                {
                    changeNo = point.FirstOrDefault().ChangeNo.ToString();
                }
            }

            return changeNo;
        }
        public UserPointCollection LoadList(string validStatus = "")
        {
            return this.Load(p =>
            {
                if (validStatus.IsNotEmpty())
                {
                    p.AppendItem("ValidStatus", validStatus);
                }
            });
        }

        public UserPointCollection LoadByTime(DateTime startTime, DateTime enTime)
        {
            return this.Load(p =>
            {
                p.AppendItem("CreateTime", startTime, ">=");
                p.AppendItem("CreateTime", enTime, "<");
            });
        }
    }
}
