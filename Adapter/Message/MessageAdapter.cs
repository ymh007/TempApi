using MCS.Library.Data.Builder;
using MCS.Library.Data.Mapping;
using MCS.Library.SOA.DataObjects;
using Seagull2.YuanXin.AppApi.Models.Message;
using SinoOcean.Seagull2.TransactionData.Meeting;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Web;
using Seagull2.YuanXin.AppApi.Models;

namespace Seagull2.YuanXin.AppApi.Adapter.Message
{
    /// <summary>
    /// 提醒消息数据访问类
    /// </summary>
    public class MessageAdapter : UpdatableAndLoadableAdapterBase<Models.Message.MessageModel, MessageCollection>
    {
        /// <summary>
        /// 页大小
        /// </summary>
        public const int PageSize = 10;

        /// <summary>
        /// 实例
        /// </summary>
        public static MessageAdapter Instance = new MessageAdapter();

        /// <summary>
        /// 数据库连接
        /// </summary>
        /// <returns></returns>
        protected override string GetConnectionName()
        {
            return ConnectionNameDefine.YuanXinBusiness;
        }

        /// <summary>
        /// 获取用户所有未读消息
        /// </summary>
        public MessageCollection GetMyNoReadMessage(string userCode)
        {
            return Load(
                w =>
                {
                    w.AppendItem("MessageStatusCode", (int)EnumMessageStatus.New);
                    w.AppendItem("ReceivePersonCode", userCode);
                    w.AppendItem("ValidStatus", 1);
                },
                o =>
                {
                    o.AppendItem("CreateTime", FieldSortDirection.Descending);
                }
            );
        }

        /// <summary>
        /// 获取用户所有消息
        /// </summary>
        public MessageCollection GetMyAllMessage(string userCode)
        {
            return Load(
                w =>
                {
                    w.AppendItem("ReceivePersonCode", userCode);
                    w.AppendItem("ValidStatus", 1);
                },
                o =>
                {
                    o.AppendItem("CreateTime", FieldSortDirection.Descending);
                }
            );
        }

        /// <summary>
        /// 更新消息集合
        /// </summary>
        /// <param name="coll"></param>
        public void UpdateMessageColl(MessageCollection coll)
        {
            StringBuilder sql = new StringBuilder();

            string insertSQl = "INSERT INTO dbo.Message " +
                "(Code, MeetingCode, MessageContent, MessageStatusCode, MessageTypeCode, MessageTitleCode, Creator, CreatorName, ReceivePersonCode, ReceivePersonName, ReceivePersonMeetingTypeCode, OverdueTime, ValidStatus, CreateTime) VALUES" +
    "            (N'{0}', N'{1}', N'{2}', N'{3}', N'{4}', N'{5}', N'{6}', N'{7}', N'{8}', N'{9}', '{10}', '{11}', '{12}', '{13}');";
            coll.ForEach(c =>
            {
                sql.Append(string.Format(insertSQl, c.Code, c.MeetingCode, c.MessageContent, (int)c.MessageStatusCode, c.MessageTypeCode, (int)c.MessageTitleCode, c.Creator, c.CreatorName, c.ReceivePersonCode, c.ReceivePersonName, c.ReceivePersonMeetingTypeCode, c.OverdueTime, c.ValidStatus, c.CreateTime));
            });
            DbHelper.RunSql(sql.ToString(), ConnectionNameDefine.YuanXinBusiness);
        }

        /// <summary>
        /// 根据编码查询消息
        /// </summary>
        /// <param name="code"></param>
        /// <returns></returns>
        public MessageModel LoadByCode(string code)
        {
            return Load(m => m.AppendItem("Code", code)).SingleOrDefault();
        }
        private MessageCollection LoadByWhereSQL(string whereSql)
        {
            string sql = string.Format(@"SELECT * FROM dbo.Message WHERE " + whereSql);
            MessageCollection messageColl = this.QueryData(sql);
            return messageColl;
        }

        /// <summary>
        /// 获取某人(签到0/会议1)消息的最新一条
        /// </summary>
        /// <param name="messageTypeCode"></param>
        /// <param name="userCode"></param>
        /// <returns></returns>
        public Models.Message.MessageModel LoadNewMessage(string messageTypeCode, string userCode)
        {
            string sql = string.Format(@"DECLARE @maxCreateTime DATETIME;
                                        SELECT @maxCreateTime=MAX(CreateTime) FROM dbo.Message WHERE MessageTypeCode='{0}' AND ReceivePersonCode='{1}';
                                        SELECT * FROM dbo.Message WHERE CreateTime=@maxCreateTime AND ReceivePersonCode='{2}'", messageTypeCode, userCode, userCode);
            MessageCollection messColl = this.QueryData(sql);
            Models.Message.MessageModel message = messColl.Count > 0 ? messColl[0] : null;
            return message;
        }

        /// <summary>
        /// 获取某人的(签到0/会议1)消息（分页）
        /// </summary>
        /// <returns></returns>
        public MessagePageView GetMyMeetingMessageColl(string userCode, int pageIndex, string messageTypeCode)
        {
            MessagePageView view = new MessagePageView();

            string sql = string.Format(@"UPDATE dbo.Message SET MessageStatusCode='{0}' WHERE OverdueTime<=GETDATE() AND (ReceivePersonMeetingTypeCode IS NULL OR ReceivePersonMeetingTypeCode='') AND MessageStatusCode!='{1}' AND ReceivePersonCode='{2}';
                                        SELECT *,ROW_NUMBER() OVER(ORDER BY CreateTime DESC) AS rowNum INTO #a FROM dbo.Message
                                         WHERE ReceivePersonCode='{3}' AND MessageTypeCode='{4}' AND ValidStatus=1;
                                        SELECT * FROM #a WHERE rowNum BETWEEN {5} AND {6};
                                        DROP TABLE #a;"
                                        , (int)EnumMessageStatus.IsOverdue, (int)EnumMessageStatus.IsOverdue, userCode, userCode, messageTypeCode, (pageIndex - 1) * PageSize + 1, pageIndex * PageSize);
            view.MessageColl = this.QueryData(sql);
            view.MessageColl.Sort((m, n) =>
            {
                if (m.CreateTime < n.CreateTime)
                {
                    return -1;
                }
                else
                {
                    return 1;
                }
            });

            string sql1 = string.Format(@"SELECT COUNT(1) as TotalCount FROM dbo.Message WHERE ReceivePersonCode='{0}' AND  MessageTypeCode='{1}' AND ValidStatus=1", userCode, messageTypeCode);
            System.Data.DataSet dataSet = DbHelper.RunSqlReturnDS(sql1, ConnectionNameDefine.YuanXinBusiness);
            view.TotalCount = (int)dataSet.Tables[0].Rows[0]["TotalCount"];
            view.IsLastPage = view.TotalCount <= pageIndex * PageSize ? true : false;

            return view;
        }

        /// <summary>
        /// 获取某个人的全部分类消息分页
        /// </summary>
        /// <returns></returns>
        public MessageCollection GetMyAllMsg(string userCode, int pageIndex, int pageSize, string createTime)
        {
            string isQueryTime = "";
            int offset = 0;
            if (createTime != "")
            {
                offset =  pageIndex * pageSize+1;
                isQueryTime = "AND  CreateTime < '" + createTime + "'";
            }
            string sql = string.Format(@"SELECT *  FROM dbo.Message WHERE receivepersonCode='{0}' {1}  ORDER BY CreateTime desc OFFSET {2} ROW FETCH NEXT {3} ROWS ONLY ", userCode, isQueryTime, offset, pageSize);
            return this.QueryData(sql);
        }


        /// <summary>
        /// <summary>
        /// 获取某个人的全部分类消息分页
        /// </summary>
        /// <returns></returns>
        public MessageCollection GetMyAllMsgByType(string mtype, string mclass, string userCode, int pageIndex, int pageSize, string createTime)
        {
            string isQueryTime = "";
            string typeQueryStr = "";
            int offset = 0;
            if (createTime != "")
            {
                offset = pageIndex * pageSize + 1;
                isQueryTime = "AND  CreateTime < '" + createTime + "'";
            }
            if (!string.IsNullOrEmpty(mtype))
            {
                typeQueryStr = "AND ModuleType='" + mtype + "' ";
            }
            else
            {
                typeQueryStr = "AND MessageTypeCode=" + mclass;
            }
            string sql = string.Format(@"SELECT * FROM dbo.Message WHERE  receivepersonCode='{0}' {1} {2}  ORDER BY CreateTime desc OFFSET {3} ROW FETCH NEXT {4} ROWS ONLY ", userCode, typeQueryStr, isQueryTime, offset, pageSize);
            return this.QueryData(sql);
        }



        /// <summary>
        /// 更新未读消息为已读状态
        /// </summary>
        /// <returns></returns>
        public void UpdateMessageRead(string userCode)
        {
            string sql = string.Format(@"UPDATE dbo.Message SET MessageStatusCode='{0}' WHERE ReceivePersonCode='{1}' AND MessageStatusCode='{2}' AND ValidStatus=1", (int)EnumMessageStatus.IsRead, userCode, (int)EnumMessageStatus.New);
            DbHelper.RunSql(sql, ConnectionNameDefine.YuanXinBusiness);
        }




        /// <summary>
        /// 更新未读消息为已读状态
        /// </summary>
        /// <returns></returns>
        public void UpdateMessageRead(string userCode, string messageTypeCode)
        {
            string sql = string.Format(@"UPDATE dbo.Message SET MessageStatusCode='{0}' WHERE ReceivePersonCode='{1}' AND MessageTypeCode='{2}' AND MessageStatusCode='{3}' AND ValidStatus=1", (int)EnumMessageStatus.IsRead, userCode, messageTypeCode, (int)EnumMessageStatus.New);
            DbHelper.RunSql(sql, ConnectionNameDefine.YuanXinBusiness);
        }

        /// <summary>
        /// 获取我的(签到0/会议1)未读消息数量
        /// </summary> 
        /// <returns></returns>
        public int GetMyNoReadMessageCount(string userCode, string messageTypeCode)
        {
            int result = 0;

            using (SqlConnection conn = new SqlConnection(ConfigurationManager.ConnectionStrings["yuanxin"].ToString()))
            {
                conn.Close();
                conn.Open();
                string sqlnew = "SELECT COUNT(1) FROM dbo.Message WHERE MessageStatusCode=@MessageStatusCode AND ReceivePersonCode=@ReceivePersonCode AND MessageTypeCode=@MessageTypeCode AND ValidStatus=1";
                SqlCommand comand = new SqlCommand(sqlnew, conn);
                comand.CommandType = System.Data.CommandType.Text;
                comand.Parameters.Add(new SqlParameter { ParameterName = "@MessageStatusCode", Value = (int)EnumMessageStatus.New, SqlDbType = System.Data.SqlDbType.NVarChar });
                comand.Parameters.Add(new SqlParameter { ParameterName = "@ReceivePersonCode", Value = userCode, SqlDbType = System.Data.SqlDbType.NVarChar });
                comand.Parameters.Add(new SqlParameter { ParameterName = "@MessageTypeCode", Value = messageTypeCode, SqlDbType = System.Data.SqlDbType.NVarChar });
                result = (int)comand.ExecuteScalar();
            }
            return result;
        }
        /// <summary>
        /// 获取我的所有(签到和 会议1)未读消息数量
        /// </summary> 
        /// <returns></returns>
        public int GetMyNoReadMessageCount(string userCode)
        {
            int result = 0;

            using (SqlConnection conn = new SqlConnection(ConfigurationManager.ConnectionStrings["yuanxin"].ToString()))
            {
                conn.Close();
                conn.Open();
                string sqlnew = "SELECT COUNT(1) FROM dbo.Message WHERE MessageStatusCode=@MessageStatusCode AND ReceivePersonCode=@ReceivePersonCode  AND ValidStatus=1";
                SqlCommand comand = new SqlCommand(sqlnew, conn);
                comand.CommandType = System.Data.CommandType.Text;
                comand.Parameters.Add(new SqlParameter { ParameterName = "@MessageStatusCode", Value = (int)EnumMessageStatus.New, SqlDbType = System.Data.SqlDbType.NVarChar });
                comand.Parameters.Add(new SqlParameter { ParameterName = "@ReceivePersonCode", Value = userCode, SqlDbType = System.Data.SqlDbType.NVarChar });
                result = (int)comand.ExecuteScalar();
            }
            return result;
        }
        /// <summary>
        /// 接受/拒绝_会议消息
        /// </summary>
        /// <returns></returns>
        public ResultView AcceptOrRefuseMeetingMessage(string messageCode, EnumMessageTitle messageTitleCode)
        {
            ResultView resultView = new ResultView();
            try
            {
                #region 接受/拒绝会议时，给会议创建人发送提醒消息
                string messageContent = "";
                MessageModel oldMessage = this.LoadByCode(messageCode);
                if (oldMessage.ReceivePersonMeetingTypeCode == ((int)EnumMessageTitle.ReceiveMeeting).ToString())
                {
                    resultView.Result = false;
                    resultView.ErrorString = "您已经接受该会议！";
                }
                else if (oldMessage.ReceivePersonMeetingTypeCode == ((int)EnumMessageTitle.RefuseMeeting).ToString())
                {
                    resultView.Result = false;
                    resultView.ErrorString = "您已经拒绝该会议！";
                }
                else
                {
                    #region 更新旧数据
                    string oldSql = string.Format(@"UPDATE dbo.Message SET ReceivePersonMeetingTypeCode='{0}' WHERE Code='{1}'", (int)messageTitleCode, messageCode);
                    DbHelper.RunSql(oldSql, ConnectionNameDefine.YuanXinBusiness);
                    #endregion

                    if (messageTitleCode == EnumMessageTitle.ReceiveMeeting)
                    {
                        messageContent = oldMessage.ReceivePersonName + "已接受会议";
                    }
                    else if (messageTitleCode == EnumMessageTitle.RefuseMeeting)
                    {
                        messageContent = oldMessage.ReceivePersonName + "已拒绝会议";
                    }

                    #region 回复创建人消息
                    MessageModel sendMessage = new MessageModel
                    {
                        Code = Guid.NewGuid().ToString(),
                        ValidStatus = true,
                        Creator = oldMessage.ReceivePersonCode,
                        CreatorName = oldMessage.ReceivePersonName,
                        CreateTime = DateTime.Now,
                        ReceivePersonCode = oldMessage.Creator,
                        ReceivePersonName = oldMessage.CreatorName,
                        MessageContent = messageContent,
                        MeetingCode = oldMessage.MeetingCode,
                        MessageTitleCode = EnumMessageTitle.Other,
                        MessageTypeCode = oldMessage.MessageTypeCode,
                        MessageStatusCode = EnumMessageStatus.New
                    };
                    AddMessage(sendMessage);
                    #endregion

                    #region 给创建人发推送
                    var model = new PushService.Model()
                    {
                        BusinessDesc= "接受/拒绝_会议消息",
                        Title = messageContent,
                        Content = messageContent,
                        Extras = new PushService.ModelExtras()
                        {
                            action = "meetingReminder",
                            bags = ""
                        },
                        SendType = PushService.SendType.Person,
                        Ids = oldMessage.Creator
                    };
                    PushService.Push(model, out string pushResult);
                    #endregion

                    resultView.Result = true;
                }
                #endregion
            }
            catch (Exception e)
            {
                Log.WriteLog(e.Message);
                Log.WriteLog(e.StackTrace);
                resultView.Result = false;
                resultView.ErrorString = "服务器出现错误，请联系管理人员！";
            }

            return resultView;
        }
        /// <summary>
        /// 删除消息
        /// </summary>
        /// <returns></returns>
        public ResultView DeleteMessage(string messageCode)
        {
            ResultView result = new ResultView();
            try
            {
                string sql = string.Format(@"UPDATE dbo.Message SET ValidStatus=0 WHERE Code='{0}'", messageCode);
                DbHelper.RunSql(sql, ConnectionNameDefine.YuanXinBusiness);
                result.Result = true;
            }
            catch (Exception e)
            {
                Log.WriteLog(e.Message);
                Log.WriteLog(e.StackTrace);
                result.Result = false;
                result.ErrorString = "服务器出现错误，请联系管理人员！";
            }
            return result;
        }
        /// <summary>
        /// 回复消息（会议接受/拒绝时调用）
        /// </summary>
        /// <returns></returns>
        public ResultView AddMessage(Models.Message.MessageModel message)
        {
            ResultView result = new ResultView();
            try
            {
                Update(message);
                result.Result = true;
            }
            catch (Exception e)
            {
                Log.WriteLog(e.Message);
                Log.WriteLog(e.StackTrace);
                result.Result = false;
                result.ErrorString = "服务器出现错误，请联系管理人员！";
            }
            return result;
        }

        /// <summary>
        /// 添加会议消息(远洋移动办公接口使用)
        /// </summary>
        public ResultView AddMessage(MessageForMeetingYuanXin message)
        {
            ResultView result = new ResultView();
            try
            {
                var personsColl = message.ReceivePerson;
                message.ModuleType.ToString();
                string sql = "";
                var meetingRoomType = message.ModuleType == 0 ? null : message.ModuleType.ToString();
                for (var i = 0; i < personsColl.Count; i++)
                {
                    sql += string.Format(@"INSERT INTO dbo.Message
                                                        ( Code ,
                                                          MeetingCode ,
                                                          MessageContent ,
                                                          MessageStatusCode ,
                                                          MessageTypeCode ,
                                                          MessageTitleCode ,
                                                          Creator ,
                                                          CreatorName ,
                                                          ReceivePersonCode ,
                                                          ReceivePersonName ,
                                                          ReceivePersonMeetingTypeCode,
                                                          OverdueTime ,
                                                          ValidStatus ,
                                                          CreateTime,
                                                          ModuleType
                                                        )
                                                VALUES  ( N'{0}' , -- Code - nvarchar(36)
                                                          N'{1}' , -- MeetingCode - nvarchar(36)
                                                          N'{2}' , -- MessageContent - nvarchar(max)
                                                          N'{3}' , -- MessageStatusCode - nvarchar(50)
                                                          N'{4}' , -- MessageTypeCode - nvarchar(50)
                                                          N'{5}' , -- MessageTitleCode - nvarchar(50)
                                                          N'{6}' , -- Creator - nvarchar(50)
                                                          N'{7}' , -- CreatorName - nvarchar(50)
                                                          N'{8}' , -- ReceivePersonCode - nvarchar(36)
                                                          N'{9}' , -- ReceivePersonName - nvarchar(50)
                                                          NULL , -- ReceivePersonMeetingTypeCode - nvarchar(50)
                                                          '{10}' , -- OverdueTime - datetime
                                                          1 , -- ValidStatus - bit
                                                          '{11}',  -- CreateTime - datetime
                                                          '{12}'  -- ModuleType
                                                        );"
                                                        , Guid.NewGuid().ToString("N"),
                                                        message.MeetingCode,
                                                        message.MessageContent,
                                                        ((int)EnumMessageStatus.New).ToString(),
                                                        MessageForMeetingYuanXin.MessageTypeCode,
                                                        (int)message.MessageTitleCode,
                                                        message.Creator,
                                                        message.CreatorName,
                                                        message.ReceivePerson[i].UserCode,
                                                        message.ReceivePerson[i].DisplsyName,
                                                        message.OverdueTime,
                                                        DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss"),
                                                        meetingRoomType
                                                        );
                    //如果发出的会议邀请通知，在接收人未操作前被取消了，则更新接收人的消息状态为取消操作
                    if (message.MessageTitleCode == EnumMessageTitle.CancelMeeting)
                    {
                        MessageCollection oldMessColl = MessageAdapter.Instance.LoadByWhereSQL("(ReceivePersonMeetingTypeCode='' OR ReceivePersonMeetingTypeCode IS NULL) AND ReceivePersonCode='" + message.ReceivePerson[i].UserCode + "' AND MeetingCode='" + message.MeetingCode + "' AND ValidStatus=1");
                        if (oldMessColl.Count > 0)
                        {
                            foreach (var old in oldMessColl)
                            {
                                sql += string.Format("UPDATE dbo.Message SET ReceivePersonMeetingTypeCode='{0}' WHERE CODE='{1}';", (int)EnumMessageTitle.CancelMeeting, old.Code);
                            }
                        }
                    }
                }
                Log.WriteLog("移动办公专用-添加会议消息(接口使用)SQL：");
                Log.WriteLog(sql);
                //string sql = string.Format(@"SELECT * FROM dbo.Message");
                DbHelper.RunSql(sql, ConnectionNameDefine.YuanXinBusiness);

                result.Result = true;
            }
            catch (Exception e)
            {
                Log.WriteLog(e.Message);
                Log.WriteLog(e.StackTrace);
                result.Result = false;
                result.ErrorString = "服务器出现错误，请联系管理人员！" + e.StackTrace;
            }
            return result;
        }

        /// <summary>
        /// 添加签到消息(远洋移动办公接口使用)
        /// </summary>
        public ResultView AddMessage(MessageForSignYuanXin message)
        {
            ResultView result = new ResultView();
            result.Result = true;
            try
            {
                Dictionary<string, string> personsColl = message.ReceivePerson;
                Dictionary<string, string>.KeyCollection personsCodeColl = personsColl.Keys;

                MessageCollection messColl = new MessageCollection();

                foreach (var personsCode in personsCodeColl)
                {
                    Models.Message.MessageModel mess = new Models.Message.MessageModel()
                    {
                        Code = Guid.NewGuid().ToString("N"),
                        CreateTime = DateTime.Now,
                        Creator = "",
                        CreatorName = "",
                        MeetingCode = "",
                        MessageContent = message.MessageContent,
                        MessageStatusCode = EnumMessageStatus.New,
                        MessageTitleCode = MessageForSignYuanXin.MessageTitleCode,
                        MessageTypeCode = MessageForSignYuanXin.MessageTypeCode,
                        ReceivePersonCode = personsCode,
                        ReceivePersonName = personsColl[personsCode],
                        ValidStatus = true,
                        ReceivePersonMeetingTypeCode = ""
                    };
                    messColl.Add(mess);
                }
                UpdateMessageColl(messColl);
            }
            catch (Exception e)
            {
                Log.WriteLog(e.Message);
                Log.WriteLog(e.StackTrace);
                result.Result = false;
                result.ErrorString = "服务器出现错误，请联系管理人员！" + e.StackTrace;
            }
            return result;
        }

        /// <summary>
        /// 批量插入系统提醒数据（系统消息）
        /// </summary>
        public void BatchInsert(List<MessageModel> model)
        {
            //构造DataTable
            var dt = new DataTable();
            dt.Columns.Add("Code", System.Type.GetType("System.String"));
            dt.Columns.Add("MeetingCode", System.Type.GetType("System.String"));
            dt.Columns.Add("MessageContent", System.Type.GetType("System.String"));
            dt.Columns.Add("MessageStatusCode", System.Type.GetType("System.String"));
            dt.Columns.Add("MessageTypeCode", System.Type.GetType("System.String"));
            dt.Columns.Add("MessageTitleCode", System.Type.GetType("System.String"));
            dt.Columns.Add("ModuleType", System.Type.GetType("System.String"));
            dt.Columns.Add("Creator", System.Type.GetType("System.String"));
            dt.Columns.Add("CreatorName", System.Type.GetType("System.String"));
            dt.Columns.Add("ReceivePersonCode", System.Type.GetType("System.String"));
            dt.Columns.Add("ReceivePersonName", System.Type.GetType("System.String"));
            dt.Columns.Add("ReceivePersonMeetingTypeCode", System.Type.GetType("System.String"));
            dt.Columns.Add("OverdueTime", System.Type.GetType("System.DateTime"));
            dt.Columns.Add("CreateTime", System.Type.GetType("System.DateTime"));
            dt.Columns.Add("ValidStatus", System.Type.GetType("System.Boolean"));
            dt.Columns.Add("MsgView", System.Type.GetType("System.String"));
            model.ForEach(m =>
            {
                DataRow dr = dt.NewRow();
                dr["Code"] = m.Code;
                dr["MeetingCode"] = m.MeetingCode;
                dr["MessageContent"] = m.MessageContent;
                dr["MessageStatusCode"] = "0";
                dr["MessageTypeCode"] = m.MessageTypeCode;
                dr["MessageTitleCode"] = (int)m.MessageTitleCode;
                dr["ModuleType"] = m.ModuleType;
                dr["Creator"] = m.Creator;
                dr["CreatorName"] = m.CreatorName;
                dr["ReceivePersonCode"] = m.ReceivePersonCode;
                dr["ReceivePersonName"] = m.ReceivePersonName;
                dr["ReceivePersonMeetingTypeCode"] = m.ReceivePersonMeetingTypeCode;
                dr["OverdueTime"] = m.OverdueTime;
                dr["CreateTime"] = m.CreateTime;
                dr["ValidStatus"] = m.ValidStatus;
                dr["MsgView"] = m.MsgView;
                dt.Rows.Add(dr);
            });
            SqlDbHelper.BulkInsertData(dt, "[dbo].[Message]", GetConnectionName());
        }


        /// <summary>
        /// 批量插入系统提醒数据（系统消息）
        /// </summary>
        public void BatchInsert(MessageCollection model)
        {
            //构造DataTable
            var dt = new DataTable();
            dt.Columns.Add("Code", System.Type.GetType("System.String"));
            dt.Columns.Add("MeetingCode", System.Type.GetType("System.String"));
            dt.Columns.Add("MessageContent", System.Type.GetType("System.String"));
            dt.Columns.Add("MessageStatusCode", System.Type.GetType("System.String"));
            dt.Columns.Add("MessageTypeCode", System.Type.GetType("System.String"));
            dt.Columns.Add("MessageTitleCode", System.Type.GetType("System.String"));
            dt.Columns.Add("ModuleType", System.Type.GetType("System.String"));
            dt.Columns.Add("Creator", System.Type.GetType("System.String"));
            dt.Columns.Add("CreatorName", System.Type.GetType("System.String"));
            dt.Columns.Add("ReceivePersonCode", System.Type.GetType("System.String"));
            dt.Columns.Add("ReceivePersonName", System.Type.GetType("System.String"));
            dt.Columns.Add("ReceivePersonMeetingTypeCode", System.Type.GetType("System.String"));
            dt.Columns.Add("OverdueTime", System.Type.GetType("System.DateTime"));
            dt.Columns.Add("CreateTime", System.Type.GetType("System.DateTime"));
            dt.Columns.Add("ValidStatus", System.Type.GetType("System.Boolean"));
            dt.Columns.Add("MsgView", System.Type.GetType("System.String"));
            model.ForEach(m =>
            {
                DataRow dr = dt.NewRow();
                dr["Code"] = m.Code;
                dr["MeetingCode"] = m.MeetingCode;
                dr["MessageContent"] = m.MessageContent;
                dr["MessageStatusCode"] = "0";
                dr["MessageTypeCode"] = m.MessageTypeCode;
                dr["MessageTitleCode"] = (int)m.MessageTitleCode;
                dr["ModuleType"] = m.ModuleType;
                dr["Creator"] = m.Creator;
                dr["CreatorName"] = m.CreatorName;
                dr["ReceivePersonCode"] = m.ReceivePersonCode;
                dr["ReceivePersonName"] = m.ReceivePersonName;
                dr["ReceivePersonMeetingTypeCode"] = m.ReceivePersonMeetingTypeCode;
                dr["OverdueTime"] = m.OverdueTime;
                dr["CreateTime"] = m.CreateTime;
                dr["ValidStatus"] = m.ValidStatus;
                dr["MsgView"] = m.MsgView;
                dt.Rows.Add(dr);
            });
            SqlDbHelper.BulkInsertData(dt, "[dbo].[Message]", GetConnectionName());
        }

        /// <summary>
        /// 批量插入数据
        /// </summary>
        public void MessageBatchInsert(MessageCollection models)
        {
            //构造DataTable
            var dt = new DataTable();
            dt.Columns.Add("Code", System.Type.GetType("System.String"));
            dt.Columns.Add("MeetingCode", System.Type.GetType("System.String"));
            dt.Columns.Add("MessageContent", System.Type.GetType("System.String"));
            dt.Columns.Add("MessageStatusCode", System.Type.GetType("System.String"));
            dt.Columns.Add("MessageTypeCode", System.Type.GetType("System.String"));
            dt.Columns.Add("MessageTitleCode", System.Type.GetType("System.String"));
            dt.Columns.Add("ModuleType", System.Type.GetType("System.String"));
            dt.Columns.Add("Creator", System.Type.GetType("System.String"));
            dt.Columns.Add("CreatorName", System.Type.GetType("System.String"));
            dt.Columns.Add("ReceivePersonCode", System.Type.GetType("System.String"));
            dt.Columns.Add("ReceivePersonName", System.Type.GetType("System.String"));
            dt.Columns.Add("ReceivePersonMeetingTypeCode", System.Type.GetType("System.String"));
            dt.Columns.Add("OverdueTime", System.Type.GetType("System.DateTime"));
            dt.Columns.Add("CreateTime", System.Type.GetType("System.DateTime"));
            dt.Columns.Add("ValidStatus", System.Type.GetType("System.Boolean"));
            dt.Columns.Add("MsgView", System.Type.GetType("System.String"));
            models.ForEach(model =>
            {
                DataRow dr = dt.NewRow();
                dr["Code"] = model.Code;
                dr["MeetingCode"] = model.MeetingCode;
                dr["MessageContent"] = model.MessageContent;
                dr["MessageStatusCode"] = (int)model.MessageStatusCode;
                dr["MessageTypeCode"] = model.MessageTypeCode;
                dr["MessageTitleCode"] = (int)model.MessageTitleCode;
                dr["ModuleType"] = model.ModuleType;
                dr["Creator"] = model.Creator;
                dr["CreatorName"] = model.CreatorName;
                dr["ReceivePersonCode"] = model.ReceivePersonCode;
                dr["ReceivePersonName"] = model.ReceivePersonName;
                dr["ReceivePersonMeetingTypeCode"] = model.ReceivePersonMeetingTypeCode;
                dr["OverdueTime"] = model.OverdueTime;
                dr["CreateTime"] = model.CreateTime;
                dr["ValidStatus"] = model.ValidStatus;
                dr["MsgView"] = model.MsgView;
                dt.Rows.Add(dr);
            });
            SqlDbHelper.BulkInsertData(dt, "[dbo].[Message]", GetConnectionName());
        }

        /// <summary>
        /// 删除某个用户某个类型的消息
        /// </summary>
        /// <param name="type"></param>
        /// <param name="userId"></param>
        public void DeleteByMessageType(int type, string userCode)
        {
            try
            {
                var sql = @"DELETE dbo.Message
                            WHERE   ReceivePersonCode = @UserCode";
                SqlParameter[] parameters = {
                    new SqlParameter("@UserCode", SqlDbType.NVarChar,36),
                };
                parameters[0].Value = userCode;
                var sqlHelper = new SqlDbHelper();
                sqlHelper.ExecuteNonQuery(sql, CommandType.Text, parameters);
            }
            catch (Exception ex)
            {
                throw new Exception(ex.ToString());
            }

        }

        /// <summary>
		/// 将某条消息改为已读
		/// </summary>
		public void UpdateReadTime(string messageCode)
        {
            var sql = $"UPDATE [dbo].[Message] SET [ReadTime] = '{DateTime.Now}' WHERE [Code] = '{messageCode}';";
            DbHelper.RunSql(sql.ToString(), ConnectionNameDefine.YuanXinBusiness);
        }
    }
}