using MCS.Library.SOA.DataObjects;
using Seagull2.YuanXin.AppApi.Models;
using SinoOcean.Seagull2.Framework.MasterData;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Threading.Tasks;
using System.Web;

namespace Seagull2.YuanXin.AppApi.Services
{
    public class PlanManageRate : UpdatableAndLoadableAdapterBase<EipKeyPointAchievingRateOfProjectEntity, EipKeyPointAchievingRateOfProjectEntityCollection>, IPlanManageRate
    {
        protected override string GetConnectionName()
        {
            return "SubjectDB_PlanManage";
        }

        public async Task<IEnumerable<EipKeyPointAchievingRateOfProjectEntity>> GetGroupYearEipKeyPointRateTable(string queryDate)
        {
            try
            {

                string startTime = "";

                if (queryDate != null && queryDate != "")
                {
                    int year = DateTime.Parse(queryDate).Year;
                    int month = DateTime.Parse(queryDate).Month;

                    if (year == DateTime.Now.Year && month == DateTime.Now.Month)
                    {
                        startTime = string.Format("{0}-{1}-01 00:00:00", year, month);
                    }
                    else
                    {
                        startTime = string.Format("{0}-{1}-01 00:00:00", year, month);

                    }
                }
                DataTable tempTable = new DataTable();
                tempTable.Columns.Add("CnName");
                tempTable.Columns.Add("Code");
                tempTable.Columns.Add("FirstKeyPoint");
                tempTable.Columns.Add("FirstComplete");
                tempTable.Columns.Add("SecondKeyPoint");
                tempTable.Columns.Add("SecondComplete");
                string CnName = "";
                string Code = "";
                string FirstKeyPoint = "";
                string FirstComplete = "";
                string SecondKeyPoint = "";
                string SecondComplete = "";

                string primarynode = "";
                string Secondnode = "";
                string KeyPoint = "";
                string name = "";
                string code = "";
                DateTime endTime = DateTime.Now.Date.AddMilliseconds(-1);


                if (startTime == "")
                {
                    startTime = DateTime.Now.AddMonths(-DateTime.Now.Month + 1).AddDays(-DateTime.Now.Day + 1).ToString("yyyy-MM-dd");
                }
                else
                {
                    DateTime newTime = Convert.ToDateTime(startTime);

                    if (newTime.Month != DateTime.Now.Month)
                    {
                        newTime = newTime.AddMonths(1);

                        endTime = newTime.Date.AddMilliseconds(-1);
                    }
                }

                string sqlStr = string.Format(@"select DISPLAY_NAME as CnName,GUID as Code,sum(FirstKeyPoint) as FirstKeyPoint,sum(FirstComplete) as FirstComplete,sum(SecondKeyPoint) as SecondKeyPoint,sum(SecondComplete)as SecondComplete from
                (SELECT tempOrganization.DISPLAY_NAME,tempOrganization.GUID,tempRate.EndTime,tempRate.RealEndTime,tempRate.KeyPointLevelCode, case when tempRate.KeyPointLevelCode = '1' then 1 else 0 end as FirstKeyPoint ,case when tempRate.KeyPointLevelCode = 1 and tempRate.RealEndTime is not null and tempRate.RealEndTime <= tempRate.EndTime then 1 else 0 end as FirstComplete, case when tempRate.KeyPointLevelCode = '2' THEN 1 ELSE 0 END as SecondKeyPoint,case when tempRate.KeyPointLevelCode = '2' and tempRate.RealEndTime is not null and tempRate.RealEndTime <= tempRate.EndTime then 1 else 0 end as SecondComplete FROM (select DISTINCT os.DISPLAY_NAME,os.GUID
                from SubjectDB.dbo.[Organizations_Syn] os 
                inner join SubjectDB.Common.[Corporation_OrganizationNode] cco on cco.ProjectManageCorporationCode = os.GUID  and cco.VersionEndTime is null and cco.ValidStatus = '1' 
                inner join SubjectDB.LandObtained.[Corporation_ProjectInfo] lcp on  lcp.VersionEndTime is null and lcp.ValidStatus = '1'
                inner join SubjectDB.PlanManage.[Plan] pp on pp.VersionEndTime is null and pp.ValidStatus = '1'
                inner join SubjectDB.PlanManage.[ProjectPlan] p ON  p.ProjectCode = lcp.ProjectInfoCode and p.VersionEndTime is null and p.ValidStatus = '1' WHERE os.DISPLAY_NAME LIKE'%开发事业%') AS
                tempOrganization LEFT JOIN (select  os.DISPLAY_NAME,os.GUID,t.EndTime,t.RealEndTime,ck.KeyPointLevelCode, case when ck.KeyPointLevelCode = '1' then 1 else 0 end as FirstKeyPoint ,case when ck.KeyPointLevelCode = 1 and t.RealEndTime is not null and t.RealEndTime <= t.EndTime then 1 else 0 end as FirstComplete, case when ck.KeyPointLevelCode = '2' THEN 1 ELSE 0 END as SecondKeyPoint,case when ck.KeyPointLevelCode = '2' and t.RealEndTime is not null and t.RealEndTime <= t.EndTime then 1 else 0 end as SecondComplete
                from SubjectDB.PlanManage.[ProjectPlanTask] t
                inner join SubjectDB.PlanManage.[ProjectPlan] p on t.PlanCode = p.Code and t.PlanVersionStartTime = p.VersionStartTime and p.VersionEndTime is null and p.ValidStatus = '1'
                inner join SubjectDB.PlanManage.[Plan] pp on t.PlanCode = pp.Code and t.PlanVersionStartTime = pp.VersionStartTime and pp.VersionEndTime is null and pp.ValidStatus = '1'
                inner join SubjectDB.LandObtained.[Corporation_ProjectInfo] lcp on p.ProjectCode = lcp.ProjectInfoCode and lcp.VersionEndTime is null and lcp.ValidStatus = '1'
                inner join SubjectDB.Common.[Corporation_OrganizationNode] cco on lcp.CorporationCode = cco.CorporationCode and cco.VersionEndTime is null and cco.ValidStatus = '1' 
                inner join SubjectDB.dbo.[Organizations_Syn] os on cco.ProjectManageCorporationCode = os.GUID
                inner join SubjectDB.[Common].[KeyPointInfo] ck on t.KeyPointValue = ck.Code
                where t.ValidStatus = '0' AND pp.PlanStatusCode IN (3,4,6) and t.VersionEndTime is null and t.EndTime >='{0}' and t.EndTime <='{1}')  AS tempRate ON tempRate.GUID=tempOrganization.GUID) TempQuery group by DISPLAY_NAME,GUID  order by DISPLAY_NAME COLLATE Chinese_PRC_Stroke_CS_AS_KS_WS", startTime, endTime);


                var ds = DbHelper.RunSqlReturnDS(sqlStr, GetConnectionName());


                foreach (DataRow item in ds.Tables[0].Rows)
                {

                    CnName = item["CnName"].ToString();
                    Code = item["Code"].ToString();
                    FirstKeyPoint = item["FirstKeyPoint"].ToString();
                    FirstComplete = item["FirstComplete"].ToString();
                    SecondKeyPoint = item["SecondKeyPoint"].ToString();
                    SecondComplete = item["SecondComplete"].ToString();
                    tempTable.Rows.Add(CnName, Code, FirstKeyPoint, FirstComplete, SecondKeyPoint, SecondComplete);

                }

                List<EipKeyPointAchievingRateOfProjectEntity> primary = new List<EipKeyPointAchievingRateOfProjectEntity>();


                foreach (DataRow item in tempTable.Rows)
                {
                    code = item["Code"].ToString();
                    name = item["CnName"].ToString();
                    FirstKeyPoint = item["FirstKeyPoint"].ToString();
                    FirstComplete = item["FirstComplete"].ToString();
                    SecondKeyPoint = item["SecondKeyPoint"].ToString();
                    SecondComplete = item["SecondComplete"].ToString();
                    if (Convert.ToInt32(item["FirstKeyPoint"].ToString()) != 0)
                    {
                        primarynode = (Convert.ToInt32(item["FirstComplete"].ToString()) * 1.0 / Convert.ToInt32(item["FirstKeyPoint"].ToString()) * 100).ToString("#00.00");
                    }
                    else
                    {
                        primarynode = "00.00";
                    }

                    if (Convert.ToInt32(item["SecondKeyPoint"].ToString()) != 0)
                    {
                        Secondnode = (Convert.ToInt32(item["SecondComplete"].ToString()) * 1.0 / Convert.ToInt32(item["SecondKeyPoint"].ToString()) * 100).ToString("#00.00");
                    }
                    else
                    {
                        Secondnode = "00.00";
                    }

                    if (Convert.ToInt32(item["FirstKeyPoint"].ToString()) * 2 + Convert.ToInt32(item["SecondKeyPoint"].ToString()) != 0)
                    {
                        KeyPoint = ((Convert.ToInt32(item["FirstComplete"].ToString()) * 2 + Convert.ToInt32(item["SecondComplete"].ToString())) * 1.0 / (Convert.ToInt32(item["FirstKeyPoint"].ToString()) * 2 + Convert.ToInt32(item["SecondKeyPoint"].ToString())) * 100).ToString("#00.00");
                    }
                    else
                    {
                        KeyPoint = "00.00";
                    }

                    primary.Add(new EipKeyPointAchievingRateOfProjectEntity
                    {
                        Name = name,
                        Code = code,
                        YearPrimaryNode = primarynode,
                        YearSeconedNode = Secondnode,
                        YearAllKeyPoint = KeyPoint,
                        PrimaryNode = FirstKeyPoint,
                        PrimaryComplete = FirstComplete,
                        SecondNode = SecondKeyPoint,
                        SecondComplete = SecondComplete
                    });
                }
                //List<EipKeyPointAchievingRateOfProjectEntity> primary = new List<EipKeyPointAchievingRateOfProjectEntity>();
                //primary.Add(new EipKeyPointAchievingRateOfProjectEntity
                //{
                //    Name = "weijh",
                //    Code = "12312312313",
                //    YearPrimaryNode = "11",
                //    YearSeconedNode = "22",
                //    YearAllKeyPoint = "00.00",
                //    PrimaryNode = "1",
                //    PrimaryComplete = "2",
                //    SecondNode = "3",
                //    SecondComplete = "4"
                //});
                return primary;
            }
            catch (Exception ex)
            {
                Log.WriteLog(ex.Message);
                Log.WriteLog(ex.StackTrace);
                throw ex;
            }

        }


        public async Task<IEnumerable<string>> WeiCeshi()
        {
            List<string> list = new List<string>();
            list.Add("WeiJinHuiCeShi");
            return list;
        }
    }
}