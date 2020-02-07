using Newtonsoft.Json.Linq;
using Seagull2.YuanXin.AppApi.Adapter.AddressBook;
using Seagull2.YuanXin.AppApi.Controllers;
using Seagull2.YuanXin.AppApi.Models.AddressBook;
using Seagull2.YuanXin.AppApi.ZhongChouData.Models;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Reflection;

namespace Seagull2.YuanXin.AppApi.Models
{
    public class ActivityHelper : DefaultHelper
    {
        private LogHelper log = LogFactory.GetLogger(MethodBase.GetCurrentMethod().DeclaringType);

        //计算活动参赛作品的数量
        protected int countActivityWorks(string activityCode)
        {
            string sqlStr = "SELECT COUNT(*) FROM " + tableNamePrefix_ZC + "ActivityWorks"
                        + " WHERE ProjectCode = '" + activityCode + "'";
            SqlDbHelper db = new SqlDbHelper();

            return (int)db.ExecuteScalar(sqlStr);

        }


        //存储中奖结果到WorkAwards表
        protected bool savePrizeResult(ArrayList prizeResult)
        {
            SqlDbHelper db = new SqlDbHelper();
            List<String> sqlArray = new List<String>();
            List<SqlParameter[]> sqlParmetersArray = new List<SqlParameter[]>();
            Boolean result;

            try
            {
                string sqlStr = "INSERT " + DefaultHelper.tableNamePrefix_ZC + "WorksAwards (Code, ActivityWorksCode, AwardsSettingCode, ProjectCode)"
                       + " values(@Code, @ActivityWorksCode, @AwardsSettingCode, @ProjectCode)";

                for (int i = 0; i < prizeResult.Count; i++)
                {
                    List<WorkAwardsBean> workAwardsList = (List<WorkAwardsBean>)prizeResult[i];

                    foreach (WorkAwardsBean workAwards in workAwardsList)
                    {
                        sqlArray.Add(sqlStr);

                        SqlParameter[] paramters = new SqlParameter[]{
                        new SqlParameter("@Code", workAwards.Code),
                        new SqlParameter("@ActivityWorksCode",workAwards.ActivityWorksCode),
                        new SqlParameter("@AwardsSettingCode", workAwards.AwardsSettingCode),
                        new SqlParameter("@ProjectCode", workAwards.ProjectCode),
                    };
                        sqlParmetersArray.Add(paramters);
                    }

                }

                result = db.ExecuteNonQueryTransation(sqlArray, CommandType.Text, sqlParmetersArray);
            }
            catch (Exception ex)
            {
                log.Error("Online Activity save failed! -- ", ex);
                return false;
            }

            return result;

        }


        //搜索线上活动作品的通用方法
        protected JObject SearchOnlineWorks(string activityCode, ActivityController.QUERY_TYPE queryType, string queryValue)
        {
            JObject result = new JObject();
            string sqlStr = null;
            DefaultPageable page;

            if (queryType == ActivityController.QUERY_TYPE.ByPhone)
            {
                //查询语句
                /** FOR Business.Attachment
                sqlStr = "SELECT A.Code, VoteCount ,A.Creator, A.CreateTime, A.WorksName, B.UserName, B.PhoneNumber, C.Url"
                + " FROM YuanXinBusiness.zc.ActivityWorks AS A"
                + " JOIN YuanXinBusiness.OAuth.UserInfo AS B on A.Creator = B.UserID"
                + " , YuanXinBusiness.business.Attachment AS C"
                + " WHERE A.ProjectCode = '" + activityCode + "'"
                + " AND B.PhoneNumber = '" + queryValue + "'"
                + " AND C.ResourceID = A.Code AND C.SortNo = '1' ";
                 */

                //for zc.ImageLib
                sqlStr = "SELECT Distinct(A.Code), VoteCount ,A.Creator, A.CreateTime, A.Content, sea.DisplayName AS RealName,sea.Phone AS Phone, C.Url"
                + " FROM YuanXinBusiness.zc.ActivityWorks AS A"
                + "LEFT JOIN YuanXinBusiness.OAuth.Seagull2 AS sea ON sea.UserId=A.Creator"
                + " LEFT JOIN YuanXinBusiness.Business.Attachment AS C ON C.ResourceID = A.Code"
                + " WHERE A.ProjectCode = '" + activityCode + "'"
                + " AND sea.Phone = '" + queryValue + "'";
            }
            else if (queryType == ActivityController.QUERY_TYPE.ByName)
            {
                /** FOR Business.Attachment
                sqlStr = "SELECT A.Code, VoteCount ,A.Creator, A.CreateTime, A.WorksName, B.UserName, B.PhoneNumber, C.Url"
                + " FROM YuanXinBusiness.zc.ActivityWorks AS A"
                + " JOIN YuanXinBusiness.OAuth.UserInfo AS B on A.Creator = B.UserID"
                + " , YuanXinBusiness.business.Attachment AS C"
                + " WHERE A.ProjectCode = '" + activityCode + "'"
                + " AND B.UserName = '" + queryValue + "'"
                + " AND C.ResourceID = A.Code AND C.SortNo = '1' ";
                **/

                //for zc.ImageLib
                sqlStr = "SELECT Distinct(A.Code), VoteCount ,A.Creator, A.CreateTime, A.Content,sea.DisplayName AS RealName,sea.Phone AS Phone, C.Url"
                + " FROM YuanXinBusiness.zc.ActivityWorks AS A"
                + " LEFT JOIN YuanXinBusiness.OAuth.Seagull2 AS sea ON sea.UserId=A.Creator"
                + " LEFT JOIN YuanXinBusiness.Business.Attachment AS C ON C.ResourceID = A.Code"
                + " WHERE A.ProjectCode = '" + activityCode + "'"
                + " AND sea.DisplayName = '" + queryValue + "'";
            }
            else
            {
                return result;
            }

            string sessionId = queryByPage(sqlStr);
            if (sessionId == null)
            {
                return result;
            }

            page = queryBySession(sessionId, 0);

            //OnlineOrderList 生成
            result = createOnlineWorkList(page);
            result.Add("sessionId", sessionId);

            return result;
        }


        //生成线上活动的作品信息
        protected JObject createOnlineWorkList(DefaultPageable page)
        {
            JArray worksList = new JArray();
            JObject result = new JObject();

            for (int i = 0; i < page.dataTable.Rows.Count; i++)
            {
                ActivityWorksInfo worksJson = new ActivityWorksInfo();
                worksJson.worksCode = (string)page.dataTable.Rows[i]["Code"];
                //若作品名称为Null时，则不进行赋值
                if (page.dataTable.Rows[i]["Content"] != DBNull.Value)
                {
                    worksJson.worksContent = (string)page.dataTable.Rows[i]["Content"];
                }

                worksJson.createTime = ((DateTime)page.dataTable.Rows[i]["CreateTime"]).ToString(DateFormat);
                worksJson.creator = (string)page.dataTable.Rows[i]["RealName"];
                ContactsModel user = ContactsAdapter.Instance.LoadByCode(worksJson.creator);
                worksJson.phone = user == null ? "" : user.MP;
                worksJson.voteCount = (int)page.dataTable.Rows[i]["VoteCount"];
                worksJson.worksImg = page.dataTable.Rows[i]["Url"] == DBNull.Value ? "" : (string)page.dataTable.Rows[i]["Url"];
                if (!isHttp((string)worksJson.worksImg))
                {
                    worksJson.worksImg = ActivityController.worksImagePath_Test + worksJson.worksImg;
                }

                worksList.Add((JObject)JToken.FromObject(worksJson));
            }

            result.Add("pageNo", page.cursor);
            result.Add("pageSum", page.sum);
            result.Add("worksList", worksList);

            return result;
        }

        protected JObject createOnlineWorkListOnline(DefaultPageable page, short pageNo, short pageSize, out short pageCount)
        {
            JArray worksList = new JArray();
            JObject result = new JObject();


            List<ActivityWorksInfo> list1 = new List<ActivityWorksInfo>();
            List<ActivityWorksInfo> list = new List<ActivityWorksInfo>();

            for (int i = 0; i < page.dataTable.Rows.Count; i++)
            {
                ActivityWorksInfo worksJson = new ActivityWorksInfo();
                worksJson.worksCode = (string)page.dataTable.Rows[i]["Code"];
                //若作品名称为Null时，则不进行赋值
                if (page.dataTable.Rows[i]["Content"] != DBNull.Value)
                {
                    worksJson.worksContent = (string)page.dataTable.Rows[i]["Content"];
                }

                worksJson.createTime = ((DateTime)page.dataTable.Rows[i]["CreateTime"]).ToString(DateFormat);
                worksJson.creator = (string)page.dataTable.Rows[i]["RealName"];
                ContactsModel user = ContactsAdapter.Instance.LoadByCode(worksJson.creator);
                worksJson.phone = user == null ? "" : user.MP;
                worksJson.voteCount = (int)page.dataTable.Rows[i]["VoteCount"];
                worksJson.worksImg = page.dataTable.Rows[i]["Url"] == DBNull.Value ? "" : (string)page.dataTable.Rows[i]["Url"];
                if (!isHttp((string)worksJson.worksImg))
                {
                    worksJson.worksImg = ActivityController.worksImagePath_Test + worksJson.worksImg;
                }

                //worksList.Add((JObject)JToken.FromObject(worksJson));
                list.Add(worksJson);
            }
            list.ForEach(one =>
            {
                if (!list1.Exists(l => l.worksCode == one.worksCode))
                {
                    one.worksImgList.Add(one.worksImg);
                    list1.Add(one);
                }
                else
                {
                    var my = list1.Find(l => l.worksCode == one.worksCode);
                    my.worksImgList.Add(one.worksImg);
                }
            });
            list1.ForEach(l =>
            {
                if (worksList.Count <= pageNo * pageSize)
                {
                    worksList.Add((JObject)JToken.FromObject(l));
                }
                else
                {
                    return;
                }
            });

            result.Add("pageNo", page.cursor);
            result.Add("pageSum", page.sum);
            result.Add("worksList", worksList);
            pageCount = (short)(list1.Count / pageSize + list1.Count % pageSize > 0 ? 1 : 0);
            return result;
        }


        //判断链接中是否包括http
        protected bool isHttp(string url)
        {
            string prefix = url.Length > 4 ? url.Substring(0, 4) : "";
            if (String.Equals(prefix, "http", StringComparison.CurrentCultureIgnoreCase))
            {
                return true;
            }
            else
            {
                return false;
            }

        }

        //搜索线下活动订单
        protected JObject SearchOfflineOrder(string activityCode, ActivityController.QUERY_TYPE queryType, string queryValue)
        {
            JObject result = new JObject();
            string sqlStr = null;
            DefaultPageable page;

            if (queryType == ActivityController.QUERY_TYPE.ByPhone)
            {
                //查询语句
                sqlStr = "SELECT A.Code,Receiver, Phone , OrderNo, Status, ProjectCode, A.CreateTime, A.PayWay"
                    + " FROM YuanXinBusiness.[zc].[Order] AS A join YuanXinBusiness.zc.OrderAddress AS B on A.Code = B.OrderCode"
                    + " WHERE ProjectCode = '" + activityCode + "' AND Phone = '" + queryValue + "'";
            }
            else if (queryType == ActivityController.QUERY_TYPE.ByOrderNo)
            {
                //查询语句
                sqlStr = "SELECT A.Code,Receiver, Phone , OrderNo, Status, ProjectCode, A.CreateTime, A.PayWay"
                    + " FROM YuanXinBusiness.[zc].[Order] AS A join YuanXinBusiness.zc.OrderAddress AS B on A.Code = B.OrderCode"
                    + " WHERE ProjectCode = '" + activityCode + "' AND OrderNo = '" + queryValue + "'";
            }
            else if (queryType == ActivityController.QUERY_TYPE.ByReceiver)
            {
                //查询语句
                sqlStr = "SELECT A.Code,Receiver, Phone , OrderNo, Status, ProjectCode, A.CreateTime,  A.PayWay"
                    + " FROM YuanXinBusiness.[zc].[Order] AS A join YuanXinBusiness.zc.OrderAddress AS B on A.Code = B.OrderCode"
                    + " WHERE ProjectCode = '" + activityCode + "' AND Receiver = '" + queryValue + "'";
            }
            else
            {
                return result;
            }

            string sessionId = queryByPage(sqlStr);
            if (sessionId == null)
            {
                return result;
            }

            page = queryBySession(sessionId, 0);

            //OfflineOrderList 生成
            result = createOfflineOrderList(page);
            result.Add("sessionId", sessionId);

            return result;
        }

        //生成线下活动订单
        protected JObject createOfflineOrderList(DefaultPageable page)
        {
            JArray orderList = new JArray();
            JObject result = new JObject();

            for (int i = 0; i < page.dataTable.Rows.Count; i++)
            {
                OfflineOrderInfo orderJson = new OfflineOrderInfo();
                orderJson.receiver = page.dataTable.Rows[i]["Receiver"] == DBNull.Value ? "" : (string)page.dataTable.Rows[i]["Receiver"];
                orderJson.phone = page.dataTable.Rows[i]["Phone"] == DBNull.Value ? "" : (string)page.dataTable.Rows[i]["Phone"];
                orderJson.orderNo = page.dataTable.Rows[i]["OrderNo"] == DBNull.Value ? "" : (string)page.dataTable.Rows[i]["OrderNo"];

                string code = page.dataTable.Rows[i]["Code"] == DBNull.Value ? "" : (string)page.dataTable.Rows[i]["Code"];
                Order newOrder = OrderAdapter.Instance.LoadByCodeUpdateStatus(code);
                int status = newOrder != null ? (int)newOrder.Status : (int)page.dataTable.Rows[i]["Status"];
                orderJson.status = status.ToString();

                orderJson.createTime = page.dataTable.Rows[i]["CreateTime"] == DBNull.Value ? "" : ((DateTime)page.dataTable.Rows[i]["CreateTime"]).ToString(DateFormat);
                orderJson.payWay = newOrder != null ? ((int)newOrder.PayWay).ToString() : ((int)PAY_WAY.None).ToString();
                orderList.Add((JObject)JToken.FromObject(orderJson));
            }



            result.Add("pageNo", page.cursor);
            result.Add("pageSum", page.sum);
            result.Add("orderList", orderList);

            return result;
        }

        //根据前端活动json对象生成线下活动信息
        protected Boolean createOfflineActivity(ProjectBean activityInfo, JObject activityJson, List<AttachmentBean> attArray)
        {
            activityInfo.Code = Guid.NewGuid().ToString();
            activityInfo.Type = ((int)PROJECT_TYPE.Offline).ToString();
            activityInfo.Name = (string)activityJson.SelectToken("name");
            //string userId = CurrentUserIUser.ID;

            JObject provinceJson = (JObject)activityJson.SelectToken("provinceInfo");
            if (provinceJson == null || provinceJson.Count == 0)
            {
                log.Error("provinceJson is null or empty!");
                return false;
            }
            activityInfo.ProvinceCode = (string)provinceJson.SelectToken("provinceCode");
            activityInfo.Province = (string)provinceJson.SelectToken("provinceName");

            JObject cityJson = (JObject)activityJson.SelectToken("cityInfo");
            if (cityJson == null || cityJson.Count == 0)
            {
                log.Error("cityJson is null or empty!");
                return false;
            }
            activityInfo.CityCode = (string)cityJson.SelectToken("cityCode");
            activityInfo.City = (string)cityJson.SelectToken("cityName");

            if (isProvinceCity(activityInfo.CityCode, activityInfo.City))
            {
                activityInfo.CityCode = (String)provinceJson.SelectToken("provinceCode");
                activityInfo.City = (String)provinceJson.SelectToken("provinceName");

            }

            activityInfo.Address = (string)activityJson.SelectToken("address");
            activityInfo.StartTime = ((DateTime)activityJson.SelectToken("startTime")).ToString(DateFormat);
            activityInfo.EndTime = ((DateTime)activityJson.SelectToken("endTime")).ToString(DateFormat);
            activityInfo.EnrollDeadline = ((DateTime)activityJson.SelectToken("enrollDeadline")).ToString(DateFormat);
            activityInfo.Detail = (string)activityJson.SelectToken("detail");
            activityInfo.SubItemJoinLimit = (int)activityJson.SelectToken("subItemJoinLimit");
            //发布范围
            activityInfo.IssueRange = (int)ActivityController.ISSUE_RANGE.staff;

            activityInfo.AuditStatus = (int)AUDIT_STATUS.Auditing;

            activityInfo.SupportNo = 0; //支持人数默认为0
            activityInfo.PraiseNo = 0; //点赞数量默认为0
            activityInfo.ShareNo = 0; //分享数量默认为0
            activityInfo.CommentNo = 0; //话题数量默认为0

            JArray activityEventArray = (JArray)activityJson.SelectToken("activityEvent");
            if (activityEventArray == null || activityEventArray.Count == 0)
            {
                log.Info("activityEventArray is null or empty!");
                return false;
            }
            activityInfo.SubItemNo = activityEventArray.Count; //场次数量

            //存储创建人信息
            activityInfo.Creator = CurrentUserCode;
            activityInfo.CreateTime = DateTime.Now;
            activityInfo.CompanyCode = queryCompanyIdByUser(activityInfo.Creator);

            //活动图片信息
            JArray activityImg = (JArray)activityJson.SelectToken("activityImg");
            if (activityImg == null || activityImg.Count == 0)
            {
                log.Error("activityImg is empty!");
                return false;
            }
            List<AttachmentBean> activityImgList = new List<AttachmentBean>();
            if (!createImageList(activityImgList, activityImg, activityInfo.Code, (int)ATT_TYPE_CODE.ActivityImg))
            {
                return false;
            }
            foreach (AttachmentBean activityImageBean in activityImgList)
            {
                attArray.Add(activityImageBean);
                //存储Project表中的CoverImg字段
                if (activityImageBean.SortNo == 1)
                {
                    activityInfo.CoverImg = activityImageBean.URL;
                }
            }

            //活动详情图片
            JArray detailImg = (JArray)activityJson.SelectToken("detailImg");
            if (detailImg == null || detailImg.Count == 0)
            {
                log.Error("detailImg is empty!");
                return false;
            }

            List<AttachmentBean> detailImgList = new List<AttachmentBean>();
            if (!createImageList(detailImgList, detailImg, activityInfo.Code, (int)ATT_TYPE_CODE.ActivityDetailImg))
            {
                return false;
            }
            foreach (AttachmentBean detailImgBean in detailImgList)
            {
                attArray.Add(detailImgBean);
            }

            return true;
        }

        //生成线下活动的场次信息
        protected Boolean createActivityEvent(List<ActivityEventBean> eventsList, String projectCode, JObject activityJson)
        {
            JArray activityEventArray = (JArray)activityJson.SelectToken("activityEvent");
            if (activityEventArray == null || activityEventArray.Count == 0)
            {
                log.Info("activityEventArray is null or empty!");
                return false;
            }

            for (int i = 0; i < activityEventArray.Count; i++)
            {
                ActivityEventBean eventInfo = new ActivityEventBean();
                eventInfo.Code = Guid.NewGuid().ToString();
                eventInfo.StartTime = ((DateTime)activityEventArray[i].SelectToken("startTime")).ToString(DateFormat);
                eventInfo.Hours = float.Parse(activityEventArray[i].SelectToken("hours").ToString());
                eventInfo.Price = float.Parse(activityEventArray[i].SelectToken("price").ToString());
                eventInfo.LimitNo = (int)activityEventArray[i].SelectToken("limitNo");
                eventInfo.SortNo = (int)activityEventArray[i].SelectToken("sortNo");
                eventInfo.ProjectCode = projectCode;

                eventsList.Add(eventInfo);
            }

            return true;
        }

        //线下活动数据持久化
        protected Boolean saveOfflineActivity(ProjectBean activityInfo, List<ProjectTagBean> projectTagList, List<ActivityEventBean> eventsArray, List<ActivityBroadcastBean> broadcastBeanList, List<AttachmentBean> attInfoArray)
        {
            SqlDbHelper db = new SqlDbHelper();
            List<String> sqlArray = new List<String>();
            List<SqlParameter[]> sqlParmetersArray = new List<SqlParameter[]>();
            string sqlstr;
            Boolean result = false;
            //存储事务操作
            try
            {
                //活动信息sql
                sqlstr = "insert zc.Project (Code,Creator,CreateTime,Type,Name,AuditStatus,ProvinceCode,Province,CityCode,City,Address,CoverImg,StartTime,EndTime,IsValid,Detail,CompanyCode,SupportNo,SubItemJoinLimit,EnrollDeadline, PraiseNo, ShareNo, CommentNo, SubItemNo,issueRange) "
                    + " values(@Code,@Creator,@CreateTime,@Type,@Name,@AuditStatus,@ProvinceCode,@Province,@CityCode,@City,@Address,@CoverImg,@StartTime,@EndTime,@IsValid,@Detail,@CompanyCode,@SupportNo,@SubItemJoinLimit,@EnrollDeadline, @PraiseNo, @ShareNo, @CommentNo, @SubItemNo,@issueRange)";
                sqlArray.Add(sqlstr);

                SqlParameter[] projectParamters = new SqlParameter[]{
                    new SqlParameter("@Code", activityInfo.Code),
                    new SqlParameter("@Creator",activityInfo.Creator),
                    new SqlParameter("@CreateTime", DateTime.Now),
                    new SqlParameter("@Type", activityInfo.Type),
                    new SqlParameter("@Name", activityInfo.Name),
                    new SqlParameter("@AuditStatus", activityInfo.AuditStatus),
                    new SqlParameter("@ProvinceCode", activityInfo.ProvinceCode),
                    new SqlParameter("@Province", activityInfo.Province),
                    new SqlParameter("@CityCode", activityInfo.CityCode),
                    new SqlParameter("@City", activityInfo.City),
                    new SqlParameter("@Address", activityInfo.Address),
                    new SqlParameter("@CoverImg", activityInfo.CoverImg),
                    new SqlParameter("@StartTime", activityInfo.StartTime),
                    new SqlParameter("@EndTime", activityInfo.EndTime),
                    new SqlParameter("@EnrollDeadline", activityInfo.EnrollDeadline),
                    new SqlParameter("@Detail", activityInfo.Detail),
                    new SqlParameter("@CompanyCode", activityInfo.CompanyCode),
                    new SqlParameter("@SupportNo", activityInfo.SupportNo),
                    new SqlParameter("@SubItemJoinLimit", activityInfo.SubItemJoinLimit),
                    new SqlParameter("@PraiseNo", activityInfo.PraiseNo),
                    new SqlParameter("@ShareNo", activityInfo.ShareNo),
                    new SqlParameter("@CommentNo", activityInfo.CommentNo),
                    new SqlParameter("@SubItemNo", activityInfo.SubItemNo),
                    new SqlParameter("@IsValid", (int)ActivityController.IS_VALID.Valid),
                    new SqlParameter("@issueRange",activityInfo.IssueRange),
                };
                sqlParmetersArray.Add(projectParamters);

                //项目标签sql
                foreach (ProjectTagBean projectTag in projectTagList)
                {
                    sqlstr = "insert zc.ProjectTag (Code, ProjectCode, TagCode, TagName, SortNo)"
                        + " values(@Code, @ProjectCode, @TagCode, @TagName, @SortNo)";
                    sqlArray.Add(sqlstr);
                    SqlParameter[] projectTagParamters = new SqlParameter[]{
                    new SqlParameter("@Code", projectTag.Code),
                    new SqlParameter("@ProjectCode",projectTag.ProjectCode),
                    new SqlParameter("@TagCode", projectTag.TagCode),
                    new SqlParameter("@TagName", projectTag.TagName),
                    new SqlParameter("@SortNo", projectTag.SortNo),
                    };
                    sqlParmetersArray.Add(projectTagParamters);
                }

                //活动场次sql
                foreach (ActivityEventBean eventInfo in eventsArray)
                {
                    sqlstr = "insert zc.ActivityEvent (Code, ProjectCode,StartTime,EndTime, Hours, Price, LimitNo, SortNo)"
                        + "values(@Code,@ProjectCode, @StartTime,@EndTime,@Hours, @Price, @LimitNo, @SortNo)";
                    sqlArray.Add(sqlstr);

                    SqlParameter[] eventParamters = new SqlParameter[]{
                        new SqlParameter("@Code", eventInfo.Code),
                        new SqlParameter("@ProjectCode",eventInfo.ProjectCode),
                        new SqlParameter("@StartTime", eventInfo.StartTime),
                        new SqlParameter("@EndTime", DateTime.Parse(eventInfo.StartTime).AddHours(eventInfo.Hours)),
                        new SqlParameter("@Hours", eventInfo.Hours),
                        new SqlParameter("@Price", eventInfo.Price),
                        new SqlParameter("@LimitNo", eventInfo.LimitNo),
                        new SqlParameter("@SortNo", eventInfo.SortNo),
                    };
                    sqlParmetersArray.Add(eventParamters);
                }

                //直播sql
                foreach (ActivityBroadcastBean broadcastBean in broadcastBeanList)
                {
                    sqlstr = "insert " + tableNamePrefix_BS + "ActivityBroadCast (Code,ProjectCode,Name, StartTime, CoverImg, SortNo, Detail, Creator, CreateTime)"
                        + "values(@Code, @ProjectCode,@Name, @StartTime, @CoverImg, @SortNo, @Detail, @Creator, @CreateTime)";
                    sqlArray.Add(sqlstr);

                    SqlParameter[] eventParamters = new SqlParameter[]{
                        new SqlParameter("@Code", broadcastBean.Code.Replace("-","")),
                        new SqlParameter("@ProjectCode",broadcastBean.ProjectCode),
                        new SqlParameter("@Name", broadcastBean.Name),
                        new SqlParameter("@StartTime", broadcastBean.StartTime),
                        new SqlParameter("@CoverImg", broadcastBean.CoverImg),
                        new SqlParameter("@SortNo", broadcastBean.SortNo),
                        new SqlParameter("@Detail", broadcastBean.Detail),
                        new SqlParameter("@Creator", broadcastBean.Creator),
                        new SqlParameter("@CreateTime", broadcastBean.CreateTime),
                    };
                    sqlParmetersArray.Add(eventParamters);
                }

                //附件（图片）sql
                foreach (AttachmentBean attInfo in attInfoArray)
                {
                    sqlstr = "insert Business.Attachment (Code,Creator,ResourceID,CnName,URL,FileSize,VersionStartTime,VersionEndTime,ValidStatus,Suffix,SortNo,AttachmentTypeCode)"
                        + "values(@Code,@Creator,@ResourceID,@CnName,@URL,@FileSize,@VersionStartTime,@VersionEndTime,@ValidStatus,@Suffix,@SortNo,@AttachmentTypeCode)";
                    sqlArray.Add(sqlstr);

                    SqlParameter[] attParamters = new SqlParameter[]{
                        new SqlParameter("@Code", attInfo.Code),
                        new SqlParameter("@Creator",attInfo.Creator),
                        new SqlParameter("@ResourceID", attInfo.ResourceID),
                        new SqlParameter("@CnName", attInfo.CnName),
                        new SqlParameter("@URL", attInfo.URL),
                        new SqlParameter("@FileSize", attInfo.FileSize),
                        new SqlParameter("@VersionStartTime", attInfo.VersionStartTime),
                        new SqlParameter("@VersionEndTime", attInfo.VersionEndTime),
                        new SqlParameter("@ValidStatus", attInfo.ValidStatus),
                        new SqlParameter("@Suffix", attInfo.Suffix),
                        new SqlParameter("@SortNo", attInfo.SortNo),
                        new SqlParameter("@AttachmentTypeCode", attInfo.AttachmentTypeCode),
                    };
                    sqlParmetersArray.Add(attParamters);
                }

                result = db.ExecuteNonQueryTransation(sqlArray, CommandType.Text, sqlParmetersArray);
            }
            catch (Exception ex)
            {
                log.Error("Offline Activity save failed! -- ", ex);
                return false;
            }

            return result;
        }

        //根据前端活动json对象生成线上活动信息
        protected Boolean createOnlineActivity(ProjectBean activityInfo, JObject activityJson, List<AttachmentBean> attArray)
        {

            activityInfo.Code = Guid.NewGuid().ToString();
            activityInfo.Type = ((int)PROJECT_TYPE.Online).ToString();
            activityInfo.Name = (string)activityJson.SelectToken("name");
            activityInfo.EndTime = (string)activityJson.SelectToken("endTime");
            activityInfo.Detail = (string)activityJson.SelectToken("detail");
            activityInfo.WorksSelectedType = (int)activityJson.SelectToken("worksSelectedType");
            activityInfo.EnrollDeadline = (string)activityJson.SelectToken("enrollDeadline");
            activityInfo.AuditStatus = (int)AUDIT_STATUS.Auditing;
            activityInfo.EnrollDeadline = (string)activityJson.SelectToken("enrollDeadline");

            activityInfo.IssueRange = (int)ActivityController.ISSUE_RANGE.staff;

            activityInfo.SupportNo = 0; //支持人数默认为0
            activityInfo.PraiseNo = 0; //点赞数量默认为0
            activityInfo.ShareNo = 0; //分享数量默认为0
            activityInfo.CommentNo = 0; //话题数量默认为0

            //存储创建人信息
            activityInfo.Creator = CurrentUserCode; ;
            activityInfo.CreateTime = DateTime.Now;
            activityInfo.CompanyCode = queryCompanyIdByUser(activityInfo.Creator);

            //存储活动图片
            JArray activityImg = (JArray)activityJson.SelectToken("activityImg");
            List<AttachmentBean> activityImgList = new List<AttachmentBean>();
            if (!createImageList(activityImgList, activityImg, activityInfo.Code, (int)ATT_TYPE_CODE.ActivityImg))
            {
                return false;
            }
            foreach (AttachmentBean activityImageBean in activityImgList)
            {
                attArray.Add(activityImageBean);
                //存储Project表中的CoverImg字段
                if (activityImageBean.SortNo == 1)
                {
                    activityInfo.CoverImg = activityImageBean.URL;
                }
            }

            //活动详情图片
            JArray detailImg = (JArray)activityJson.SelectToken("detailImg");
            if (detailImg == null || detailImg.Count == 0)
            {
                log.Error("detailImg is empty!");
                return false;
            }

            List<AttachmentBean> detailImgList = new List<AttachmentBean>();
            if (!createImageList(detailImgList, detailImg, activityInfo.Code, (int)ATT_TYPE_CODE.ActivityDetailImg))
            {
                return false;
            }
            foreach (AttachmentBean detailImgBean in detailImgList)
            {
                attArray.Add(detailImgBean);
            }

            return true;

        }

        //根据前端活动json对象生成项目标签信息
        protected Boolean createProjectTag(List<ProjectTagBean> projectTagList, JObject activityJson, string projectCode)
        {
            JArray projectTagJsonArray = (JArray)activityJson.SelectToken("activityTag");
            if (projectTagJsonArray == null || projectTagJsonArray.Count == 0)
            {
                log.Info("projectTagJsonArray is null!");
                return false;
            }
            for (int i = 0; i < projectTagJsonArray.Count; i++)
            {
                ProjectTagBean projectTag = new ProjectTagBean();
                projectTag.Code = Guid.NewGuid().ToString();
                projectTag.TagCode = (string)projectTagJsonArray[i].SelectToken("tagCode");
                projectTag.TagName = (string)projectTagJsonArray[i].SelectToken("tagName");
                projectTag.SortNo = (int)projectTagJsonArray[i].SelectToken("sortNo");

                projectTag.ProjectCode = projectCode;

                projectTagList.Add(projectTag);
            }

            return true;
        }

        //根据前端活动json对象生成奖项信息
        protected Boolean createAwardsSetting(ProjectBean activityBean, List<AwardsSettingBean> awardsList, JObject activityJson, string projectCode, List<AttachmentBean> attArray)
        {
            JArray awardsJsonArray = (JArray)activityJson.SelectToken("awardsSetting");
            if (awardsJsonArray == null || awardsJsonArray.Count == 0)
            {
                log.Info("awardsJsonArray is null or empty!");
                return false;
            }

            for (int i = 0; i < awardsJsonArray.Count; i++)
            {
                AwardsSettingBean awardsInfo = new AwardsSettingBean();
                awardsInfo.Code = Guid.NewGuid().ToString();
                awardsInfo.AwardsName = (string)awardsJsonArray[i].SelectToken("awardsName");
                awardsInfo.StartRanking = (int)awardsJsonArray[i].SelectToken("startRanking");
                awardsInfo.StopRanking = (int)awardsJsonArray[i].SelectToken("stopRanking");
                int awardsCount = awardsInfo.StopRanking - awardsInfo.StartRanking + 1;
                awardsInfo.AwardsCount = awardsCount;
                awardsInfo.AwardsContent = (string)awardsJsonArray[i].SelectToken("awardsContent");
                awardsInfo.SortNo = (int)awardsJsonArray[i].SelectToken("sortNo");
                awardsInfo.ProjectCode = projectCode;
                awardsList.Add(awardsInfo);
                //存储奖品图片
                JArray awardsImgArray = (JArray)awardsJsonArray[i].SelectToken("awardsImg");
                List<AttachmentBean> imageBeanList = new List<AttachmentBean>();

                if (!createImageList(imageBeanList, awardsImgArray, awardsInfo.Code, (int)ATT_TYPE_CODE.AwardsImg))
                {
                    return false;
                }

                foreach (AttachmentBean imageBean in imageBeanList)
                {
                    attArray.Add(imageBean);
                }
            }

            return true;

        }

        //将线上活动信息数据库持久化
        protected Boolean saveOnlineActivity(ProjectBean activityInfo, List<ProjectTagBean> projectTagList, List<AwardsSettingBean> awardsArray, List<ActivityBroadcastBean> broadcastBeanList, List<AttachmentBean> attInfoArray)
        {
            SqlDbHelper db = new SqlDbHelper();
            List<String> sqlArray = new List<String>();
            List<SqlParameter[]> sqlParmetersArray = new List<SqlParameter[]>();
            string sqlstr;
            Boolean result;
            //存储事务操作
            try
            {
                //活动信息sql
                sqlstr = "insert " + tableNamePrefix_ZC + "Project (Code,Creator,CreateTime,Type,Name,AuditStatus,CoverImg,StartTime,EndTime,IsValid,Detail,CompanyCode,SupportNo,WorksSelectedType,EnrollDeadline, PraiseNo, ShareNo, CommentNo,IssueRange) "
                    + " values(@Code,@Creator,@CreateTime,@Type,@Name,@AuditStatus,@CoverImg,@StartTime,@EndTime,@IsValid,@Detail,@CompanyCode,@SupportNo,@WorksSelectedType,@EnrollDeadline,@PraiseNo,@ShareNo,@CommentNo,@IssueRange)";
                sqlArray.Add(sqlstr);

                SqlParameter[] projectParamters = new SqlParameter[]{
                    new SqlParameter("@Code", activityInfo.Code),
                    new SqlParameter("@Creator",activityInfo.Creator),
                    new SqlParameter("@CreateTime", activityInfo.CreateTime),
                    new SqlParameter("@Type", activityInfo.Type),
                    new SqlParameter("@Name", activityInfo.Name),
                    new SqlParameter("@AuditStatus", activityInfo.AuditStatus),
                    new SqlParameter("@CoverImg", activityInfo.CoverImg),
                    new SqlParameter("@StartTime", DateTime.Now),
                    new SqlParameter("@EndTime", activityInfo.EndTime),
                    new SqlParameter("@Detail", activityInfo.Detail),
                    new SqlParameter("@CompanyCode", activityInfo.CompanyCode),
                    new SqlParameter("@SupportNo", activityInfo.SupportNo),
                    new SqlParameter("@WorksSelectedType", activityInfo.WorksSelectedType),
                    new SqlParameter("@EnrollDeadline", activityInfo.EnrollDeadline),
                    new SqlParameter("@PraiseNo", activityInfo.PraiseNo),
                    new SqlParameter("@ShareNo", activityInfo.ShareNo),
                    new SqlParameter("@CommentNo", activityInfo.CommentNo),
                    new SqlParameter("@IsValid",(int)ActivityController.IS_VALID.Valid),
                    new SqlParameter("@IssueRange", activityInfo.IssueRange)

                };
                sqlParmetersArray.Add(projectParamters);

                foreach (ProjectTagBean projectTag in projectTagList)
                {
                    sqlstr = "insert " + tableNamePrefix_ZC + "ProjectTag (Code, ProjectCode, TagCode, TagName, SortNo)"
                        + " values(@Code, @ProjectCode, @TagCode, @TagName, @SortNo)";
                    sqlArray.Add(sqlstr);
                    SqlParameter[] projectTagParamters = new SqlParameter[]{
                    new SqlParameter("@Code", projectTag.Code),
                    new SqlParameter("@ProjectCode",projectTag.ProjectCode),
                    new SqlParameter("@TagCode", projectTag.TagCode),
                    new SqlParameter("@TagName", projectTag.TagName),
                    new SqlParameter("@SortNo", projectTag.SortNo),
                    };
                    sqlParmetersArray.Add(projectTagParamters);
                }

                //奖项设置sql
                foreach (AwardsSettingBean award in awardsArray)
                {

                    sqlstr = "insert " + tableNamePrefix_ZC + "AwardsSetting (Code, ProjectCode, AwardsName, AwardsCount, StartRanking, StopRanking, AwardsContent, SortNo)"
                        + " values (@Code, @ProjectCode, @AwardsName, @AwardsCount, @StartRanking, @StopRanking, @AwardsContent, @SortNo)";
                    sqlArray.Add(sqlstr);

                    SqlParameter[] awardParamters = new SqlParameter[]{
                    new SqlParameter("@Code", award.Code),
                    new SqlParameter("@ProjectCode",award.ProjectCode),
                    new SqlParameter("@AwardsName",award.AwardsName),
                    new SqlParameter("@StartRanking", award.StartRanking),
                    new SqlParameter("@StopRanking", award.StopRanking),
                    new SqlParameter("@AwardsCount", award.AwardsCount),
                    new SqlParameter("@AwardsContent", award.AwardsContent),
                    new SqlParameter("@SortNo", award.SortNo),
                    };
                    sqlParmetersArray.Add(awardParamters);

                }


                //直播sql
                foreach (ActivityBroadcastBean broadcastBean in broadcastBeanList)
                {
                    sqlstr = "insert " + tableNamePrefix_BS + "ActivityBroadCast (Code,ProjectCode,Name, StartTime, CoverImg, SortNo, Detail, Creator, CreateTime)"
                        + "values(@Code, @ProjectCode,@Name, @StartTime, @CoverImg, @SortNo, @Detail, @Creator, @CreateTime)";
                    sqlArray.Add(sqlstr);

                    SqlParameter[] eventParamters = new SqlParameter[]{
                        new SqlParameter("@Code", broadcastBean.Code.Replace("-","")),
                        new SqlParameter("@ProjectCode",broadcastBean.ProjectCode),
                        new SqlParameter("@Name", broadcastBean.Name),
                        new SqlParameter("@StartTime", broadcastBean.StartTime),
                        new SqlParameter("@CoverImg", broadcastBean.CoverImg),
                        new SqlParameter("@SortNo", broadcastBean.SortNo),
                        new SqlParameter("@Detail", broadcastBean.Detail),
                        new SqlParameter("@Creator", broadcastBean.Creator),
                        new SqlParameter("@CreateTime", broadcastBean.CreateTime),
                    };
                    sqlParmetersArray.Add(eventParamters);
                }

                //附件（图片）sql
                foreach (AttachmentBean attInfo in attInfoArray)
                {
                    sqlstr = "insert " + tableNamePrefix_BS + "Attachment (Code,Creator,ResourceID,CnName,URL,FileSize,VersionStartTime,VersionEndTime,ValidStatus,Suffix,SortNo,AttachmentTypeCode)"
                        + "values(@Code,@Creator,@ResourceID,@CnName,@URL,@FileSize,@VersionStartTime,@VersionEndTime,@ValidStatus,@Suffix,@SortNo,@AttachmentTypeCode)";
                    sqlArray.Add(sqlstr);

                    SqlParameter[] attParamters = new SqlParameter[]{
                        new SqlParameter("@Code", attInfo.Code),
                        new SqlParameter("@Creator",attInfo.Creator),
                        new SqlParameter("@ResourceID", attInfo.ResourceID),
                        new SqlParameter("@CnName", attInfo.CnName),
                        new SqlParameter("@URL", attInfo.URL),
                        new SqlParameter("@FileSize", attInfo.FileSize),
                        new SqlParameter("@VersionStartTime", attInfo.VersionStartTime),
                        new SqlParameter("@VersionEndTime", attInfo.VersionEndTime),
                        new SqlParameter("@ValidStatus", attInfo.ValidStatus),
                        new SqlParameter("@Suffix", attInfo.Suffix),
                        new SqlParameter("@SortNo", attInfo.SortNo),
                        new SqlParameter("@AttachmentTypeCode", attInfo.AttachmentTypeCode),
                    };
                    sqlParmetersArray.Add(attParamters);
                }
                result = db.ExecuteNonQueryTransation(sqlArray, CommandType.Text, sqlParmetersArray);
            }
            catch (Exception ex)
            {
                log.Error("Online Activity save failed! -- ", ex);
                return false;
            }
            return result;
        }

        //检查线上活动是否已开奖
        protected Boolean checkPrizeStatus(string activityCode)
        {
            string tableName = tableNamePrefix_ZC + "WorksAwards";
            DataTable worksAwards = queryByProjectCode(tableName, activityCode);
            if (worksAwards == null || worksAwards.Rows.Count == 0)
            {
                return false;
            }
            else
            {
                return true;
            }

        }

        //存储活动直播信息
        protected bool saveActivityBroadcast(List<ActivityBroadcastBean> broadcastBeanList, List<AttachmentBean> attArray)
        {
            SqlDbHelper db = new SqlDbHelper();
            List<String> sqlArray = new List<String>();
            List<SqlParameter[]> sqlParmetersArray = new List<SqlParameter[]>();
            string sqlstr;
            Boolean result = false;
            //存储事务操作
            try
            {
                //直播sql
                foreach (ActivityBroadcastBean broadcastBean in broadcastBeanList)
                {
                    sqlstr = "insert " + tableNamePrefix_BS + "ActivityBroadCast (Code,ProjectCode,Name, StartTime, CoverImg, SortNo, Detail, Creator, CreateTime)"
                        + " values(@Code, @ProjectCode,@Name, @StartTime, @CoverImg, @SortNo, @Detail, @Creator, @CreateTime)";
                    sqlArray.Add(sqlstr);

                    SqlParameter[] eventParamters = new SqlParameter[]{
                        new SqlParameter("@Code", broadcastBean.Code.Replace("-","")),
                        new SqlParameter("@ProjectCode",broadcastBean.ProjectCode),
                        new SqlParameter("@Name", broadcastBean.Name),
                        new SqlParameter("@StartTime", broadcastBean.StartTime),
                        new SqlParameter("@CoverImg", broadcastBean.CoverImg),
                        new SqlParameter("@SortNo", broadcastBean.SortNo),
                        new SqlParameter("@Detail", broadcastBean.Detail),
                        new SqlParameter("@Creator", broadcastBean.Creator),
                        new SqlParameter("@CreateTime", broadcastBean.CreateTime),
                    };
                    sqlParmetersArray.Add(eventParamters);
                }

                //附件（图片）sql
                foreach (AttachmentBean attInfo in attArray)
                {
                    sqlstr = "insert Business.Attachment (Code,Creator,ResourceID,CnName,URL,FileSize,VersionStartTime,VersionEndTime,ValidStatus,Suffix,SortNo,AttachmentTypeCode)"
                        + "values(@Code,@Creator,@ResourceID,@CnName,@URL,@FileSize,@VersionStartTime,@VersionEndTime,@ValidStatus,@Suffix,@SortNo,@AttachmentTypeCode)";
                    sqlArray.Add(sqlstr);

                    SqlParameter[] attParamters = new SqlParameter[]{
                        new SqlParameter("@Code", attInfo.Code),
                        new SqlParameter("@Creator",attInfo.Creator),
                        new SqlParameter("@ResourceID", attInfo.ResourceID),
                        new SqlParameter("@CnName", attInfo.CnName),
                        new SqlParameter("@URL", attInfo.URL),
                        new SqlParameter("@FileSize", attInfo.FileSize),
                        new SqlParameter("@VersionStartTime", attInfo.VersionStartTime),
                        new SqlParameter("@VersionEndTime", attInfo.VersionEndTime),
                        new SqlParameter("@ValidStatus", attInfo.ValidStatus),
                        new SqlParameter("@Suffix", attInfo.Suffix),
                        new SqlParameter("@SortNo", attInfo.SortNo),
                        new SqlParameter("@AttachmentTypeCode", attInfo.AttachmentTypeCode),
                    };
                    sqlParmetersArray.Add(attParamters);
                }

                result = db.ExecuteNonQueryTransation(sqlArray, CommandType.Text, sqlParmetersArray);
            }
            catch (Exception ex)
            {
                log.Error("Activity Broadcast save failed! -- ", ex);
                return false;
            }

            return result;
        }

        //创建活动直播信息
        protected bool createActivivtyBroadcast(List<ActivityBroadcastBean> broadcastBeanList, JArray broadcastJArray, string activityCode, List<AttachmentBean> attArray, int maxSortNo)
        {

            foreach (JObject broadcastJson in broadcastJArray)
            {
                ActivityBroadcastBean broadcastBean = new ActivityBroadcastBean();
                broadcastBean.Code = Guid.NewGuid().ToString().Replace("-", "");
                broadcastBean.ProjectCode = activityCode;
                broadcastBean.Name = (string)broadcastJson.SelectToken("name");
                broadcastBean.StartTime = ((DateTime)broadcastJson.SelectToken("startTime")).ToString(DateFormat);
                broadcastBean.Detail = (string)broadcastJson.SelectToken("detail");
                broadcastBean.SortNo = (int)broadcastJson.SelectToken("sortNo") + maxSortNo;
                broadcastBean.Creator = CurrentUserCode;

                broadcastBean.CreateTime = (DateTime)DateTime.Now;

                //活动图片信息
                JArray coverImgJarray = (JArray)broadcastJson.SelectToken("coverImg");
                if (coverImgJarray == null || coverImgJarray.Count == 0)
                {
                    log.Error(" ActivivtyBroadcast coverImg is empty!");
                    return false;
                }

                if (!createImageList(attArray, coverImgJarray, broadcastBean.Code, (int)ATT_TYPE_CODE.BroadcastCoverImg))
                {
                    return false;
                }
                else
                {
                    broadcastBean.CoverImg = (string)coverImgJarray[0].SelectToken("URL");
                }

                broadcastBeanList.Add(broadcastBean);
            }
            return true;
        }


        //根据活动code获取broadcastList信息
        protected JArray getBroadcastList(string activityCode)
        {
            JArray broadcastList = new JArray();

            DataTable broadcast = queryByProjectCode(tableNamePrefix_BS + "ActivityBroadcast", activityCode);

            if (broadcast == null || broadcast.Rows.Count == 0)
            {
                return broadcastList;
            }
            //按照sortNo排序
            broadcast.DefaultView.Sort = "SortNo ASC";
            broadcast = broadcast.DefaultView.ToTable();


            for (int i = 0; i < broadcast.Rows.Count; i++)
            {

                JObject broadcastInfo = new JObject();
                string broadcastCode = (string)broadcast.Rows[i]["Code"];
                broadcastInfo.Add("code", broadcastCode);
                broadcastInfo.Add("name", (string)broadcast.Rows[i]["Name"]);
                broadcastInfo.Add("startTime", ((DateTime)broadcast.Rows[i]["StartTime"]).ToString(DateFormat));
                // broadcastInfo.Add("detail", (string)broadcast.Rows[i]["Detail"]);

                //添加直播封面信息
                //DataTable broadcastImage = queryAttachmentByResourceID(broadcastCode);
                //broadcastInfo.Add("URL", (string)broadcastImage.Rows[0]["URL"]);

                broadcastList.Add(broadcastInfo);
            }

            return broadcastList;
        }
        //获取活动直播的最大sortNo
        protected int getBroadcastMaxSortNo(string activityCode)
        {
            int maxSortNo = 0;
            DataTable broadcast = queryByProjectCode(tableNamePrefix_BS + "ActivityBroadcast", activityCode);
            if (broadcast == null || broadcast.Rows.Count == 0)
            {
                return maxSortNo;
            }
            //按照sortNo排序
            broadcast.DefaultView.Sort = "SortNo DESC";
            broadcast = broadcast.DefaultView.ToTable();

            maxSortNo = (int)broadcast.Rows[0]["SortNo"];

            return maxSortNo;

        }

    }
}