using log4net;
using MCS.Library.SOA.DataObjects;
using Seagull2.YuanXin.AppApi.Models;
using Seagull2.YuanXin.AppApi.Models.ActivityNew;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Web;
using System.Data.SqlClient;
using System.Data;

namespace Seagull2.YuanXin.AppApi.Adapter.ActivityNew
{
    /// <summary>
    /// 活动记录适配器
    /// </summary>
    public class ActivityRecordAdapter : UpdatableAndLoadableAdapterBase<ActivityRecordModel, ActivityRecordCollection>
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
        public static readonly ActivityRecordAdapter Instance = new ActivityRecordAdapter();

        /// <summary>
        /// 获取记录总数
        /// </summary>
        /// <param name="activityCode">活动编码</param>
        /// <param name="type">类型，0：浏览记录；1：关注记录</param>
        public int GetCount(string activityCode, ActivityRecordType type)
        {
            var sql = "SELECT COUNT(0) FROM [office].[ActivityRecord] WHERE [ActivityCode] = @ActivityCode AND [Type] = @Type;";

            SqlParameter[] parameters = { new SqlParameter("@ActivityCode", SqlDbType.NVarChar, 36), new SqlParameter("@Type", SqlDbType.Int, 4) };
            parameters[0].Value = activityCode;
            parameters[1].Value = (int)type;

            var result = new SqlDbHelper().ExecuteScalar(sql, CommandType.Text, parameters);
            return Convert.ToInt32(result);
        }
    }
}