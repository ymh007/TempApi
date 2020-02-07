using MCS.Library.OGUPermission;
using Newtonsoft.Json.Linq;
using Seagull2.Core.Models;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Reflection;
using System.Web.Http;
using YuanXin.Framework.OAuth.Identity;

namespace Seagull2.YuanXin.AppApi.Models
{
    /**
     * 数据库增、删、改、查的通用方法类
     */
    public abstract class DefaultHelper : ApiController
    {
        private LogHelper log = LogFactory.GetLogger(MethodBase.GetCurrentMethod().DeclaringType);

        public static string tableNamePrefix_ZC = "YuanXinBusiness.zc.";

        public static string tableNamePrefix_BASE = "YuanXinBusiness.Base.";

        public static string tableNamePrefix_BS = "YuanXinBusiness.Business.";

        private static QueryCache localCache = QueryCache.getInstance();

        private static Dictionary<string, DefaultPageable> pages = new Dictionary<string, DefaultPageable>();

        public static string DateFormat = "yyyy/M/d HH:mm";

        public string CurrentUserCode
        {
            get
            {
                try
                {
                    return ((Seagull2Identity)User.Identity).Id;
                }
                catch (Exception e)
                {
                    try
                    {
                        OguObjectCollection<IUser> users = OguMechanismFactory.GetMechanism().GetObjects<IUser>(SearchOUIDType.LogOnName, User.Identity.Name);
                        return users[0].ID;
                    }
                    catch (Exception e1)
                    {
                        if (ConfigurationManager.AppSettings["IsTest"] == "true")
                        {
                            //测试模式
                            string userCode = "f4048590-feec-4c15-990d-2f7693146937";   //刘闽辉
                            //userCode = "ef31cf3b-a784-4cbe-bf74-a53a25be6559";   //周佳良
                            userCode = "d48366cc-cf21-b855-4e16-6f3230f4e71c";  //朱鹏树
                            //userCode = "fb6883f9-a5ae-8af7-4f0d-5dd92466c414";  //肖小勇
                            Log.WriteLog("获取用户编码失败，已采用测试用户编码：" + userCode);
                            return userCode;
                        }
                        else
                        {
                            Log.WriteLog("非测试模式下：获取用户编码失败!");
                            return null;
                        }
                    }
                }
            }
        }

        //project的类型
        public enum PROJECT_TYPE
        {
            Bargain = 1,  //特价房
            Onsell = 2,  //在售房
            Offline = 6, //线下活动，案场活动
            Online = 7  //线上活动，征集活动
        }

        //project的审核状态
        protected enum AUDIT_STATUS { None, Auditing, Failed, Success }

        //附件的类型
        protected enum ATT_TYPE_CODE
        {
            BuildingCoverImg = 10, //楼盘封面图片
            HouseTypeImg = 11, //户型图
            ActivityImg = 12, //活动图片
            ActivityDetailImg = 13, //活动详情图片
            AwardsImg = 14, //奖项图片
            BroadcastCoverImg = 15 //直播封面图片

        }

        //订单状态
        public enum ORDER_STATUS
        {
            All = -1, //全部订单
            Onsell_None = 0, //在售-待支付
            Onsell_Paid = 1, //在售-已支付
            Onsell_Refunded = 2, //在售-已退款
            Onsell_Confirmed = 3, //在售-已确认

            Bargain_NotShared = 10, //特价-待分享
            Bargain_Shared = 11, //特价-待现场确认（已分享）
            Bargain_Confirmed = 12, //特价-已确认（保留不使用）
            Bargain_Completed = 13, //特价-已完成

            Offline_NotEnroll = 20, //线下活动-未报名（待支付)
            Offline_Enrolled = 21, //线下活动-已报名
            Offline_Signed = 22,//线下活动-已签到
            Offline_Evaluated = 23//线下活动-已评价
        }

        //订单的支付方式 PayWay
        public enum PAY_WAY
        {
            None = 0,
            Alipay = 1, //支付宝
            Wxpay = 2 //微信支付
        }



        //根据token获得userId
        protected string getUserId()
        {
            //UserID test data for Unit TEST
            //string userId = "b231f35a76a346449b2f4c5ddb4f88e7";
            //UserID for run environment
            string userId = this.User.Identity.GetCurrentUserId();

            return userId;
        }


        /**
         * 通过Code查询Project的信息
         */
        protected DataTable queryProjectByCode(string projectCode)
        {

            string tableName = tableNamePrefix_ZC + "Project";
            Dictionary<string, Object> queryCondition = new Dictionary<string, Object>();
            queryCondition.Add("Code", projectCode);

            return query(tableName, queryCondition);

        }

        /**
         * 通过projectCode查询相关联表数据的通用方法
         */
        protected DataTable queryByProjectCode(string tableName, string projectCode)
        {

            Dictionary<string, Object> queryCondition = new Dictionary<string, Object>();
            queryCondition.Add("ProjectCode", projectCode);

            return query(tableName, queryCondition);

        }
        /**
         * 通过projectCode查询building数据的方法
         */
        protected DataTable queryBuildingByProjectCode(string projectCode)
        {

            DataTable result = null;

            DataTable project = queryProjectByCode(projectCode);
            if (project == null || project.Rows.Count == 0)
            {
                return result;
            }

            string buildingCode = (string)project.Rows[0]["BuildingCode"];

            result = queryByCode(tableNamePrefix_ZC + "Building", buildingCode);

            return result;
        }

        /**
         * 通过ResourceID查询相关表数据的通用方法
         */
        protected DataTable queryByResourceID(string tableName, string resourceID)
        {
            Dictionary<string, Object> queryCondition = new Dictionary<string, Object>();
            queryCondition.Add("ResourceID", resourceID);

            return query(tableName, queryCondition);
        }

        /**
         * 通过phone查询orderAddress信息
         */
        protected DataTable queryOrderAddressByPhone(string phone)
        {

            string tableName = tableNamePrefix_ZC + "OrderAddress";
            Dictionary<string, Object> queryCondition = new Dictionary<string, Object>();
            queryCondition.Add("Phone", phone);

            return query(tableName, queryCondition);
        }

        /**
         * 通过姓名查询orderAddress信息
         */
        protected DataTable queryOrderAddressByName(string name)
        {

            string tableName = tableNamePrefix_ZC + "OrderAddress";
            Dictionary<string, Object> queryCondition = new Dictionary<string, Object>();
            queryCondition.Add("Receiver", name);

            return query(tableName, queryCondition);
        }

        /**
         *  通过订单号projectCode, orderNo查询订单Order信息
         */
        protected DataTable queryOrderByNo(string projectCode, string orderNo)
        {

            string tableName = tableNamePrefix_ZC + "[Order]";
            Dictionary<string, Object> queryCondition = new Dictionary<string, Object>();
            queryCondition.Add("OrderNo", orderNo);
            queryCondition.Add("ProjectCode", projectCode);

            return query(tableName, queryCondition);
        }

        /**
         * 通过OrderCode查询订单地址OrderAddress
         */
        protected DataTable queryOrderAddressByOrder(string orderCode)
        {

            string tableName = tableNamePrefix_ZC + "OrderAddress";
            Dictionary<string, Object> queryCondition = new Dictionary<string, Object>();
            queryCondition.Add("OrderCode", orderCode);

            return query(tableName, queryCondition);
        }

        /**
         * 通过code查询对象信息
         */
        protected DataTable queryByCode(string tableName, string code)
        {

            Dictionary<string, Object> queryCondition = new Dictionary<string, Object>();
            queryCondition.Add("Code", code);

            return query(tableName, queryCondition);
        }

        /**
         * 通过resourceID查询attachment信息
         */
        protected DataTable queryAttachmentByResourceID(string resourceID)
        {
            string tableName = tableNamePrefix_BS + "Attachment";
            Dictionary<string, Object> queryCondition = new Dictionary<string, Object>();
            queryCondition.Add("ResourceID", resourceID);
            queryCondition.Add("VersionEndTime", DateTime.MaxValue);
            queryCondition.Add("ValidStatus", true);

            return query(tableName, queryCondition);
        }

        /**
         * 更新ActivityLottery到数据库
         */
        protected Boolean updateActivityLottery(ActivityLotteryBean activityLottery)
        {

            string sqlStr = "UPDATE " + tableNamePrefix_ZC + "ActivityLottery "
                + " SET BallCode = '" + activityLottery.BallCode + "',"
                + " LotteryTime = '" + activityLottery.LotteryTime + "',"
                + " OrderCode = '" + activityLottery.OrderCode + "',"
                + " LotteryResult = '" + activityLottery.LotteryResult + "',"
                + " QiHao = '" + activityLottery.LotteryNo + "'"
                + " WHERE ProjectCode = '" + activityLottery.ProjectCode + "'";


            SqlDbHelper db = new SqlDbHelper();
            int result = db.ExecuteNonQuery(sqlStr);

            if (result == 1)
            {
                return true;
            }
            else
            {
                return false;
            }

        }

        /**
         * 通用的数据库查询方法
         */
        protected DataTable query(string tableName, Dictionary<string, Object> queryCondition)
        {

            DataTable result = null;
            if (queryCondition == null)
            {
                return null;
            }

            int i = 0;
            string sqlStr = "SELECT * FROM " + tableName + " WHERE ";

            foreach (string key in queryCondition.Keys)
            {
                if (i == 0)
                {
                    sqlStr = sqlStr + key + "= '" + queryCondition[key] + "'";
                }
                else
                {
                    sqlStr = sqlStr + " AND " + key + "= '" + queryCondition[key] + "'";
                }
                i++;
            }
            SqlDbHelper db = new SqlDbHelper();
            result = db.ExecuteDataTable(sqlStr);

            return result;

        }

        /**
         * 通用的数据库查询方法，返回结果为数据集（DataTable）
         */
        protected DataTable query(string sqlString)
        {
            SqlDbHelper db = new SqlDbHelper();
            DataTable result = db.ExecuteDataTable(sqlString);
            return result;
        }



        /**
         * 分页方式查询，将查询结果存储到本地缓存，返回SessionID，该方法适用于单表查询
         */
        protected string queryByPage(string tableName, Dictionary<string, Object> queryCondition)
        {

            DataTable result = query(tableName, queryCondition);

            if (result == null || result.Rows.Count == 0)
                return null;

            return localCache.putQueryDataTable(result);

        }

        /**
         * 分页方式查询，将查询结果存储到本地缓存，返回sessionID，该方法可以用户自定义查询sql语句
         */
        protected string queryByPage(string sqlString)
        {
            SqlDbHelper db = new SqlDbHelper();
            DataTable result = db.ExecuteDataTable(sqlString);

            result.DefaultView.Sort = "CreateTime DESC";
            result = result.DefaultView.ToTable();

            if (result == null || result.Rows.Count == 0)
                return null;

            return localCache.putQueryDataTable(result);
        }

        /**
         * 分页方式查询，将查询结果根据指定排序方式存储到本地缓存，返回sessionID，该方法可以用户自定义查询sql语句
         */
        protected string queryByPage(string sqlString, string sort)
        {
            SqlDbHelper db = new SqlDbHelper();
            DataTable result = db.ExecuteDataTable(sqlString);
            result.DefaultView.Sort = sort;
            result = result.DefaultView.ToTable();

            if (result == null || result.Rows.Count == 0)
                return null;

            return localCache.putQueryDataTable(result);
        }

        /**
        * 分页方式查询，将查询结果根据指定排序方式存储到本地缓存，返回sessionID，该方法可以用户自定义查询sql语句
        * 该方法主要用于GetPrizaDetailInfoBySession方法，可以指定获取数据的起始位置
        */
        protected string queryByPage(string sqlString, string sort, int startNo, int endNo)
        {
            SqlDbHelper db = new SqlDbHelper();
            DataTable result = db.ExecuteDataTable(sqlString);
            result.DefaultView.Sort = sort;
            result = result.DefaultView.ToTable();

            if (result == null || result.Rows.Count == 0)
            {
                return null;
            }
            else
            {
                //获取指定位置的数据
                DataTable resultTemp = result.Copy();
                resultTemp.Clear();
                for (int i = startNo - 1; i < endNo; i++)
                {
                    if (i >= result.Rows.Count)
                    {
                        break;
                    }
                    resultTemp.Rows.Add(result.Rows[i].ItemArray);
                }

                result = resultTemp;
            }

            if (result.Rows.Count == 0)
            {
                return null;
            }

            return localCache.putQueryDataTable(result);
        }

        /**
         * 分页方式查询，获得SessionID对应的缓存数据，与queryByPage()配合使用
         */
        protected DefaultPageable queryBySession(string sessionId)
        {
            if (pages.ContainsKey(sessionId))
            {
                return queryBySession(sessionId, (short)(pages[sessionId].cursor + 1));
            }
            else
            {
                return queryBySession(sessionId, (short)0);
            }
        }

        /**
         * 分页方式查询，获得SessionID对应的缓存数据，并对更新page中的相关参数
         */
        protected DefaultPageable queryBySession(string sessionId, short pageNumber)
        {

            DefaultPageable page = null;

            DataTable dtList = localCache.pullQueryDataTable(sessionId);
            //本地缓存未存储sessionId对应的记录，则返回null
            if (dtList == null)
            {
                return null;
            }

            if (!pages.ContainsKey(sessionId))
            {

                page = new DefaultPageable();

                short sum = (short)(dtList.Rows.Count / DefaultPageable.PREPAGE);
                if (dtList.Rows.Count % DefaultPageable.PREPAGE != 0)
                {
                    sum++;
                }
                page.sum = sum;
            }
            else
            {
                page = pages[sessionId];
            }


            if (pageNumber != 0)
            {
                page.cursor = (short)(pageNumber - 1);
            }

            if (page.cursor < page.sum)
            {

                DataTable pageDT = dtList.Copy();
                pageDT.Clear();

                for (int i = 0; i < DefaultPageable.PREPAGE
                        && i + page.cursor * DefaultPageable.PREPAGE < dtList.Rows.Count; i++)
                {
                    pageDT.Rows.Add(dtList.Rows[i + page.cursor * DefaultPageable.PREPAGE].ItemArray);
                }
                page.cursor = (short)(page.cursor + 1);
                page.dataTable = pageDT;
            }


            if (!pages.ContainsKey(sessionId))
            {
                pages.Add(sessionId, page);
            }
            else
            {
                pages[sessionId] = page;
            }

            return page;
        }

        /**
        * 全部查询
        */
        protected DefaultPageable queryBySessionAll(string sessionId, short pageNumber)
        {

            DefaultPageable page = null;

            DataTable dtList = localCache.pullQueryDataTable(sessionId);
            //本地缓存未存储sessionId对应的记录，则返回null
            if (dtList == null)
            {
                return null;
            }

            if (!pages.ContainsKey(sessionId))
            {

                page = new DefaultPageable();

                short sum = (short)(dtList.Rows.Count / DefaultPageable.PREPAGE);
                if (dtList.Rows.Count % DefaultPageable.PREPAGE != 0)
                {
                    sum++;
                }
                page.sum = sum;
            }
            else
            {
                page = pages[sessionId];
            }


            if (pageNumber != 0)
            {
                page.cursor = (short)(pageNumber - 1);
            }

            if (page.cursor < page.sum)
            {

                DataTable pageDT = dtList.Copy();
                pageDT.Clear();

                for (int i = 0; i < dtList.Rows.Count; i++)
                {
                    pageDT.Rows.Add(dtList.Rows[i + page.cursor * DefaultPageable.PREPAGE].ItemArray);
                }
                page.cursor = (short)(page.cursor + 1);
                page.dataTable = pageDT;
            }


            if (!pages.ContainsKey(sessionId))
            {
                pages.Add(sessionId, page);
            }
            else
            {
                pages[sessionId] = page;
            }

            return page;
        }


        /**
         * 通过用户ID查询其所归属的公司ID
         */
        protected string queryCompanyIdByUser(string creator)
        {

            string companyId = null;
            string sqlStr = "select CompanyCode from Business.CompanyUser where UserID = '" + creator + "'";
            SqlDbHelper db = new SqlDbHelper();
            companyId = (string)db.ExecuteScalar(sqlStr, CommandType.Text);

            return companyId;
        }



        /*
         * 生成需要保存图片信息的列表
         */
        protected Boolean createImageList(List<AttachmentBean> attArray, JArray imageArray, string resourceID, int attachmentTypeCode)
        {
            if (imageArray == null || imageArray.Count == 0)
            {
                log.Info("ImageArray is null or empty!");
                return false;
            }

            foreach (JObject imageJson in imageArray)
            {
                AttachmentBean att = new AttachmentBean();

                if (!createImage(att, imageJson, resourceID, attachmentTypeCode))
                {
                    return false;
                }
                attArray.Add(att);
            }
            return true;
        }

        /*
         * 生成需要保存单个图片的信息
         */
        protected Boolean createImage(AttachmentBean att, JObject imgJson, string resourceID, int attachmentTypeCode)
        {

            try
            {
                att.Code = Guid.NewGuid().ToString();
                att.ResourceID = resourceID;
                att.CnName = (string)imgJson.SelectToken("cnName");
                att.URL = (string)imgJson.SelectToken("URL");
                att.FileSize = (int)imgJson.SelectToken("fileSize");
                att.Creator = CurrentUserCode; ;
                att.VersionStartTime = DateTime.Now.ToString(DateFormat);
                att.VersionEndTime = DateTime.MaxValue.ToString();
                att.ValidStatus = 1;
                att.SortNo = (int)imgJson.SelectToken("sortNo");
                att.Suffix = (string)imgJson.SelectToken("suffix");
                att.AttachmentTypeCode = attachmentTypeCode;
            }
            catch (Exception ex)
            {
                log.Error(" create Image Bean failed! -- " + ex);
                return false;
            }

            return true;

        }

        //判断省份是否是直辖市,是则返回true
        protected bool isProvinceCity(string cityCode, string cityName)
        {
            string tableName = tableNamePrefix_BASE + "Area";
            Dictionary<string, Object> queryCondition = new Dictionary<string, Object>();
            queryCondition.Add("AreaName", cityName);
            queryCondition.Add("Code", cityCode);

            DataTable area = query(tableName, queryCondition);
            if (area == null || area.Rows.Count == 0)
            {
                return false;
            }

            int areaType = (int)area.Rows[0]["AreaType"];
            if (areaType == 2)
            {
                return true;
            }
            else
            {
                return false;
            }

        }
    }
}