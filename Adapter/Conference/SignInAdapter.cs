using MCS.Library.Data.Builder;
using MCS.Library.Data.Mapping;
using MCS.Library.SOA.DataObjects;
using Seagull2.YuanXin.AppApi.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Web;

namespace Seagull2.YuanXin.AppApi.Adapter.Conference
{

    /// <summary>
    /// 签到
    /// </summary>
    public class SignInAdapter : UpdatableAndLoadableAdapterBase<SignInModel, SignInModelCollection>
    {
        public static readonly SignInAdapter Instance = new SignInAdapter();
        private SignInAdapter() { }

        protected override string GetConnectionName()
        {
            return ConnectionNameDefine.YuanXinBusiness;
        }

        /// <summary>
        /// 根据用户编码 获取签到详情
        /// </summary>
        /// <param name="Usercode"></param>
        /// <returns></returns>
        public SignInModel LoadByCode(string Usercode, string AgendaID)
        {
            return this.Load(p =>
            {
                p.AppendItem("AttendeeID", Usercode, "=");
                p.AppendItem("AgendaID", AgendaID, "=");
            }).FirstOrDefault();
        }

        /// <summary>
        /// 更新签到
        /// </summary>
        /// <param name="Usercode"></param>
        /// <returns></returns>
        public bool AddSignIn(SignInModel signInModel)
        {
            var mapping = ORMapping.GetMappingInfo<SignInModel>();

            StringBuilder strB = new StringBuilder(200);
            strB.Append(TSqlBuilder.Instance.DBStatementSeperator);
            strB.Append(ORMapping.GetInsertSql(signInModel, TSqlBuilder.Instance, ""));
            return DbHelper.RunSqlWithTransaction(strB.ToString(), GetConnectionName()) > 0 ? true : false;
        }

        /// <summary>
        /// 更新签到
        /// </summary>
        /// <param name="agendaID">议程编码</param>
        /// <returns></returns>
        public int SignInCount(string agendaID)
        {
            string sql = string.Format(@"select count(*) from Office.SignDetail where AgendaID='{0}'", agendaID);
            return (int)DbHelper.RunSqlReturnScalar(sql, GetConnectionName());
        }
         

        public object del()
        {
            string sql = "DELETE from Office.SignDetail ";
            SqlDbHelper dbHelper = new SqlDbHelper();
            return dbHelper.ExecuteScalar(sql);
        }


        /// <summary>
        ///根据ID删除签到信息
        /// </summary>
        /// <param name="ID"></param>
        /// <param name="builder"></param>
        /// <returns></returns>
        public bool DelUserSignIn(string ID)
        {
            var mapping = ORMapping.GetMappingInfo<SignInModel>();
            StringBuilder strB = new StringBuilder(200);
            strB.Append(string.Format("DELETE FROM {0} WHERE ID={1}", mapping.TableName, ID));

            return DbHelper.RunSqlWithTransaction(strB.ToString(), GetConnectionName()) > 0 ? true : false;
        }
    }
}