using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Web;
using MCS.Library.SOA.DataObjects;
using Seagull2.YuanXin.AppApi.Models;
using Seagull2.YuanXin.AppApi.Models.AddressBook;

namespace Seagull2.YuanXin.AppApi.Adapter.AddressBook
{
    public class EmailTempleAdapter: UpdatableAndLoadableAdapterBase<EmailTempleModel, EmailTemplateCollection>
    {
        /// <summary>
        /// 实例
        /// </summary>
        public static readonly EmailTempleAdapter Instance = new EmailTempleAdapter();
        /// <summary>
        /// 数据库连接名称
        /// </summary>
        protected override string GetConnectionName() => Models.ConnectionNameDefine.YuanXinBusiness;



        /// <summary>
        /// 查询总记录数
        /// </summary>
        public int GetListByPage()
        {
            var sql = @"SELECT COUNT(*) FROM [office].[EmailTemple]";
            var helper = new SqlDbHelper();
            var result = helper.ExecuteScalar(sql, CommandType.Text);

            return Convert.ToInt32(result);
        }

        /// <summary>
        /// 查询当前页数据
        /// </summary>
        public EmailTemplateCollection GetListByPage(int pageIndex, int pageSize)
        {
            pageIndex--;
            var sql = @"select * from [office].[EmailTemple] order by createTime desc OFFSET "+ (pageSize * pageIndex) + " ROW FETCH NEXT "+ pageSize + " rows only"; 
            return this.QueryData(sql); 
        }



    }


}