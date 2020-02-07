using log4net;
using MCS.Library.SOA.DataObjects;
using Seagull2.YuanXin.AppApi.Models;
using Seagull2.YuanXin.AppApi.Models.ActivityNew;
using System;
using System.Reflection;
using System.Data.SqlClient;
using System.Data;

namespace Seagull2.YuanXin.AppApi.Adapter.ActivityNew
{

    /// <summary>
    /// 报名人员 Adapter
    /// </summary>
    public class ActivityApplyInfoAdapter : UpdatableAndLoadableAdapterBase<ActivityApplyInfoModel, ActivityApplyInfoCollection>
    {
        /// <summary>
        /// 数据库连接字符串
        /// </summary>
        protected override string GetConnectionName()
        {
            return ConnectionNameDefine.YuanXinBusiness;
        }
        /// <summary>
        /// 日志实例化
        /// </summary>
        ILog log = LogManager.GetLogger(MethodBase.GetCurrentMethod().DeclaringType);

        /// <summary>
        /// 实例化
        /// </summary>
        public static readonly ActivityApplyInfoAdapter Instance = new ActivityApplyInfoAdapter();

        /// <summary>
        /// 获取活动报名人数
        /// </summary>
        public int GetApplyCountByActivityCode(string activityCode)
        {
            var sql = "SELECT COUNT(0) FROM [office].[ActivityApplyInfo] WHERE [ActivityCode] = @ActivityCode";
            SqlParameter[] parameters = { new SqlParameter("@ActivityCode", SqlDbType.NVarChar, 36) };
            parameters[0].Value = activityCode;
            var result = new SqlDbHelper().ExecuteScalar(sql, CommandType.Text, parameters);
            return Convert.ToInt32(result);
        }
    }
}