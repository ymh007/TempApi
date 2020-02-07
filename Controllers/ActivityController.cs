using MCS.Library.OGUPermission;
using Newtonsoft.Json.Linq;
using Seagull2.YuanXin.AppApi.Models;
using Seagull2.YuanXin.AppApi.ZhongChouData.Models;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Reflection;
using System.Text.RegularExpressions;
using System.Web.Http;

namespace Seagull2.YuanXin.AppApi.Controllers
{
    /// <summary>
    /// 活动Controller
    /// </summary>
    [AllowAnonymous]
    [RoutePrefix("api/Activity")]   //下周开始迁移活动控制器所有接口，将接口调试通过？？？？
    public class ActivityController : ActivityHelper
    {
        #region 属性

        public static string worksImagePath_Test = "http://mtest.sinooceanland.com/FileUpload/CrowdfundingProject/Pics/";

        public static string worksImagePath_Run = "http://www.yuanxin2015.com/OutSiteUploadFile/CrowdfundingImg/";

        public static log4net.ILog Log4netInstance = log4net.LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);

        //作品评选方式
        private enum WORKS_SELECTED_TYPE
        {
            None = 0, //无
            UserVote = 1, //用户投票
            BusinessSelection = 2 //自行评选
        }

        public enum QUERY_TYPE
        {
            ByPhone, ByOrderNo, ByReceiver, ByName
        }

        public enum IS_VALID { Invalid, Valid }

        //报名多场活动限制要求
        public enum JOIN_LIMIT { Unlimit, Limit }

        //发布范围--最新需求：移动办公发布的活动只能移动办公看，  也就是发布范围只能是移动办公专用
        public enum ISSUE_RANGE
        {
            Client = 0, //客户端
            Business = 1, //商家端
            staff = 2 //员工端--(最新)移动办公专用
        }

        //线上活动订单排序要求
        public enum SORT
        {
            ByTime = 0, //根据时间排序
            ByVote = 1, //根据票数排序
        }

        private LogHelper log = LogFactory.GetLogger(MethodBase.GetCurrentMethod().DeclaringType);

        #endregion

        #region 发布活动

        /**
        * 发布活动-在线活动
        * 返回结果：发布成功则返回ture，发布失败则返回false
        */
        [Route("PublishOnlineActivity")]
        [HttpPost]
        public Boolean PublishOnlineActivity([FromBody]JObject onlineActivityJson)
        {
            //注释1：商家发布活动
            //注释2：商家用户可查询所属商家的所有活动列表
            string userId = CurrentUserCode;
            if (userId == null)
            {
                return false;
            }
            log.Info("PublishOnlineActivity input info: " + onlineActivityJson + ", userId = " + userId);

            //判断数据是否合法
            if (onlineActivityJson.Count == 0 || onlineActivityJson == null)
            {
                log.Error("PublishOnlineActivity method input data is NULL.");
                return false;
            };
            //图片信息list
            List<AttachmentBean> attArray = new List<AttachmentBean>();


            //初始化线上活动的基本信息
            ProjectBean activityBean = new ProjectBean();
            if (!createOnlineActivity(activityBean, onlineActivityJson, attArray))
            {
                return false;
            }
            //初始化线上活动的标签
            string projectCode = activityBean.Code;
            List<ProjectTagBean> projectTagList = new List<ProjectTagBean>();
            if (!createProjectTag(projectTagList, onlineActivityJson, projectCode))
            {
                return false;
            }
            //初始化奖项设置
            List<AwardsSettingBean> awardsList = new List<AwardsSettingBean>();
            if (!createAwardsSetting(activityBean, awardsList, onlineActivityJson, projectCode, attArray))
            {
                return false;
            }

            //初始化活动直播信息 
            List<ActivityBroadcastBean> broadcastBeanList = new List<ActivityBroadcastBean>();
            JArray broadcastJArray = (JArray)onlineActivityJson.SelectToken("broadcastInfo");
            if (!createActivivtyBroadcast(broadcastBeanList, broadcastJArray, projectCode, attArray, 0))
            {
                return false;
            }

            //将数据进行持久化
            if (!saveOnlineActivity(activityBean, projectTagList, awardsList, broadcastBeanList, attArray))
            {
                return false;
            }

            log.Info("PublishOnlineActivity Return success");
            return true;

        }

        /**
        * 发布活动-线下活动
        * 返回结果：发布成功则返回ture，发布失败则返回false
        */
        [Route("PublishOfflineActivity")]
        [HttpPost]
        public Boolean PublishOfflineActivity([FromBody]JObject offlineActivityJson)
        {
            //注释1：商家发布活动
            //注释2：商家用户可查询所属商家的所有活动列表
            try
            {
                string userId = CurrentUserCode;
                if (userId == null)
                {
                    return false;
                }
                log.Info("PublishOfflineActivity input : UserID : " + userId + ", data :" + offlineActivityJson);
                //判断数据是否合法
                if (offlineActivityJson == null || offlineActivityJson.Count == 0)
                {
                    log.Info("PublishOfflineActivity method input data is NULL.");
                    return false;
                };

                //图片信息list
                List<AttachmentBean> attArray = new List<AttachmentBean>();

                //初始化线下活动信息
                ProjectBean activityBean = new ProjectBean();
                if (!createOfflineActivity(activityBean, offlineActivityJson, attArray))
                {
                    return false;
                }

                //初始化线下活动的标签
                string projectCode = activityBean.Code;
                List<ProjectTagBean> projectTagList = new List<ProjectTagBean>();
                if (!createProjectTag(projectTagList, offlineActivityJson, projectCode))
                {
                    return false;
                }

                //初始化线下活动场次
                List<ActivityEventBean> eventsList = new List<ActivityEventBean>();
                if (!createActivityEvent(eventsList, projectCode, offlineActivityJson))
                {
                    return false;
                }

                //初始化活动直播信息 
                List<ActivityBroadcastBean> broadcastBeanList = new List<ActivityBroadcastBean>();
                JArray broadcastJArray = (JArray)offlineActivityJson.SelectToken("broadcastInfo");
                if (!createActivivtyBroadcast(broadcastBeanList, broadcastJArray, projectCode, attArray, 0))
                {
                    return false;
                }

                //将数据进行持久化
                if (!saveOfflineActivity(activityBean, projectTagList, eventsList, broadcastBeanList, attArray))
                {
                    return false;
                }
                log.Info("PublishOfflineActivity Return success");
                return true;
            }
            catch (Exception e)
            {
                log.Error(e.Message);
                log.Error(e.StackTrace);
                return false;
            }
        }

        #endregion

        #region 活动列表查询

        /**
        （分页查询）已发布线上活动的列表查询，分页显示
         * 首次查询时，sessionId = "-1"， pageNo = 0
         * 后续查询时，可通过sessionId和pageNo来指定查询的页数，若pageNo为0，则默认为顺序查询
         */
        [Route("QueryOnlineListBySession")]
        [HttpGet]
        public JObject QueryOnlineListBySession(string sessionId, short pageNo)
        {
            //注释1：用户发布活动
            //注释2：查询用户创建的活动
            JObject result = new JObject();
            try
            {
                string userId = CurrentUserCode;
                //测试： string userId = "337168018efe468a88a44eec74ab9c41";
                if (userId == null)
                {
                    result.Add("error", "获取用户信息失败!");
                    return result;
                }

                log.Info("QueryOnlineListBySession Method: the agruments SessionID = " + sessionId + ", PageNo = " + pageNo + ", userId = " + userId);


                //初始化变量
                string sessionID = null;
                DefaultPageable page;
                //查询语句
                string sqlStr = "SELECT Code, Name, AuditStatus, CoverImg, SupportNo, PraiseNo, ShareNo, CommentNo, CreateTime"
                                 + " FROM YuanXinBusiness.zc.Project"
                                 + " WHERE Creator = '" + userId + "' AND Type = '" + (int)PROJECT_TYPE.Online + "'";
                //若sessionId传入为null，则返回空
                if (sessionId == null)
                {
                    log.Info("QueryOnlineListBySession Method: 传入参数： SessionID is null");
                    return result;
                }

                if (sessionId == "-1")
                {
                    sessionID = queryByPage(sqlStr);
                    if (sessionID == null)
                    {
                        log.Info("QueryOnlineListBySession Method: 直接查询--未查到数据： return sessionID is null,SQL=" + sqlStr);
                        return result;
                    }
                }
                else
                {
                    sessionID = sessionId;
                }

                page = queryBySession(sessionID, pageNo);
                //若获得的当前Page不包括sessionId所对应的数据，则再次查询
                if (page == null)
                {

                    sessionID = queryByPage(sqlStr);
                    if (sessionID == null)
                    {
                        log.Info("QueryOnlineListBySession Method: 缓存查询--未查到数据： return sessionID is null,SQL=" + sqlStr);
                        return result;
                    }
                    else
                    {
                        page = queryBySession(sessionID, pageNo);
                        //再次查询结果仍未null，则返回空结果
                        if (page == null)
                        {
                            log.Info("QueryOnlineListBySession Method: 缓存查询--已将查询结果缓存至本地，再次冲缓存查询时未查到： return page is null");
                            return result;
                        }
                    }
                }

                JArray activityInfoList = new JArray();

                for (int i = 0; i < page.dataTable.Rows.Count; i++)
                {
                    ActivityInfo activityInfo = new ActivityInfo();
                    activityInfo.code = (string)page.dataTable.Rows[i]["Code"];
                    activityInfo.name = (string)page.dataTable.Rows[i]["Name"];
                    activityInfo.auditStatus = (string)page.dataTable.Rows[i]["AuditStatus"];
                    activityInfo.activityImg = (string)page.dataTable.Rows[i]["CoverImg"];
                    activityInfo.praiseNo = (int)page.dataTable.Rows[i]["PraiseNo"];
                    activityInfo.shareNo = (int)page.dataTable.Rows[i]["ShareNo"];
                    activityInfo.supportNo = (int)page.dataTable.Rows[i]["SupportNo"];
                    if (page.dataTable.Rows[i]["CommentNo"] == DBNull.Value)
                    {
                        activityInfo.subItemNo = 0;
                    }
                    else
                    {
                        activityInfo.subItemNo = (int)page.dataTable.Rows[i]["CommentNo"];
                    }

                    activityInfoList.Add((JObject)JToken.FromObject(activityInfo));
                }

                result.Add("sessionId", sessionID);
                result.Add("pageNo", page.cursor);
                result.Add("pageSum", page.sum);
                result.Add("activityInfoList", activityInfoList);

                log.Info("QueryOnlineListBySession Return Success ");

            }
            catch (Exception e)
            {
                result.Add("error", "服务器报错!");
                log.Error(e.Message);
                log.Error(e.StackTrace);
            }
            return result;
        }

        /**
         *（分页查询）已发布线线下活动的列表查询，分页显示
          * 首次查询时，sessionId = "-1"， pageNo = 0
          * 后续查询时，可通过sessionId和pageNo来指定查询的页数，若pageNo为0，则默认为顺序查询
          */
        [Route("QueryOfflineListBySession")]
        [HttpGet]
        public JObject QueryOfflineListBySession(string sessionId, short pageNo)
        {
            //注释1：用户发布活动
            //注释2：查询用户创建的活动
            JObject result = new JObject();
            try
            {
                //获取creatorID
                string userId = CurrentUserCode;
                if (userId == null)
                {
                    result.Add("error", "获取用户信息失败!");
                    return result;
                }
                log.Info("QueryOfflineListBySession Method: the agruments SessionID = " + sessionId + ", PageNo = " + pageNo + ",UserId =" + userId);
                //初始化变量
                string sessionID = null;
                DefaultPageable page;

                //查询语句
                string sqlStr = "SELECT Code, Name, AuditStatus, CoverImg, SupportNo, PraiseNo, ShareNo, CommentNo, ProvinceCode, Province, CityCode, City, CreateTime"
                                 + " FROM YuanXinBusiness.zc.Project"
                                 + " WHERE Creator = '" + userId + "' AND Type = '" + (int)PROJECT_TYPE.Offline + "'"; ;
                //若sessionId传入为null，则返回空
                if (sessionId == null)
                {
                    log.Info("QueryOfflineListBySession Method: 传入参数： SessionID is null");
                    return result;
                }

                if (sessionId == "-1")
                {

                    sessionID = queryByPage(sqlStr);
                    if (sessionID == null)
                    {
                        log.Info("QueryOfflineListBySession Method: 直接查询--未查到数据： return sessionID is null,SQL=" + sqlStr);
                        return result;
                    }
                }
                else
                {
                    sessionID = sessionId;
                }

                page = queryBySession(sessionID, pageNo);
                //若获得的当前Page不包括sessionId所对应的数据，则再次查询
                if (page == null)
                {
                    sessionID = queryByPage(sqlStr);
                    if (sessionID == null)
                    {
                        log.Info("QueryOfflineListBySession Method: 缓存查询--未查到数据： return sessionID is null,SQL=" + sqlStr);
                        return result;
                    }
                    else
                    {
                        page = queryBySession(sessionID, pageNo);
                        //再次查询结果仍未null，则返回空结果
                        if (page == null)
                        {
                            log.Info("QueryOfflineListBySession Method: 缓存查询--已将查询结果缓存至本地，再次冲缓存查询时未查到： return page is null");
                            return result;
                        }
                    }
                }

                JArray activityInfoList = new JArray();

                for (int i = 0; i < page.dataTable.Rows.Count; i++)
                {
                    ActivityInfo activityInfo = new ActivityInfo();
                    activityInfo.code = (string)page.dataTable.Rows[i]["Code"];
                    activityInfo.name = (string)page.dataTable.Rows[i]["Name"];
                    activityInfo.auditStatus = (string)page.dataTable.Rows[i]["AuditStatus"];
                    activityInfo.activityImg = (string)page.dataTable.Rows[i]["CoverImg"];
                    activityInfo.praiseNo = (int)page.dataTable.Rows[i]["PraiseNo"];
                    activityInfo.shareNo = (int)page.dataTable.Rows[i]["ShareNo"];


                    string city = (string)page.dataTable.Rows[i]["City"];
                    string province = (string)page.dataTable.Rows[i]["Province"];
                    if (String.Equals(province, city))
                    {
                        activityInfo.cityName = (string)page.dataTable.Rows[i]["Province"];
                    }
                    else
                    {
                        activityInfo.cityName = (string)page.dataTable.Rows[i]["Province"] + (string)page.dataTable.Rows[i]["City"];
                    }

                    activityInfo.supportNo = (int)page.dataTable.Rows[i]["SupportNo"];
                    if (page.dataTable.Rows[i]["CommentNo"] == DBNull.Value)
                    {
                        activityInfo.subItemNo = 0;
                    }
                    else
                    {
                        activityInfo.subItemNo = (int)page.dataTable.Rows[i]["CommentNo"]; //话题数量
                    }

                    activityInfoList.Add((JObject)JToken.FromObject(activityInfo));
                }

                result.Add("sessionId", sessionID);
                result.Add("pageNo", page.cursor);
                result.Add("pageSum", page.sum);
                result.Add("activityInfoList", activityInfoList);

                log.Info("QueryOfflineListBySession Return Success : " + result);
            }
            catch (Exception e)
            {
                result.Add("error", "服务器报错!");
                log.Error(e.Message);
                log.Error(e.StackTrace);
            }
            return result;
        }

        #endregion

        #region 活动详情查询

        /**
         * 该方法用于查询线上活动详情
         */
        [Route("QueryOnlineInfo")]
        [HttpGet]
        public JObject QueryOnlineInfo(string activityCode)
        {
            JObject result = new JObject();
            try
            {
                log.Info("QueryOnlineInfo Method: the agruments activityCode = " + activityCode);
                OnlineActivityDetail activityDetail = new OnlineActivityDetail();

                //获得活动信息（activity）-name、coverImg、enrollDeadline、EndTime、worksSelectedType、AuditStatus
                DataTable activity = queryProjectByCode(activityCode);
                if (activity == null || activity.Rows.Count == 0)
                {
                    log.Info("This ActivityCode doesn't exist! -- activityCode: " + activityCode);
                    return result;
                }
                activityDetail.name = (string)activity.Rows[0]["Name"];
                activityDetail.coverImg = (string)activity.Rows[0]["CoverImg"];
                activityDetail.enrollDeadline = ((DateTime)activity.Rows[0]["EnrollDeadline"]).ToString(DateFormat);
                activityDetail.endTime = ((DateTime)activity.Rows[0]["EndTime"]).ToString(DateFormat);
                activityDetail.worksSelectedType = (int)activity.Rows[0]["WorksSelectedType"];
                activityDetail.auditStatus = (string)activity.Rows[0]["AuditStatus"];
                activityDetail.supportNo = (int)activity.Rows[0]["SupportNo"];
                activityDetail.systemTime = DateTime.Now.ToString(DateFormat);

                //获得审核信息（opinion）-审核意见、审核时间
                DataTable opinion = queryByResourceID(tableNamePrefix_ZC + "Opinion", activityCode);
                if (opinion == null || opinion.Rows.Count == 0)
                {
                    activityDetail.opinion = "";
                }
                else
                {
                    activityDetail.opinion = (string)opinion.Rows[0]["Content"];
                    activityDetail.auditDate = ((DateTime)opinion.Rows[0]["CreateTime"]).ToString(DateFormat);
                }
                //获取是否已开奖状态
                Boolean isOpenPrize = checkPrizeStatus(activityCode);
                activityDetail.isOpenPrize = isOpenPrize;

                //获取奖项信息，code、awardsName、startRanking、stopRanking、awardsContent、奖品图片
                DataTable awardsSetting = queryByProjectCode(tableNamePrefix_ZC + "AwardsSetting", activityCode);

                //按照sortNo排序
                awardsSetting.DefaultView.Sort = "SortNo ASC";
                awardsSetting = awardsSetting.DefaultView.ToTable();

                JArray awardsSettingList = new JArray();
                for (int i = 0; i < awardsSetting.Rows.Count; i++)
                {

                    JObject awardsInfo = new JObject();
                    string awardsCode = (string)awardsSetting.Rows[i]["Code"];
                    awardsInfo.Add("code", awardsCode);
                    awardsInfo.Add("awardsName", (string)awardsSetting.Rows[i]["AwardsName"]);
                    awardsInfo.Add("startRanking", (int)awardsSetting.Rows[i]["StartRanking"]);
                    awardsInfo.Add("stopRanking", (int)awardsSetting.Rows[i]["StopRanking"]);
                    awardsInfo.Add("awardsContent", (string)awardsSetting.Rows[i]["AwardsContent"]);

                    //添加奖项图片信息
                    DataTable awardsImage = queryAttachmentByResourceID(awardsCode);

                    JArray awardsImageList = new JArray();
                    foreach (DataRow dr in awardsImage.Rows)
                    {
                        JObject image = new JObject();
                        image.Add("URL", (string)dr["URL"]);
                        awardsImageList.Add(image);
                    }
                    awardsInfo.Add("awardsImage", awardsImageList);

                    awardsSettingList.Add(awardsInfo);
                }

                //获取活动直播信息
                JArray broadcastList = getBroadcastList(activityCode);

                result = JObject.FromObject(activityDetail);
                result.Add("awardsSettingList", awardsSettingList);
                result.Add("broadcastList", broadcastList);

                log.Info("QueryOnlineInfo return success!");
            }
            catch (Exception e)
            {
                result.Add("error", "服务器报错!");
                log.Error(e.Message);
                log.Error(e.StackTrace);
            }

            return result;

        }

        /**
         * 该方法用于查询线下活动详情，包括场次详细信息
         */
        [Route("QueryOfflineInfo")]
        [HttpGet]
        public JObject QueryOfflineInfo(string activityCode)
        {

            JObject result = new JObject();
            try
            {
                log.Info("QueryOfflineInfo Method: the agruments activityCode = " + activityCode);
                OfflineActivityDetail activityDetail = new OfflineActivityDetail();

                //获得活动信息（activity）-name、coverImg、startTime、EndTime、address、enrollNo、AuditStatus
                DataTable activity = queryProjectByCode(activityCode);
                if (activity == null || activity.Rows.Count == 0)
                {
                    log.Info("This ActivityCode doesn't exist! -- activityCode: " + activityCode);
                    return result;
                }
                activityDetail.name = (string)activity.Rows[0]["Name"];
                activityDetail.coverImg = (string)activity.Rows[0]["CoverImg"];
                activityDetail.startTime = ((DateTime)activity.Rows[0]["StartTime"]).ToString(DateFormat);
                activityDetail.endTime = ((DateTime)activity.Rows[0]["EndTime"]).ToString(DateFormat);
                activityDetail.address = (string)activity.Rows[0]["Address"];
                activityDetail.auditStatus = (string)activity.Rows[0]["AuditStatus"];
                activityDetail.supportNo = (int)activity.Rows[0]["SupportNo"];
                activityDetail.systemTime = DateTime.Now.ToString(DateFormat);

                string city = (string)activity.Rows[0]["City"];
                string province = (string)activity.Rows[0]["Province"];
                if (String.Equals(province, city))
                {
                    activityDetail.cityName = province;
                }
                else
                {
                    activityDetail.cityName = province + city;
                }

                //获得审核信息（opinion）-审核意见、审核时间
                DataTable opinion = queryByResourceID(tableNamePrefix_ZC + "Opinion", activityCode);
                if (opinion == null || opinion.Rows.Count == 0)
                {
                    activityDetail.opinion = "";
                }
                else
                {
                    activityDetail.opinion = (string)opinion.Rows[0]["Content"];
                    activityDetail.auditDate = ((DateTime)opinion.Rows[0]["CreateTime"]).ToString(DateFormat);
                }

                //获得场次信息：开始时间StartTime、场次时长Hours、场次金额Price、限制人数LimitNo
                DataTable activityEvent = queryByProjectCode(tableNamePrefix_ZC + "ActivityEvent", activityCode);
                //按照sortNo排序
                activityEvent.DefaultView.Sort = "SortNo ASC";
                activityEvent = activityEvent.DefaultView.ToTable();

                JArray activityEventInfoList = new JArray();
                for (int i = 0; i < activityEvent.Rows.Count; i++)
                {
                    ActivityEventInfo eventInfo = new ActivityEventInfo();
                    eventInfo.startTime = ((DateTime)activityEvent.Rows[i]["StartTime"]).ToString(DateFormat);
                    eventInfo.hours = float.Parse(activityEvent.Rows[i]["Hours"].ToString());
                    eventInfo.endTime = ((DateTime)activityEvent.Rows[i]["StartTime"]).AddHours(eventInfo.hours).ToString(DateFormat);
                    eventInfo.price = float.Parse(activityEvent.Rows[i]["Price"].ToString());
                    eventInfo.limitNo = (int)activityEvent.Rows[i]["LimitNo"];

                    activityEventInfoList.Add((JObject)JToken.FromObject(eventInfo));
                }

                //获取直播列表信息
                JArray broadcastList = getBroadcastList(activityCode);

                result = JObject.FromObject(activityDetail);
                result.Add("activityEventList", activityEventInfoList);
                result.Add("broadcastList", broadcastList);

                log.Info("QueryOfflineInfo Return Success ");
            }
            catch (Exception e)
            {
                result.Add("error", "服务器报错!");
                log.Error(e.Message);
                log.Error(e.StackTrace);
            }
            return result;
        }

        #endregion

        #region 线下活动的订单列表管理

        /**
         * （分页查询）线下活动的订单管理，获取活动所有的订单列表，里面包括订单详情
         * 首次查询时，sessionId = -1， pageNo = 0
         * 后续查询时，可通过sessionId和pageNo来指定查询的页数，若pageNo为0，则默认为顺序查询，直至最后一页
         * 若需要重新查询（即更新查询结果）时，sessionId = -1， pageNo = 0
         */
        [Route("QueryOfflineOrderBySession")]
        [HttpGet]
        public JObject QueryOfflineOrderBySession(string activityCode, ORDER_STATUS orderStatus, string sessionId, short pageNo)
        {
            log.Info("QueryOfflineOrderBySession Method: the agruments activityCode = " + activityCode + ", orderStatus = " + orderStatus + ", sessionId = " + sessionId + ", pageNo = " + pageNo);

            JObject result = new JObject();
            try
            {
                string sessionID = null;
                DefaultPageable page;

                string sqlStr = null;

                //若sessionId传入为null，则返回空
                if (sessionId == null)
                {
                    return result;
                }
                if (orderStatus == ORDER_STATUS.All)
                {
                    sqlStr = string.Format(@"SELECT AA.Code,AA.Receiver,AA.Phone,AA.OrderNo,AA.ProjectCode,AA.CreateTime,AA.PayWay,MAX(Status) AS Status FROM (
                                            SELECT A.Code,Receiver, Phone, OrderNo, ProjectCode, A.CreateTime, PayWay,
                                            CASE WHEN A.Status!='20' THEN A.Status WHEN A.Total=0 THEN '21' WHEN ti.TradeNo IS NULL THEN '20' 
                                            WHEN ti.TradeState=1 THEN '21' WHEN ti.TradeState=0 THEN '20' END AS Status 
                                            FROM [YuanXinBusiness].[zc].[Order] as A 
                                            Left join [AppPay].[TransactionInfo] ti ON  A.Code=ti.ObjectId
                                            LEFT join YuanXinBusiness.zc.OrderAddress as B  on B.OrderCode = A.Code
                                            ) AA
                                            WHERE AA.ProjectCode = '{0}'
                                            GROUP BY AA.Receiver,AA.Phone,AA.OrderNo,AA.ProjectCode,AA.CreateTime,AA.PayWay,AA.Code
                    ", activityCode);
                }
                else if (orderStatus == ORDER_STATUS.Offline_NotEnroll || orderStatus == ORDER_STATUS.Offline_Enrolled
                       || orderStatus == ORDER_STATUS.Offline_Signed || orderStatus == ORDER_STATUS.Offline_Evaluated)
                {
                    sqlStr = string.Format(@"SELECT * FROM ( 
                                            SELECT AA.Code,AA.Receiver,AA.Phone,AA.OrderNo,AA.ProjectCode,AA.CreateTime,AA.PayWay,MAX(Status) AS Status FROM
                                            (SELECT A.Code,Receiver, Phone, OrderNo, ProjectCode, A.CreateTime, PayWay, 
                                            CASE WHEN A.Status!='20' THEN A.Status WHEN A.Total=0 THEN '21' WHEN ti.TradeNo IS NULL THEN '20' 
                                            WHEN ti.TradeState=1 THEN '21' WHEN ti.TradeState=0 THEN '20' END AS Status 
                                            FROM [YuanXinBusiness].[zc].[Order] as A 
                                            Left join [AppPay].[TransactionInfo] ti ON  A.Code=ti.ObjectId
                                            LEFT join YuanXinBusiness.zc.OrderAddress as B  on B.OrderCode = A.Code
                                            ) AA
                                            WHERE AA.ProjectCode = '{0}'
                                            GROUP BY AA.Receiver,AA.Phone,AA.OrderNo,AA.ProjectCode,AA.CreateTime,AA.PayWay,AA.Code
                                            ) BB
                                            WHERE BB.Status='{1}'
                    ", activityCode, (int)orderStatus);
                }
                else
                {
                    return result;
                }

                if (sessionId == "-1")
                {
                    sessionID = queryByPage(sqlStr);
                    if (sessionID == null)
                    {
                        return result;
                    }
                }
                else
                {
                    sessionID = sessionId;
                }

                page = queryBySession(sessionID, pageNo);

                //生成线下活动订单
                result = createOfflineOrderList(page);
                result.Add("sessionId", sessionID);

                log.Info("QueryOfflineOrderBySession Return Success ");
            }
            catch (Exception e)
            {
                result.Add("error", "服务器报错！");
                log.Error(e.Message);
                log.Error(e.StackTrace);
            }
            return result;

        }

        #endregion

        #region 线上活动的作品管理
        /**
         * 分页查询-线上活动的作品管理，获取活动所有的订单列表
         * 首次查询时，sessionId = -1， pageNo = 0
         * 后续查询时，可通过sessionId和pageNo来指定查询的页数，若pageNo为0，则默认为顺序查询，直至最后一页
         * 若需要重新查询（即更新查询结果）时，sessionId = -1， pageNo = 0
         */
        [Route("QueryOnlineWorkBySession")]
        [HttpGet]
        public JObject QueryOnlineWorkBySession(string activityCode, SORT sort, string sessionId, short pageNo)
        {

            Log4netInstance.Info("QueryOnlineWorkBySession Method: the agruments activityCode = " + activityCode + ", sort = " + sort + ", sessionId = " + sessionId + ", pageNo = " + pageNo);
            string sessionID = null;
            DefaultPageable page;
            JObject result = new JObject();
            string sqlStr = null;
            string sortStr = null;
            if (sort == SORT.ByVote)
            {
                sortStr = "VoteCount DESC, CreateTime ASC";
            }
            else if (sort == SORT.ByTime)
            {
                sortStr = "CreateTime DESC";
            }
            else
            {
                return result;
            }

            //若sessionId传入为null，则返回空
            if (sessionId == null)
            {
                return result;
            }

            //sqlStr = "SELECT Distinct(A.Code), A.VoteCount ,A.Creator, A.CreateTime, A.Content, B.RealName, B.Phone, C.Url"
            //    + " FROM YuanXinBusiness.zc.ActivityWorks AS A"
            //    + " JOIN YuanXinBusiness.zc.UserInfo AS B on A.Creator = B.Creator"
            //    + " , YuanXinBusiness.zc.ImageLib AS C"
            //    + " WHERE A.ProjectCode = '" + activityCode + "'"
            //    + " AND C.ResourceID = A.Code ";


            //???商家端用户信息，，也需要调整吗？？？
            sqlStr += string.Format(@"
                                    SELECT Distinct(A.Code), A.VoteCount ,A.Creator, A.CreateTime, A.Content,sea.DisplayName AS RealName, C.Url
                                    FROM YuanXinBusiness.zc.ActivityWorks AS A
                                    --LEFT JOIN YuanXinBusiness.zc.UserInfo AS B on A.Creator = B.Creator
                                    LEFT JOIN YuanXinBusiness.OAuth.Seagull2 AS sea ON sea.UserId=A.Creator
                                    LEFT JOIN YuanXinBusiness.Business.Attachment AS C ON C.ResourceID = A.Code
                                    WHERE A.ProjectCode = '{0}'
                                    ", activityCode);


            if (sessionId == "-1")
            {
                sessionID = queryByPage(sqlStr, sortStr);

                if (sessionID == null)
                {
                    return result;
                }
            }
            else
            {
                sessionID = sessionId;
            }

            page = queryBySessionAll(sessionID, pageNo);

            short pageSum = 1;
            //生成线上活动作品的列表
            result = createOnlineWorkListOnline(page, pageNo, DefaultPageable.PREPAGE, out pageSum);
            result.Remove("pageSum");
            result.Add("pageSum", pageSum);

            result.Add("sessionId", sessionID);

            log.Info("QueryOnlineWorkBySession Return Success! ");
            return result;
        }

        #endregion

        #region 根据ActivityWorks的Code获得该作品所有的图片
        /**
         * 根据ActivityWorks的Code获得该作品所有的图片
         */
        [Route("GetActivityWorksImage")]
        [HttpGet]
        public JArray GetActivityWorksImage(string worksCode)
        {
            JArray result = new JArray();
            string sqlStr = "SELECT URL AS URL FROM Business.Attachment WHERE ResourceID = '" + worksCode + "'";

            DataTable worksImage = query(sqlStr);

            //对url做处理，如果不是绝对路径，需要加上路径前缀
            for (int i = 0; i < worksImage.Rows.Count; i++)
            {
                string url = (string)worksImage.Rows[i]["URL"];

                if (!isHttp(url))
                {
                    url = worksImagePath_Test + url;
                }

                JObject worksImageUrl = new JObject();
                worksImageUrl.Add("URL", url);
                result.Add(worksImageUrl);
            }

            log.Info("GetActivityWorksImage Return Success. ");
            return result;

        }

        #endregion

        #region 活动的作品搜索

        /**
         * 分页 - 线上活动的作品搜索，通过作品作者姓名、手机号进行搜索
         * 该方法用于首次查询
         */
        [Route("SearchOnlineWorksBySession")]
        [HttpGet]
        public JObject SearchOnlineWorksBySession(string activityCode, string queryValue)
        {
            log.Info("SearchOnlineWorksBySession Method: the agruments activityCode = " + activityCode + "queryValue = " + queryValue);
            JObject result = new JObject();
            //判断搜索关键字的类型
            if (Regex.IsMatch(queryValue, @"^[1]+[3,5,7,8]+\d{9}$"))//输入为手机号
            {
                result = SearchOnlineWorks(activityCode, QUERY_TYPE.ByPhone, queryValue);
            }
            else
            {
                result = SearchOnlineWorks(activityCode, QUERY_TYPE.ByName, queryValue);
            }

            log.Info("SearchOnlineWorksBySession Return Success");
            return result;
        }

        /**
         * 分页 - 线上活动的作品搜索，通过作品作者姓名、手机号进行搜索
         * 该方法用于再次查询
         */
        [Route("SearchOnlineWorksBySession")]
        [HttpGet]
        public JObject SearchOnlineWorksBySession(string sessionId, short pageNo)
        {
            log.Info("SearchOnlineWorksBySession Method: the agruments sessionId = " + sessionId + "pageNo = " + pageNo);
            DefaultPageable page;
            JObject result = new JObject();

            //若sessionId传入为null，则返回空
            if (sessionId == null)
            {
                return result;
            }

            page = queryBySession(sessionId, pageNo);

            //若获得的当前Page不包括sessionId所对应的数据，则再次查询
            if (page == null)
            {
                return result;
            }

            //OnsellOrderList 生成
            result = createOnlineWorkList(page);
            result.Add("sessionId", sessionId);

            log.Info("SearchOnlineWorksBySession Return Success");
            return result;

        }

        #endregion

        #region 线下活动的订单搜索

        /**
         * 分页- 线下活动的订单搜索, 通过订单号、姓名、手机号进行搜索  
         * 该方法用于首次查询
         */
        [Route("SearchOfflineOrderBySession")]
        [HttpGet]
        public JObject SearchOfflineOrderBySession(string activityCode, string queryValue)
        {

            log.Info("SearchOfflineOrderBySession Method: the agruments activityCode = " + activityCode + "queryValue = " + queryValue);
            JObject result = new JObject();

            //判断搜索关键字的类型
            if (Regex.IsMatch(queryValue, @"^[1]+[3,5,7,8]+\d{9}$"))//输入为手机号
            {
                result = SearchOfflineOrder(activityCode, QUERY_TYPE.ByPhone, queryValue);
            }
            else if (queryValue.Length == 12)//输入为订单号
            {
                result = SearchOfflineOrder(activityCode, QUERY_TYPE.ByOrderNo, queryValue);
            }
            else//输入为姓名
            {
                result = SearchOfflineOrder(activityCode, QUERY_TYPE.ByReceiver, queryValue);
            }

            log.Info("SearchOfflineOrderBySession Return Success");
            return result;

        }

        /**
         * 分页- 线下活动的订单搜索, 通过订单号、姓名、手机号进行搜索
         * 该方法用于订单首次搜索的后续结果获取
         */
        [Route("SearchOfflineOrderBySession")]
        [HttpGet]
        public JObject SearchOfflineOrderBySession(string sessionId, short pageNo)
        {
            log.Info("SearchOfflineOrderBySession Method: the agruments sessionId = " + sessionId + "pageNo = " + pageNo);
            DefaultPageable page;
            JObject result = new JObject();

            //若sessionId传入为null，则返回空
            if (sessionId == null)
            {
                return result;
            }

            page = queryBySession(sessionId, pageNo);

            //若获得的当前Page不包括sessionId所对应的数据，则再次查询
            if (page == null)
            {
                return result;
            }

            //OfflineOrder 生成
            result = createOfflineOrderList(page);
            result.Add("sessionId", sessionId);

            log.Info("SearchOfflineOrderBySession Return Success");
            return result;
        }

        #endregion

        #region 获取活动标签列表

        /**
         * 该方法用于获取活动标签列表
         */
        [Route("GetActivityTag")]
        [HttpGet]
        public JArray GetActivityTag()
        {

            JArray tagArray = new JArray();
            JObject activityTag = new JObject();
            //从数据库获取活动标签模板
            string sqlStr = "SELECT Code AS tagCode , Name AS tagName  FROM zc.TagTemplate Where Type='1'";
            DataTable result = query(sqlStr);

            if (result == null || result.Rows.Count == 0)
            {
                return tagArray;
            }
            //生成JObject结果
            foreach (DataRow dr in result.Rows)
            {
                JObject tagJson = new JObject();
                tagJson.Add("tagCode", (string)dr["tagCode"]);
                tagJson.Add("tagName", (string)dr["tagName"]);

                tagArray.Add(tagJson);
            }
            log.Info("GetActivityTag Return Success");
            return tagArray;
        }

        #endregion

        #region 线下验票

        /**
         * 该方法用于线下活动验票
         */
        [Route("CheckTicket")]
        [HttpGet]
        public Boolean CheckTicket(string activityCode, string orderNo, string userId = "")
        {
            //get userid
            if (userId.IsEmptyOrNull())
            {
                userId = CurrentUserCode;
            }
            if (userId.IsEmptyOrNull())
            {
                Log4netInstance.Info("用户信息获取失败");
                return false;
            }
            Log4netInstance.Info("CheckTicket Method1 : The agruments activityCode = " + activityCode + ", orderNo = " + orderNo + ", userId = " + userId);

            //已确认状态
            int orderStatus;
            List<string> sqlArray = new List<string>();
            string sqlStr;
            int status;
            string orderCode;
            //判断project类型
            DataTable project = queryProjectByCode(activityCode);

            if (project == null || project.Rows.Count == 0)
            {
                return false;
            }
            else
            {
                //确认订单是否存在
                DataTable order = queryOrderByNo(activityCode, orderNo);
                if (order == null || order.Rows.Count == 0)
                {
                    return false;
                }
                status = int.Parse((string)order.Rows[0]["Status"]);

                orderCode = (string)order.Rows[0]["Code"];

                Order newOrder = OrderAdapter.Instance.LoadByCodeUpdateStatus(orderCode);
                status = (int)newOrder.Status;

                //确认订单状态是否为已支付状态
                if (status != (int)ORDER_STATUS.Offline_Enrolled)
                {
                    return false;
                }
                //更新订单状态
                orderStatus = (int)ORDER_STATUS.Offline_Signed;
                sqlStr = "UPDATE [YuanXinBusiness].[zc].[Order]"
                    + " SET Status = '" + orderStatus + "' , ScanTime = '" + DateTime.Now + "'"
                    + " WHERE OrderNo = '" + orderNo + "'"
                    + " And ProjectCode = '" + activityCode + "'";

                Log4netInstance.Info(sqlStr);

                SqlDbHelper db = new SqlDbHelper();
                int result = db.ExecuteNonQuery(sqlStr);

                if (result == 1)
                {
                    Log4netInstance.Info("CheckTicket Success!");
                    return true;
                }
                else
                {
                    Log4netInstance.Info("CheckTicket Failed!");
                    return false;
                }
            }
        }

        #endregion

        #region 线上活动中奖详情

        /**
         * 该方法用于获取线上活动中奖详情
         */
        [Route("GetWinningWorksBySession")]
        [HttpGet]
        public JObject GetWinningWorksBySession(string activityCode, string awardsCode, string sessionId, short pageNo)
        {
            log.Info("GetWinningWorksBySession Method: the agruments activityCode = " + activityCode + ", awardsCode = " + awardsCode + ", SessionID = " + sessionId + ", PageNo = " + pageNo);
            string sessionID = null;
            DefaultPageable page;
            JObject result = new JObject();
            string sqlStr;
            string sortStr = "VoteCount DESC, CreateTime ASC";


            //若sessionId传入为null，则返回空
            if (sessionId == null)
            {
                return result;
            }
            //获取奖项信息
            string tableName = tableNamePrefix_ZC + "AwardsSetting";
            DataTable awardsSetting = queryByCode(tableName, awardsCode);
            if (awardsSetting == null || awardsSetting.Rows.Count == 0)
            {
                return result;
            }
            int startRanking = (int)awardsSetting.Rows[0]["StartRanking"];
            int stopRanking = (int)awardsSetting.Rows[0]["StopRanking"];

            //for zc.ImageLib
            sqlStr = "SELECT Distinct(A.Code), A.VoteCount ,A.Creator, A.CreateTime, A.Content, sea.DisplayName AS RealName, C.Url"
                + " FROM YuanXinBusiness.zc.ActivityWorks AS A"
                + " LEFT JOIN YuanXinBusiness.OAuth.Seagull2 AS sea ON sea.UserId=A.Creator"
                + " LEFT JOIN YuanXinBusiness.Business.Attachment AS C ON C.ResourceID = A.Code"
                + " WHERE A.ProjectCode = '" + activityCode + "'";


            if (sessionId == "-1")
            {
                sessionID = queryByPage(sqlStr, sortStr, startRanking, stopRanking);

                if (sessionID == null)
                {
                    return result;
                }
            }
            else
            {
                sessionID = sessionId;
            }

            page = queryBySession(sessionID, pageNo);

            //生成线上活动作品的列表
            result = createOnlineWorkList(page);
            result.Add("sessionId", sessionID);

            log.Info("GetWinningWorksBySession Return Success ");
            return result;

        }

        #endregion

        #region 线上活动颁奖结果的确认

        /**
        * 该方法用于线上活动颁奖结果的确认
        */
        [Route("ConfirmPrizeResult")]
        [HttpGet]
        public Boolean ConfirmPrizeResult(string activityCode)
        {

            log.Info("ConfirmPrizeResult Method: the agruments activityCode： " + activityCode);

            if (checkPrizeStatus(activityCode))
            {
                log.Error("Activity has already Presented Prize!");
                return false;
            }

            //获得奖项信息
            string tableName = tableNamePrefix_ZC + "AwardsSetting";
            DataTable awardsSetting = queryByProjectCode(tableName, activityCode);

            if (awardsSetting == null || awardsSetting.Rows.Count == 0)
            {
                return false;
            }

            ArrayList prizeResult = new ArrayList();
            for (int i = 0; i < awardsSetting.Rows.Count; i++)
            {
                int startRanking = (int)awardsSetting.Rows[i]["StartRanking"];
                int stopRanking = (int)awardsSetting.Rows[i]["StopRanking"];
                string awardsSettingCode = (string)awardsSetting.Rows[i]["Code"];

                tableName = tableNamePrefix_ZC + "ActivityWorks";
                DataTable activityWorks = queryByProjectCode(tableName, activityCode);
                //作品按投票数量进行排序,若票数相同则按提交先后顺序排序
                activityWorks.DefaultView.Sort = "VoteCount DESC, CreateTime ASC";
                activityWorks = activityWorks.DefaultView.ToTable();

                List<WorkAwardsBean> workAwardsList = new List<WorkAwardsBean>();
                for (int j = startRanking - 1; j < stopRanking; j++)
                {
                    //若作品数量少于奖项设置的名次，则结束颁奖
                    if (activityWorks.Rows.Count <= j)
                    {
                        break;
                    }
                    WorkAwardsBean workAwards = new WorkAwardsBean();
                    workAwards.Code = Guid.NewGuid().ToString();
                    workAwards.ProjectCode = activityCode;
                    workAwards.ActivityWorksCode = (string)activityWorks.Rows[j]["Code"];
                    workAwards.AwardsSettingCode = awardsSettingCode;

                    workAwardsList.Add(workAwards);
                }

                prizeResult.Add(workAwardsList);
            }

            //存储中奖结果
            if (savePrizeResult(prizeResult))
            {

                log.Info("ConfirmPrizeResult Success! ");
                return true;

            }
            else
            {

                log.Info("ConfirmPrizeResult failed! ");
                return false;

            }

        }

        #endregion

        #region 获取线上活动奖项信息

        /**
         * 该方法用于获取线上活动奖项信息
         */
        [Route("PresentPrize")]
        [HttpGet]
        public JArray PresentPrize(string activityCode)
        {
            JArray awardsArray = new JArray();
            string tableName = tableNamePrefix_ZC + "AwardsSetting";
            DataTable awardsSetting = queryByProjectCode(tableName, activityCode);

            awardsSetting.DefaultView.Sort = "SortNo ASC";
            awardsSetting = awardsSetting.DefaultView.ToTable();

            int worksCount = countActivityWorks(activityCode);

            foreach (DataRow dr in awardsSetting.Rows)
            {
                JObject awards = new JObject();
                awards.Add("code", (string)dr["Code"]);
                awards.Add("awardsName", (string)dr["AwardsName"]);
                awards.Add("startRanking", (int)dr["StartRanking"]);
                int stopRanking = (int)dr["StopRanking"];
                awards.Add("stopRanking", stopRanking);

                int awardsCount = (int)dr["AwardsCount"];
                if (stopRanking > worksCount)
                {
                    awardsCount = awardsCount - (stopRanking - worksCount);
                    if (awardsCount < 0)
                    {
                        awardsCount = 0;
                    }
                }
                awards.Add("awardsCount", awardsCount);

                awardsArray.Add(awards);
            }

            return awardsArray;
        }

        #endregion

        #region 发布活动直播

        /**
        * 该接口用于“发布活动直播”
        */
        [Route("PublishActivityBroadcast")]
        [HttpPost]
        public Boolean PublishActivityBroadcast([FromBody]JObject broadcastJson)
        {
            string userId = CurrentUserCode;
            if (userId == null)
            {
                return false;
            }
            log.Info("PublishActivityBroadcast input : UserID : " + userId + ", data :" + broadcastJson);
            //判断数据是否合法
            if (broadcastJson == null || broadcastJson.Count == 0)
            {
                log.Info("PublishActivityBroadcast method input data is NULL.");
                return false;
            };

            //图片信息list
            List<AttachmentBean> attArray = new List<AttachmentBean>();

            string activityCode = (string)broadcastJson.SelectToken("code");
            JArray broadcastJArray = (JArray)broadcastJson.SelectToken("broadcastInfo");
            //初始化直播信息
            List<ActivityBroadcastBean> broadcastBeanList = new List<ActivityBroadcastBean>();
            //获得该活动已添加直播sortNo的最大值
            int maxSortNo = getBroadcastMaxSortNo(activityCode);

            if (!createActivivtyBroadcast(broadcastBeanList, broadcastJArray, activityCode, attArray, maxSortNo))
            {
                return false;
            }

            if (!saveActivityBroadcast(broadcastBeanList, attArray))
            {
                return false;
            }

            log.Info("PublishActivityBroadcast Return success");
            return true;
        }

        #endregion
    }
}