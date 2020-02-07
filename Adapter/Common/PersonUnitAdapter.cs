using MCS.Library.Data;
using MCS.Library.SOA.DataObjects;
using Newtonsoft.Json.Linq;
using Seagull2.YuanXin.AppApi.Models;
using Seagull2.YuanXin.AppApi.Models.Common;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;

namespace Seagull2.YuanXin.AppApi.Adapter.Common
{
    /// <summary>
    /// 人员管理单元适配器
    /// </summary>
    public class PersonUnitAdapter : UpdatableAndLoadableAdapterBase<PersonUnitModel, PersonUnitModelCollection>
    {
        /// <summary>
        /// 实例
        /// </summary>
        public static readonly PersonUnitAdapter Instance = new PersonUnitAdapter();

        /// <summary>
        /// 获取链接
        /// </summary>
        /// <returns></returns>
        protected override string GetConnectionName()
        {
            return ConnectionNameDefine.YuanXinBusiness;
        }


        /// <summary>
        /// 添加人员单元数据
        /// </summary>
        /// <param name="list"></param>
        /// <param name="relation"></param>
        /// <param name="currentUser"></param>
        /// <param name="module"></param>
        public void UpdatePersionUnit(List<object> list, string relation, string currentUser,int module)
        {
            this.Delete(d => d.AppendItem("RelationCode", relation));
            list.ForEach(f =>
            {
                this.Update(new PersonUnitModel()
                {
                    Code = Guid.NewGuid().ToString(),
                    CreateTime = DateTime.Now,
                    Creator = currentUser,
                    ValidStatus = true,
                    RelationCode = relation,
                    ID = (f as JObject)["id"].ToString(),
                    DisplayName = (f as JObject)["displayName"].ToString(),
                    ObjectType = (f as JObject)["objectType"].ToString(),
                });
            });
            this.UpdatePserson(relation, module);
        }


        /// <summary>
        /// 更新权限人员
        /// </summary>
        public void UpdatePserson(string relation,int module)
        {
            try
            {
                SqlParameter[] paramerers = { new SqlParameter("@RelationCode", SqlDbType.NVarChar, 36),
                new SqlParameter("@Module",module)};
                paramerers[0].Value = relation;
                paramerers[1].Value = module;
                var helper = new SqlDbHelper();
                helper.ExecuteNonQuery("[dbo].[ProcUnitToUser]", CommandType.StoredProcedure, paramerers);
            }
            catch (Exception e)
            {
                Log.WriteLog($"更新权限管理单元人员 - 失败：{e.Message}");
            }
        } 
    }


    /// <summary>
    /// 人员权限模块适配器
    /// </summary>
    public class AppCommonPermissionAdapter : UpdatableAndLoadableAdapterBase<AppCommonPermissionModel, AppCommonPermissionModelCollection>
    {
        /// <summary>
        /// 实例
        /// </summary>
        public static readonly AppCommonPermissionAdapter Instance = new AppCommonPermissionAdapter();

        /// <summary>
        /// 获取链接
        /// </summary>
        /// <returns></returns>
        protected override string GetConnectionName()
        {
            return ConnectionNameDefine.YuanXinBusiness;
        }



        /// <summary>
        /// 查询所有分配过权限的模块
        /// </summary>
        /// <param name="module"></param>
        /// <returns></returns>
        public List<string> GetAllModule(int module)
        {
            string sql = $"SELECT RelationCode FROM  [Office].[AppCommonPermission] WHERE  Module={module} GROUP  BY RelationCode";
           return this.QueryData(sql).ToList().ConvertAll(c=>c.RelationCode);
        }
    }
}