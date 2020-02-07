using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Configuration;
using Newtonsoft.Json;
using MCS.Library.Core;
using Seagull2.YuanXin.AppApi.ZhongChouData.Models;
using Seagull2.YuanXin.AppApi.ZhongChouData.Common;
using Seagull2.YuanXin.AppApi.ZhongChouData.Enums;

namespace Seagull2.YuanXin.AppApi.ZhongChouApi.Common
{
    public static class ProcessStepHelper
    {
        private static string getConfigPath()
        {
            return "~/Config/SysSetting.xml";
        }
        public static ProcessStep GetProcessStep(string resourceID, string groupID, string creator, string key, ProcessType type)
        {
            var groupConfig = SysSettingHelper.GetGroup(ProcessStepHelper.getConfigPath(), groupID);

            var result = ProcessStepHelper.CreateProcessStep(
                type,
                resourceID,
                creator,
                groupConfig[key].Value,
                groupConfig[key].Desc);

            return result;
        }

        public static ProcessStepText GetProcessStepText(string groupID, string orderCode, OrderStatus status, bool isBussiness)
        {
            var result = new ProcessStepText();

            if (status == OrderStatus.Topic_Cancled)
            {
                var reason = FailedReasonAdapter.Instance.LoadByOrderCode(orderCode);
                result.ProcessStepName = "用户取消预约";
                result.ProcessStepDescription = reason.Cause;
            }
            else if (status == OrderStatus.Topic_Refused)
            {
                var reason = FailedReasonAdapter.Instance.LoadByOrderCode(orderCode);
                result.ProcessStepName = "生活家拒绝预约";
                result.ProcessStepDescription = reason.Cause;
            }
            else if (status == OrderStatus.Coupon_Refunding)
            {
                var reason = FailedReasonAdapter.Instance.LoadByOrderCode(orderCode);
                result.ProcessStepName = "买家发起退款";

                if (reason.ReasonTypeCode == ReasonType.ApplyRefund_NoDeliver)
                {
                    result.ProcessStepDescription = reason.Cause + "|2天内未处理，系统默认退款";
                }
                else if (reason.ReasonTypeCode == ReasonType.ApplyRefund_Delivered)
                {
                    result.ProcessStepDescription = reason.Cause + "|7天内未处理，系统默认退款";
                }


            }
            else if (status == OrderStatus.Coupon_Refunded)
            {
                var reason = FailedReasonAdapter.Instance.LoadByOrderCode(orderCode);
                result.ProcessStepName = "已退款";
                result.ProcessStepDescription = reason.Cause;
            }
            else
            {
                string key = isBussiness ? status.ToString() + "_Business" : status.ToString() + "_User";
                var groupConfig = SysSettingHelper.GetGroup(ProcessStepHelper.getConfigPath(), groupID);
                result.ProcessStepName = groupConfig[key].Value;
                result.ProcessStepDescription = groupConfig[key].Desc;
            }


            return result;
        }


      

        private static ProcessStep CreateProcessStep(ProcessType processType, string resourceID, string creator, string name, string remarks)
        {
            var result = new ProcessStep
            {
                Code = UuidHelper.NewUuidString(),
                ResourceID = resourceID,
                Name = name,
                Remarks = remarks,
                ProcessType = processType,
                Creator = creator,
                CreateTime = DateTime.Now,
            };

            return result;
        }
    }

    /// <summary>
    /// 订单状态流程步骤
    /// </summary>
    public class ProcessStepText
    {
        /// <summary>
        /// 状态
        /// </summary>
        public string ProcessStepName { get; set; }

        /// <summary>
        /// 说明
        /// </summary>
        public string ProcessStepDescription { get; set; }

    }
}