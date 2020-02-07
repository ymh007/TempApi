using MCS.Library.Data.DataObjects;
using MCS.Library.Data.Mapping;
using MobileBusiness.Common.Data;
using Seagull2.YuanXin.AppApi.ZhongChouData.Common;
using Seagull2.YuanXin.AppApi.ZhongChouData.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Seagull2.YuanXin.AppApi.ZhongChouData.Models
{
    public class CaseOrder : Order
    {
        [NoMapping]
        public Project Project
        {
            get
            {
                if (this.Loaded)
                {
                    return ProjectAdapter.Instance.LoadByCode(this.ProjectCode);
                }
                return null;
            }
        }

        /// <summary>
        /// 订单地址
        /// </summary>
        [NoMapping]
        public OrderAddress OrderAddress
        {
            get
            {
                if (this.Loaded)
                {
                    return OrderAddressAdapter.Instance.LoadByOrderCode(this.Code);
                }
                return null;
            }
        }
        /// <summary>
        /// 活动场次
        /// </summary>
        [NoMapping]
        public ActivityEvent ActivityEvent
        {
            get
            {
                if (this.Loaded)
                {
                    return ActivityEventAdapter.Instance.LoadByCode(this.SubProjectCode);
                }
                return null;
            }
        }
    }

    public class CaseOrderCollection : EditableDataObjectCollectionBase<CaseOrder>
    {

    }

    public class CaseOrderAdapter : UpdatableAndLoadableAdapterBase<CaseOrder, CaseOrderCollection>
    {
        public static readonly CaseOrderAdapter Instance = new CaseOrderAdapter();

        private CaseOrderAdapter()
        {
        }

        protected override string GetConnectionName()
        {
            return CommonHelper.GetConnectionName();
        }


        public CaseOrder LoadByCode(string code)
        {
            return this.Load(where =>
            {
                where.AppendItem("Code", code);
                where.AppendItem("IsValid", true);
            }).FirstOrDefault();
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

        public bool ExistsByUserProject(string userCode, string projectCode, OrderStatus status)
        {
            return this.Exists(where =>
            {
                where.AppendItem("Creator", userCode);
                where.AppendItem("ProjectCode", projectCode);
                where.AppendItem("Status", status.GetHashCode());
                where.AppendItem("IsValid", true);
            });
        }

        public CaseOrder LoadByUserProject(string userCode, string projectCode)
        {
            return this.Load(where =>
            {
                where.AppendItem("Creator", userCode);
                where.AppendItem("ProjectCode", projectCode);
                where.AppendItem("IsValid", true);
            }).FirstOrDefault();
        }

        public CaseOrder ValidOrder(string userCode, string projectCode)
        {
            return this.Load(where =>
            {
                where.AppendItem("Creator", userCode);
                where.AppendItem("ProjectCode", projectCode);
            }).FirstOrDefault();
        }
    }

}
