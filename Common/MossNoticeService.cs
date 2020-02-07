using System;
using System.Collections.Generic;
using System.Data;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using System.Web;
using System.Xml;
using System.Xml.Linq;
using log4net;
using Newtonsoft.Json;
using Seagull2.YuanXin.AppApi.NotificationsServer;

namespace Seagull2.YuanXin.AppApi
{
    /// <summary>
    /// Moss通知通用服务类
    /// </summary>
    public class MossNoticeService
    {
        #region WebService - 测试

        /// <summary>
        /// GetListByListID
        /// </summary>
        public static DataTable GetListByListID(string siteUrl, string webId, string listId, string userName)
        {
            MossListOperationSoapClient moss = new MossListOperationSoapClient();
            string str = moss.getListByListID(siteUrl, Guid.Parse(webId), Guid.Parse(listId), userName);
            return XmlToTable(str);
        }

        /// <summary>
        /// GetListFieldsByListItemID
        /// </summary>
        public static DataTable GetListFieldsByListItemID(string siteUrl, string webId, string listId, string userName)
        {
            MossListOperationSoapClient moss = new MossListOperationSoapClient();
            string str = moss.getListFieldsByListItemID(siteUrl, Guid.Parse(webId), Guid.Parse(listId), userName);
            return XmlToTable(str);
        }

        /// <summary>
        /// GetListFieldsByListItemID
        /// </summary>
        public static DataTable GetListFieldsByListItemID1(string siteUrl, string webId, string listId, string userName)
        {
            NotificationsServer.MossListOperationSoapClient moss = new NotificationsServer.MossListOperationSoapClient();
            string str = moss.getListFieldsByListItemID(siteUrl, Guid.Parse(webId), Guid.Parse(listId), userName);
            return XmlToTable(str);
        }

        /// <summary>
        /// GetListItemAgentByListItemID
        /// </summary>
        public static DataTable GetListItemAgentByListItemID(string siteUrl, string webId, string listId, int listItemId)
        {
            MossListOperationSoapClient moss = new MossListOperationSoapClient();
            string str = moss.getListItemAgentByListItemID(siteUrl, Guid.Parse(webId), Guid.Parse(listId), listItemId);
            return XmlToTable(str);
        }

        /// <summary>
        /// GetListItemAttachmentsByListItemID
        /// </summary>
        public static DataTable GetListItemAttachmentsByListItemID(string siteUrl, string webId, string listId, int listItemId, string userName)
        {
            MossListOperationSoapClient moss = new MossListOperationSoapClient();
            string str = moss.getListItemAttachmentsByListItemID(siteUrl, Guid.Parse(webId), Guid.Parse(listId), listItemId, userName);
            return XmlToTable(str);
        }

        /// <summary>
        /// GetListItemByListItemID
        /// </summary>
        public static DataTable GetListItemByListItemID(string siteUrl, string webId, string listId, string[] fieldName, int listItemId, string userName)
        {
            MossListOperationSoapClient moss = new MossListOperationSoapClient();
            string str = moss.getListItemByListItemID(siteUrl, Guid.Parse(webId), Guid.Parse(listId), fieldName, listItemId, userName);
            return XmlToTable(str);
        }

        /// <summary>
        /// GetListItemCollectionByQuery
        /// </summary>
        public static DataTable GetListItemCollectionByQuery(string siteUrl, string webId, string listId, string queryString, string viewFieldsString, string viewAttributesString, string userName, int rowLimit)
        {
            MossListOperationSoapClient moss = new MossListOperationSoapClient();
            string str = moss.getListItemCollectionByQuery(siteUrl, Guid.Parse(webId), Guid.Parse(listId), queryString, viewFieldsString, viewAttributesString, userName, rowLimit);
            return XmlToTable(str);
        }

        /// <summary>
        /// GetNotificationsStringBySPSiteDataQuery
        /// </summary>
        public static DataTable GetNotificationsStringBySPSiteDataQuery(string siteUrl, string webUrl, string queryListsString, string viewFieldsString, string queryString, string queryWebs, int rowLimit, string userName)
        {
            MossListOperationSoapClient moss = new MossListOperationSoapClient();
            string str = moss.getNotificationsStringBySPSiteDataQuery(siteUrl, webUrl, queryListsString, viewFieldsString, queryString, queryWebs, rowLimit, userName);
            return XmlToTable(str);
        }

        /// <summary>
        /// GetWebByID
        /// </summary>
        public static DataTable GetWebByID(string siteUrl, string webId, string userName)
        {
            MossListOperationSoapClient moss = new MossListOperationSoapClient();
            string str = moss.getWebByID(siteUrl, Guid.Parse(webId), userName);
            return XmlToTable(str);
        }

        #endregion

        /// <summary>
        /// 日志
        /// </summary>
        ILog log = LogManager.GetLogger(MethodBase.GetCurrentMethod().DeclaringType);
        /// <summary>
        /// 配置
        /// </summary>
        ReadXmlModel config;
        /// <summary>
        /// Web 字典，ID，Name
        /// </summary>
        public static Dictionary<string, string> Webs = new Dictionary<string, string>();
        /// <summary>
        /// List 字典，ID，Name
        /// </summary>
        static Dictionary<string, string> Lists = new Dictionary<string, string>();

        #region 构造
        /// <summary>
        /// 构造
        /// </summary>
        /// <param name="xmlName">Xml配置文件名称，如：Notifications.xml</param>
        public MossNoticeService(string xmlName)
        {
            var xmlPath = AppDomain.CurrentDomain.SetupInformation.ApplicationBase + "XmlConfig\\" + xmlName;
            var xmlConfig = new XmlDocument();
            xmlConfig.Load(xmlPath);
            string siteUrl = string.Empty;
            StringBuilder query = new StringBuilder();

            XmlNode sites = xmlConfig.DocumentElement;
            foreach (XmlNode site in sites)
            {
                siteUrl = site.Attributes["Path"].Value;
                foreach (XmlNode webs in site)
                {
                    foreach (XmlNode web in webs)
                    {
                        var webId = web.Attributes["ID"].Value;
                        var webName = web.Attributes["ComPanyName"].Value;
                        if (!Webs.ContainsKey(webId))
                        {
                            Webs.Add(webId, webName);
                        }
                        foreach (XmlNode lists in web)
                        {
                            foreach (XmlNode list in lists)
                            {
                                var listId = list.Attributes["ID"].Value;
                                var listName = list.Attributes["Name"].Value;
                                if (!Lists.ContainsKey(listId))
                                {
                                    Lists.Add(listId, listName);
                                }
                                query.Append("<List ID=\"" + list.Attributes["ID"].Value + "\" />");
                            }
                        }
                    }
                }
            }

            config = new ReadXmlModel()
            {
                SiteUrl = siteUrl,
                WebUrl = string.Empty,
                QueryListsString = "<Lists>" + query.ToString() + "</Lists>",
                Count = Convert.ToInt32(sites.Attributes["ReadCount"].Value)
            };
        }
        /// <summary>
        /// 构造
        /// </summary>
        /// <param name="xmlName">Xml配置文件名称，如：Notifications.xml</param>
        /// <param name="companys">公司名称列表</param>
        public MossNoticeService(string xmlName, string[] companys)
        {
            var xmlPath = AppDomain.CurrentDomain.SetupInformation.ApplicationBase + "XmlConfig\\" + xmlName;
            var xmlConfig = new XmlDocument();
            xmlConfig.Load(xmlPath);

            string siteUrl = string.Empty;
            StringBuilder query = new StringBuilder();

            XmlNode sites = xmlConfig.DocumentElement;
            foreach (XmlNode site in sites)
            {
                siteUrl = site.Attributes["Path"].Value;
                foreach (XmlNode webs in site)
                {
                    foreach (XmlNode web in webs)
                    {
                        var webId = web.Attributes["ID"].Value;
                        var webName = web.Attributes["ComPanyName"].Value;
                        if (companys.Where(w => w == webName).FirstOrDefault() != null)
                        {
                            if (!Webs.ContainsKey(webId))
                            {
                                Webs.Add(webId, webName);
                            }
                            foreach (XmlNode lists in web)
                            {
                                foreach (XmlNode list in lists)
                                {
                                    var listId = list.Attributes["ID"].Value;
                                    var listName = list.Attributes["Name"].Value;
                                    if (!Lists.ContainsKey(listId))
                                    {
                                        Lists.Add(listId, listName);
                                    }
                                    query.Append("<List ID=\"" + list.Attributes["ID"].Value + "\" />");
                                }
                            }
                        }
                    }
                }
            }

            config = new ReadXmlModel()
            {
                SiteUrl = siteUrl,
                WebUrl = string.Empty,
                QueryListsString = "<Lists>" + query.ToString() + "</Lists>",
                Count = Convert.ToInt32(sites.Attributes["ReadCount"].Value)
            };
        }
        #endregion

        #region 获取WebName
        /// <summary>
        /// 获取WebName
        /// </summary>
        public string GetWebName(string webId)
        {
            if (!Webs.ContainsKey(webId))
            {
                return string.Empty;
            }
            return Webs[webId];
        }
        #endregion

        #region 获取通知纪要列表
        /// <summary>
        /// 获取通知纪要列表
        /// </summary>
        public List<NoticeList> GetNoticeList(string userName, string type, string created, string company, string keyword)
        {
            List<NoticeList> list = new List<NoticeList>();
            try
            {
                var queryListsString = config.QueryListsString;

                var viewFields = new string[] { "Title", "Created", "_x4e3b__x9001__x5355__x4f4d_" };
                var viewFieldsString = CreateViewFieldsString(viewFields);

                StringBuilder queryString = new StringBuilder();
                queryString.Append("<Where>");
                 

                /*
                 <Where>

                 <And>
                 <Or>
                 <Eq><FieldRef Name='_x7a0b__x5e8f__x540d__x79f0_' /><Value Type='Text'>MEETING</Value></Eq>
                 <Eq><FieldRef Name='_x7a0b__x5e8f__x540d__x79f0_' /><Value Type='Text'>N</Value></Eq>
                 </Or>
                 <Lt><FieldRef Name='Created' /><Value Type='DateTime' IncludeTimeValue='True'>2020-02-06</Value>
                 </Lt>
                 </And>
                 
                </Where>
                 <OrderBy><FieldRef Name = 'Created' Ascending = 'false' /></OrderBy >
                 
                 */

                if (string.IsNullOrWhiteSpace(company) && string.IsNullOrWhiteSpace(keyword))
                {
                    //company为空，keyword为空
                    if (type != "FAWEN")
                    {
                        queryString.Append("<And>");
                        queryString.Append("<Neq><FieldRef Name=\"PushArticleType\" /><Value Type=\"Text\">project</Value></Neq>");
                        queryString.Append("<And>");
                        queryString.Append("<Lt><FieldRef Name=\"Created\" /><Value Type=\"DateTime\" IncludeTimeValue=\"True\">" + created + "</Value></Lt>");
                        queryString.Append("<Eq><FieldRef Name=\"_x7a0b__x5e8f__x540d__x79f0_\" /><Value Type=\"Text\">" + type + "</Value></Eq>");
                        queryString.Append("</And>");
                        queryString.Append("</And>");
                    }
                    else
                    {
                        queryString.Append("<And>");
                        queryString.Append("<Or>");
                        queryString.Append("<Eq><FieldRef Name=\"_x7a0b__x5e8f__x540d__x79f0_\" /><Value Type=\"Text\">" + type + "</Value></Eq>");
                        queryString.Append("<Eq><FieldRef Name=\"_x7a0b__x5e8f__x540d__x79f0_\" /><Value Type=\"Text\">NOTICE</Value></Eq>");
                        queryString.Append("</Or>");
                        queryString.Append("<Lt><FieldRef Name=\"Created\" /><Value Type=\"DateTime\" IncludeTimeValue=\"True\">" + created + "</Value></Lt>");
                        queryString.Append("</And>");
                    }
                }
                else if (!string.IsNullOrWhiteSpace(company) && string.IsNullOrWhiteSpace(keyword))
                {
                    //company有值，keyword为空
                    if (type != "FAWEN")
                    {
                        queryString.Append("<And>");

                        queryString.Append("<Eq><FieldRef Name=\"_x4e3b__x9001__x5355__x4f4d_\" /><Value Type=\"Text\">" + company + "</Value></Eq>");

                        queryString.Append("<And>");
                        queryString.Append("<Lt><FieldRef Name=\"Created\" /><Value Type=\"DateTime\" IncludeTimeValue=\"True\">" + created + "</Value></Lt>");
                        queryString.Append("<Eq><FieldRef Name=\"_x7a0b__x5e8f__x540d__x79f0_\" /><Value Type=\"Text\">" + type + "</Value></Eq>");
                        queryString.Append("</And>");

                        queryString.Append("</And>");
                    }
                    else
                    {
                        queryString.Append("<And>");

                        queryString.Append("<Eq><FieldRef Name=\"_x4e3b__x9001__x5355__x4f4d_\" /><Value Type=\"Text\">" + company + "</Value></Eq>");

                        queryString.Append("<And>");
                        queryString.Append("<Lt><FieldRef Name=\"Created\" /><Value Type=\"DateTime\" IncludeTimeValue=\"True\">" + created + "</Value></Lt>");

                        queryString.Append("<Or>");
                        queryString.Append("<Eq><FieldRef Name=\"_x7a0b__x5e8f__x540d__x79f0_\" /><Value Type=\"Text\">" + type + "</Value></Eq>");
                        queryString.Append("<Eq><FieldRef Name=\"_x7a0b__x5e8f__x540d__x79f0_\" /><Value Type=\"Text\">NOTICE</Value></Eq>");
                        queryString.Append("</Or>");
                        queryString.Append("</And>");

                        queryString.Append("</And>");
                    }

                }
                else if (string.IsNullOrWhiteSpace(company) && !string.IsNullOrWhiteSpace(keyword))
                {
                    //company为空，keyword有值

                    if (type != "FAWEN")
                    {
                        queryString.Append("<And>");

                        queryString.Append("<Contains><FieldRef Name=\"Title\" /><Value Type=\"Text\">" + keyword + "</Value></Contains>");

                        queryString.Append("<And>");
                        queryString.Append("<Lt><FieldRef Name=\"Created\" /><Value Type=\"DateTime\" IncludeTimeValue=\"True\">" + created + "</Value></Lt>");
                        queryString.Append("<Eq><FieldRef Name=\"_x7a0b__x5e8f__x540d__x79f0_\" /><Value Type=\"Text\">" + type + "</Value></Eq>");
                        queryString.Append("</And>");

                        queryString.Append("</And>");
                    }
                    else
                    {
                        queryString.Append("<And>");

                        queryString.Append("<Contains><FieldRef Name=\"Title\" /><Value Type=\"Text\">" + keyword + "</Value></Contains>");

                        queryString.Append("<And>");
                        queryString.Append("<Lt><FieldRef Name=\"Created\" /><Value Type=\"DateTime\" IncludeTimeValue=\"True\">" + created + "</Value></Lt>");
                        queryString.Append("<Or>");
                        queryString.Append("<Eq><FieldRef Name=\"_x7a0b__x5e8f__x540d__x79f0_\" /><Value Type=\"Text\">" + type + "</Value></Eq>");
                        queryString.Append("<Eq><FieldRef Name=\"_x7a0b__x5e8f__x540d__x79f0_\" /><Value Type=\"Text\">NOTICE</Value></Eq>");
                        queryString.Append("</Or>");
                        queryString.Append("</And>");
                        queryString.Append("</And>");

                    }
                }
                else
                {
                    //company有值，keyword有值
                    if (type != "FAWEN")
                    {
                        queryString.Append("<And>");

                        queryString.Append("<And>");
                        queryString.Append("<Eq><FieldRef Name=\"_x4e3b__x9001__x5355__x4f4d_\" /><Value Type=\"Text\">" + company + "</Value></Eq>");
                        queryString.Append("<Contains><FieldRef Name=\"Title\" /><Value Type=\"Text\">" + keyword + "</Value></Contains>");
                        queryString.Append("</And>");

                        queryString.Append("<And>");
                        queryString.Append("<Lt><FieldRef Name=\"Created\" /><Value Type=\"DateTime\" IncludeTimeValue=\"True\">" + created + "</Value></Lt>");
                        queryString.Append("<Eq><FieldRef Name=\"_x7a0b__x5e8f__x540d__x79f0_\" /><Value Type=\"Text\">" + type + "</Value></Eq>");
                        queryString.Append("</And>");

                        queryString.Append("</And>");
                    }
                    else
                    {
                        queryString.Append("<And>");

                        queryString.Append("<And>");
                        queryString.Append("<Eq><FieldRef Name=\"_x4e3b__x9001__x5355__x4f4d_\" /><Value Type=\"Text\">" + company + "</Value></Eq>");
                        queryString.Append("<Contains><FieldRef Name=\"Title\" /><Value Type=\"Text\">" + keyword + "</Value></Contains>");
                        queryString.Append("</And>");

                        queryString.Append("<And>");
                        queryString.Append("<Lt><FieldRef Name=\"Created\" /><Value Type=\"DateTime\" IncludeTimeValue=\"True\">" + created + "</Value></Lt>");
                        queryString.Append("<Or>");
                        queryString.Append("<Eq><FieldRef Name=\"_x7a0b__x5e8f__x540d__x79f0_\" /><Value Type=\"Text\">" + type + "</Value></Eq>");
                        queryString.Append("<Eq><FieldRef Name=\"_x7a0b__x5e8f__x540d__x79f0_\" /><Value Type=\"Text\">NOTICE</Value></Eq>");
                        queryString.Append("</Or>");
                        queryString.Append("</And>");

                        queryString.Append("</And>");
                    }

                }

                queryString.Append("</Where>");

                queryString.Append("<OrderBy>");
                queryString.Append("<FieldRef Name=\"Created\" Ascending=\"False\" />");
                queryString.Append("</OrderBy>");

                var queryWebs = "<Webs Scope=\"SiteCollection\" />";

                Stopwatch stopwatch = new Stopwatch();
                stopwatch.Start();

                MossListOperationSoapClient mos = new MossListOperationSoapClient();
                string result = mos.getNotificationsStringBySPSiteDataQuery(config.SiteUrl, config.WebUrl,
                    queryListsString,
                    viewFieldsString,
                    queryString.ToString(),
                    queryWebs,
                    config.Count,
                    userName);

                stopwatch.Stop();
                log.Info("----------获取通知纪要列表原始数据----------用时：" + stopwatch.ElapsedMilliseconds + "毫秒");

                DataTable dt = XmlToTable(result);
                list = DataConvertHelper<NoticeList>.ConvertToList(dt);
            }
            catch (Exception e)
            {
                log.Error("Error - 获取通知纪要列表失败：" + JsonConvert.SerializeObject(e));
            }
            return list;
        }

        /// <summary>
        /// 通知纪要列表实体类
        /// </summary>
        public class NoticeList : BaseField
        {
            /// <summary>
            /// 标题
            /// </summary>
            public string Title { set; get; }

            /// <summary>
            /// 创建时间
            /// </summary>
            public string Created { set; get; }

            /// <summary>
            /// 主送单位
            /// </summary>
            public string _x4e3b__x9001__x5355__x4f4d_ { set; get; }
        }
        #endregion

        #region 获取重要发文列表

        /// <summary>
        /// 获取重要发文列表 - 推荐数据
        /// </summary>
        public List<ImportantNoticeListModel> GetImportantNoticeList(string userName, string created, string guid, string company, string keyword)
        {
            List<ImportantNoticeListModel> list = new List<ImportantNoticeListModel>();
            try
            {
                var queryListsString = config.QueryListsString;

                var viewFields = new string[] { "GUID", "Title", "Created", "_x4e3b__x9001__x5355__x4f4d_" };
                var viewFieldsString = CreateViewFieldsString(viewFields);

                StringBuilder queryString = new StringBuilder();
                queryString.Append("<Where>");


                if (string.IsNullOrWhiteSpace(company) && string.IsNullOrWhiteSpace(keyword))
                {
                    //company为空，keyword为空
                    queryString.Append("<And>");

                    queryString.Append("<Neq><FieldRef Name=\"GUID\" /><Value Type=\"Guid\">" + guid + "</Value></Neq>");

                    queryString.Append("<And>");
                    queryString.Append("<Lt><FieldRef Name =\"Created\" /><Value Type =\"DateTime\" IncludeTimeValue=\"True\">" + created + "</Value></Lt>");
                    queryString.Append("<Eq><FieldRef Name=\"PushArticleType\" /><Value Type=\"Text\">project</Value></Eq>");
                    queryString.Append("</And>");

                    queryString.Append("</And>");
                }
                else if (!string.IsNullOrWhiteSpace(company) && string.IsNullOrWhiteSpace(keyword))
                {
                    //company有值，keyword为空
                    queryString.Append("<And>");

                    queryString.Append("<And>");
                    queryString.Append("<Neq><FieldRef Name=\"GUID\" /><Value Type=\"Guid\">" + guid + "</Value></Neq>");
                    queryString.Append("<Eq><FieldRef Name=\"_x4e3b__x9001__x5355__x4f4d_\" /><Value Type=\"Text\">" + company + "</Value></Eq>");
                    queryString.Append("</And>");

                    queryString.Append("<And>");
                    queryString.Append("<Lt><FieldRef Name =\"Created\" /><Value Type =\"DateTime\" IncludeTimeValue=\"True\">" + created + "</Value></Lt>");
                    queryString.Append("<Eq><FieldRef Name=\"PushArticleType\" /><Value Type=\"Text\">project</Value></Eq>");
                    queryString.Append("</And>");

                    queryString.Append("</And>");
                }
                else if (string.IsNullOrWhiteSpace(company) && !string.IsNullOrWhiteSpace(keyword))
                {
                    //company为空，keyword有值
                    queryString.Append("<And>");

                    queryString.Append("<And>");
                    queryString.Append("<Neq><FieldRef Name=\"GUID\" /><Value Type=\"Guid\">" + guid + "</Value></Neq>");
                    queryString.Append("<Contains><FieldRef Name=\"Title\" /><Value Type=\"Text\">" + keyword + "</Value></Contains>");
                    queryString.Append("</And>");

                    queryString.Append("<And>");
                    queryString.Append("<Lt><FieldRef Name =\"Created\" /><Value Type =\"DateTime\" IncludeTimeValue=\"True\">" + created + "</Value></Lt>");
                    queryString.Append("<Eq><FieldRef Name=\"PushArticleType\" /><Value Type=\"Text\">project</Value></Eq>");
                    queryString.Append("</And>");

                    queryString.Append("</And>");
                }
                else
                {
                    //company有值，keyword有值
                    queryString.Append("<And>");//--

                    queryString.Append("<Neq><FieldRef Name=\"GUID\" /><Value Type=\"Guid\">" + guid + "</Value></Neq>");

                    queryString.Append("<And>");//----

                    queryString.Append("<And>");
                    queryString.Append("<Eq><FieldRef Name=\"_x4e3b__x9001__x5355__x4f4d_\" /><Value Type=\"Text\">" + company + "</Value></Eq>");
                    queryString.Append("<Contains><FieldRef Name=\"Title\" /><Value Type=\"Text\">" + keyword + "</Value></Contains>");
                    queryString.Append("</And>");

                    queryString.Append("<And>");
                    queryString.Append("<Lt><FieldRef Name =\"Created\" /><Value Type =\"DateTime\" IncludeTimeValue=\"True\">" + created + "</Value></Lt>");
                    queryString.Append("<Eq><FieldRef Name=\"PushArticleType\" /><Value Type=\"Text\">project</Value></Eq>");
                    queryString.Append("</And>");

                    queryString.Append("</And>");//----

                    queryString.Append("</And>");//--
                }

                queryString.Append("</Where>");

                queryString.Append("<OrderBy>");
                queryString.Append("<FieldRef Name=\"Created\" Ascending=\"False\" />");
                queryString.Append("</OrderBy>");

                var queryWebs = "<Webs Scope=\"SiteCollection\" />";

                MossListOperationSoapClient mos = new MossListOperationSoapClient();
                string result = mos.getNotificationsStringBySPSiteDataQuery(config.SiteUrl, config.WebUrl,
                    queryListsString,
                    viewFieldsString,
                    queryString.ToString(),
                    queryWebs,
                    string.IsNullOrWhiteSpace(guid) ? config.Count : config.Count - 1,
                    userName);
                DataTable dt = XmlToTable(result);
                list = DataConvertHelper<ImportantNoticeListModel>.ConvertToList(dt);
            }
            catch (Exception e)
            {
                log.Error("Error - 获取重要发文列表失败：" + JsonConvert.SerializeObject(e));
            }
            return list;
        }



        /// <summary>
        /// 重要发文列表实体类
        /// </summary>
        public class ImportantNoticeListModel : BaseField
        {
            /// <summary>
            /// Guid
            /// </summary>
            public string Guid
            {
                set { guid = value; }
                get
                {
                    return System.Guid.Parse(guid).ToString().ToUpper();
                }
            }
            private string guid;

            /// <summary>
            /// 标题
            /// </summary>
            public string Title { set; get; }

            /// <summary>
            /// 创建时间
            /// </summary>
            public string Created { set; get; }

            /// <summary>
            /// 主送单位
            /// </summary>
            public string _x4e3b__x9001__x5355__x4f4d_ { set; get; }
        }
        #endregion




        #region 面向项目的首页和普通页面数据 


        /// <summary>
        /// 获取面向项目的首页数据   
        /// </summary>
        /// <param name="isForProject"></param>
        /// <param name="rowLimit"></param>
        /// <param name="loginName"></param>
        /// <returns></returns>
        public List<ImportantNoticeListModel> GetForProjectFirstPage(bool isForProject, int rowLimit, string loginName)
        {
            int topCount = 1;//默认取置顶1条
            int recommendedCount = 5;//默认取推荐5条
            int secondRecommendedCount = 3;//默认取次推荐3条
            var viewFields = new string[] { "GUID", "Title", "Created", "_x4e3b__x9001__x5355__x4f4d_" };
            string queryWebs = "<Webs Scope=\"SiteCollection\" />";
            string viewFieldsString = CreateViewFieldsString(viewFields);
            string queryString = string.Empty;
            var dataRows = new List<DataRow>();
            List<ImportantNoticeListModel> list = new List<ImportantNoticeListModel>();
            queryString = BuildAndQueryString(isForProject, true, null);
            var task1 = GetPrjOrLastNotificationsToDT(config.QueryListsString, viewFieldsString, queryString, queryWebs, loginName, topCount);

            queryString = BuildAndQueryString(isForProject, null, true);
            var task2 = GetPrjOrLastNotificationsToDT(config.QueryListsString, viewFieldsString, queryString, queryWebs, loginName, recommendedCount + topCount);
            queryString = "<Where>" +
                            "<And>" +
                                "<Eq><FieldRef Name='SecondRecommended'/><Value Type='Boolean'>1</Value></Eq>" +
                                "<Eq><FieldRef Name='PushArticleType'/><Value Type='Text'>project</Value></Eq></And></Where>" +
                    "<OrderBy><FieldRef Name='Created' Ascending='false'/></OrderBy>";
            var task3 = GetPrjOrLastNotificationsToDT(config.QueryListsString, viewFieldsString, queryString, queryWebs, loginName, secondRecommendedCount + recommendedCount + topCount);
            queryString = BuildAndQueryString(isForProject, null, null);
            var task4 = GetPrjOrLastNotificationsToDT(config.QueryListsString, viewFieldsString, queryString, queryWebs, loginName, rowLimit);
            Task.WaitAll(task1, task2, task3, task4);
            if (task1.Result != null && task1.Result.Rows.Count > 0)
            {
                foreach (DataRow row in task1.Result.Rows)
                {
                    dataRows.Add(row);
                }
            }
            if (task2.Result != null && task2.Result.Rows.Count > 0)
            {
                int count = 0;
                foreach (DataRow row in task2.Result.Rows)
                {
                    if (dataRows.Count(r => string.Equals(r["ID"].ToString(), row["ID"].ToString())) == 0 && count < recommendedCount)
                    {
                        dataRows.Add(row);
                        count++;
                    }
                }
            }
            if (task3.Result != null && task3.Result.Rows.Count > 0)
            {
                int count = 0;
                foreach (DataRow row in task3.Result.Rows)
                {
                    if (dataRows.Count(r => string.Equals(r["ID"].ToString(), row["ID"].ToString())) == 0 && count < secondRecommendedCount)
                    {
                        dataRows.Add(row);
                        count++;
                    }
                }
            }
            if (task4.Result != null && task4.Result.Rows.Count > 0)
            {
                foreach (DataRow row in task4.Result.Rows)
                {
                    if (dataRows.Count(r => string.Equals(r["ID"].ToString(), row["ID"].ToString())) == 0 && dataRows.Count < rowLimit)
                    {
                        dataRows.Add(row);
                    }
                }
            }

            if (dataRows.Count > 0)
            {
                list = DataConvertHelper<ImportantNoticeListModel>.ConvertToList(dataRows.CopyToDataTable());
            }
            return list;
        }


        /// <summary>
        /// 获取面向项目的正常数据 
        /// </summary>
        public List<ImportantNoticeListModel> GetForProjectNormal(string userName, string created, string company, string keyword)
        {
            List<ImportantNoticeListModel> list = new List<ImportantNoticeListModel>();
            try
            {
                var queryListsString = config.QueryListsString;
                string guid = string.Empty;
                var viewFields = new string[] { "GUID", "Title", "Created", "_x4e3b__x9001__x5355__x4f4d_" };
                var viewFieldsString = CreateViewFieldsString(viewFields);
                string queryString = "<Where><And><Neq><FieldRef Name=\"GUID\" /><Value Type=\"Guid\">" + guid + "</Value></Neq><And><Lt><FieldRef Name =\"Created\" /><Value Type =\"DateTime\" IncludeTimeValue=\"True\">" + created + "</Value></Lt><Eq><FieldRef Name=\"PushArticleType\" /><Value Type=\"Text\">project</Value></Eq></And></And></Where><OrderBy><FieldRef Name=\"Created\" Ascending=\"False\" /></OrderBy>";
                var queryWebs = "<Webs Scope=\"SiteCollection\" />";
                MossListOperationSoapClient mos = new MossListOperationSoapClient();
                string result = mos.getNotificationsStringBySPSiteDataQuery(config.SiteUrl, config.WebUrl,
                    queryListsString,
                    viewFieldsString,
                    queryString,
                    queryWebs,
                    config.Count,
                    userName);
                DataTable dt = XmlToTable(result);
                list = DataConvertHelper<ImportantNoticeListModel>.ConvertToList(dt);
            }
            catch (Exception e)
            {
                log.Error("Error - 获取面向项目的正常列表失败：" + JsonConvert.SerializeObject(e));
            }
            return list;
        }

        #endregion
        #region  最新发文的首页数据和普通数据 
        /// <summary>
        /// 获取最新发文的首页数据  最新改版跟pc 端同步
        /// </summary>
        /// <param name="isForProject"></param>
        /// <param name="rowLimit"></param>
        /// <param name="loginName"></param>
        /// <returns></returns>
        public List<ImportantNoticeListModel> GetNewNotifyFirstPage(bool isForProject, int rowLimit, string loginName)
        {
            int topCount = 1;//默认取置顶1条
            int recommendedCount = 5;//默认取推荐5条
            var viewFields = new string[] { "GUID", "Title", "Created", "_x4e3b__x9001__x5355__x4f4d_" };
            string queryWebs = "<Webs Scope=\"SiteCollection\" />";
            string viewFieldsString = CreateViewFieldsString(viewFields);
            string queryString = string.Empty;
            var dataRows = new List<DataRow>();
            List<ImportantNoticeListModel> list = new List<ImportantNoticeListModel>();
            queryString = BuildAndQueryString(isForProject, true, null);
            var task1 = GetPrjOrLastNotificationsToDT(config.QueryListsString, viewFieldsString, queryString, queryWebs, loginName, topCount);

            queryString = BuildAndQueryString(isForProject, null, true);
            var task2 = GetPrjOrLastNotificationsToDT(config.QueryListsString, viewFieldsString, queryString, queryWebs, loginName, recommendedCount + topCount);

            queryString = BuildAndQueryString(isForProject, null, null);
            var task3 = GetPrjOrLastNotificationsToDT(config.QueryListsString, viewFieldsString, queryString, queryWebs, loginName, rowLimit);
            Task.WaitAll(task1, task2, task3);
            if (task1.Result != null && task1.Result.Rows.Count > 0)
            {
                foreach (DataRow row in task1.Result.Rows)
                {
                    dataRows.Add(row);
                }
            }
            if (task2.Result != null && task2.Result.Rows.Count > 0)
            {
                int count = 0;
                foreach (DataRow row in task2.Result.Rows)
                {
                    if (dataRows.Count(r => string.Equals(r["ID"].ToString(), row["ID"].ToString())) == 0 && count < recommendedCount)
                    {
                        dataRows.Add(row);
                        count++;
                    }
                }
            }
            if (task3.Result != null && task3.Result.Rows.Count > 0)
            {
                foreach (DataRow row in task3.Result.Rows)
                {
                    if (dataRows.Count(r => string.Equals(r["ID"].ToString(), row["ID"].ToString())) == 0 && dataRows.Count < rowLimit)
                    {
                        dataRows.Add(row);
                    }
                }
            }

            if (dataRows.Count > 0)
            {
                list = DataConvertHelper<ImportantNoticeListModel>.ConvertToList(dataRows.CopyToDataTable());
            }
            return list;
        }


        /// <summary>
        /// 获取最新发文的正常数据 
        /// </summary>
        public List<ImportantNoticeListModel> GetNewNotifyNormal(string userName, string created, string company, string keyword)
        {
            List<ImportantNoticeListModel> list = new List<ImportantNoticeListModel>();
            try
            {
                var queryListsString = config.QueryListsString;
                string guid = string.Empty;
                var viewFields = new string[] { "GUID", "Title", "Created", "_x4e3b__x9001__x5355__x4f4d_" };
                var viewFieldsString = CreateViewFieldsString(viewFields);
                string queryString = "<Where><And><Neq><FieldRef Name=\"GUID\" /><Value Type=\"Guid\">" + guid + "</Value></Neq><And><Lt><FieldRef Name =\"Created\" /><Value Type =\"DateTime\" IncludeTimeValue=\"True\">" + created + "</Value></Lt><Neq><FieldRef Name=\"PushArticleType\" /><Value Type=\"Text\">project</Value></Neq></And></And></Where><OrderBy><FieldRef Name=\"Created\" Ascending=\"False\" /></OrderBy>";
                var queryWebs = "<Webs Scope=\"SiteCollection\" />";
                MossListOperationSoapClient mos = new MossListOperationSoapClient();
                string result = mos.getNotificationsStringBySPSiteDataQuery(config.SiteUrl, config.WebUrl,
                    queryListsString,
                    viewFieldsString,
                    queryString,
                    queryWebs,
                    config.Count,
                    userName);
                DataTable dt = XmlToTable(result);
                list = DataConvertHelper<ImportantNoticeListModel>.ConvertToList(dt);
            }
            catch (Exception e)
            {
                log.Error("Error - 获取最新发文列表失败：" + JsonConvert.SerializeObject(e));
            }
            return list;
        }

        #endregion
        #region 首页私有方法
        private string BuildAndQueryString(bool? pushArticleType, bool? top, bool? recommended)
        {
            string queryString = string.Empty;
            List<string> conditions = new List<string>();
            if (pushArticleType.HasValue)
            {
                string con = pushArticleType.Value ? "<Eq><FieldRef Name='PushArticleType'/><Value Type='Text'>project</Value></Eq>"
                                                    : "<Neq><FieldRef Name='PushArticleType'/><Value Type='Text'>project</Value></Neq>";
                conditions.Add(con);
            }

            if (top.HasValue)
            {
                string con = "<Eq><FieldRef Name='Top'/><Value Type='Boolean'>" + (top.Value ? 1 : 0) + "</Value></Eq>";
                conditions.Add(con);
            }

            if (recommended.HasValue)
            {
                string con = "<Eq><FieldRef Name='Recommended'/><Value Type='Boolean'>" + (recommended.Value ? 1 : 0) + "</Value></Eq>";
                conditions.Add(con);
            }
            if (conditions.Count > 0)
            {
                StringBuilder queryStr = new StringBuilder();
                queryStr.Append("<Where>");
                string conditionsStr = BuildAndConditons(conditions, 0).ToString(SaveOptions.DisableFormatting);
                queryStr.Append(conditionsStr);
                queryStr.Append("</Where>");
                queryStr.Append("<OrderBy><FieldRef Name = 'Created' Ascending = 'false' /></OrderBy >");
                queryString = queryStr.ToString();
            }

            return queryString;
        }
        private XElement BuildAndConditons(List<string> conditions, int index)
        {
            if (index == conditions.Count - 1)
            {
                return XElement.Parse(conditions[index]);
            }
            else
            {
                XElement andClause = new XElement("And");
                andClause.Add(XElement.Parse(conditions[index]));
                andClause.Add(BuildAndConditons(conditions, index + 1));

                return andClause;
            }
        }

        private Task<DataTable> GetPrjOrLastNotificationsToDT(string queryListsString, string viewFieldsString, string queryString, string queryWebs, string loginName, int rowLimit)
        {
            return Task.Run(() =>
            {
                MossListOperationSoapClient mos = new MossListOperationSoapClient();
                string result = string.Empty;
                if (!string.IsNullOrEmpty(queryListsString))
                {
                    try
                    {
                        lock (queryString)
                        {
                            result = mos.getNotificationsStringBySPSiteDataQuery(config.SiteUrl, config.WebUrl,
                           queryListsString, viewFieldsString, queryString.ToString(),
                           queryWebs, rowLimit, loginName);
                        }
                    }
                    catch (Exception ex)
                    {
                        log.Error("Error - 获取首页发文列表失败：" + JsonConvert.SerializeObject(ex));
                    }
                }
                mos.Close();
                return XmlToTable(result);
            });
        }
        #endregion




        #region 获取通知纪要详细内容
        /// <summary>
        /// 获取通知纪要详细内容
        /// </summary>
        public NoticeArticleModel GetNoticeDetail(string userName, string webId, string listId, int listItemId)
        {
            NoticeArticleModel model = null;
            try
            {
                MossListOperationSoapClient mos = new MossListOperationSoapClient();
                var fieldName = new string[] { "Title", "Created", "_x7a0b__x5e8f__x540d__x79f0_", "_x4e3b__x9001__x5355__x4f4d_", "_x6267__x884c__x4eba_", "_x6267__x884c__x4eba__x5355__x4f", "Body1", "_x6587__x53f7_GUID", "_x9644__x4ef6__x539f__x59cb__x54" };
                string result = mos.getListItemByListItemID(config.SiteUrl, Guid.Parse(webId), Guid.Parse(listId), fieldName, listItemId, userName);
                DataTable dt = XmlToTable(result);
                model = DataConvertHelper<NoticeArticleModel>.ConvertToModel(dt, 0, 3);
            }
            catch (Exception e)
            {
                log.Error("Error - 获取通知纪要详细信息失败：" + JsonConvert.SerializeObject(e));
            }
            return model;
        }
        /// <summary>
        /// 详细信息实体类
        /// </summary>
        public class NoticeArticleModel
        {
            /// <summary>
            /// 标题
            /// </summary>
            public string Title { set; get; }
            /// <summary>
            /// 创建时间
            /// </summary>
            public string Created { set; get; }
            /// <summary>
            /// 类型（单位通知、部门通知、会议纪要）
            /// </summary>
            public string _x7a0b__x5e8f__x540d__x79f0_ { set; get; }
            /// <summary>
            /// 主送单位
            /// </summary>
            public string _x4e3b__x9001__x5355__x4f4d_ { set; get; }
            /// <summary>
            /// 执行人
            /// </summary>
            public string _x6267__x884c__x4eba_ { set; get; }
            /// <summary>
            /// 执行人单位
            /// </summary>
            public string _x6267__x884c__x4eba__x5355__x4f { set; get; }
            /// <summary>
            /// 正文
            /// </summary>
            public string Body1 { set; get; }
            /// <summary>
            /// 文号GUID
            /// </summary>
            public string _x6587__x53f7_GUID { set; get; }
            /// <summary>
            /// 保存附件原始名称列表的xml字符串
            /// </summary>
            public string _x9644__x4ef6__x539f__x59cb__x54 { set; get; }
            /// <summary>
            /// 原始文件名列表
            /// key:开发事业四部关于发布“供应商关系”服务包供应商管理规范的通知.docx
            /// value:[正文]开发事业四部关于发布“供应商关系”服务包供应商管理规范的通知.docx
            /// </summary>
            public Dictionary<string, string> OriginalFileList
            {
                get
                {
                    var list = new Dictionary<string, string>();
                    if (string.IsNullOrWhiteSpace(_x9644__x4ef6__x539f__x59cb__x54))
                    {
                        return list;
                    }
                    XmlDocument doc = new XmlDocument();
                    doc.LoadXml(_x9644__x4ef6__x539f__x59cb__x54);
                    foreach (XmlNode node in doc.SelectNodes("//file[@name]"))
                    {
                        var key = node.Attributes["name"].InnerText;
                        var value = node.InnerText;
                        if (!list.ContainsKey(key))
                        {
                            list.Add(key, value);
                        }
                    }
                    return list;
                }
            }
        }
        #endregion

        #region 获取通知纪要附件列表
        /// <summary>
        /// 获取通知纪要附件列表
        /// </summary>
        public List<NoticeAttachmentModel> GetNoticeAttachments(string userName, string webId, string listId, int listItemId)
        {
            List<NoticeAttachmentModel> list = new List<NoticeAttachmentModel>();
            try
            {
                MossListOperationSoapClient mos = new MossListOperationSoapClient();
                string result = mos.getListItemAttachmentsByListItemID(config.SiteUrl, Guid.Parse(webId), Guid.Parse(listId), listItemId, userName);
                DataTable dt = XmlToTable(result);
                foreach (DataRow dr in dt.Rows)
                {
                    list.Add(new NoticeAttachmentModel()
                    {
                        Name = dr[0].ToString(),
                        Url = dr[1].ToString()
                    });
                }
            }
            catch (Exception e)
            {
                log.Error("Error - 获取通知纪要附件列表失败：" + JsonConvert.SerializeObject(e));
            }
            return list;
        }
        /// <summary>
        /// 通知纪要附件实体类
        /// </summary>
        public class NoticeAttachmentModel
        {
            /// <summary>
            /// 名称
            /// </summary>
            public string Name { set; get; }
            /// <summary>
            /// 路径
            /// </summary>
            public string Url { set; get; }
        }
        #endregion


        #region  获取党建发文列表

        /// <summary>
        /// 获取党建发文 置顶
        /// </summary>
        public List<ImportantNoticeListModel> GetDangJianNoticeTop(string userName, string company, string keyword)
        {
            List<ImportantNoticeListModel> list = new List<ImportantNoticeListModel>();
            try
            {
                var queryListsString = config.QueryListsString;

                var viewFields = new string[] { "GUID", "Title", "Created", "_x4e3b__x9001__x5355__x4f4d_" };
                var viewFieldsString = CreateViewFieldsString(viewFields);

                StringBuilder queryString = new StringBuilder();
                queryString.Append("<Where>");

                if (string.IsNullOrWhiteSpace(company) && string.IsNullOrWhiteSpace(keyword))
                {

                    //company为空，keyword为空
                    queryString.Append("<And>");
                    //queryString.Append("<Eq><FieldRef Name=\"Recommended\" /><Value Type=\"Boolean\">1</Value></Eq>");
                    queryString.Append("<Eq><FieldRef Name=\"PushArticleType\" /><Value Type=\"Text\">project</Value></Eq>");
                    queryString.Append("<Eq><FieldRef Name=\"Top\" /><Value Type=\"Boolean\">1</Value></Eq>");
                    queryString.Append("</And>");
                }
                else if (!string.IsNullOrWhiteSpace(company) && string.IsNullOrWhiteSpace(keyword))
                {
                    //company有值，keyword为空
                    queryString.Append("<And>");

                    queryString.Append("<Eq><FieldRef Name=\"_x4e3b__x9001__x5355__x4f4d_\" /><Value Type=\"Text\">" + company + "</Value></Eq>");

                    queryString.Append("<And>");
                    queryString.Append("<Eq><FieldRef Name=\"PushArticleType\" /><Value Type=\"Text\">project</Value></Eq>");
                    queryString.Append("<Eq><FieldRef Name=\"Top\" /><Value Type=\"Boolean\">1</Value></Eq>");
                    queryString.Append("</And>");

                    queryString.Append("</And>");
                }
                else if (string.IsNullOrWhiteSpace(company) && !string.IsNullOrWhiteSpace(keyword))
                {
                    //company为空，keyword有值
                    queryString.Append("<And>");

                    queryString.Append("<Contains><FieldRef Name=\"Title\" /><Value Type=\"Text\">" + keyword + "</Value></Contains>");

                    queryString.Append("<And>");
                    queryString.Append("<Eq><FieldRef Name=\"PushArticleType\" /><Value Type=\"Text\">project</Value></Eq>");
                    queryString.Append("<Eq><FieldRef Name=\"Top\" /><Value Type=\"Boolean\">1</Value></Eq>");
                    queryString.Append("</And>");

                    queryString.Append("</And>");
                }
                else
                {
                    //company有值，keyword有值
                    queryString.Append("<And>");

                    queryString.Append("<And>");
                    queryString.Append("<Eq><FieldRef Name=\"_x4e3b__x9001__x5355__x4f4d_\" /><Value Type=\"Text\">" + company + "</Value></Eq>");
                    queryString.Append("<Contains><FieldRef Name=\"Title\" /><Value Type=\"Text\">" + keyword + "</Value></Contains>");
                    queryString.Append("</And>");

                    queryString.Append("<And>");
                    queryString.Append("<Eq><FieldRef Name=\"PushArticleType\" /><Value Type=\"Text\">project</Value></Eq>");
                    queryString.Append("<Eq><FieldRef Name=\"Top\" /><Value Type=\"Boolean\">1</Value></Eq>");
                    queryString.Append("</And>");

                    queryString.Append("</And>");
                }

                queryString.Append("</Where>");

                queryString.Append("<OrderBy>");
                queryString.Append("<FieldRef Name=\"Created\" Ascending=\"False\" />");
                queryString.Append("</OrderBy>");

                var queryWebs = "<Webs Scope=\"SiteCollection\" />";

                MossListOperationSoapClient mos = new MossListOperationSoapClient();
                string result = mos.getNotificationsStringBySPSiteDataQuery(config.SiteUrl, config.WebUrl,
                    queryListsString,
                    viewFieldsString,
                    queryString.ToString(),
                    queryWebs,
                    1,
                    userName);
                DataTable dt = XmlToTable(result);
                list = DataConvertHelper<ImportantNoticeListModel>.ConvertToList(dt);
            }
            catch (Exception e)
            {
                log.Error("Error - 获取重要发文置顶列表失败：" + JsonConvert.SerializeObject(e));
            }
            return list;
        }

        /// <summary>
        /// 获取党建发文
        /// </summary>
        /// <param name="userName"></param>
        /// <param name="created"></param>
        /// <param name="guid"></param>
        /// <param name="company"></param>
        /// <param name="keyword"></param>
        /// <returns></returns>
        public List<ImportantNoticeListModel> GetDangJianNoticeList(string userName, string created, string guid, string company, string keyword)
        {
            List<ImportantNoticeListModel> list = new List<ImportantNoticeListModel>();
            try
            {
                var queryListsString = config.QueryListsString;

                var viewFields = new string[] { "GUID", "Title", "Created", "_x4e3b__x9001__x5355__x4f4d_" };
                var viewFieldsString = CreateViewFieldsString(viewFields);

                StringBuilder queryString = new StringBuilder();
                queryString.Append("<Where>");
                if (string.IsNullOrWhiteSpace(company) && string.IsNullOrWhiteSpace(keyword))
                {
                    //company为空，keyword为空
                    queryString.Append("<And>");

                    queryString.Append("<Neq><FieldRef Name=\"GUID\" /><Value Type=\"Guid\">" + guid + "</Value></Neq>");

                    queryString.Append("<And>");
                    queryString.Append("<Lt><FieldRef Name =\"Created\" /><Value Type =\"DateTime\" IncludeTimeValue=\"True\">" + created + "</Value></Lt>");
                    queryString.Append("<Eq><FieldRef Name=\"PushArticleType\" /><Value Type=\"Text\">project</Value></Eq>");
                    queryString.Append("</And>");

                    queryString.Append("</And>");
                }
                else if (!string.IsNullOrWhiteSpace(company) && string.IsNullOrWhiteSpace(keyword))
                {
                    //company有值，keyword为空
                    queryString.Append("<And>");

                    queryString.Append("<And>");
                    queryString.Append("<Neq><FieldRef Name=\"GUID\" /><Value Type=\"Guid\">" + guid + "</Value></Neq>");
                    queryString.Append("<Eq><FieldRef Name=\"_x4e3b__x9001__x5355__x4f4d_\" /><Value Type=\"Text\">" + company + "</Value></Eq>");
                    queryString.Append("</And>");

                    queryString.Append("<And>");
                    queryString.Append("<Lt><FieldRef Name =\"Created\" /><Value Type =\"DateTime\" IncludeTimeValue=\"True\">" + created + "</Value></Lt>");
                    queryString.Append("<Eq><FieldRef Name=\"PushArticleType\" /><Value Type=\"Text\">project</Value></Eq>");
                    queryString.Append("</And>");

                    queryString.Append("</And>");
                }
                else if (string.IsNullOrWhiteSpace(company) && !string.IsNullOrWhiteSpace(keyword))
                {
                    //company为空，keyword有值
                    queryString.Append("<And>");

                    queryString.Append("<And>");
                    queryString.Append("<Neq><FieldRef Name=\"GUID\" /><Value Type=\"Guid\">" + guid + "</Value></Neq>");
                    queryString.Append("<Contains><FieldRef Name=\"Title\" /><Value Type=\"Text\">" + keyword + "</Value></Contains>");
                    queryString.Append("</And>");

                    queryString.Append("<And>");
                    queryString.Append("<Lt><FieldRef Name =\"Created\" /><Value Type =\"DateTime\" IncludeTimeValue=\"True\">" + created + "</Value></Lt>");
                    queryString.Append("<Eq><FieldRef Name=\"PushArticleType\" /><Value Type=\"Text\">project</Value></Eq>");
                    queryString.Append("</And>");

                    queryString.Append("</And>");
                }
                else
                {
                    //company有值，keyword有值
                    queryString.Append("<And>");//--

                    queryString.Append("<Neq><FieldRef Name=\"GUID\" /><Value Type=\"Guid\">" + guid + "</Value></Neq>");

                    queryString.Append("<And>");//----

                    queryString.Append("<And>");
                    queryString.Append("<Eq><FieldRef Name=\"_x4e3b__x9001__x5355__x4f4d_\" /><Value Type=\"Text\">" + company + "</Value></Eq>");
                    queryString.Append("<Contains><FieldRef Name=\"Title\" /><Value Type=\"Text\">" + keyword + "</Value></Contains>");
                    queryString.Append("</And>");

                    queryString.Append("<And>");
                    queryString.Append("<Lt><FieldRef Name =\"Created\" /><Value Type =\"DateTime\" IncludeTimeValue=\"True\">" + created + "</Value></Lt>");
                    queryString.Append("<Eq><FieldRef Name=\"PushArticleType\" /><Value Type=\"Text\">project</Value></Eq>");
                    queryString.Append("</And>");

                    queryString.Append("</And>");//----

                    queryString.Append("</And>");//--
                }
                queryString.Append("</Where>");
                queryString.Append("<OrderBy>");
                queryString.Append("<FieldRef Name=\"Created\" Ascending=\"False\" />");
                queryString.Append("</OrderBy>");
                var queryWebs = "<Webs Scope=\"SiteCollection\" />";

                MossListOperationSoapClient mos = new MossListOperationSoapClient();
                string result = mos.getNotificationsStringBySPSiteDataQuery(config.SiteUrl, config.WebUrl,
                    queryListsString,
                    viewFieldsString,
                    queryString.ToString(),
                    queryWebs,
                    string.IsNullOrWhiteSpace(guid) ? config.Count : config.Count - 1,
                    userName);
                DataTable dt = XmlToTable(result);
                list = DataConvertHelper<ImportantNoticeListModel>.ConvertToList(dt);
            }
            catch (Exception e)
            {
                log.Error("Error - 获取重要发文列表失败：" + JsonConvert.SerializeObject(e));
            }
            return list;
        }

        #endregion




        #region ReadXmlModel实体
        /// <summary>
        /// ReadXmlModel实体
        /// </summary>
        class ReadXmlModel
        {
            /// <summary>
            /// SiteUrl
            /// </summary>
            public string SiteUrl { get; set; }
            /// <summary>
            /// WebUrl
            /// </summary>
            public string WebUrl { get; set; }
            /// <summary>
            /// QueryListsString
            /// </summary>
            public string QueryListsString { get; set; }
            /// <summary>
            /// ReadCount
            /// </summary>
            public int Count { get; set; }
        }
        #endregion

        #region 生成显示字段的字符串
        /// <summary>
        /// 生成显示字段的字符串
        /// </summary>
        string CreateViewFieldsString(string[] fields)
        {
            StringBuilder viewFieldsString = new StringBuilder();
            foreach (string field in fields)
            {
                viewFieldsString.Append("<FieldRef Name=\"" + field + "\" />");
            }
            return viewFieldsString.ToString();
        }
        #endregion

        #region 将Xml(Base64)字符串转化成Table表
        /// <summary>
        /// 将Xml(Base64)字符串转化成Table表
        /// </summary>
        static DataTable XmlToTable(string base64Str)
        {
            try
            {
                DataSet ds = new DataSet();
                byte[] data = Convert.FromBase64String(base64Str);
                using (MemoryStream ms = new MemoryStream(data))
                {
                    ds.ReadXml(ms);
                }
                return ds.Tables[0];
            }
            catch (Exception ex)
            {
                return null;
            }
        }
        #endregion

        #region 基础字段
        /// <summary>
        /// 基础字段（WebId、WebName、ListId、ListName、ID）
        /// </summary>
        public class BaseField
        {
            /// <summary>
            /// WebId（如：A8F5690C-24D1-4A28-AF6A-B1A634E0AE50）
            /// </summary>
            public string WebId
            {
                set { _WebId = value; }
                get
                {
                    return Guid.Parse(_WebId).ToString().ToUpper();
                }
            }
            private string _WebId;
            /// <summary>
            /// WebName
            /// </summary>
            public string WebCompanyName
            {
                get
                {
                    if (string.IsNullOrWhiteSpace(WebId))
                    {
                        return string.Empty;
                    }
                    if (!Webs.ContainsKey(WebId))
                    {
                        return string.Empty;
                    }
                    return Webs[WebId];
                }
            }
            /// <summary>
            /// ListId（如：FBB85F14-D478-4F56-B120-B3F163710DD9）
            /// </summary>
            public string ListId
            {
                set { _ListId = value; }
                get
                {
                    return Guid.Parse(_ListId).ToString().ToUpper();
                }
            }
            private string _ListId;
            /// <summary>
            /// ListName
            /// </summary>
            public string ListName
            {
                get
                {
                    if (string.IsNullOrWhiteSpace(ListId))
                    {
                        return string.Empty;
                    }
                    if (!Lists.ContainsKey(ListId))
                    {
                        return string.Empty;
                    }
                    return Lists[ListId];
                }
            }
            /// <summary>
            /// 编号（如：2189）
            /// </summary>
            public string ID { set; get; }
        }
        #endregion
    }
}