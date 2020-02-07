using MCS.Library.Data;
using MCS.Library.SOA.DataObjects;
using Seagull2.YuanXin.AppApi.Models;
using Seagull2.YuanXin.AppApi.Models.AddressBook;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Web;

namespace Seagull2.YuanXin.AppApi.Adapter.AddressBook
{
    /// <summary>
    /// 远洋通讯录 Adapter
    /// </summary>
    public class ContactsAdapter : UpdatableAndLoadableAdapterBase<ContactsModel, ContactsCollection>
    {
        /// <summary>
        /// 实例
        /// </summary>
        public static readonly ContactsAdapter Instance = new ContactsAdapter();

        /// <summary>
        /// 数据库连接字符串
        /// </summary>
        protected string ConnectionString = DbConnectionManager.GetConnectionString(ConnectionNameDefine.EmployeeAttendance);

        /// <summary>
        /// 数据库连接
        /// </summary>
        protected override string GetConnectionName()
        {
            return ConnectionNameDefine.EmployeeAttendance;
        }

        /// <summary>
        /// 搜索前N条数据
        /// </summary>
        public ContactsCollection SearchTopN(int count, string keyword)
        {
            var sql = $@"
                SELECT TOP {count} * FROM [dbo].[Contacts]
                WHERE
	                [FullPath] LIKE '机构人员\远洋集团%' AND (
	                [LOGON_NAME] LIKE '%{keyword}%' OR
	                [DisplayName] LIKE '%{keyword}%' OR
	                [Mail] LIKE '%{keyword}%' OR
	                [WP] LIKE '%{keyword}%' OR 
	                [MP] LIKE '%{keyword}%');";
            return QueryData(sql);
        }

        /// <summary>
        /// 获取通讯第一级、第二级部门列表
        /// </summary>
        public ContactsCollection GetDepartList()
        {
            var sql = @"
                SELECT * FROM [dbo].[Contacts]
                WHERE
	                [FullPath] LIKE '机构人员\远洋集团%' AND 
	                [SchemaType] = 'Organizations' AND 
	                (LEN([GlobalSort]) / 6 = 3 OR LEN([GlobalSort]) / 6 = 4)
                ORDER BY [GlobalSort] ASC;";
            return QueryData(sql);
        }

        /// <summary>
        /// 根据组织编码获取数据列表
        /// </summary>
        public DataTable GetListByOrganizationCode(string organizationCode)
        {
            var sql = @"SELECT *, RIGHT([FullPath], CHARINDEX('\', REVERSE([FullPath])) - 1) AS [Name] FROM [dbo].[Contacts] WHERE [ParentID] = '{0}' ORDER BY [InnerSort];";

            sql = string.Format(sql, organizationCode);

            var helper = new SqlDbHelper(ConnectionString);
            return helper.ExecuteDataTable(sql);
        }

        /// <summary>
        /// 获取组织下人员总数（不包括子级组织、不包括非主职人员）
        /// </summary>
        public int GetUserCountByPId(string organizationCode)
        {
            var sql = @"SELECT COUNT(*) FROM [dbo].[Contacts] WHERE [SchemaType] = 'Users' AND [IsDefault] = 1 AND [ParentID] = '{0}';";

            sql = string.Format(sql, organizationCode);

            var helper = new SqlDbHelper(ConnectionString);
            var result = helper.ExecuteScalar(sql);

            return Convert.ToInt32(result);
        }

        /// <summary>
        /// 获取组织下人员总数（不包括子级组织）
        /// </summary>
        public int GetUserCountByPId1(string organizationCode)
        {
            var sql = @"SELECT COUNT(*) FROM [dbo].[Contacts] WHERE [SchemaType] = 'Users' AND [ParentID] = '{0}';";

            sql = string.Format(sql, organizationCode);

            var helper = new SqlDbHelper(ConnectionString);
            var result = helper.ExecuteScalar(sql);

            return Convert.ToInt32(result);
        }

        /// <summary>
        /// 获取组织下人员总数（包括子级组织）
        /// </summary>
        public int GetUserCountByPName(string organizationFullPath)
        {
            var sql = @"SELECT COUNT(*) FROM [dbo].[Contacts] WHERE [SchemaType] = 'Users' AND [FullPath] LIKE '{0}%';";

            sql = string.Format(sql, organizationFullPath);

            var helper = new SqlDbHelper(ConnectionString);
            var result = helper.ExecuteScalar(sql);

            return Convert.ToInt32(result);
        }

        /// <summary>
        /// 查询所有组织编码和名称列表
        /// </summary>
        public List<object> GetAllOrganizations()
        {
            var sql = "SELECT [ObjectID], [DisplayName] FROM [dbo].[Contacts] WHERE [SchemaType] = 'Organizations' AND [FullPath] LIKE '机构人员\\远洋集团%' ORDER BY [GlobalSort] ASC;";

            var helper = new SqlDbHelper(ConnectionString);
            var dt = helper.ExecuteDataTable(sql);

            var list = new List<object>();
            foreach (DataRow dr in dt.Rows)
            {
                list.Add(new { Code = dr["ObjectID"], Name = dr["DisplayName"] });
            }
            return list;
        }

        /// <summary>
        /// 根据编码列表获取数据
        /// </summary>
        public ContactsCollection LoadListByUserCodeList(List<string> codes)
        {
            if (codes.Count > 0)
            {
                return Load(w =>
                {
                    codes.ForEach(code =>
                    {
                        w.AppendItem("ObjectID", code);
                    });
                    w.LogicOperator = MCS.Library.Data.Builder.LogicOperatorDefine.Or;
                }, o =>
                {
                    o.AppendItem("GlobalSort", MCS.Library.Data.Builder.FieldSortDirection.Ascending);
                });
            }
            else
            {
                return new ContactsCollection();
            }
        }

        /// <summary>
        /// 根据域帐号列表获取数据
        /// </summary>
        public ContactsCollection LoadListByLogonNameList(List<string> logonNames)
        {
            if (logonNames.Count > 0)
            {
                return Load(w =>
                {
                    logonNames.ForEach(logonName =>
                    {
                        w.AppendItem("LOGON_NAME", logonName);
                    });
                    w.LogicOperator = MCS.Library.Data.Builder.LogicOperatorDefine.Or;
                }, o =>
                {
                    o.AppendItem("GlobalSort", MCS.Library.Data.Builder.FieldSortDirection.Ascending);
                });
            }
            else
            {
                return new ContactsCollection();
            }
        }

        /// <summary>
        /// 获取子级组织
        /// </summary>
        public ContactsCollection LoadOrganizations(string parentCode)
        {
            return Load(
                w =>
                {
                    w.AppendItem("ParentID", parentCode);
                    w.AppendItem("SchemaType", "Organizations");
                },
                o =>
                {
                    o.AppendItem("GlobalSort", MCS.Library.Data.Builder.FieldSortDirection.Ascending);
                });
        }

        /// <summary>
        /// 获取子级人员
        /// </summary>
        public ContactsCollection LoadUsers(string parentCode)
        {
            return Load(
                w =>
                {
                    w.AppendItem("ParentID", parentCode);
                    w.AppendItem("SchemaType", "Users");
                },
                o =>
                {
                    o.AppendItem("GlobalSort", MCS.Library.Data.Builder.FieldSortDirection.Ascending);
                });
        }

        /// <summary>
        /// 递归查询组织机构下的所有组织机构和人员
        /// </summary>
        public ContactsCollection LoadChildren(string code)
        {
            var sql = @"
                WITH [Temp] AS
                (
	                SELECT [A].* FROM [dbo].[Contacts] A WHERE [A].[ObjectID] = '{0}' AND [A].[IsDefault] = 1
	                UNION ALL
	                SELECT [A].* FROM [dbo].[Contacts] A INNER JOIN [Temp] B ON A.[ParentID] = B.[ObjectID] AND [A].[IsDefault] = 1
                )
                SELECT * FROM [Temp] ORDER BY [Temp].[GlobalSort] ASC;";
            sql = string.Format(sql, code);

            return QueryData(sql);
        }
        /// <summary>
        /// 递归查询某一个部门下的所有人员  消息推送使用
        /// </summary>
        public ContactsCollection LoadChildrenUsers(string FullPath)
        {
            string sql = $"SELECT ObjectID,DisplayName FROM [dbo].[Contacts] WHERE [SchemaType] = 'Users' AND[IsDefault] = 1  AND[FullPath] LIKE '{FullPath}%' GROUP BY ObjectID,DisplayName";
            return QueryData(sql);
        }

        /// <summary>
        /// 获取所有父级路径列表
        /// </summary>
        public ContactsCollection LoadPathListForParent(string code)
        {
            var sql = @"
                WITH [Temp] AS
                (
	                SELECT * FROM [dbo].[Contacts] WHERE [ObjectID] = '{0}' AND [IsDefault] = 1
	                UNION ALL
	                SELECT [A].* FROM [dbo].[Contacts] A, [Temp] B WHERE A.[ObjectID] = B.[ParentID] AND [A].[IsDefault] = 1
                )
                SELECT * FROM [Temp] ORDER BY [Temp].[GlobalSort] ASC;";
            sql = string.Format(sql, code);

            return QueryData(sql);
        }

        /// <summary>
        /// 根据部门编码获取所有父级部门列表
        /// </summary>
        public ContactsCollection GetParentDepartListByDepartCode(string departCode)
        {
            var sql = @"
                WITH [Temp] AS
                (
	                SELECT * FROM [dbo].[Contacts] WHERE [ObjectID] = '{0}'
	                UNION ALL
	                SELECT [A].* FROM [dbo].[Contacts] A, [Temp] B WHERE A.[ObjectID] = B.[ParentID]
                )
                SELECT * FROM [Temp] ORDER BY [Temp].[GlobalSort] ASC;";
            sql = string.Format(sql, departCode);

            return QueryData(sql);
        }

        /// <summary>
        /// 根据邮箱查找数据
        /// </summary>
        public ContactsModel LoadByMail(string mail)
        {
            return this.Load(p =>
            {
                p.AppendItem("Mail", mail).AppendItem("IsDefault", 1);
            }).FirstOrDefault();
        }

        /// <summary>
        /// 根据登录名查找
        /// </summary>
        public ContactsModel LoadByLoginName(string loginName)
        {
            return this.Load(p =>
            {
                p.AppendItem("LOGON_NAME", loginName);
            }).FirstOrDefault();
        }

        /// <summary>
        /// 根据编码查找数据（组织或人员）
        /// </summary>
        public ContactsModel LoadByCode(string code)
        {
            return this.Load(p =>
            {
                p.AppendItem("ObjectID", code);
            }).FirstOrDefault();
        }

        /// <summary>
        /// 查询ObjectID查询列表
        /// </summary>
        public ContactsCollection LoadCollectionByObjectId(string objectId)
        {
            return this.Load(w => { w.AppendItem("ObjectID", objectId); }, o => { o.AppendItem("GlobalSort", MCS.Library.Data.Builder.FieldSortDirection.Ascending); });
        }

        /// <summary>
        /// 根据编码查找人员
        /// </summary>
        public ContactsModel LoadUserByCode(string code)
        {
            return this.Load(p =>
            {
                p.AppendItem("ObjectID", code);
                p.AppendItem("SchemaType", "Users");
                p.AppendItem("IsDefault", 1);
            }).FirstOrDefault();
        }

        /// <summary>
        /// 根据关键字查找用户
        /// </summary>
        public ContactsCollection LoadUserByKeyword(string keyword)
        {
            return QueryData($"SELECT * FROM [dbo].[Contacts] WHERE [SchemaType] = 'Users' AND ([DisplayName] = '{keyword}' OR [LOGON_NAME] = '{keyword}')");
        }

        /// <summary>
        /// 获取子级成员
        /// </summary>
        public ContactsCollection LoadChild(string parentCode)
        {
            return Load(
                w =>
                {
                    w.AppendItem("ParentID", parentCode);
                },
                o =>
                {
                    o.AppendItem("InnerSort", MCS.Library.Data.Builder.FieldSortDirection.Ascending);
                });
        }
    }
}