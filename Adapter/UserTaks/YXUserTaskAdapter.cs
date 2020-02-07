using MCS.Library.Data.Builder;
using MCS.Library.SOA.DataObjects;
using Seagull2.YuanXin.AppApi.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Web;

namespace Seagull2.YuanXin.AppApi.Adapter.UserTaks
{
    public class YXUserTaskAdapter: UpdatableAndLoadableAdapterBase<UserTask,UserTaskCollection>
    {
        public static readonly YXUserTaskAdapter Instance = new YXUserTaskAdapter();
        protected  override string GetConnectionName()
        {
            return "YXWORKFLOW";
        }

        /// <summary>
        /// 更新读待办取时间
        /// </summary>
        public  string SetUserTaskReadFlag(string resourceId)
        {
            string IsResult = "";
            try
            {
                string sql = "UPDATE WF.USER_TASK SET READ_TIME = GETDATE() WHERE RESOURCE_ID = " + TSqlBuilder.Instance.CheckQuotationMark(resourceId, true);

                DbHelper.RunSql(sql, GetConnectionName());
                IsResult = "Yes";
                return IsResult;
            }
            catch (Exception)
            {
                throw;
            }

        }

        /// <summary>
        /// 查找代办
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public UserTask LoadUserTaskbyGUID(string id) {
           return  this.Load(p=> {
                p.AppendItem("TASK_GUID",id);
            }).FirstOrDefault();
        }

    }
}