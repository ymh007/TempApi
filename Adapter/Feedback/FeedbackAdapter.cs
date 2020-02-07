using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Web;
using MCS.Library.SOA.DataObjects;
using Seagull2.YuanXin.AppApi.Models.Feedback;

namespace Seagull2.YuanXin.AppApi.Adapter.Feedback
{
    /// <summary>
    /// 意见反馈 Adapter
    /// </summary>
    public class FeedbackAdapter : UpdatableAndLoadableAdapterBase<FeedbackModel, FeedbackCollection>
    {
        /// <summary>
        /// 实例
        /// </summary>
        public static readonly FeedbackAdapter Instance = new FeedbackAdapter();

        /// <summary>
        /// 数据库连接名称
        /// </summary>
        protected override string GetConnectionName()
        {
            return Models.ConnectionNameDefine.YuanXinBusiness;
        }



        /// <summary>
        /// 更新最新的回复消息到原始问题上便于分页显示
        /// </summary>
        /// <param name="code">反馈问题主键</param>
        /// <param name="rcode">回复人code</param>
        /// <param name="rname">回复人名字</param>
        /// <param name="way">回复方式</param>
        /// <param name="IsNewMsg">是否显示新消息</param>
        /// <returns></returns>
        public FeedbackModel UpdateNewReply(string code, string rcode, string rname, int way,bool IsNewMsg)
        {
            FeedbackModel fm = this.Load(p => p.AppendItem("Code", code)).FirstOrDefault();
            if (fm != null)
            {
                fm.IsReply = IsNewMsg;//用于提示是最新消息
                fm.ReplyWay = way;
                fm.ReplyUserCode = rcode;
                fm.ReplyUserName = rname;
                fm.ReplyDateTime = DateTime.Now;
                this.Update(fm);
            }
            return fm;
        }

        #region 获取意见反馈列表 - APP

        /// <summary>
        /// 获取意见反馈列表 - APP - 总记录数
        /// </summary>
        public int GetListForAPP(string userCode)
        {
            var sql = @"SELECT COUNT(0) FROM [office].[Feedback] WHERE [UserCode] = '{0}'";
            sql = string.Format(sql, userCode);
            var result = DbHelper.RunSqlReturnScalar(sql, GetConnectionName());
            int count;
            if (int.TryParse(result.ToString(), out count))
            {
                return count;
            }
            return 0;
        }

        /// <summary>
        /// 获取意见反馈列表 - APP - 当前页数据
        /// </summary>
        /// <param name="userCode">用户编码</param>
        /// <param name="pageSize">页大小</param>
        /// <param name="pageIndex">页索引，从1开始</param>
        public FeedbackCollection GetListForAPP(string userCode, int pageSize, int pageIndex)
        {
            pageIndex--;
            var sql = @"WITH [Temp] AS
                        (
	                        SELECT ROW_NUMBER() OVER (ORDER BY  [CreateTime] DESC) AS [Row], * FROM [office].[Feedback] WHERE [UserCode] = '{0}'
                        )
                        SELECT * FROM [Temp] WHERE [Temp].[Row] BETWEEN {1} AND {2}";
            sql = string.Format(sql, userCode, pageIndex * pageSize + 1, pageIndex * pageSize + pageSize);
            return QueryData(sql);
        }

        #endregion

        #region 获取意见反馈列表 - PC

        /// <summary>
        /// 获取意见反馈列表 - PC - 总记录数  ReplyWay 1 待回复  
        /// </summary>
        public int GetListForPC(int isReply, int isMark)
        {
            var sql = @"SELECT COUNT(0) FROM [office].[Feedback]";
            if (isReply != 1 && isMark == 1)
            {
                sql = @"SELECT COUNT(0) FROM [office].[Feedback] where ReplyWay=" + (isReply == 2 ? 0 : 1) + "";
            }
            if (isReply == 1 && isMark != 1)
            {
                sql = @"SELECT COUNT(0) FROM [office].[Feedback] where mark=" + (isMark == 2 ? 1 : 0) + "";
            }
            if (isReply != 1 && isMark != 1)
            {
                sql = @"SELECT COUNT(0) FROM [office].[Feedback] where ReplyWay =" + (isReply == 2 ? 0 : 1) + " and  mark=" + (isMark == 2 ? 1 : 0) + "";
            }
            var result = DbHelper.RunSqlReturnScalar(sql, GetConnectionName());
            int count;
            if (int.TryParse(result.ToString(), out count))
            {
                return count;
            }
            return 0;
        }

        /// <summary>
        /// 获取意见反馈列表 - PC - 总记录数  未回复 
        /// </summary>
        /// <param name="replyWay"> 回复方式   1 app  0 pc </param>
        /// <returns></returns>
        public int GetNoReplayCount(int replyWay)
        {
            var sql = @"  SELECT COUNT(0) FROM YuanXinBusiness.[office].[Feedback] where  ReplyWay=" + replyWay;
            var result = DbHelper.RunSqlReturnScalar(sql, GetConnectionName());
            int count;
            if (int.TryParse(result.ToString(), out count))
            {
                return count;
            }
            return 0;
        }


        /// <summary>
        /// 获取意见反馈列表 - PC - 当前页数据
        /// </summary>
        /// <param name="pageSize">页大小</param>
        /// <param name="pageIndex">页索引，从1开始</param>
        /// <param name="isReply">是否回复</param>
        /// <param name="isMark">是否标记</param>
        public FeedbackCollection GetListForPC(int pageSize, int pageIndex, int isReply, int isMark)
        {
            pageIndex--;
            var where = "";
            var sql = @"WITH [Temp] AS
                   (
	            SELECT ROW_NUMBER() OVER (ORDER BY [CreateTime] DESC) AS [Row], * FROM [office].[Feedback] {0}
                   )
                    SELECT * FROM [Temp] WHERE [Temp].[Row] BETWEEN {1} AND {2}";
            if (isReply != 1 && isMark == 1)
            {
                where = @"where ReplyWay=" + (isReply == 2 ? 0 : 1) + "";
            }
            if (isReply == 1 && isMark != 1)
            {
                where = @"where mark=" + (isMark == 2 ? 1 : 0) + "";
            }
            if (isReply != 1 && isMark != 1)
            {
                where = @"where ReplyWay =" + (isReply == 2 ? 0 : 1) + " and  mark=" + (isMark == 2 ? 1 : 0) + "";
            }
            sql = string.Format(sql,where, pageIndex * pageSize + 1, pageIndex * pageSize + pageSize);
            return QueryData(sql);
        }

        #endregion
    }


    /// <summary>
    /// 意见回复 adapter
    /// </summary>
    public class FeedbackReplyAdapter : UpdatableAndLoadableAdapterBase<FeedbackReply, FeedbackReplyCollection>
    {

        /// <summary>
        /// 实体
        /// </summary>
        public static readonly FeedbackReplyAdapter Instance = new FeedbackReplyAdapter();

        /// <summary>
        /// 数据库连接名称
        /// </summary>
        protected override string GetConnectionName()
        {
            return Models.ConnectionNameDefine.YuanXinBusiness;
        }

        /// <summary>
        /// 根据意见反馈code 查询所有回复内容
        /// </summary>
        /// <param name="fbCode"></param>
        /// <returns></returns>
        public FeedbackReplyCollection GetReplyByFeedBackCode(string fbCode)
        {
            var sql = @"SELECT * FROM OFFICE.FEEDBACKREPLY WHERE FeedbackCode='" + fbCode + "'";
            return QueryData(sql);
        }


    }



}