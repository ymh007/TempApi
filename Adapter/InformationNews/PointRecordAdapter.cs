using MCS.Library.SOA.DataObjects;
using System;
using System.Configuration;
using System.Data;
using System.Data.SqlClient;

namespace Seagull2.YuanXin.AppApi.Models.InformationNews
{
    /// <summary>
    /// 点赞记录适配器
    /// </summary>
    public class PointRecordAdapter : UpdatableAndLoadableAdapterBase<PointPraiseRecordModel, PointPraiseRecordCollection>
    {
        /// <summary>
        /// 实例
        /// </summary>
        public static readonly PointRecordAdapter Instance = new PointRecordAdapter();
        /// <summary>
        /// 数据库连接名称
        /// </summary>
        protected override string GetConnectionName()
        {
            return ConnectionNameDefine.SinooceanLandAddressList;
        }

        /// <summary>
        /// 用户是否存在点赞记录
        /// </summary>
        /// <param name="userName">用户名 如：v-sunzhh</param>
        /// <param name="type">1：文章点赞、0：评论点赞</param>
        /// <param name="relationId">RelationID</param>
        /// <returns></returns>
        public bool IsPoint(string userName, int type, int relationId)
        {
            userName = "SINOOCEANLAND\\" + userName;
            return Exists(w =>
            {
                w.AppendItem("UserID", userName);
                w.AppendItem("PointPraiseType", type);
                w.AppendItem("RelationID", relationId);
            });
        }

        #region 资讯新闻点赞功能
        /// <summary>
        /// 资讯新闻点赞功能
        /// </summary>
        /// <param name="commentType">1：文章点赞；0：回复点赞</param>
        /// <param name="relationID">文章ID</param>
        /// <param name="userID">用户ID</param>
        /// <returns></returns>
        public DataTable CommentPointOfPraise(string commentType, string relationID, string userID)
        {
            try
            {
                SqlParameter[] parms = new SqlParameter[3];
                var commentContentParam = new SqlParameter("UserID", userID);
                parms[0] = commentContentParam;
                SqlParameter commentTypeParameter = new SqlParameter("CommentType", commentType);
                parms[1] = commentTypeParameter;
                var relationIDParam = new SqlParameter("RelationID", relationID);
                parms[2] = relationIDParam;
                SqlDbHelper helper = new SqlDbHelper(ConfigurationManager.ConnectionStrings["SinooceanLandAddressList"].ConnectionString);
                return helper.ExecuteDataTable("dbo.CommentPointOfpraise", CommandType.StoredProcedure, parms);

            }
            catch (Exception e)
            {
                throw e;
            }
        }
        #endregion

        #region 获取点赞数
        /// <summary>
        /// 获取点赞数
        /// </summary>
        /// <param name="id"></param>
        /// <param name="userID"></param>
        /// <returns></returns>
        public DataTable GetArticlePointOfPraise(string id, string userID)
        {
            try
            {
                string sqlstr = string.Format("SELECT newid() as id,PointOfPraise,(SELECT Count (y.ID) FROM  Table_PointPraiseRecord AS y  WHERE  y.UserID = '{0}' AND y.RelationID = a.ID AND y.PointPraiseType = 1) AS TotalPointPraise FROM   dbo.Table_ArticlePointOfPraise AS a WHERE  id = {1}", userID, id);
                SqlDbHelper helper = new SqlDbHelper(ConfigurationManager.ConnectionStrings["SinooceanLandAddressList"].ConnectionString);
                return helper.ExecuteDataTable(sqlstr, CommandType.Text);
            }
            catch (Exception e)
            {
                throw e;
            }

        }
        #endregion

    }
}