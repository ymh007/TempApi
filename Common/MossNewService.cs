using log4net;
using Newtonsoft.Json;
using Seagull2.YuanXin.AppApi.NewsServer;
using System;
using System.Collections.Generic;
using System.Data;
using System.IO;
using System.Reflection;
using System.Text;
using System.Text.RegularExpressions;
using System.Web;
using System.Xml;

namespace Seagull2.YuanXin.AppApi
{
    /// <summary>
    /// Moss新闻通用服务类
    /// </summary>
    public class MossNewService
    {
        #region WebService - 测试
        public static List<string> FieldName =new List<string>();


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
        public static DataTable GetListItemByListItemID(string siteUrl, string webId, string listId, int listItemId, string userName)
        {
            MossListOperationSoapClient moss = new MossListOperationSoapClient();
            string str = moss.getListItemByListItemID(siteUrl, Guid.Parse(webId), Guid.Parse(listId), FieldName.ToArray(), listItemId, userName);
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
        /// GetNewsStringBySPSiteDataQuery
        /// </summary>
        public static DataTable GetNewsStringBySPSiteDataQuery(string siteUrl, string webUrl, string queryListsString, string viewFieldsString, string queryString, string queryWebs, int rowLimit, string userName)
        {
            MossListOperationSoapClient moss = new MossListOperationSoapClient();
            string str = moss.getNewsStringBySPSiteDataQuery(siteUrl, webUrl, queryListsString, viewFieldsString, queryString, queryWebs, rowLimit, userName);
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
        static Dictionary<string, string> Webs = new Dictionary<string, string>();
        /// <summary>
        /// List 字典，ID，Name
        /// </summary>
        static Dictionary<string, string> Lists = new Dictionary<string, string>();

        #region 构造
        /// <summary>
        /// 构造
        /// </summary>
        /// <param name="xmlName">Xml配置文件名称，如：News.xml</param>
        public MossNewService(string xmlName)
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
                        var webName = web.Attributes["Name"].Value;
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
        #endregion

        #region 获取网站信息
        /// <summary>
        /// 获取网站信息
        /// </summary>
        public WebModel GetWebModel(string webId, string userName)
        {
            WebModel model = null;
            try
            {
                MossListOperationSoapClient moss = new MossListOperationSoapClient();
                string str = moss.getWebByID(config.SiteUrl, Guid.Parse(webId), userName);
                DataTable dt = XmlToTable(str);
                if (dt.Rows.Count > 0)
                {
                    dt.Columns["网站ID"].ColumnName = "Id";
                    dt.Columns["网站名称"].ColumnName = "Name";
                    dt.Columns["网站标题"].ColumnName = "Title";
                    model = DataConvertHelper<WebModel>.ConvertToModel(dt.Rows[0]);
                }
            }
            catch (Exception e)
            {
                log.Error("Error - 获取网站信息失败：" + JsonConvert.SerializeObject(e));
            }
            return model;
        }
        /// <summary>
        /// 网站信息实体类
        /// </summary>
        public class WebModel
        {
            /// <summary>
            /// 网站ID（如：fbb85f14-d478-4f56-b120-b3f163710dd9）
            /// </summary>
            public string Id { set; get; }
            /// <summary>
            /// 网站名称（如：groupnews）
            /// </summary>
            public string Name { set; get; }
            /// <summary>
            /// 网站标题（如：集团新闻）
            /// </summary>
            public string Title { set; get; }
        }
        #endregion

        #region 获取管理报告列表
        /// <summary>
        /// 获取管理报告列表
        /// </summary>
        public List<ManagementReportListModel> GetManagementReportList(DateTime Created, string userName)
        {
            List<ManagementReportListModel> list = new List<ManagementReportListModel>();
            try
            {
                var queryListsString = config.QueryListsString;

                var viewFields = new string[] { "Title", "Created", "EncodedAbsUrl", "FileRef", "PublishingPageContent" };
                var viewFieldsString = CreateViewFieldsString(viewFields);

                StringBuilder queryString = new StringBuilder();
                queryString.Append("<Where>");
                queryString.Append("<And>");
                queryString.Append("<Lt><FieldRef Name =\"Created\" /><Value Type =\"DateTime\">" + Created.ToString("yyyy-MM-dd HH:mm:ss") + "</Value></Lt>");
                queryString.Append("<Eq><FieldRef Name=\"NewsCategory\" /><Value Type=\"Choice\">管理报告</Value></Eq>");
                queryString.Append("</And>");
                queryString.Append("</Where>");

                queryString.Append("<OrderBy>");
                queryString.Append("<FieldRef Name=\"Created\" Ascending=\"False\" />");
                queryString.Append("</OrderBy>");

                StringBuilder queryWebs = new StringBuilder();
                queryWebs.Append("<Webs Scope=\"SiteCollection\" />");

                MossListOperationSoapClient mos = new MossListOperationSoapClient();
                string result = mos.getNewsStringBySPSiteDataQuery(config.SiteUrl, config.WebUrl,
                    queryListsString,
                    viewFieldsString,
                    queryString.ToString(),
                    queryWebs.ToString(),
                    config.Count,
                    userName);
                DataTable dt = XmlToTable(result);
                list = DataConvertHelper<ManagementReportListModel>.ConvertToList(dt);
            }
            catch (Exception e)
            {
                log.Error("Error - 获取管理报告列表失败：" + JsonConvert.SerializeObject(e));
            }
            return list;
        }
        /// <summary>
        /// 管理报告列表实体类
        /// </summary>
        public class ManagementReportListModel
        {
            /// <summary>
            /// WebId（如：A8F5690C-24D1-4A28-AF6A-B1A634E0AE50）
            /// </summary>
            public string WebId { set; get; }
            /// <summary>
            /// ListId（如：FBB85F14-D478-4F56-B120-B3F163710DD9）
            /// </summary>
            public string ListId { set; get; }
            /// <summary>
            /// 编号（如：2189）
            /// </summary>
            public string ID { set; get; }
            /// <summary>
            /// 标题（如：品味四季•悦享人生——青岛远洋万和四季盛大交付）
            /// </summary>
            public string Title { set; get; }
            /// <summary>
            /// 创建时间（如：2017-07-03 11:18:05）
            /// </summary>
            public string Created { set; get; }
            /// <summary>
            /// 编码形式的绝对 URL（如：http://km.sinooceanland.com/）
            /// </summary>
            public string EncodedAbsUrl { set; get; }
            /// <summary>
            /// URL 路径（如：2187;#sites/NewsCenter/departmentnews/Pages/品味四季悦享人生青岛远洋万和四季盛大交付.aspx）
            /// </summary>
            public string FileRef { set; get; }
            /// <summary>
            /// 文章URL（自定义）
            /// </summary>
            public string ArticleUrl
            {
                get
                {
                    var url = EncodedAbsUrl + FileRef;
                    url = Regex.Replace(url, @"[0-9]*;#", "");//替换url中的 ID;#（2187;#）
                    return url;
                }
            }
            /// <summary>
            /// 文章内容（HTML格式）
            /// </summary>
            public string PublishingPageContent { set; get; }
        }
        #endregion

        #region 获取管理报告详细内容
        /// <summary>
        /// 获取管理报告详细内容
        /// </summary>
        public ManagementReportArticleModel GetManagementReportModel(string webId, string listId, int listItemId, string userName)
        {
            ManagementReportArticleModel model = null;
            try
            {
                MossListOperationSoapClient mos = new MossListOperationSoapClient();
                string result = mos.getListItemByListItemID(config.SiteUrl, Guid.Parse(webId), Guid.Parse(listId), FieldName.ToArray(), listItemId, userName);
                DataTable dt = XmlToTable(result);
                model = DataConvertHelper<ManagementReportArticleModel>.ConvertToModel(dt, 0, 3);
            }
            catch (Exception e)
            {
                log.Error("Error - 获取管理报告详细内容失败：" + JsonConvert.SerializeObject(e));
            }
            return model;
        }
        /// <summary>
        /// 管理报告详情实体类
        /// </summary>
        public class ManagementReportArticleModel
        {
            /// <summary>
            /// 标题
            /// </summary>
            public string Title { set; get; }
            /// <summary>
            /// 完整URL路径
            /// </summary>
            public string EncodedAbsUrl { set; get; }
            /// <summary>
            /// 作者（如：李承文）
            /// </summary>
            public string ArticleByLine { set; get; }
            /// <summary>
            /// 页面内容
            /// </summary>
            public string PublishingPageContent { set; get; }
        }
        #endregion

        #region 获取图片新闻、推荐新闻
        /// <summary>
        /// 获取图片新闻、推荐新闻
        /// </summary>
        /// <param name="userName">用户名</param>
        /// <param name="type">图片、推荐</param>
        public List<ImageRecommendNews> GetImageRecommendNews(string userName, string type)
        {
            List<ImageRecommendNews> list = new List<ImageRecommendNews>();
            try
            {
                var queryListsString = config.QueryListsString;

                var viewFields = new string[] { "Title", "NewsAddress", "RecOrder", "PicOrder", "EIPImage", "Created", "Modified" };
                var viewFieldsString = CreateViewFieldsString(viewFields);

                StringBuilder queryString = new StringBuilder();
                queryString.Append("<Where>");
                if (type == "图片")
                {
                    queryString.Append("<Contains><FieldRef Name=\"PicOrder\" /><Value Type=\"Choice\">图片</Value></Contains>");
                }
                if (type == "推荐")
                {
                    queryString.Append("<Contains><FieldRef Name=\"RecOrder\" /><Value Type=\"Choice\">推荐</Value></Contains>");
                }
                if (type == "党建图片")
                {
                    queryString.Append("<Contains><FieldRef Name=\"PicOrder\" /><Value Type=\"Choice\">党建</Value></Contains>");
                }
                if (type == "党建推荐")
                {
                    queryString.Append("<Contains><FieldRef Name=\"RecOrder\" /><Value Type=\"Choice\">党建</Value></Contains>");
                }
                queryString.Append("</Where>");
              
                var queryWebs = "<Webs Scope=\"SiteCollection\" />";

                MossListOperationSoapClient mos = new MossListOperationSoapClient();
                string result = mos.getNewsStringBySPSiteDataQuery(config.SiteUrl, config.WebUrl,
                    queryListsString,
                    viewFieldsString,
                    queryString.ToString(),
                    queryWebs,
                    config.Count,
                    userName);
                DataTable dt = XmlToTable(result);
                list = DataConvertHelper<ImageRecommendNews>.ConvertToList(dt);
            }
            catch (Exception e)
            {
                log.Error("Error - 获取" + type + "新闻列表失败：" + JsonConvert.SerializeObject(e));
            }
            return list;
        }
        /// <summary>
        /// 图片新闻、推荐新闻
        /// </summary>
        public class ImageRecommendNews : BaseField
        {
            /// <summary>
            /// 标题
            /// </summary>
            public string Title { set; get; }
            /// <summary>
            /// 新闻地址（https://workflow.sinooceangroup.com/THRWebApp/eip/#/news/22929311-7D15-4DB5-9AE9-45D5DEFC7A30）
            /// </summary>
            public string NewsAddress { set; get; }
            /// <summary>
            /// 推荐新闻排序（推荐第二条 #2）
            /// </summary>
            public string RecOrder { set; get; }
            /// <summary>
            /// 图片新闻排序（图片新闻第二条 #2）
            /// </summary>
            public string PicOrder { set; get; }
            /// <summary>
            /// 封面图，两个URL是一样的，取其中之一即可（http://km.sinooceanland.com/sites/NewsCenter/departmentnews/PublishingImages/共建一线养老服务人才的“黄埔军校”——2017年全国“椿萱之星”系列技能大赛圆满落幕/1-1.jpg, http://km.sinooceanland.com/sites/NewsCenter/departmentnews/PublishingImages/共建一线养老服务人才的“黄埔军校”——2017年全国“椿萱之星”系列技能大赛圆满落幕/1-1.jpg）
            /// </summary>
            public string EIPImage { set; get; }
            /// <summary>
            /// 创建时间（如：2017-07-03 11:18:05）
            /// </summary>
            public string Created { set; get; }

            /// <summary>
            /// 修改时间（如：2017-07-03 11:18:05）
            /// </summary>
            public string Modified { set; get; }
        }
        #endregion

        #region 获取新闻列表
        /// <summary>
        /// 获取新闻列表
        /// </summary>
        /// <param name="userName">用户名</param>
        /// <param name="created">时间</param>
        /// <param name="company">所属公司</param>
        /// <param name="keyword">标题关键词</param>
        /// <param name="newsCategory">新闻分类</param>
        public List<NewsList> GetNewsList(string userName, string created, string company, string keyword, string newsCategory)
        {
            List<NewsList> list = new List<NewsList>();
            try
            {
                var queryListsString = config.QueryListsString;

                var viewFields = new string[] { "GUID", "Title", "Created"};
                var viewFieldsString = CreateViewFieldsString(viewFields);
                viewFieldsString += "<FieldRef Name=\"Company1\" Nullable=\"TRUE\" />";

                StringBuilder queryString1 = new StringBuilder();
                StringBuilder queryString2 = new StringBuilder();


                queryString1.Append("<Where>");

                queryString2.Append("<And>");

                queryString2.Append("<And>");
                queryString2.Append("<Lt><FieldRef Name=\"Created\" /><Value Type=\"DateTime\" IncludeTimeValue=\"True\">" + created + "</Value></Lt>");
                queryString2.Append("<Neq><FieldRef Name=\"NoPubDisplay\" /><Value Type=\"Text\">1</Value></Neq>");
                queryString2.Append("</And>");

                queryString2.Append("<And>");
                queryString2.Append("<Neq><FieldRef Name=\"NewsPosition\" /><Value Type=\"Choice\">外网发布</Value></Neq>");
                queryString2.Append("<Neq><FieldRef Name=\"NewsType\" /><Value Type=\"Choice\">视频新闻</Value></Neq>");
                queryString2.Append("</And>");

                queryString2.Append("</And>");

                // company
                if (!string.IsNullOrWhiteSpace(company))
                {
                    queryString1.Append("<And>");
                    queryString2.Append("<Eq><FieldRef Name=\"Company1\" /><Value Type=\"Choice\">" + company + "</Value></Eq>");
                    queryString2.Append("</And>");
                }

                // keyword
                if (!string.IsNullOrWhiteSpace(keyword))
                {
                    queryString1.Append("<And>");
                    queryString2.Append("<Contains><FieldRef Name=\"Title\" /><Value Type=\"Text\">" + keyword + "</Value></Contains>");
                    queryString2.Append("</And>");
                }

                // newsCategory
                if (!string.IsNullOrWhiteSpace(newsCategory))
                {
                    queryString1.Append("<And>");
                    queryString2.Append("<Eq><FieldRef Name=\"NewsCategory\" /><Value Type=\"Choice\">" + newsCategory + "</Value></Eq>");
                    queryString2.Append("</And>");
                }

                queryString2.Append("</Where>");

                queryString2.Append("<OrderBy>");
                queryString2.Append("<FieldRef Name=\"Created\" Ascending=\"False\" />");
                queryString2.Append("</OrderBy>");

                var queryWebs = "<Webs Scope=\"SiteCollection\" />";

                MossListOperationSoapClient mos = new MossListOperationSoapClient();
                string result = mos.getNewsStringBySPSiteDataQuery(config.SiteUrl, config.WebUrl,
                    queryListsString,
                    viewFieldsString,
                    queryString1.ToString() + queryString2.ToString(),
                    queryWebs,
                    config.Count,
                    userName);
                DataTable dt = XmlToTable(result);
                list = DataConvertHelper<NewsList>.ConvertToList(dt);
            }
            catch (Exception e)
            {
                log.Error("Error - 获取新闻列表失败：" + JsonConvert.SerializeObject(e));
            }
            return list;
        }
        /// <summary>
        /// 新闻列表
        /// </summary>
        public class NewsList : BaseField
        {
            /// <summary>
            /// 新闻GUID
            /// </summary>
            public string GUID { get; set; }
            /// <summary>
            /// 标题
            /// </summary>
            public string Title { set; get; }
            /// <summary>
            /// 创建时间（如：2017-07-03 11:18:05）
            /// </summary>
            public string Created { set; get; }
            /// <summary>
            /// 发布单位
            /// </summary>
            public string Company1 { set; get; }
        }
        #endregion

        #region 获取新闻详情
        /// <summary>
        /// 获取新闻详情
        /// </summary>
        public NewsDetail GetNewsDetailByListItemId(string userName, string webId, string listId, int id)
        {
            NewsDetail model = null;
            try
            {
                MossListOperationSoapClient mos = new MossListOperationSoapClient();
                string result = mos.getListItemByListItemID(config.SiteUrl, Guid.Parse(webId), Guid.Parse(listId), FieldName.ToArray(), id, userName);
                DataTable dt = XmlToTable(result); 
                model = DataConvertHelper<NewsDetail>.ConvertToModel(dt, 0, 3);
            }
            catch (Exception e)
            {
                log.Error("Error - 获取新闻详情失败：" + JsonConvert.SerializeObject(e));
            }
            return model;
        }
        /// <summary>
        /// 获取新闻详情
        /// </summary>
        public NewsDetail GetNewsDetailByGuid(string userName, string guid)
        {
            NewsDetail model = new NewsDetail();
            try
            {
                var queryListsString = config.QueryListsString;

                var viewFields = new string[] { "Title", "Created", "PublishingPageContent", "EncodedAbsUrl", "FileRef" };
                var viewFieldsString = CreateViewFieldsString(viewFields);

                StringBuilder queryString = new StringBuilder();
                queryString.Append("<Where>");
                queryString.Append("<And>");
                queryString.Append("<Eq><FieldRef Name=\"GUID\" /><Value Type=\"Guid\">" + guid + "</Value></Eq>");
                queryString.Append("<Neq><FieldRef Name=\"NewsPosition\" /><Value Type=\"Choice\">外网发布</Value></Neq>");
                queryString.Append("</And>");
                queryString.Append("</Where>");
                queryString.Append("<OrderBy>");
                queryString.Append("<FieldRef Name=\"Created\" Ascending=\"False\" />");
                queryString.Append("</OrderBy>");

                var queryWebs = "<Webs Scope=\"SiteCollection\" />";

                MossListOperationSoapClient mos = new MossListOperationSoapClient();
                string result = mos.getNewsStringBySPSiteDataQuery(config.SiteUrl, config.WebUrl,
                    queryListsString,
                    viewFieldsString,
                    queryString.ToString(),
                    queryWebs,
                    config.Count,
                    userName);
                DataTable dt = XmlToTable(result);
                model = DataConvertHelper<NewsDetail>.ConvertToModel(dt.Rows[0]);
            }
            catch (Exception e)
            {
                log.Error("Error - 获取新闻详情失败：" + JsonConvert.SerializeObject(e));
            }
            return model;
        }
        /// <summary>
        /// 新闻详情
        /// </summary>
        public class NewsDetail
        {
            /// <summary>
            /// 标题
            /// </summary>
            public string Title { set; get; }
            /// <summary>
            /// 创建时间（如：2017-07-03 11:18:05）
            /// </summary>
            public string Created { set; get; }
            /// <summary>
            /// 新闻内容
            /// </summary>
            public string PublishingPageContent { set; get; }
            /// <summary>
            /// 新闻地址（推荐新闻有该字段，如：https://workflow.sinooceangroup.com/THRWebApp/eip/#/news/72D86FE3-F2D6-4C6C-B7C3-2430AC5C49FA）
            /// </summary>
            public string NewsAddress { set; get; }
            /// <summary>
            /// 新闻地址（普通新闻有该字段，如：http://km.sinooceanland.com/sites/NewsCenter/groupnews/Pages/新证金融正式进驻亿街区共享智慧社区生态价值.aspx）
            /// </summary>
            public string EncodedAbsUrl { set; get; }
            /// <summary>
            /// 推荐新闻地址字符串
            /// </summary>
            public string FileRef { set; get; }
            /// <summary>
            /// 发布公司（普通新闻有该字段）
            /// </summary>
            public string Company1 { set; get; }
        }
        #endregion

        #region 获取资讯信息列表
        /// <summary>
        /// 获取新闻列表
        /// </summary>
        /// <param name="userName">用户名</param>
        /// <param name="created">时间</param>
        public List<InformationList> GetInformationList(string userName, string created, string company, string keyword)
        {
            List<InformationList> list = new List<InformationList>();
            try
            {
                var queryListsString = config.QueryListsString;

                var viewFields = new string[] { "Title", "Created", "ClassB" , "PublishOrganization" };
                var viewFieldsString = CreateViewFieldsString(viewFields);

                StringBuilder queryString1 = new StringBuilder();
                StringBuilder queryString2 = new StringBuilder();


                queryString1.Append("<Where>");

                queryString2.Append("<And>");
                queryString2.Append("<Lt><FieldRef Name=\"Created\" /><Value Type=\"DateTime\" IncludeTimeValue=\"True\">" + created + "</Value></Lt>");
                queryString2.Append("<Eq><FieldRef Name=\"ClassA\" /><Value Type=\"Text\">资讯信息</Value></Eq>");
                queryString2.Append("</And>");

                // company
                if (!string.IsNullOrWhiteSpace(company))
                {
                    queryString1.Append("<And>");
                    queryString2.Append("<Eq><FieldRef Name=\"PublishOrganization\" /><Value Type=\"Text\">" + company + "</Value></Eq>");
                    queryString2.Append("</And>");
                }

                // keyword
                if (!string.IsNullOrWhiteSpace(keyword))
                {
                    queryString1.Append("<And>");
                    queryString2.Append("<Contains><FieldRef Name=\"Title\" /><Value Type=\"Text\">" + keyword + "</Value></Contains>");
                    queryString2.Append("</And>");
                }

                queryString2.Append("</Where>");

                queryString2.Append("<OrderBy>");
                queryString2.Append("<FieldRef Name=\"Created\" Ascending=\"False\" />");
                queryString2.Append("</OrderBy>");

                var queryWebs = "<Webs Scope=\"SiteCollection\" />";

                MossListOperationSoapClient mos = new MossListOperationSoapClient();
                string result = mos.getNewsStringBySPSiteDataQuery(config.SiteUrl, config.WebUrl,
                    queryListsString,
                    viewFieldsString,
                    queryString1.ToString() + queryString2.ToString(),
                    queryWebs,
                    config.Count,
                    userName);
                DataTable dt = XmlToTable(result);
                list = DataConvertHelper<InformationList>.ConvertToList(dt);
            }
            catch (Exception e)
            {
                log.Error("Error - 获取资讯信息列表失败：" + JsonConvert.SerializeObject(e));
            }
            return list;
        }
        /// <summary>
        /// 资讯信息列表
        /// </summary>
        public class InformationList : BaseField
        {
            /// <summary>
            /// 标题
            /// </summary>
            public string Title { set; get; }
            /// <summary>
            /// 创建时间（如：2017-07-03 11:18:05）
            /// </summary>
            public string Created { set; get; }
            /// <summary>
            /// 分类
            /// </summary>
            public string ClassB { set; get; }
        }
        #endregion

        #region 获取附件列表
        /// <summary>
        /// 获取附件列表
        /// </summary>
        public List<AttachmentModel> GetAttachments(string userName, string webId, string listId, int listItemId)
        {
            List<AttachmentModel> list = new List<AttachmentModel>();
            try
            {
                MossListOperationSoapClient mos = new MossListOperationSoapClient();
                string result = mos.getListItemAttachmentsByListItemID(config.SiteUrl, Guid.Parse(webId), Guid.Parse(listId), listItemId, userName);
                DataTable dt = XmlToTable(result);
                foreach (DataRow dr in dt.Rows)
                {
                    list.Add(new AttachmentModel()
                    {
                        Name = dr[0].ToString(),
                        Url = dr[1].ToString()
                    });
                }
            }
            catch (Exception e)
            {
                log.Error("Error - 获取附件列表失败：" + JsonConvert.SerializeObject(e));
            }
            return list;
        }
        /// <summary>
        /// 附件实体类
        /// </summary>
        public class AttachmentModel
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
            catch
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
            public string WebId { set; get; }
            /// <summary>
            /// WebName
            /// </summary>
            public string WebName
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
            public string ListId { set; get; }
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