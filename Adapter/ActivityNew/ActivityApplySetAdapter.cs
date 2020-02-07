using log4net;
using MCS.Library.SOA.DataObjects;
using Seagull2.YuanXin.AppApi.Models;
using Seagull2.YuanXin.AppApi.Models.ActivityNew;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Web;
using System.Text;

namespace Seagull2.YuanXin.AppApi.Adapter.ActivityNew
{
    /// <summary>
    /// 活动参加范围设置适配器
    /// </summary>
    public class ActivityApplySetAdapter : UpdatableAndLoadableAdapterBase<ActivityApplySetModel, ActivityApplySetCollection>
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
        public static readonly ActivityApplySetAdapter Instance = new ActivityApplySetAdapter();

        /// <summary>
        /// 批量插入报名设置数据
        /// </summary>
        public void Insert(List<string> userList, string activityCode, string creator)
        {
            if (userList.Count < 1)
            {
                return;
            }
            StringBuilder sql = new StringBuilder();
            var len = userList.Count;
            int math = len / 800;
            for (int j = 0; j <= math; j++)
            {
                sql.Clear();
                sql.Append(@"INSERT INTO [office].[ActivityApplySet] ([Code], [ActivityCode], [UserCode], [Creator], [CreateTime], [ValidStatus]) VALUES ");
                int length = (j + 1) * 800 > len ? len : (j + 1) * 800;
                for (int i = j * 800; i < length; i++)
                {
                    sql.AppendFormat("(N'{0}', N'{1}', N'{2}', N'{3}', '{4}', 1)",
                        Guid.NewGuid().ToString(),
                        activityCode,
                        userList[i],
                        creator,
                        DateTime.Now);
                    if (i < length - 1)
                    {
                        sql.Append(",");
                    }
                }
                string test = sql.ToString();
                DbHelper.RunSql(sql.ToString(), GetConnectionName());
            }
          
        }
    }
}