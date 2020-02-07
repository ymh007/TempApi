using MCS.Library.SOA.DataObjects;
using Seagull2.YuanXin.AppApi.Models.AddressBook;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Web;
using static Seagull2.YuanXin.AppApi.Models.AddressBook.InvitedRecordModel;

namespace Seagull2.YuanXin.AppApi.Adapter.AddressBook
{
    public class InvitedRecordAdapter : UpdatableAndLoadableAdapterBase<InvitedRecordModel, InvitedRecordCollection>
    {
        /// <summary>
        /// 实例化对象
        /// </summary>
        public static readonly InvitedRecordAdapter Instance = new InvitedRecordAdapter();

        /// <summary>
        /// 创建连接
        /// </summary>
        /// <returns></returns>
        protected override string GetConnectionName() => Models.ConnectionNameDefine.YuanXinBusiness;

        /// <summary>
        /// 查询当前页数据
        /// </summary>
        public DataTable GetListByPage(int pageIndex, int pageSize, string code)
        {
            pageIndex--;
            var sql = @"select * from [dbo].[InvitedRecord] where RecipientCode = @code order by [sendTime] desc OFFSET @pageIndex ROW FETCH NEXT @pageSize rows only";

            SqlParameter[] parameters = {
                new SqlParameter("@PageIndex", SqlDbType.Int),
                new SqlParameter("@PageSize", SqlDbType.Int),
                new SqlParameter("@Code", SqlDbType.VarChar),
            };
            parameters[0].Value = pageSize * pageIndex;
            parameters[1].Value = pageSize;
            parameters[2].Value = code;

            var helper = new SqlDbHelper();
            return helper.ExecuteDataTable(sql, CommandType.Text, parameters);
        }

        /// <summary>
        /// 查询总记录数
        /// </summary>
        public int GetListByPage(string code)
        {
            var sql = @"SELECT COUNT(*) FROM [dbo].[InvitedRecord] WHERE RecipientCode = @code";
            SqlParameter[] parameters = {
                new SqlParameter("@Code", SqlDbType.VarChar),
            };

            parameters[0].Value = code;
            var helper = new SqlDbHelper();
            var result = helper.ExecuteScalar(sql, CommandType.Text,parameters);

            return Convert.ToInt32(result);
        }

    }


}