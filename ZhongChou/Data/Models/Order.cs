using System;
using System.Linq;
using System.Web;
using System.Collections.Generic;
using MCS.Library.Core;
using MCS.Library.Data;
using MCS.Library.Data.Mapping;
using MCS.Library.Data.DataObjects;
using MobileBusiness.Common.Data;
using System.Configuration;
using MCS.Library.Data.Builder;
using Seagull2.YuanXin.AppApi.ZhongChouData.Enums;
using Seagull2.YuanXin.AppApi.ZhongChouData.Common;
using MCS.Library.OGUPermission;
using Seagull2.YuanXin.AppApi.Adapter.AddressBook;
using Seagull2.YuanXin.AppApi.Models.AddressBook;
using Seagull2.YuanXin.AppApi.ZhongChou.Data.Models;

namespace Seagull2.YuanXin.AppApi.ZhongChouData.Models
{
    /// <summary>
    /// 订单
    /// </summary>
    [Serializable]
    [ORTableMapping("zc.[Order]")]
    public class Order : ILoadableDataEntity
    {
        /// <summary>
        /// 编码
        /// </summary>
        [ORFieldMapping("Code", PrimaryKey = true)]
        public string Code { get; set; }

        /// <summary>
        /// 订单类型（数据库未添加）
        /// </summary>
        [ORFieldMapping("Type")]
        [SqlBehavior(EnumUsage = EnumUsageTypes.UseEnumValue)]
        public OrderType Type { get; set; }

        /// <summary>
        /// 众筹项目编码
        /// </summary>
        [ORFieldMapping("ProjectCode")]
        public string ProjectCode { get; set; }

        /// <summary>
        /// 子编码
        /// </summary>
        [ORFieldMapping("SubProjectCode")]
        public string SubProjectCode { get; set; }
        /// <summary>
        /// 订单号
        /// </summary>
        [ORFieldMapping("OrderNo")]
        public string OrderNo { get; set; }

        /// <summary>
        /// 交易号
        /// </summary>
        [ORFieldMapping("TradeNo")]
        public string TradeNo { get; set; }
        /// <summary>
        /// 支付方式
        /// </summary>
        [ORFieldMapping("PayWay")]
        [SqlBehavior(EnumUsage = EnumUsageTypes.UseEnumValue)]
        public PayWayType PayWay { get; set; }

        [NoMapping]
        public string PayWayName
        {
            get
            {
                return EnumItemDescriptionAttribute.GetDescription(PayWay);
            }
        }
        /// <summary>
        /// 送货时间
        /// </summary>
        [ORFieldMapping("DeliverTime")]
        public DateTime DeliverTime { get; set; }
        /// <summary>
        /// 使用时间
        /// </summary>
        [ORFieldMapping("UsedTime")]
        public DateTime UsedTime { get; set; }

        /// <summary>
        /// 备注
        /// </summary>
        [ORFieldMapping("Remarks")]
        public string Remarks { get; set; }

        /// <summary>
        /// 商品总价
        /// </summary>
        [ORFieldMapping("GoodsPrice")]
        public float GoodsPrice { get; set; }

        /// <summary>
        /// 商品份数
        /// </summary>
        [ORFieldMapping("GoodsCount")]
        public int GoodsCount { get; set; }

        /// <summary>
        /// 优惠券价格
        /// </summary>
        [ORFieldMapping("VoucherPrice")]
        public decimal VoucherPrice { get; set; }

        /// <summary>
        /// 订单金额
        /// </summary>
        [ORFieldMapping("Total")]
        public float Total { get; set; }

        /// <summary>
        /// 下单人编码
        /// </summary>
        [ORFieldMapping("Creator")]
        public string Creator { get; set; }

        /// <summary>
        /// 下单时间
        /// </summary>
        [ORFieldMapping("CreateTime")]
        public DateTime CreateTime { get; set; }

        /// <summary>
        /// 下单时间
        /// </summary>
        [NoMapping]
        public String CreateTimeFormat
        {
            get
            {
                return CreateTime.ToString("yyyy-MM-dd HH:mm:ss");
            }
        }

        /// <summary>
        /// 扫码时间
        /// </summary>
        [ORFieldMapping("ScanTime")]
        public DateTime ScanTime { get; set; }

        /// <summary>
        /// 评论时间
        /// </summary>
        [ORFieldMapping("EvaluationTime")]
        public DateTime EvaluationTime { get; set; }
        /// <summary>
        /// 订单状态
        /// </summary>
        [ORFieldMapping("Status")]
        [SqlBehavior(EnumUsage = EnumUsageTypes.UseEnumValue)]
        public Enums.OrderStatus Status { get; set; }


        /// <summary>
        /// 是否快递送货
        /// </summary>
        [ORFieldMapping("IsDelivery")]
        public bool IsDelivery { get; set; }

        /// <summary>
        /// 有效性
        /// </summary>
        [ORFieldMapping("IsValid")]
        public bool IsValid { get; set; }
        /// <summary>
        /// 商家编码
        /// </summary>
        [ORFieldMapping("CompanyCode")]
        public string CompanyCode { get; set; }
        /// <summary>
        /// 是否从数据库加载
        /// </summary>
        [NoMapping]
        public bool Loaded { get; set; }

        /// <summary>
        /// 用户信息
        /// </summary>
        [NoMapping]
        public ContactsModel UserInfo
        {
            get
            {
                ContactsModel model = ContactsAdapter.Instance.LoadByCode(this.Creator);

                return model;
            }
        }

        /// <summary>
        /// 订单状态
        /// </summary>
        [NoMapping]
        public string StatusName
        {
            get
            {
                //return EnumItemDescriptionAttribute.GetDescription(Status);
                var projectState = EnumItemDescriptionAttribute.GetDescriptionList(typeof(OrderStatus)).Where(w => w.Category.IndexOf(this.Type.ToString("D")) >= 0 && w.EnumValue == (int)this.Status).FirstOrDefault();
                return projectState.Description;
            }
        }

        private DateTime _payTime = DateTime.MinValue;
        /// <summary>
        /// 支付时间
        /// </summary>
        [ORFieldMapping("PayTime")]
        public DateTime PayTime
        {
            get { return _payTime; }
            set { _payTime = value; }
        }
        [NoMapping]
        public string PayTimeFormat
        {
            get
            {
                if (PayTime != DateTime.MinValue)
                {
                    return PayTime.ToString();
                }
                else
                {
                    return "暂无";
                }
            }
        }

    }

    /// <summary>
    /// 订单集合
    /// </summary>
    [Serializable]
    public class OrderCollection : EditableDataObjectCollectionBase<Order>
    {
    }

    /// <summary>
    /// 订单操作类
    /// </summary>
    public class OrderAdapter : UpdatableAndLoadableAdapterBase<Order, OrderCollection>
    {
        public static readonly OrderAdapter Instance = new OrderAdapter();

        private OrderAdapter()
        {
        }

        protected override string GetConnectionName()
        {
            return CommonHelper.GetConnectionName();
        }

        public Order LoadByCode(string code)
        {
            return this.Load(where =>
            {
                where.AppendItem("Code", code);
            }).FirstOrDefault();
        }

        public Order LoadByCodeUpdateStatus(string code)
        {
            Order order = this.Load(where =>
             {
                 where.AppendItem("Code", code);
             }).FirstOrDefault();

            TransactionInfo model = TransactionInfoAdapter.Instance.GetByObjectID(order.Code);
            var oldStatus = order.Status;
            if (oldStatus == OrderStatus.Anchang_NotEnroll)
            {
                if (order.Total == 0)
                {
                    order.Status = OrderStatus.Anchang_Enrolled;
                }
                else
                {

                    if (model != null && model.TradeState == 1)
                    {
                        order.Status = OrderStatus.Anchang_Enrolled;
                    }
                }
            }
            if (model != null && order.Status != OrderStatus.Anchang_NotEnroll)
            {
                order.PayWay = (PayWayType)model.TradeType;
            }
            return order;
        }


        public Order LoadByProjectCode(string projectCode)
        {
            return this.Load(where =>
            {
                where.AppendItem("ProjectCode", projectCode);
            }).FirstOrDefault();
        }
        public Order LoadByOrderNo(string orderNo)
        {
            return this.Load(where =>
            {
                where.AppendItem("OrderNo", orderNo);
                where.AppendItem("IsValid", true);
            }).FirstOrDefault();
        }

        public OrderCollection LoadByStatus(OrderStatus status)
        {
            return this.Load(where =>
            {
                where.AppendItem("Status", status);
                where.AppendItem("IsValid", true);
            });
        }
        public Order LoadByTradeNo(string TradeNo)
        {
            return this.Load(where =>
            {
                where.AppendItem("TradeNo", TradeNo);
                where.AppendItem("IsValid", true);
            }).FirstOrDefault();
        }
        public void DeleteByCode(string code, bool trueDelete = false)
        {
            //逻辑删除
            this.SetFields("IsValid", false, where => where.AppendItem("Code", code));

            //物理删除
            if (trueDelete)
            {
                this.Delete(where => where.AppendItem("Code", code));
            }
        }
        public void DeleteByProjectCode(string projectCode, bool trueDelete = false)
        {
            //逻辑删除
            this.SetFields("IsValid", false, where => where.AppendItem("ProjectCode", projectCode));

            //物理删除
            if (trueDelete)
            {
                this.Delete(where => where.AppendItem("ProjectCode", projectCode));
            }
        }
        public void Delete(string userCode, string projectCode)
        {

            this.Delete(where =>
            {
                where.AppendItem("Creator", userCode);
                where.AppendItem("ProjectCode", projectCode);
            });

        }

        public OrderCollection LoadAll()
        {
            return this.Load(where => where.AppendItem("1", 1));
        }

        public OrderCollection LoadAll(bool isValid)
        {
            return this.Load(where => where.AppendItem("IsValid", isValid));
        }

        public bool ExistsByProjectCode(string projectCode)
        {
            return this.Exists(where =>
            {
                where.AppendItem("ProjectCode", projectCode);
            });
        }


        public bool ExistsByUserProject(string userCode, string projectCode)
        {
            return this.Exists(where =>
            {
                where.AppendItem("Creator", userCode);
                where.AppendItem("ProjectCode", projectCode);
                where.AppendItem("IsValid", true);
            });
        }

        public Order LoadByUserProject(string userCode, string projectCode)
        {
            return this.Load(where =>
            {
                where.AppendItem("Creator", userCode);
                where.AppendItem("ProjectCode", projectCode);
                where.AppendItem("IsValid", true);
            }).FirstOrDefault();
        }

        public OrderCollection LoadCaseOrderByUserProject(string userCode, string projectCode)
        {
            return this.Load(where =>
            {
                where.AppendItem("Creator", userCode);
                where.AppendItem("ProjectCode", projectCode);
                where.AppendItem("IsValid", true);
            });
        }
        public OrderCollection LoadByUserCode(string userCode)
        {
            return this.Load(where =>
            {
                where.AppendItem("Creator", userCode);
                where.AppendItem("IsValid", true);
            });
        }
        /// <summary>
        /// 获得场次订单
        /// </summary>
        /// <param name="actColl"></param>
        /// <returns></returns>
        public OrderCollection LoadByActivity(ActivityEventCollection actColl)
        {
            OrderCollection orderColl = new OrderCollection();
            foreach (ActivityEvent activiEnvent in actColl)
            {
                orderColl.Concat(this.Load(where =>
               {
                   where.AppendItem("Type", OrderType.Anchang);
                   where.AppendItem("SubProjectCode", activiEnvent.Code);
                   where.AppendItem("IsValid", true);
               }));
            }
            return orderColl;
        }
        public Order LoadCaseOrderBySubProject(string userCode, string subProjectCode)
        {
            return this.Load(where =>
            {
                where.AppendItem("Creator", userCode);
                where.AppendItem("SubProjectCode", subProjectCode);
                where.AppendItem("IsValid", true);
            }).FirstOrDefault();
        }

        public void UpdateOrderStatus(string orderCode, OrderStatus orderStatus)
        {
            this.SetFields(
                update =>
                {
                    update.AppendItem("Status", orderStatus.ToString("D"));
                },
                where =>
                {
                    where.AppendItem("Code", orderCode);
                },
                this.GetConnectionName());
        }

        public void UpdateOrderStatus(string[] orderCodes, OrderStatus orderStatus)
        {
            InSqlClauseBuilder inSql = new InSqlClauseBuilder();
            inSql.AppendItem(orderCodes);

            this.SetFields(
                update =>
                {
                    update.AppendItem("Status", orderStatus.ToString("D"));
                },
                where =>
                {
                    where.AppendItem("Code", inSql.ToSqlStringWithInOperator(TSqlBuilder.Instance), "", true);
                },
                this.GetConnectionName());
        }



        /// <summary>
        /// 更新扫码时间
        /// </summary>
        /// <param name="orderCode"></param>
        public void UpdateScanTime(string orderCode)
        {
            this.SetFields(
                update =>
                {
                    update.AppendItem("Status", OrderStatus.Tejiafang_Confirmed.ToString("D"));
                    update.AppendItem("ScanTime", DateTime.Now);
                },
                where =>
                {
                    where.AppendItem("Code", orderCode);
                },
                this.GetConnectionName());
        }

        public void UpdateOrderStatus(string orderCode, OrderStatus orderStatus, PayWayType payWay)
        {
            this.SetFields(
                update =>
                {
                    update.AppendItem("OrderStatus", orderStatus.ToString("D"));
                    update.AppendItem("PayWay", payWay.ToString("D"));
                },
                where =>
                {
                    where.AppendItem("Code", orderCode);
                },
                this.GetConnectionName());
        }

        public string HaveUserOrder(string userCode, string projectCode)
        {
            //project、order 内连接查询，返回OrderCode
            string strSql = @" ";

            return DbHelper.RunSqlReturnScalar(strSql).ToString();
        }


        /// <summary>
        /// 根据开奖结果获取中奖订单号码
        /// </summary>
        /// <param name="projectCode"></param>
        /// <param name="lotteryResult"></param>
        /// <returns></returns>
        public string GetOrderCodeByLotteryResult(string projectCode, int lotteryResult)
        {
            string strSql = string.Format(@"SELECT o.Code
                FROM zc.[Order] o
                    INNER JOIN zc.OrderLottery oLoty ON o.Code = oLoty.OrderCode
                    INNER JOIN zc.Project p ON o.ProjectCode = p.Code
                    WHERE p.Code = '{0}' AND oLoty.LotteryNo ={1} ", projectCode, lotteryResult);

            return DbHelper.RunSqlReturnScalar(strSql, this.GetConnectionName()).ToString();
        }
        /* public string ReBackOrderForAlipay(string detail_data)
         {
             //把请求参数打包成数组
             SortedDictionary<string, string> sParaTemp = new SortedDictionary<string, string>();
             sParaTemp.Add("partner", AlipayConfig.Partner);
             sParaTemp.Add("_input_charset", AlipayConfig.Input_charset.ToLower());
             sParaTemp.Add("service", AlipayConfig.RefundService);
             sParaTemp.Add("notify_url", AlipayConfig.NotifyUrl);
             sParaTemp.Add("seller_email", AlipayConfig.SellerEmail);
             sParaTemp.Add("refund_date", AlipayConfig.RefundDate);
             sParaTemp.Add("batch_no", AlipayConfig.BatchNo);
             sParaTemp.Add("batch_num", AlipayConfig.BatchNum);
             sParaTemp.Add("detail_data", detail_data);

             //建立请求
             string sHtmlText = Submit.BuildRequest(sParaTemp, "get", "确认");
             return sHtmlText;
         }*/
        /// <summary>
        /// 根据订单微信退款（单个退款）
        /// </summary>
        /// <param name="order"></param>
        public string ReBackOrderForWX(Order order)
        {
            if (order.Status == OrderStatus.ZaiShou_Paid)
            {
                string GAccount = ConfigurationManager.AppSettings["GAccount"];
                string res = "";
                if (order.Total == 0)
                {
                    res = "SUCCESS";
                }
                else
                {
                    res = WxRefundAdapter.Instance.Refund(GAccount, order.OrderNo, (int)(order.Total * 100), "Crowdfunding", (int)(order.Total * 100));//参数：公众号账号、订单号、业务类型
                }
                //res = "SUCCESS";
                if (res == "SUCCESS")
                {
                    #region 修改订单状态
                    order.Status = OrderStatus.ZaiShou_Refunded;
                    OrderAdapter.Instance.Update(order);
                    #endregion
                    #region 发送通知

                    //string path = "~/Config/MsgTpl.xml";
                    //var msgModule = MsgTplHelper.GetModuleByID(path, "ReBackOrder");//获取消息模块
                    //CfProject project = CfProjectAdapter.Instance.LoadByCode(order.ProductCode);
                    //string title = string.Format(msgModule.Title.TplValue, project.CnName, EnumItemDescriptionAttribute.GetDescription(project.CurrentStage));
                    //string content = string.Format(msgModule.Content.TplValue, user.CnName,
                    //     project.CnName,
                    //     EnumItemDescriptionAttribute.GetDescription(project.CurrentStage),
                    //     order.OrderAmount,
                    //     order.ActualAmount,
                    //     order.RedEnvelopeAmount);

                    //MsgAdapter.Instance.SendMsg(MsgTypeEnum.SysNotice, title, content, "SYSTEM", order.Creator);
                    #endregion
                }
                else
                {
                    log4net.ILog log = log4net.LogManager.GetLogger(typeof(Order));
                    log.Fatal("fatal", new Exception(res));
                }
                return res;
            }
            else
            {
                return "faile";
            }
        }

        /// <summary>
        /// 获取超时订单
        /// </summary>
        /// <param name="status">订单状态</param>
        /// <param name="min">分钟</param>
        /// <returns></returns>
        public OrderCollection GetOvertimeOrders(OrderStatus status, int min)
        {
            return this.Load(where =>
            {
                where.AppendItem("Status", status);
                where.AppendItem("CreateTime", DateTime.Now.AddMinutes(-min), "<=");
                where.AppendItem("IsValid", true);
            });
        }
    }
}

