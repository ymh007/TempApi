using MCS.Library.Data.Builder;
using MCS.Library.Data.DataObjects;
using MCS.Library.Data.Mapping;
using MobileBusiness.Common.Data;
using Seagull2.YuanXin.AppApi.ZhongChouData.Common;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Seagull2.YuanXin.AppApi.ZhongChouData.Models
{
    /// <summary>
    /// 用户消息
    /// </summary>
    [ORTableMapping("zc.UserMessage")]
    public class UserMessage
    {
        /// <summary>
        /// 编码
        /// </summary>
        [ORFieldMapping("Code", PrimaryKey = true)]
        public string Code { get; set; }
        /// <summary>
        /// 状态
        /// </summary>
        [ORFieldMapping("Status")]
        public UserMessageStatus Status { get; set; }
        /// <summary>
        /// 用户编码
        /// </summary>
        [ORFieldMapping("UserInfoCode")]
        public string UserInfoCode { get; set; }
        /// <summary>
        /// 消息编码
        /// </summary>
        [ORFieldMapping("MessageCode")]
        public string MessageCode { get; set; }
    }
    /// <summary>
    /// 用户信息集合
    /// </summary>
    public class UserMessageCollection : EditableDataObjectCollectionBase<UserMessage>
    {
    }

    /// <summary>
    /// 用户信息操作类
    /// </summary>
    public class UserMessageAdapter : UpdatableAndLoadableAdapterBase<UserMessage, UserMessageCollection>
    {
        public static readonly UserMessageAdapter Instance = new UserMessageAdapter();

        public UserMessage LoadByUserAndMessageCode(string userCode, string messCode)
        {
            return this.Load(p =>
            {
                p.AppendItem("UserInfoCode", userCode);
                p.AppendItem("MessageCode", messCode);
            }).FirstOrDefault();
        }

        private UserMessageAdapter()
        {
        }

        protected override string GetConnectionName()
        {
            return CommonHelper.GetConnectionName();
        }

        public UserMessage LoadByCode(string code)
        {
            return this.Load(p =>
            {
                p.AppendItem("Code", code);
            }).FirstOrDefault();
        }

        public UserMessageCollection LoadByMessCode(string code)
        {
            return this.Load(p =>
            {
                p.AppendItem("MessageCode", code);
            });
        }

        /// <summary>
        /// 更新消息状态
        /// </summary>
        /// <param name="userCode">用户编码</param>
        /// <param name="msgCodes">消息组</param>
        /// <param name="statusWhere">状态（作为查询条件）</param>
        /// <param name="statusSet">状态（作为更新值）</param>
        public void UpdateStatus(string userCode, string[] msgCodes, UserMessageStatus statusWhere, UserMessageStatus statusSet)
        {
            if (msgCodes.Length == 0)
            {
                return;
            }
            else
            {
                InSqlClauseBuilder inSql = new InSqlClauseBuilder();
                inSql.AppendItem(msgCodes);

                this.SetFields(
                        update =>
                        {
                            update.AppendItem("Status", statusSet.ToString("D"));
                        },
                        where =>
                        {
                            where.AppendItem("UserInfoCode", userCode);
                            where.AppendItem("Status", statusWhere.ToString("D"));

                            where.AppendItem("MessageCode", inSql.ToSqlStringWithInOperator(TSqlBuilder.Instance), "", true);

                        },
                        this.GetConnectionName());
            }

        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="userCode"></param>
        /// <param name="status"></param>
        /// <returns></returns>
        public UserMessageCollection LoadByUserCodeAndStatus(string userCode, UserMessageStatus status)
        {

            return this.Load(where =>
            {
                where.AppendItem("UserInfoCode", userCode);
                where.AppendItem("Status", status.ToString("D"));
            });
        }

        public bool Exists(string[] msgCodes, UserMessageStatus status)
        {
            if (msgCodes.Length == 0)
            {
                return false;
            }
            else
            {
                InSqlClauseBuilder inSql = new InSqlClauseBuilder();
                inSql.AppendItem(msgCodes);

                return this.Exists(where =>
                {
                    where.AppendItem("Status", status.ToString("D"));
                    where.AppendItem("MessageCode", inSql.ToSqlStringWithInOperator(TSqlBuilder.Instance), "", true);
                });
            }

        }
    }
}
