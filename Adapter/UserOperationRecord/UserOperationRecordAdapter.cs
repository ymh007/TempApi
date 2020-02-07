using MCS.Library.Data;
using MCS.Library.SOA.DataObjects;
using Seagull2.YuanXin.AppApi.Models;
using Seagull2.YuanXin.AppApi.Models.UserOperationRecord;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Seagull2.YuanXin.AppApi.Adapter.UserOperationRecord
{
    /// <summary>
    /// 用户操作记录 Adapter
    /// </summary>
    public class UserOperationRecordAdapter : UpdatableAndLoadableAdapterBase<UserOperationRecordModel, UserOperationRecordCollection>
    {
        /// <summary>
        /// 实例
        /// </summary>
        public static readonly UserOperationRecordAdapter Instance = new UserOperationRecordAdapter();

        /// <summary>
        /// 数据库连接
        /// </summary>
        protected override string GetConnectionName()
        {
            return ConnectionNameDefine.YuanXinBusiness;
        }

        /// <summary>
        /// 数据库连接字符串
        /// </summary>
        protected string ConnectionString = DbConnectionManager.GetConnectionString(ConnectionNameDefine.YuanXinBusiness);

        /// <summary>
        /// 是否存在用户的操作记录
        /// </summary>
        public bool IsExist(string module, string userCode)
        {
            return Exists(
                w =>
                {
                    w.AppendItem("Module", module);
                    w.AppendItem("Creator", userCode);
                }
            );
        }
    }
}