using MCS.Library.SOA.DataObjects;
using Seagull2.YuanXin.AppApi.Models.Sys_Menu;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Web;

namespace Seagull2.YuanXin.AppApi.Adapter.Sys_Menu
{
    /// <summary>
    /// 用户表
    /// </summary>
    public class Sys_UserAdapter : UpdatableAndLoadableAdapterBase<Sys_UserModel, Sys_UserCollection>
    {

        /// <summary>
        /// 实例
        /// </summary>
        public static readonly Sys_UserAdapter Instance = new Sys_UserAdapter();
        /// <summary>
        /// 数据库连接名称
        /// </summary>
        protected override string GetConnectionName() => Models.ConnectionNameDefine.YuanXinBusiness;

        /// <summary>
        /// 根据用户编码获取权限信息
        /// </summary>
        public Sys_UserModel GetModelByUserCode(string userCode)
        {
            return Load(w => w.AppendItem("UserCode", userCode)).SingleOrDefault();
        }

        /// <summary>
        /// 获取超级管理员数量   总管  supver 1　valid 0   超管 supver 1  valid 1 分管 supver 0  valid 1
        /// </summary>
        public Sys_UserCollection GetSuperManagerCount()
        {
            return Load(w => w.AppendItem("Super", 1).AppendItem("ValidStatus",1));
        }

        /// 查询总记录数
        /// </summary>
        public int GetListByPage(int supers)
        {
            string whereSql = string.Empty;
            if (supers == -1)
            {

                var sql1 = @"SELECT COUNT(*) FROM [office].[Sys_User] ";



                var helper = new SqlDbHelper();
                var result = helper.ExecuteScalar(sql1, CommandType.Text);

                return Convert.ToInt32(result);
            }
            else
            {
                bool super = false;
                if (supers == 0)
                {
                    super = false;
                }
                else
                {
                    super = true;
                }

                var sql = @"SELECT COUNT(*) FROM [office].[Sys_User] WHERE [Super]=@Super";

                SqlParameter[] parameters = { new SqlParameter("@Super", SqlDbType.NVarChar, 36) };
                parameters[0].Value = super;

                var helper = new SqlDbHelper();
                var result = helper.ExecuteScalar(sql, CommandType.Text, parameters);

                return Convert.ToInt32(result);
            }



        }
        /// 查询总记录数
        /// </summary>
        public DataTable GetListBySearch(int supers, string searchText,string menuCode)
        {
            string whereSql = string.Empty;
          
                bool super = false;
                if (supers == 0)
                {
                    super = false;
                }
                else
                {
                    super = true;
                }

              var sql = @"SELECT * FROM [office].[Sys_User] WHERE [Super]=@Super and ( [UserName] LIKE '%'+@searchText+'%'OR [Account] LIKE '%'+@searchText+'%')
                        AND [UserCode] IN (SELECT [UserCode] FROM [office].[Sys_UserMenu] WHERE  [MenuCode]=@menuCode)";
              

            SqlParameter[] parameters = { new SqlParameter("@Super", SqlDbType.NVarChar, 36),
                new SqlParameter("@searchText", SqlDbType.NVarChar, 36),
                new SqlParameter("@menuCode", SqlDbType.NVarChar, 36)            };

                parameters[0].Value = super;
             parameters[1].Value = searchText;
            parameters[2].Value = menuCode;
            var helper = new SqlDbHelper();
                var result = helper.ExecuteDataTable(sql, CommandType.Text, parameters);

            return result;





        }

        /// <summary>
        /// 查询当前页数据
        /// </summary>
        public DataTable GetListByPage(int pageIndex, int pageSize, int supers)
        {
            if (supers == -1)
            {
                pageIndex--;
                var sql = @"
                    WITH [Temp] AS
                    (
                        SELECT ROW_NUMBER() OVER(ORDER BY [CREATETIME] DESC) AS [Row], * FROM [office].[Sys_User] 
                    )
                    SELECT * FROM [Temp] WHERE [Temp].[Row] BETWEEN @PageIndex AND @PageSize;";

                SqlParameter[] parameters = {
                    new SqlParameter("@PageIndex", SqlDbType.Int, 4),
                    new SqlParameter("@PageSize", SqlDbType.Int, 4),
                };
                parameters[0].Value = pageSize * pageIndex + 1;
                parameters[1].Value = pageSize * pageIndex + pageSize;

                var helper = new SqlDbHelper();
                return helper.ExecuteDataTable(sql, CommandType.Text, parameters);
            }
            else
            {
                bool super = false;
                if (supers == 0)
                {
                    super = false;
                }
                else
                {
                    super = true;
                }

                pageIndex--;
                var sql = @"
                    WITH [Temp] AS
                    (
                        SELECT ROW_NUMBER() OVER(ORDER BY [CREATETIME] DESC) AS [Row], * FROM [office].[Sys_User] WHERE [Super]=@Super
                    )
                    SELECT * FROM [Temp] WHERE [Temp].[Row] BETWEEN @PageIndex AND @PageSize;";

                SqlParameter[] parameters = {
                    new SqlParameter("@PageIndex", SqlDbType.Int, 4),
                    new SqlParameter("@PageSize", SqlDbType.Int, 4),
                    new SqlParameter("@Super", SqlDbType.NVarChar, 36)
                };
                parameters[0].Value = pageSize * pageIndex + 1;
                parameters[1].Value = pageSize * pageIndex + pageSize;
                parameters[2].Value = super;

                var helper = new SqlDbHelper();
                return helper.ExecuteDataTable(sql, CommandType.Text, parameters);
            }
        }
    }
}