using MCS.Library.Data.Builder;
using MCS.Library.SOA.DataObjects;
using MCS.Library.SOA.DataObjects.Workflow;
using System;
using System.Collections.Generic;
using System.Data;
using System.Threading.Tasks;
using System.Web;
using System.Linq;
using MCS.Library.Core;





namespace Seagull2.YuanXin.AppApi.Services
{
    public class UserSettingsService :IUserSettingsService
    {
        private static string GetConnectionName()
        {
            return WfRuntime.ProcessContext.SimulationContext.GetConnectionName(WorkflowSettings.GetConfig().ConnectionName);
            // return "";
        }

        public async Task<IEnumerable<UserSettings>> LoadA(string deliverTime = "")
        {
            UserSettings userSettings = new UserSettings();

            int taskPageSize = userSettings.GetUserTaskPageSize();

            WhereSqlClauseBuilder builder = new WhereSqlClauseBuilder();

            builder.AppendItem("SEND_TO_USER", "");

            if (!deliverTime.IsNullOrEmpty())
            {
                builder.AppendItem("U.DELIVER_TIME", deliverTime, "<");
            }

            string sql = string.Format(@"SELECT TOP {0} U.TASK_TITLE,U.RESOURCE_ID,U.ACTIVITY_ID,U.READ_TIME,TASK_GUID,U.DELIVER_TIME,U.URL " +
                                        "FROM WF.USER_TASK(NOLOCK) U LEFT JOIN WF.PROCESS_RELATIVE_PARAMS(NOLOCK) P ON U.PROCESS_ID = P.PROCESS_ID AND P.PARAM_KEY = 'ProjectName' " +
                                        "WHERE {1} AND (((STATUS = N'2') AND ((EXPIRE_TIME is null) OR (EXPIRE_TIME is not null AND EXPIRE_TIME > getdate()))) OR (STATUS = N'1')) " +
                                        "ORDER BY TOP_FLAG DESC,DELIVER_TIME DESC",
                                        taskPageSize, builder.ToSqlString(TSqlBuilder.Instance));

            DataTable table = DbHelper.RunSqlReturnDS(sql, GetConnectionName()).Tables[0];

            var result = new List<UserSettings>();

            foreach (DataRow item in table.Rows)
            {
                result.Add(ToUserSettings(item));
            }
            //UserTaskCollection result = new UserTaskCollection();

            //ORMapping.DataViewToCollection(result, table.DefaultView);

            return result;
        }


        private UserSettings ToUserSettings(DataRow dataRow)
        {
            return new UserSettings()
            {
                 //Categories =(string)dataRow["TaskTitle"]
                //TaskTitle = (string)dataRow["TaskTitle"],
                //ResourceID = (string)dataRow["RESOURCE_ID"],
                //ActivityID = (string)dataRow["ACTIVITY_ID"],
                //ReadTime = (DateTime)dataRow["READ_TIME"],
                //TaskID = (string)dataRow["TASK_GUID"],
                //DeliverTime = (DateTime)dataRow["DELIVER_TIME"],
                //Url = (string)dataRow["URL"]
            };
        }
    }
}
