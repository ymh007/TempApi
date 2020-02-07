using System;
using System.Linq;
using MCS.Library.Core;
using MCS.Library.Data.Mapping;
using MCS.Library.Data.DataObjects;
using MobileBusiness.Common.Data;
using MCS.Library.Data.Builder;
using System.Data;
using Seagull2.YuanXin.AppApi.ZhongChouData.Enums;
using Seagull2.YuanXin.AppApi.ZhongChouData.Common;

namespace Seagull2.YuanXin.AppApi.ZhongChouData.Models
{
    /// <summary>
    /// 消息
    /// </summary>
    [Serializable]
    [ORTableMapping("zc.Message")]
    public class Message
    {
        /// <summary>
        /// 编码
        /// </summary>
        [ORFieldMapping("Code", PrimaryKey = true)]
        public string Code { get; set; }

        /// <summary>
        /// 消息类型
        /// </summary>
        [ORFieldMapping("Type")]
        [SqlBehavior(EnumUsage = EnumUsageTypes.UseEnumValue)]
        public Enums.MessageType Type { get; set; }
        /// <summary>
        /// 消息类型名称
        /// </summary>
        [NoMapping]
        public string TypeName
        {
            get
            {
                return EnumItemDescriptionAttribute.GetDescription(Type);
            }
        }
        /// <summary>
        /// 消息标题
        /// </summary>
        [ORFieldMapping("Title")]
        public string Title { get; set; }

        /// <summary>
        /// 消息内容
        /// </summary>
        [ORFieldMapping("Content")]
        public string Content { get; set; }

        /// <summary>
        /// 详情链接
        /// </summary>
        [ORFieldMapping("Link")]
        public string Link { get; set; }

        /// <summary>
        /// 图文详情
        /// </summary>
        [ORFieldMapping("Detail")]
        public string Detail { get; set; }

        /// <summary>
        /// 接收人数量
        /// </summary>
        [ORFieldMapping("ReceiverNumber")]
        public int ReceiverNumber { get; set; }

        /// <summary>
        /// 发送人
        /// </summary>
        [ORFieldMapping("Sender")]
        public string Sender { get; set; }

        /// <summary>
        /// 发送时间
        /// </summary>
        [ORFieldMapping("SendTime")]
        public DateTime SendTime { get; set; }

        /// <summary>
        /// 消息类型(文本、链接、图文)
        /// </summary>
        [ORFieldMapping("MessageMode")]
        public MessageMode MessageMode { get; set; }

        /// <summary>
        /// 有效性
        /// </summary>
        [ORFieldMapping("IsValid")]
        public bool IsValid { get; set; }
        /// <summary>
        /// 是否推送
        /// </summary>
        [ORFieldMapping("IsPush")]
        public bool IsPush { get; set; }

        /// <summary>
        /// 资源编码
        /// </summary>
        [ORFieldMapping("ResourceID")]
        public string ResourceID { get; set; }

        /// <summary>
        /// 消息图片
        /// </summary>
        [ORFieldMapping("MessageImage")]
        public string MessageImage { get; set; }

        [NoMapping]
        public string IsPushStr { get { if (IsPush) { return "消息已推送"; } else { return ""; } } }
        /// <summary>
        /// 是否全部发送
        /// </summary>
        [ORFieldMapping("IsNoLimit")]
        public bool IsNoLimit { get; set; }
        [NoMapping]
        public string IsNoLimitStr {
            get { if (IsNoLimit) { return "(全体发送)"; } else { return ""; } }             
        }

    }

    /// <summary>
    /// 消息集合
    /// </summary>
    [Serializable]
    public class MessageCollection : EditableDataObjectCollectionBase<Message>
    {
        /// <summary>
        /// 转化为ListDataView
        /// </summary>
        /// <param name="pageIndex">当前页码</param>
        /// <param name="pageSize">页大小</param>
        /// <param name="totalCount">总行数</param>
        /// <returns></returns>
        public ListDataView ToListDataView(int pageIndex, int pageSize, int totalCount)
        {
            var result = new ListDataView
            {
                PageIndex = pageIndex,
                PageCount = totalCount % pageSize > 0 ? totalCount / pageSize + 1 : totalCount / pageSize,
                TotalCount = totalCount,
                ListData = this
            };

            return result;
        }
    }

    /// <summary>
    /// 消息操作类
    /// </summary>
    public class MessageAdapter : UpdatableAndLoadableAdapterBase<Message, MessageCollection>
    {
        public static readonly MessageAdapter Instance = new MessageAdapter();

        private MessageAdapter()
        {
        }

        protected override string GetConnectionName()
        {
            return CommonHelper.GetConnectionName();
        }

        public Message LoadByCode(string code)
        {
            return this.Load(where =>
            {
                where.AppendItem("Code", code);
            }).FirstOrDefault();
        }

        public void DeleteByCode(string code, bool trueDelete = false)
        {
            //默认逻辑删除
            this.SetFields("IsValid", false, where => where.AppendItem("Code", code));

            //物理删除
            if (trueDelete)
            {
                this.Delete(where => where.AppendItem("Code", code));
            }
        }
        public void DeleteBySender(string sender, bool trueDelete = false)
        {
            //默认逻辑删除
            this.SetFields("IsValid", false, where => where.AppendItem("Sender", sender));

            //物理删除
            if (trueDelete)
            {
                this.Delete(where => where.AppendItem("Sender", sender));
            }
        }
        public void DeleteByUserCode(string code, bool trueDelete = false)
        {
            //默认逻辑删除
            this.SetFields("IsValid", false, where => where.AppendItem("Code", code));

            //物理删除
            if (trueDelete)
            {
                this.Delete(where => where.AppendItem("Receiver", code));
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="resourceID"></param>
        /// <returns></returns>
        public Message LoadByResourceID(string resourceID)
        {
            return this.Load(where => {
                where.AppendItem("IsValid", true);
                where.AppendItem("ResourceID", resourceID);
            }).FirstOrDefault();
        }

        /// <summary>
        /// 根据资源编码用户编码获取
        /// </summary>
        /// <param name="resourceID"></param>
        /// <param name="userCode"></param>
        /// <returns></returns>
        public Message Load(string resourceID, string userCode)
        {
         
            var result = new MessageCollection();


            string sql = string.Format(@"SELECT m.* FROM zc.UserMessage um

                                        INNER JOIN zc.Message m 
                                        ON um.MessageCode=m.Code

                                        WHERE um.UserInfoCode='{0}' AND m.ResourceID='{1}'"
                                        , resourceID,userCode);

            DataView dv = DbHelper.RunSqlReturnDS(sql.ToString(), GetConnectionName()).Tables[0].DefaultView;
            ORMapping.DataViewToCollection(result, dv);

            return result.FirstOrDefault();
        }

        public MessageCollection LoadAll()
        {
            return this.Load(where => where.AppendItem("1", 1));
        }

        public MessageCollection LoadAll(bool isValid)
        {
            return this.Load(where => where.AppendItem("IsValid", isValid), order => order.AppendItem("SendTime", FieldSortDirection.Descending));
        }

        public MessageCollection LoadUserMessage(string userCode, string oldActualTime)
        {
            return this.Load(where =>
            {
                if (oldActualTime != "")
                {
                    where.AppendItem("SendTime", oldActualTime, ">");
                }
                where.AppendItem("Receiver", "('" + userCode + "','SYSTEM')", "in", true);
                where.AppendItem("IsValid", true);
            });
        }
        public MessageCollection LoadUserMessage(string userCode)
        {
            return this.Load(where =>
            {
                where.AppendItem("Receiver", "('" + userCode + "','SYSTEM')", "in", true);
                where.AppendItem("IsValid", true);
            });
        }

        public MessageCollection LoadBySender(string sender)
        {
            return this.Load(where =>
            {
                where.AppendItem("Sender", sender);
                where.AppendItem("IsValid", true);
            });
        }

        public MessageCollection LoadByType(Enums.MessageType type)
        {
            return this.Load(where =>
            {
                where.AppendItem("Type", type.ToString("D"));
                where.AppendItem("IsValid", true);
            });
        }

   

        /// <summary>
        /// 根据编码串，类型获取消息数量
        /// </summary>
        /// <param name="msgCodes"></param>
        /// <param name="type"></param>
        /// <returns></returns>
        public int MessageNo(string[] msgCodes, Enums.MessageType type)
        {
            if (msgCodes.Length == 0)
            {
                return 0;
            }
            else
            {
                InSqlClauseBuilder inSql = new InSqlClauseBuilder();
                inSql.AppendItem(msgCodes);

                return this.Load(where =>
                {
                    where.AppendItem("Type", type.ToString("D"));
                    where.AppendItem("Code", inSql.ToSqlStringWithInOperator(TSqlBuilder.Instance), "", true);
                }).Count();
            }

        }
        public bool Exists(string[] msgCodes, Enums.MessageType type)
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
                    where.AppendItem("Type", type.ToString("D"));
                    where.AppendItem("Code", inSql.ToSqlStringWithInOperator(TSqlBuilder.Instance), "", true);
                });
            }

        }

    }


}

