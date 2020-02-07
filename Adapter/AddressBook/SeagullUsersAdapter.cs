using MCS.Library.SOA.DataObjects;
using Seagull2.YuanXin.AppApi.Models;
using Seagull2.YuanXin.AppApi.Models.AddressBook;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Seagull2.YuanXin.AppApi.Adapter.AddressBook
{
    public class SeagullUsersAdapter : UpdatableAndLoadableAdapterBase<SeagullUsersModel, SeagullUsersCollection>
    {
        /// <summary>
        /// 实例
        /// </summary>
        public static readonly SeagullUsersAdapter Instance = new SeagullUsersAdapter();

        /// <summary>
        /// 数据库连接
        /// </summary>
        protected override string GetConnectionName()
        {
            return ConnectionNameDefine.YuanXinBusiness;
        }
        /// <summary>
        /// 筛选版本
        /// </summary>
        public SeagullUsersCollection LoadVersion(string version)
        {
            var sql = $@"SELECT UserId,AppVersion,InviteCount,IsValid FROM [YuanXinBusiness].[Business].[SeagullUsers] where leaveTime ='1908-01-01 00:00:00.000' and AppVersion= '" + version+"'";
            return QueryData(sql);
        }

        /// <summary>
        /// 获取所有未激活/已激活用户
        /// </summary>
        public SeagullUsersCollection LoadUsers(bool isValid)
        {
            var sql = $@"SELECT UserId,AppVersion,InviteCount,IsValid FROM [YuanXinBusiness].[Business].[SeagullUsers] where leaveTime ='1908-01-01 00:00:00.000' and IsValid= " + (isValid ? 1 : 0);
            return QueryData(sql);
        }

        /// <summary>
        /// 获取所有未激活/已激活用户
        /// </summary>
        public SeagullUsersCollection LoadUsers()
        {
            var sql = $@"SELECT UserId,AppVersion,InviteCount,IsValid FROM [YuanXinBusiness].[Business].[SeagullUsers] where leaveTime ='1908-01-01 00:00:00.000' ";
            return QueryData(sql);
        }

        /// <summary>
        /// 邀请次数加一
        /// </summary>
        public void UpdateInviteCount(string userCode)
        {
            var sql = string.Format("UPDATE [Business].[SeagullUsers] SET [InviteCount] = ISNULL([InviteCount], 0) + 1 WHERE [UserId] = '{0}';", userCode);

            SqlDbHelper helper = new SqlDbHelper();
            helper.ExecuteNonQuery(sql, System.Data.CommandType.Text);
        }
        /// <summary>
        /// 获取版本
        /// </summary>
        public SeagullUsersCollection GetVersion()
        {
            var sql = $@"SELECT appVersion   FROM [YuanXinBusiness].[Business].[SeagullUsers]  where  appVersion is not null group by  appVersion order by appVersion desc";
            return QueryData(sql);
        }
    }
}