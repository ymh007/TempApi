using MCS.Library.Core;
using MCS.Library.Data.Builder;
using MobileBusiness.Common.Data;
using Seagull2.YuanXin.AppApi.ZhongChouData.Common;
using Seagull2.YuanXin.AppApi.ZhongChouData.Enums;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Web;

namespace Seagull2.YuanXin.AppApi.ZhongChouData.ViewData
{
    public class OrderViewDataQuery : ObjectDataSourceQueryAdapterBase<OrderViewData, OrderViewDataCollection>
    {
        #region 基类方法
        protected override void OnBuildQueryCondition(QueryCondition qc)
        {
            //--订单状态（线下活动有订单，线上活动没有订单）
            //--使用平台的支付功能，订单状态也是从平台获取
            //--最新需求：如果是订单状态是支付后的状态显示支付后的状态； 
            //--如果订单状态是未支付，则判断订单金额是否为0，0则显示已支付，大于0则关联平台支付表，如果TradeState = 1则显示已支付，否则显示未支付

            qc.OrderByClause = this.OrderByClause;
            qc.SelectFields = @"o.Code,o.ProjectCode,o.OrderNo,o.PayWay,o.DeliverTime,o.Remarks,o.GoodsCount,o.VoucherPrice,o.Total,o.Creator,o.CreateTime,o.Status,
                                o.IsValid,o.Type,o.TradeNo,o.ScanTime,o.SubProjectCode,o.PayTime,o.UsedTime,o.EvaluationTime,o.CompanyCode,o.ConfirmTime,o.IsDelivery
                    ,p.Name AS ProjectName,p.Type AS ProjectType ,p.Province,p.City,p.Address,p.CoverImg AS ProjectCoverImg,
                    p.StartTime AS ProjectStartTime,p.EndTime AS ProjectEndTime,
                    oLoty.LotteryNo as RealLotteryNo,oLoty.CreateTime SureTime
                    ,oAddr.Receiver,oAddr.Phone ReceiverPhone,
                    al.PlanLotteryTime AS ProjectPlanLotteryTime, al.LotteryResult,oAddr.IDNumber,r.Content RewardContent,al.LotteryTime,alOrderAddress.Phone ActivityLotteryUserPhone,alOrderAddress.Receiver ActivityLotteryUser";
            qc.FromClause = @"zc.[Order] o
                INNER JOIN zc.OrderAddress oAddr ON o.Code = oAddr.OrderCode
                INNER JOIN zc.Project p ON o.ProjectCode = p.Code
                LEFT JOIN zc.Reward r ON r.ProjectCode=p.Code    --回报信息
				LEFT JOIN zc.ActivityLottery al ON al.ProjectCode = p.Code   --活动开奖信息
                LEFT JOIN zc.[Order] alOrder ON al.OrderCode=alOrder.Code    --中奖订单号
                LEFT JOIN zc.OrderAddress alOrderAddress ON alOrder.Code=alOrderAddress.OrderCode   --中奖订单地址
                LEFT JOIN zc.OrderLottery oLoty ON o.Code = oLoty.OrderCode";

            qc.WhereClause = this.ToWhere();
        }

        protected override string GetConnectionName()
        {
            return CommonHelper.GetConnectionName();
        }

        #endregion

        private string _orderByClause = "o.CreateTime DESC";
        public string OrderByClause
        {
            get { return _orderByClause; }
            set { _orderByClause = value; }
        }

        public string ProjectCreator { get; set; }
        /// <summary>
        /// 订单号
        /// </summary>
        public string OrderNo { get; set; }
        /// <summary>
        /// 订单编码
        /// </summary>
        public string OrderCode { get; set; }
        public string UserName { get; set; }
        public string ActivityName { get; set; }
        public string TelPhone { get; set; }
        public string Payway { get; set; }
        public string ProjectCode { get; set; }
        public string OrderCreator { get; set; }
        /// <summary>
        /// 众筹项目类型
        /// </summary>
        public ProjectTypeEnum ProjectType { get; set; }
        public bool IsValid { get; set; }

        /// <summary>
        /// 如果多个状态，示例：1,2,3
        /// </summary>
        public string OrderStatus { get; set; }

        /// <summary>
        /// 下单时间的起始时间
        /// </summary>
        public string StartCreateTime { get; set; }

        /// <summary>
        /// 下单时间的截止时间
        /// </summary>
        public string EndCreateTime { get; set; }

        public string ToWhere()
        {
            var where = new WhereSqlClauseBuilder();

            if (this.OrderNo.IsNotEmpty())
            {
                where.AppendItem("o.OrderNo", this.OrderNo);
            }
            if (this.OrderCode.IsNotEmpty())
            {
                where.AppendItem("o.Code", this.OrderCode);
            }
            if (this.UserName.IsNotEmpty())
            {
                where.AppendItem("oAddr.Receiver", "%" + this.UserName + "%", "Like");
            }
            if (this.ActivityName.IsNotEmpty())
            {
                where.AppendItem("p.Name", "%" + this.ActivityName + "%", "Like");
            }
            if (this.TelPhone.IsNotEmpty())
            {
                where.AppendItem("usr.Phone", this.TelPhone);
            }
            if (this.ProjectCode.IsNotEmpty())
            {
                where.AppendItem("p.Code", this.ProjectCode);
            }
            if (this.OrderCreator.IsNotEmpty())
            {
                where.AppendItem("o.Creator", this.OrderCreator);
            }
            if (this.ProjectCreator.IsNotEmpty())
            {
                where.AppendItem("p.Creator", this.ProjectCreator);
            }
            if (IsValid)
            {
                where.AppendItem("o.IsValid", true);
            }
            if (this.Payway.IsNotEmpty())
            {
                where.AppendItem("o.Payway", this.Payway);
            }
            where.AppendItem("p.Type", Convert.ToInt32(ProjectType));
            if (this.OrderStatus.IsNotEmpty())
            {
                InSqlClauseBuilder inSql = new InSqlClauseBuilder();
                inSql.AppendItem(this.OrderStatus.Split(','));
                where.AppendItem("o.Status", inSql.ToSqlStringWithInOperator(TSqlBuilder.Instance), "", true);
            }
            if (this.StartCreateTime.IsNotEmpty())
            {
                where.AppendItem("o.CreateTime", this.StartCreateTime, ">=");
            }
            if (this.EndCreateTime.IsNotEmpty())
            {
                where.AppendItem("o.CreateTime", DateTime.Parse(this.EndCreateTime).AddDays(1).ToString(), "<=");
            }
            return where.ToSqlString(TSqlBuilder.Instance);
        }


        public DataSet QueryExportData()
        {
            string sqlStr = string.Format(@"  SELECT o.*,
                    p.Name AS ProjectName,p.Province,p.City,p.Address,p.CoverImg AS ProjectCoverImg,
                    p.StartTime AS ProjectStartTime,p.EndTime AS ProjectEndTime,
                    oLoty.LotteryNo as RealLotteryNo,oLoty.CreateTime SureTime,
                    usr.Phone,usr.HeadImage,usr.RealName AS CreatorName,usr.Nickname AS CreatorNickname,
                    oAddr.Receiver,oAddr.Phone ReceiverPhone,
                    al.PlanLotteryTime AS PlanLotteryTime, al.LotteryResult,oAddr.IDNumber,

                   CASE o.PayWay
                     WHEN '1' THEN '支付宝'
                     WHEN '2' THEN '微信'
                     ELSE '其他' END PayWayStr,

                    CASE o.Status 
					 WHEN '0' THEN '待支付'
					 WHEN '1' THEN '已支付'
					 WHEN '2' THEN '已退款'
					 WHEN '10' THEN '待分享'
				     WHEN '11' THEN '待现场确认'
					WHEN '12' THEN '已确认'
					WHEN '13' THEN '已完成'
					WHEN '20' THEN '未报名'
					WHEN '21' THEN '已报名'
					WHEN '22' THEN '已签到'
						  ELSE '其他' END StatusStr

					FROM zc.[Order] o
                INNER JOIN zc.OrderAddress oAddr ON o.Code = oAddr.OrderCode
                INNER JOIN zc.Project p ON o.ProjectCode = p.Code
				LEFT JOIN zc.ActivityLottery al ON al.ProjectCode = p.Code
                INNER JOIN zc.UserInfo usr ON usr.Code = o.Creator
                LEFT JOIN zc.OrderLottery oLoty ON o.Code = oLoty.OrderCode
                WHERE {0} ORDER BY o.CreateTime DESC", this.ToWhere());
            DataSet ds = DbHelper.RunSqlReturnDS(sqlStr, this.GetConnectionName());
            return ds;
        }
    }
}
