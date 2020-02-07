using MCS.Library.Core;
using MCS.Library.Data.DataObjects;
using MCS.Library.Data.Mapping;
using MobileBusiness.Common.Data;
using Seagull2.YuanXin.AppApi.Adapter.AddressBook;
using Seagull2.YuanXin.AppApi.Models.AddressBook;
using Seagull2.YuanXin.AppApi.ZhongChouData.Enums;
using Seagull2.YuanXin.AppApi.ZhongChouData.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Seagull2.YuanXin.AppApi.ZhongChouData.ViewData
{
    /// <summary>
    /// 特价房订单详情
    /// </summary>
    public class OrderViewData : Order
    {
        /// <summary>
        /// 行号
        /// </summary>
        public string RowNumberForSplit { get; set; }


        /// <summary>
        /// 项目名称
        /// </summary>
        public string ProjectName { get; set; }

        /// <summary>
        /// 项目类型
        /// </summary>
        public ProjectTypeEnum ProjectType { get; set; }

        /// <summary>
        /// 省
        /// </summary>
        public string Province { get; set; }

        /// <summary>
        /// 市
        /// </summary>
        public string City { get; set; }

        /// <summary>
        /// 详细地址
        /// </summary>
        public string Address { get; set; }

        /// <summary>
        /// 接收人
        /// </summary>
        public string Receiver { get; set; }

        /// <summary>
        /// 接收人手机号
        /// </summary>
        public string ReceiverPhone { get; set; }

        /// <summary>
        /// 封面图片
        /// </summary>
        public string ProjectCoverImg { get; set; }

        /// <summary>
        /// 项目开始时间
        /// </summary>
        public DateTime ProjectStartTime { get; set; }

        /// <summary>
        /// 项目结束时间
        /// </summary>
        public DateTime ProjectEndTime { get; set; }


        public string ProjectStartTimeFormat { get { return this.ProjectStartTime.ToString("MM/dd HH:mm"); } }


        public string ProjectEndTimeFormat { get { return this.ProjectEndTime.ToString("MM/dd HH:mm"); } }

        /// <summary>
        /// 下单人信息
        /// </summary>
        public string CreatorName { get; set; }

        /// <summary>
        /// 下单时间
        /// </summary>
        public string CreateDate
        {
            get
            {
                return CreateTime.ToString("MM月dd日");
            }
        }

        /// <summary>
        /// 计划开奖时间
        /// </summary>
        public DateTime ProjectPlanLotteryTime { get; set; }

        public string PlanLotteryTime
        {
            get
            {
                return this.ProjectPlanLotteryTime.ToString("yyyy-MM-dd");
            }
        }
        /// <summary>
        /// 下单人信息
        /// </summary>
        public ContactsModel UserInfo
        {
            get
            {
                return ContactsAdapter.Instance.LoadByMail(this.Creator);
            }
        }

        /// <summary>
        /// 确认时间
        /// </summary>
        public DateTime SureTime { get; set; }
        /// <summary>
        /// 订单状态
        /// </summary>
        public string StatusStr
        {
            get
            {
                return EnumItemDescriptionAttribute.GetDescription(Status);
            }
        }

        /// <summary>
        /// 抽奖号码，使用这个字段
        /// </summary>
        public string RealLotteryNo { get; set; }

        /// <summary>
        /// **********暂时作为中奖号码，值等同于LotteryResult
        /// </summary>
        [NoMapping]
        public string LotteryNo
        {
            get { return LotteryResult; }
        }

        [NoMapping]
        /// <summary>
        /// **********暂时作为中奖号码，值等同于LotteryResult
        /// </summary>
        public string LotteryCode
        {
            get
            {
                return this.LotteryResult;
            }
        }

        /// <summary>
        /// 中奖号码
        /// </summary>
        public string LotteryResult { get; set; }

        /// <summary>
        /// 活动场次
        /// </summary>
        [NoMapping]
        public ActivityEvent ActivityEvent
        {
            get
            {
                if (this.SubProjectCode.IsNotEmpty())
                {
                    return ActivityEventAdapter.Instance.LoadByCode(this.SubProjectCode);
                }
                return null;
            }
        }
        #region 扩展属性
        /// <summary>
        /// 回报内容
        /// </summary>
        public string RewardContent { get; set; }
        /// <summary>
        /// 接收人身份证号
        /// </summary>
        public string IDNumber { get; set; }
        /// <summary>
        /// 获奖者
        /// </summary>
        public string ActivityLotteryUser { get; set; }
        /// <summary>
        /// 获奖者手机号
        /// </summary>
        public string ActivityLotteryUserPhone { get; set; }
        /// <summary>
        /// 实际开奖时间（参考）（展示时使用计划开奖时间）
        /// </summary>
        public string LotteryTime { get; set; }
        /// <summary>
        /// 众筹是否结束
        /// </summary>
        public bool IsEnd
        {
            get
            {
                var a = DateTime.Now > this.ProjectEndTime ? true : false;
                if (a == true && this.Status == OrderStatus.Tejiafang_None) this.Status = OrderStatus.Tejiafang_Close;
                return a;
            }
        }
        #endregion


    }

    public class OrderViewDataCollection : EditableDataObjectCollectionBase<OrderViewData>
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
