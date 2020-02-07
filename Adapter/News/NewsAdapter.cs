using MCS.Library.OGUPermission;
using MCS.Library.Principal;
using MCS.Library.SOA.DataObjects;
using Seagull2.YuanXin.AppApi.Models;
using Seagull2.YuanXin.AppApi.NewsServer;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Data.SqlClient;
using System.IO;
using System.Linq;
using System.Text;
using System.Web;
using System.Xml;

namespace Seagull2.YuanXin.AppApi.Adapter.News
{
    /// <summary>
    /// 新闻数据适配器
    /// </summary>
    public class NewsAdapter : UpdatableAndLoadableAdapterBase<NewsModel, NewsModelCollection>
    {
        /// <summary>
        /// 实例
        /// </summary>
        public static NewsAdapter Instance = new NewsAdapter();

       






        /// <summary>
        /// 根据用户ID查询具有权限的新闻
        /// </summary>
        /// <param name="logionUser">用户的对象</param>
        /// <param name="CreateTime"></param>
        public NewsModelCollection GetNewsModelCollectionByLogion(IUser logionUser, string CreateTime = "")
        {
            DataTable newsTable = GetNewsModelCollectionByLogion(16, logionUser.LogOnName, CreateTime);

            return GetNewsCollectionForTable(newsTable);
        }

        /// <summary>
        /// 根据用户ID查询具有权限的新闻（过滤推荐新闻）
        /// </summary>
        public NewsModelCollection GetNoRecommendedNewsModelCollectionByLogion(string userName, string CreateTime = "")
        {
            int count = 0;
            DataTable newsTable = GetNoRecommendedNewsModelCollectionByLogion(16, userName, ref count, CreateTime);
            NewsModelCollection newsModelCollection = GetNewsCollectionForTable(newsTable);
            //从结果中过滤掉图片新闻和推荐新闻并返回
            return DeleteRecommendedNews(newsModelCollection, userName, count);
        }

        /// <summary>
        /// 排除推荐的新闻
        /// </summary>
        /// <param name="newsModelCollection">查询新闻结果集</param>
        /// <param name="logionUser">当前用户</param>
        /// <param name="count">新闻实际查询结果的条数</param>
        /// <returns>排除掉推荐新闻的新闻结果集</returns>
        public NewsModelCollection DeleteRecommendedNews(NewsModelCollection newsModelCollection, string userNames, int count)
        {
            //实际上新闻列表要显示的数量
            int showCount = 16;
            //如果查询结果的数量小于列表要显示的数量，则重新给列表显示数量复制
            if (newsModelCollection.Count < showCount)
            {
                showCount = newsModelCollection.Count;
            }
            NewsModelCollection newsNoRecommendedModelCollection = new NewsModelCollection();
            //图片新闻列表
            RecommendedImageNewsModelCollection recommendedImageNewsModelList =
             RecommendedImageNewsModelAdapter.Instance.GetRecommendedImageNewsModelCollectionByLogion(userNames);
            //推荐新闻列表
            RecommendedNewsModelCollection recommendedNewsModelList =
               RecommendedNewsModelAdapter.Instance.GetRecommendedNewsModelCollectionByLogion(userNames);
            int num = 0;
            if (newsModelCollection != null && newsModelCollection.Count > 0)
            {
                foreach (NewsModel news in newsModelCollection)
                {
                    bool isRecommended = false;
                    string newsLink = news.Link.ToLower().Trim();
                    if (newsLink.Contains('?'))
                    {
                        newsLink = newsLink.Split('?')[0];
                    }
                    string[] newsLinkStringArray = newsLink.Split('/');

                    #region 判断新闻是否是图片新闻
                    if (recommendedImageNewsModelList != null && recommendedImageNewsModelList.Count > 0)
                    {
                        foreach (RecommendedImageNewsModel recommendedImageNews in recommendedImageNewsModelList)
                        {
                            string link = recommendedImageNews.Link.ToLower().Trim();
                            if (link.Contains('?'))
                            {
                                link = link.Split('?')[0];
                            }
                            string[] recommendedImageLinkStringArray = link.Split('/');
                            if (recommendedImageLinkStringArray != null
                                && newsLinkStringArray != null
                                && HttpUtility.UrlDecode(newsLinkStringArray[newsLinkStringArray.Length - 1]) == HttpUtility.UrlDecode(recommendedImageLinkStringArray[recommendedImageLinkStringArray.Length - 1]))
                            {
                                isRecommended = true;
                            }
                        }
                    }
                    #endregion

                    #region 判断新闻是否是推荐新闻
                    if (recommendedNewsModelList != null && recommendedNewsModelList.Count > 0)
                    {
                        foreach (RecommendedNewsModel recommendedNews in recommendedNewsModelList)
                        {
                            string link = recommendedNews.Link.ToLower().Trim();
                            if (link.Contains('?'))
                            {
                                link = link.Split('?')[0];
                            }
                            string[] recommendedLinkStringArray = link.Split('/');
                            if (recommendedLinkStringArray != null && newsLinkStringArray != null && HttpUtility.UrlDecode(newsLinkStringArray[newsLinkStringArray.Length - 1]) == HttpUtility.UrlDecode(recommendedLinkStringArray[recommendedLinkStringArray.Length - 1]))
                            {
                                isRecommended = true;
                            }
                        }
                    }
                    #endregion

                    if (!isRecommended)
                    {
                        //如果当前条数小于要显示的数量，则继续添加，否则退出循环
                        if (num < showCount)
                        {
                            newsNoRecommendedModelCollection.Add(news);
                            num = num + 1;
                        }
                        else
                        {
                            break;
                        }
                    }
                }
            }
            return newsNoRecommendedModelCollection;
        }

        /// <summary>
        /// 首次加载查询未读新闻（读取是最新的，未读的）
        /// </summary>
        /// <param name="logionUser">用户的对象</param>
        public NewsLoadModel GetNewsModelCollectionByLoad(string userName, string CreateTime = "")
        {
            DataTable newsTable = GetNewsModelCollectionByLoad(10, userName, CreateTime);
            NewsModelCollection newsModelCollection = GetNewsCollectionForTable(newsTable);
            newsModelCollection = DeleteRecommendedNewsList(newsModelCollection, userName);
            return GetNewsLoadModelNoReadCode(newsModelCollection, DateTime.Parse(CreateTime));
        }

        /// <summary>
        /// 排除推荐的新闻返回首次加载的列表
        /// </summary>
        public NewsModelCollection DeleteRecommendedNewsList(NewsModelCollection newsModelCollection, string userName)
        {
            NewsModelCollection newsNoRecommendedModelCollection = new NewsModelCollection();
            RecommendedImageNewsModelCollection RecommendedImageNewsModelList =
             RecommendedImageNewsModelAdapter.Instance.GetRecommendedImageNewsModelCollectionByLogion(
                userName);
            RecommendedNewsModelCollection RecommendedNewsModelList =
               RecommendedNewsModelAdapter.Instance.GetRecommendedNewsModelCollectionByLogion(
                  userName);
            if (newsModelCollection != null && newsModelCollection.Count > 0)
            {
                foreach (NewsModel news in newsModelCollection)
                {
                    bool isRecommended = false;
                    string[] newsLinkStringArray = news.Link.Split('/');
                    if (RecommendedImageNewsModelList != null && RecommendedImageNewsModelList.Count > 0)
                    {
                        foreach (RecommendedImageNewsModel recommendedImageNews in RecommendedImageNewsModelList)
                        {
                            string link = recommendedImageNews.Link.ToLower();
                            if (link.Contains('?'))
                            {
                                link = link.Split('?')[0];
                            }
                            string[] recommendedImageLinkStringArray = link.Split('/');
                            if (recommendedImageLinkStringArray != null && newsLinkStringArray != null && newsLinkStringArray[newsLinkStringArray.Length - 1] == recommendedImageLinkStringArray[recommendedImageLinkStringArray.Length - 1])
                            {
                                isRecommended = true;
                            }
                        }
                    }
                    if (RecommendedNewsModelList != null && RecommendedNewsModelList.Count > 0)
                    {
                        foreach (RecommendedNewsModel recommendedNews in RecommendedNewsModelList)
                        {
                            string link = recommendedNews.Link.ToLower();
                            if (link.Contains('?'))
                            {
                                link = link.Split('?')[0];
                            }
                            string[] recommendedLinkStringArray = link.Split('/');
                            if (recommendedLinkStringArray != null && newsLinkStringArray != null && newsLinkStringArray[newsLinkStringArray.Length - 1] == recommendedLinkStringArray[recommendedLinkStringArray.Length - 1])
                            {
                                isRecommended = true;
                            }
                        }
                    }
                    if (!isRecommended)
                    {
                        newsNoRecommendedModelCollection.Add(news);
                    }
                }
            }
            return newsNoRecommendedModelCollection;
        }
        /// <summary>
        /// 构造函数
        /// </summary>
        /// <param name="strUrlPath">xml文件的路径</param>
        /// <param name="strColumnArray">要取得的字段的名称</param>
        public XmlDocument XmlManager(string strUrlPath)
        {
            XmlDocument xmlDocument = new XmlDocument();
            if (!File.Exists(strUrlPath))
            {
                throw new Exception("指定的文件路径错误 请重新指定");
            }
            try
            {
                xmlDocument.Load(strUrlPath);
            }
            catch
            {
                throw new Exception("加载XML文档时发生错误");
            }
            return xmlDocument;
        }

        /// <summary>
        /// 根据新闻所有该类型的数据
        /// </summary>
        /// <returns></returns>
        public DataTable GetNewsModelCollectionByLogion(int count, string userName, string CreateTime)
        {

            string strWebPath = string.Empty;
            StringBuilder queryListsString = new StringBuilder();
            var model = ReadXml(false, "");

            queryListsString.Append("<Lists>");
            queryListsString.Append(model.QueryListsString);
            queryListsString.Append("</Lists>");
            StringBuilder queryString = new StringBuilder();
            if (!string.IsNullOrEmpty(CreateTime))
            {
                queryString.Append("<Where><And><Lt><FieldRef IncludeTimeValue='true' Name=\"Created\" /><Value Type=\"DateTime\">" + CreateTime + "</Value></Lt><Neq><FieldRef Name=\"NewsPosition\" /><Value Type=\"Choice\">外网发布</Value></Neq></And></Where>");
            }
            else
            {
                queryString.Append("<Where>");
                queryString.Append("<Neq><FieldRef Name=\"NewsPosition\" /><Value Type=\"Choice\">外网发布</Value></Neq>");
                queryString.Append("</Where>");
            }
            queryString.Append("<OrderBy>");
            queryString.Append("<FieldRef Name=\"Created\" Ascending=\"False\" />");
            queryString.Append("</OrderBy>");
            string ViewFieldsString = "<FieldRef Name=\"GUID\" /><FieldRef Name=\"Title\" /><FieldRef Name=\"Modified\"/><FieldRef Name=\"LinkFilename\" /><FieldRef Name=\"NewsType\"/><FieldRef Name=\"ArticleStartDate\"/><FieldRef Name=\"Created\"/><FieldRef Name=\"_ModerationStatus\"/><FieldRef Name=\"Company1\" Nullable=\"TRUE\" />";
            string queryWebsString = "<Webs Scope='SiteCollection' />";
            if (!model.QueryLists)
            {
                queryListsString = new StringBuilder();
            }
            MossListOperationSoapClient mos = new MossListOperationSoapClient();
            string s = string.Empty;
            try
            {
                s = mos.getNewsStringBySPSiteDataQuery(model.StrSitePath, strWebPath, queryListsString.ToString(),
                ViewFieldsString,
                queryString.ToString(), queryWebsString, count, "sinooceanland\\" + userName);
            }
            catch (Exception)
            {
            }
            return XMLtoDataTable(s);
        }

        /// <summary>
        /// 根据新闻所有该类型的数据（过滤推荐新闻）
        /// </summary>
        /// <returns></returns>
        public DataTable GetNoRecommendedNewsModelCollectionByLogion(int count, string userName, ref int realCount, string CreateTime)
        {
            XmlDocument xmlDocument = new XmlDocument();
            xmlDocument = XmlManager(HttpContext.Current.Server.MapPath("../XmlConfig/News.xml"));

            string strSitePath = string.Empty;
            string strWebPath = string.Empty;
            //得到SITE的集合
            XmlNodeList xmlNodeList = xmlDocument.GetElementsByTagName("Site");
            //读取从每个库中取得数据的条数
            int readCount = Convert.ToInt32(xmlDocument.DocumentElement.Attributes["ReadCount"].Value);
            if (readCount > 0)
            {
                //因为把推荐新闻和图片新闻的数量扣除掉所有要增加查询量
                count = readCount + 10;
            }
            XmlNodeList xmlWebs;
            XmlNodeList xmlWeb;
            XmlNodeList xmlLists;
            XmlNodeList xmlList;
            StringBuilder queryListsString = new StringBuilder();
            bool queryLists = false;
            queryListsString.Append("<Lists>");
            for (int i = 0; i < xmlNodeList.Count; i++)
            {
                strSitePath = xmlNodeList[i].Attributes["Path"].Value;
                xmlWebs = xmlNodeList[i].ChildNodes;
                for (int j = 0; j < xmlWebs.Count; j++)
                {
                    xmlWeb = xmlWebs[j].ChildNodes;
                    for (int m = 0; m < xmlWeb.Count; m++)
                    {
                        xmlLists = xmlWeb[m].ChildNodes;
                        for (int n = 0; n < xmlLists.Count; n++)
                        {
                            xmlList = xmlLists[n].ChildNodes;
                            for (int h = 0; h < xmlList.Count; h++)
                            {
                                XmlElement xmlEle = (XmlElement)xmlList[h];
                                queryListsString.Append("<List ID='" + xmlEle.Attributes["ID"].Value + "'></List>");
                                queryLists = true;
                            }
                        }
                    }
                }
            }
            queryListsString.Append("</Lists>");
            StringBuilder queryString = new StringBuilder();
            if (!string.IsNullOrEmpty(CreateTime))
            {
                queryString.Append("<Where><And><Lt><FieldRef IncludeTimeValue='true' Name=\"Created\" /><Value Type=\"DateTime\">" + CreateTime + "</Value></Lt><Neq><FieldRef Name=\"NewsPosition\" /><Value Type=\"Choice\">外网发布</Value></Neq></And></Where>");
            }
            else
            {
                queryString.Append("<Where>");
                queryString.Append("<Neq><FieldRef Name=\"NewsPosition\" /><Value Type=\"Choice\">外网发布</Value></Neq>");
                queryString.Append("</Where>");
            }
            queryString.Append("<OrderBy>");
            queryString.Append("<FieldRef Name=\"ArticleStartDate\" Ascending=\"False\" /><FieldRef Name=\"Created\" Ascending=\"False\" />");
            queryString.Append("</OrderBy>");
            string ViewFieldsString = "<FieldRef Name=\"GUID\" /><FieldRef Name=\"Created\" /><FieldRef Name=\"Title\" /><FieldRef Name=\"Modified\"/><FieldRef Name=\"LinkFilename\" /><FieldRef Name=\"NewsType\"/><FieldRef Name=\"ArticleStartDate\"/><FieldRef Name=\"Created\"/><FieldRef Name=\"_ModerationStatus\"/><FieldRef Name=\"Company1\" Nullable=\"TRUE\" />";
            string queryWebsString = "<Webs Scope='SiteCollection' />";
            if (!queryLists)
            {
                queryListsString = new StringBuilder();
            }
            MossListOperationSoapClient mos = new MossListOperationSoapClient();
            string s = string.Empty;
            try
            {
                s = mos.getNewsStringBySPSiteDataQuery(strSitePath, strWebPath, queryListsString.ToString(),
                ViewFieldsString,
                queryString.ToString(), queryWebsString, count, "sinooceanland\\" + userName);
            }
            catch (Exception)
            {
            }
            DataTable dt = XMLtoDataTable(s);
            //实际查询出的记录条数
            realCount = dt?.Rows.Count ?? 0;
            return dt;
        }

        /// <summary>
        /// 根据新闻所有该类型的数据
        /// </summary>
        /// <returns></returns>
        public DataTable GetNewsModelCollectionByLoad(int count, string userName, string CreateTime)
        {
            XmlDocument xmlDocument = new XmlDocument();
            xmlDocument = XmlManager(HttpContext.Current.Server.MapPath("../XmlConfig/News.xml"));

            string strSitePath = string.Empty;
            string strWebPath = string.Empty;
            //得到SITE的集合
            XmlNodeList xmlNodeList = xmlDocument.GetElementsByTagName("Site");
            ////读取从每个库中取得数据的条数
            count = 400;
            XmlNodeList xmlWebs;
            XmlNodeList xmlWeb;
            XmlNodeList xmlLists;
            XmlNodeList xmlList;
            StringBuilder queryListsString = new StringBuilder();
            bool queryLists = false;
            queryListsString.Append("<Lists>");
            for (int i = 0; i < xmlNodeList.Count; i++)
            {
                strSitePath = xmlNodeList[i].Attributes["Path"].Value;
                xmlWebs = xmlNodeList[i].ChildNodes;
                for (int j = 0; j < xmlWebs.Count; j++)
                {
                    xmlWeb = xmlWebs[j].ChildNodes;
                    for (int m = 0; m < xmlWeb.Count; m++)
                    {
                        xmlLists = xmlWeb[m].ChildNodes;
                        for (int n = 0; n < xmlLists.Count; n++)
                        {
                            xmlList = xmlLists[n].ChildNodes;
                            for (int h = 0; h < xmlList.Count; h++)
                            {
                                XmlElement xmlEle = (XmlElement)xmlList[h];
                                queryListsString.Append("<List ID='" + xmlEle.Attributes["ID"].Value + "'></List>");
                                queryLists = true;
                            }
                        }
                    }
                }
            }
            queryListsString.Append("</Lists>");
            StringBuilder queryString = new StringBuilder();
            if (!string.IsNullOrEmpty(CreateTime))
            {
                queryString.Append("<Where><And><Geq><FieldRef IncludeTimeValue='true' Name=\"Created\" /><Value Type=\"DateTime\">" + CreateTime + "</Value></Geq><Neq><FieldRef Name=\"NewsPosition\" /><Value Type=\"Choice\">外网发布</Value></Neq></And></Where>");
            }
            else
            {
                queryString.Append("<Where>");
                queryString.Append("<Neq><FieldRef Name=\"NewsPosition\" /><Value Type=\"Choice\">外网发布</Value></Neq>");
                queryString.Append("</Where>");
            }
            queryString.Append("<OrderBy>");
            queryString.Append("<FieldRef Name=\"Created\" Ascending=\"False\" />");
            queryString.Append("</OrderBy>");
            string ViewFieldsString = "<FieldRef Name=\"GUID\" /><FieldRef Name=\"Title\" /><FieldRef Name=\"Modified\"/><FieldRef Name=\"LinkFilename\" /><FieldRef Name=\"NewsType\"/><FieldRef Name=\"ArticleStartDate\"/><FieldRef Name=\"Created\"/><FieldRef Name=\"_ModerationStatus\"/><FieldRef Name=\"Company1\" Nullable=\"TRUE\" />";
            string queryWebsString = "<Webs Scope='SiteCollection' />";
            if (!queryLists)
            {
                queryListsString = new StringBuilder();
            }
            MossListOperationSoapClient mos = new MossListOperationSoapClient();
            string s = string.Empty;
            try
            {
                s = mos.getNewsStringBySPSiteDataQuery(strSitePath, strWebPath, queryListsString.ToString(),
                ViewFieldsString,
                queryString.ToString(), queryWebsString, count, "sinooceanland\\" + userName);
            }
            catch (Exception e)
            {

            }
            return XMLtoDataTable(s);
        }

        /// <summary>
        /// 将xml字符串转化成table表
        /// </summary>
        /// <param name="XMlstr">XML字符串</param>
        /// <returns></returns>
        public static DataTable XMLtoDataTable(string XMlstr)
        {
            DataSet ds = new DataSet();
            if (!string.IsNullOrEmpty(XMlstr))
            {

                byte[] bstr = Convert.FromBase64String(XMlstr);
                System.Text.Encoding ed = System.Text.Encoding.UTF8;
                string returnValue = ed.GetString(bstr, 0, bstr.Length).Trim();
                MemoryStream ms = new MemoryStream(bstr);
                System.Xml.XmlTextReader xtw = new System.Xml.XmlTextReader(ms);
                ds.ReadXml(xtw);

                if (ds != null & ds.Tables.Count > 0)
                {
                    return ds.Tables[0];
                }
                else
                {

                    return null;
                }

            }
            else
            {
                return null;
            }


        }

        /// <summary>
        /// 将table转化成model实体
        /// </summary>
        /// <param name="supervisionInfoTable"></param>
        /// <returns></returns>
        public NewsModelCollection GetNewsCollectionForTable(DataTable NewsTable)
        {
            var result = new NewsModelCollection();
            if (NewsTable != null)
            {
                for (int i = 0; i < NewsTable.Rows.Count; i++)
                {
                    NewsModel news = new NewsModel();


                    if (NewsTable.Columns.Contains("NewsCategory"))
                    {
                        news.NewsCategory = NewsTable.Rows[i]["NewsCategory"].ToString();
                    }
                    if (NewsTable.Rows[i]["_ModerationStatus"].ToString() != "0")
                    {
                        news.Title = "[未发布]" + NewsTable.Rows[i]["Title"].ToString();
                    }
                    else
                    {
                        news.Title = NewsTable.Rows[i]["Title"].ToString();
                    }
                    if (NewsTable.Columns.Contains("GUID"))
                    {
                        news.GUID = NewsTable.Rows[i]["GUID"].ToString().Replace("{", "").Replace("}", "");
                    }
                    if (NewsTable.Columns.Contains("WebId"))
                    {
                        news.WebId = NewsTable.Rows[i]["WebId"].ToString();
                    }
                    if (NewsTable.Columns.Contains("Id"))
                    {
                        news.Id = NewsTable.Rows[i]["Id"].ToString();
                    }
                    if (NewsTable.Columns.Contains("ListId"))
                    {
                        news.ListId = NewsTable.Rows[i]["ListId"].ToString();
                    }
                    if (NewsTable.Columns.Contains("ArticleByLine"))
                    {
                        news.Author = NewsTable.Rows[i]["ArticleByLine"].ToString();
                    }

                    if (NewsTable.Columns.Contains("NewsType"))
                    {
                        news.NewsType = NewsTable.Rows[i]["NewsType"].ToString();
                    }
                    if (NewsTable.Columns.Contains("VideoAddress"))
                    {
                        news.VideoAddress = NewsTable.Rows[i]["VideoAddress"].ToString();
                    }
                    if (NewsTable.Columns.Contains("ArticleStartDate"))
                    {
                        DateTime time = DateTime.MinValue;
                        if (DateTime.TryParse(NewsTable.Rows[i]["ArticleStartDate"].ToString(), out time))
                        {
                            news.ArticleStartDate = time;
                            news.ArticleStartDateString = time.ToString("yyyy-MM-dd");
                        }
                    }
                    if (NewsTable.Columns.Contains("PublishingPageContent"))
                    {
                        news.PublishingPageContent = NewsTable.Rows[i]["PublishingPageContent"].ToString();
                    }

                    if (NewsTable.Columns.Contains("Created"))
                    {
                        DateTime time = DateTime.MinValue;
                        if (DateTime.TryParse(NewsTable.Rows[i]["Created"].ToString(), out time))
                        {
                            news.CreateTime = time;
                            news.CreateTimeString = time.ToString("yyyy-MM-dd");
                        }
                    }
                    int day = Convert.ToInt32(ConfigurationManager.AppSettings["otherDays"]);
                    DateTime now = DateTime.Now;
                    DateTime past = now.AddDays(-day);
                    if (news.CreateTime >= past && news.CreateTime <= now)
                    {
                        news.IsNew = true;
                    }
                    news.WebName = string.Empty;
                    news.Link = string.Empty;
                    Dictionary<String, String> webDictionary = GetWebDictionary(NewsTable.Rows[i]["WebID"].ToString());
                    if (webDictionary.ContainsKey("webName"))
                    {
                        if (webDictionary["webName"] == "集团新闻")
                        {
                            news.WebName = webDictionary["webName"];
                            news.WebShortName = webDictionary["webName"];
                        }
                        else
                        {
                            Dictionary<String, String> organ = GetOrganNameDictionary(NewsTable.Rows[i]["Company1"].ToString());
                            if (organ.ContainsKey("webName"))
                            {
                                news.WebName = organ["webName"];
                            }
                            if (organ.ContainsKey("webStortName"))
                            {
                                news.WebShortName = organ["webStortName"];
                            }
                        }
                    }
                    if (webDictionary.ContainsKey("webUrl"))
                    {
                        news.Link = webDictionary["webUrl"] + "/" + "Pages/" + NewsTable.Rows[i]["LinkFilename"].ToString();
                        string encodedTitle = GetTitle(NewsTable.Rows[i]["Title"].ToString());
                        news.Link += string.Format("?title={0}", encodedTitle);
                    }

                    if (webDictionary.ContainsKey("webPage"))
                    {
                        news.WebLink = webDictionary["webPage"];
                    }
                    news.CreateTimeString = news.CreateTime.ToString("yyyy-MM-dd HH:mm:ss");
                    result.Add(news);
                }
            }
            return result;
        }

        public static string GetTitle(string originalTitle)
        {
            if (originalTitle == null)
                return string.Empty;
            return HttpContext.Current.Server.UrlEncode(originalTitle.Replace(" ", "+"));
        }

        /// <summary>
        /// 根据公司名称获取相应的站点地址，web名称，以及列表名称
        /// </summary>
        /// <param name="companyName"></param>
        /// <returns></returns>
        public Dictionary<string, string> GetWebDictionary(string ID)
        {
            Dictionary<string, string> webDictionary = new Dictionary<string, string>();
            XmlDocument xmlDocument = new XmlDocument();
            xmlDocument = XmlManager(HttpContext.Current.Server.MapPath("../XmlConfig/News.xml"));
            string[] siteURL = new string[4];
            XmlNodeList xmlNodeList = xmlDocument.GetElementsByTagName("Site");

            XmlNodeList xmlWebs;
            XmlNodeList xmlWeb;
            XmlNodeList xmlLists;
            string webName = string.Empty;
            string webUrl = string.Empty;
            string webPage = string.Empty;
            for (int i = 0; i < xmlNodeList.Count; i++)
            {
                xmlWebs = xmlNodeList[i].ChildNodes;
                string strSitePath = xmlNodeList[i].Attributes["Path"].Value.ToString();
                for (int j = 0; j < xmlWebs.Count; j++)
                {
                    xmlWeb = xmlWebs[j].ChildNodes;
                    for (int m = 0; m < xmlWeb.Count; m++)
                    {
                        string strWebPath = xmlWeb[m].Attributes["Path"].Value.ToString();
                        string strWebPage = xmlWeb[m].Attributes["Page"].Value.ToString();
                        xmlLists = xmlWeb[m].ChildNodes;
                        if (xmlWeb[m].Attributes["ID"].Value.ToString() == ID)
                        {
                            webName = xmlWeb[m].Attributes["Name"].Value.ToString();
                            webUrl = strSitePath + "/" + strWebPath;
                            webPage = strSitePath + "/" + strWebPage;
                            webDictionary.Add("webName", webName);
                            webDictionary.Add("webUrl", webUrl);
                            webDictionary.Add("webPage", webPage);
                        }
                    }
                }

            }
            return webDictionary;
        }


        /// <summary>
        /// 根据公司名称获取相应的新的组织名称
        /// </summary>
        /// <param name="companyName"></param>
        /// <returns></returns>
        public Dictionary<string, string> GetOrganNameDictionary(string name)
        {
            Dictionary<string, string> webDictionary = new Dictionary<string, string>();
            XmlDocument xmlDocument = new XmlDocument();
            xmlDocument = XmlManager(HttpContext.Current.Server.MapPath("../XmlConfig/Organ.xml"));

            XmlNodeList xmlNodeList = xmlDocument.GetElementsByTagName("Site");

            for (int i = 0; i < xmlNodeList.Count; i++)
            {

                string strSitePath = xmlNodeList[i].Attributes["Old"].Value.ToString();
                if (strSitePath == name)
                {
                    webDictionary.Add("webName", xmlNodeList[i].Attributes["New"].Value.ToString());
                    webDictionary.Add("webStortName", xmlNodeList[i].Attributes["NewShort"].Value.ToString());
                    break;
                }
            }
            return webDictionary;
        }


        /// <summary>
        /// 获取NewsLoadModel对象,根据时间过滤一次，搜索查询出来的不太靠谱
        /// </summary>
        /// <param name="companyName"></param>
        /// <returns></returns>
        public static NewsLoadModel GetNewsLoadModelNoReadCode(NewsModelCollection newsModelCollection, DateTime createdTime)
        {
            NewsLoadModel newsLoadModel = new NewsLoadModel();
            List<string> NoReadCodeList = new List<string>();
            newsLoadModel.Name = "新闻中心";
            newsLoadModel.Type = "news";
            newsLoadModel.NoReadCount = 0;
            if (newsModelCollection != null && newsModelCollection.Count() > 0)
            {

                for (int i = 0; i < newsModelCollection.Count(); i++)
                {
                    string guid = newsModelCollection[i].GUID;
                    //根据时间过滤一次，搜索查询出来的不太靠谱
                    //if (EIP_ReadDetailsAdapter.Instance.GetReadDetailsByContentID(guid) == 0 && newsModelCollection[i].CreateTime >= createdTime)
                    //{
                    //    newsLoadModel.NoReadCount = newsLoadModel.NoReadCount + 1;
                    //    NoReadCodeList.Add(guid);
                    //}
                }
            }


            newsLoadModel.NoReadCodeList = NoReadCodeList;
            return newsLoadModel;
        }


        /// <summary>
        /// 获取news对象
        /// </summary>
        /// <param name="title"></param>
        /// <param name="userName"></param>
        /// <returns></returns>
        public NewsModel GetNewsModel(String title, String userName)
        {
            string strWebPath = string.Empty;
            var titlesp = title.Split(',');
            StringBuilder queryListsString = new StringBuilder();
            var xml = ReadXml(true, titlesp[0]);
            queryListsString.Append("<Lists>");
            queryListsString.Append(xml.QueryListsString);
            queryListsString.Append("</Lists>");
            StringBuilder queryString = new StringBuilder();
            queryString.Append("<Where>");
            queryString.Append("<Contains><FieldRef Name=\"FileRef\" /><Value Type=\"Lookup\">" + titlesp[1] + "</Value></Contains>");
            queryString.Append("</Where>");
            queryString.Append("<OrderBy>");
            queryString.Append("<FieldRef Name=\"Created\" Ascending=\"False\" />");
            queryString.Append("</OrderBy>");
            string ViewFieldsString = "<FieldRef Name=\"VideoAddress\" /><FieldRef Name=\"FileRef\" /><FieldRef Name=\"PublishingPageContent\" /><FieldRef Name=\"ArticleByLine\" /><FieldRef Name=\"Title\" /><FieldRef Name=\"Modified\"/><FieldRef Name=\"LinkFilename\" /><FieldRef Name=\"NewsType\"/><FieldRef Name=\"ArticleStartDate\"/><FieldRef Name=\"Created\"/><FieldRef Name=\"_ModerationStatus\"/><FieldRef Name=\"Company1\" Nullable=\"TRUE\" />";
            string queryWebsString = "<Webs Scope='SiteCollection' />";
            if (!xml.QueryLists)
            {
                queryListsString = new StringBuilder();
            }
            MossListOperationSoapClient mos = new MossListOperationSoapClient();
            string s = string.Empty;
            try
            {
                s = mos.getNewsStringBySPSiteDataQuery(xml.StrSitePath, strWebPath, queryListsString.ToString(),
                ViewFieldsString,
                queryString.ToString(), queryWebsString, 1, "sinooceanland\\" + userName);
            }
            catch (Exception)
            {
            }
            var list = GetNewsCollectionForTable(XMLtoDataTable(s));
            var model = new NewsModel();
            if (list.Count > 0)
            {
                model = list.FirstOrDefault();
            }
            return model;
        }

        /// <summary>
        /// 根据id获取news对象
        /// （http://home.sinooceangroup.com/SOLWebApp/OAPortal/NewsCenter/NewsContent?id=7316634A-1432-4A97-827C-E4D6CC581D5C）
        /// </summary>
        /// <param name="id"></param>
        /// <param name="userName"></param>
        /// <returns></returns>
        public NewsModel GetNewsModelById(String id, String userName)
        {
            XmlDocument xml = new XmlDocument();
            xml = XmlManager(HttpContext.Current.Server.MapPath("../XmlConfig/News.xml"));
            string strSitePath = string.Empty;
            string strWebPath = string.Empty;
            //得到SITE的集合
            XmlNodeList xmlNodeList = xml.GetElementsByTagName("Site");
            ////读取从每个库中取得数据的条数

            XmlNodeList xmlWebs;
            XmlNodeList xmlWeb;
            XmlNodeList xmlLists;
            XmlNodeList xmlList;
            StringBuilder queryListsString = new StringBuilder();
            bool queryLists = false;
            queryListsString.Append("<Lists>");
            //以单位为准
            for (int i = 0; i < xmlNodeList.Count; i++)
            {
                strSitePath = xmlNodeList[i].Attributes["Path"].Value.ToString();
                xmlWebs = xmlNodeList[i].ChildNodes;
                for (int j = 0; j < xmlWebs.Count; j++)
                {
                    xmlWeb = xmlWebs[j].ChildNodes;
                    for (int m = 0; m < xmlWeb.Count; m++)
                    {
                        xmlLists = xmlWeb[m].ChildNodes;
                        for (int n = 0; n < xmlLists.Count; n++)
                        {
                            xmlList = xmlLists[n].ChildNodes;
                            for (int h = 0; h < xmlList.Count; h++)
                            {
                                XmlElement xmlEle = (XmlElement)xmlList[h];
                                queryListsString.Append("<List ID='" + xmlEle.Attributes["ID"].Value.ToString() + "'></List>");
                                queryLists = true;
                            }
                        }
                    }

                }
            }

            queryListsString.Append("</Lists>");

            StringBuilder queryString = new StringBuilder();
            queryString.Append("<Where><And>");
            queryString.Append("<Neq><FieldRef Name=\"NewsPosition\" /><Value Type=\"Choice\">外网发布</Value></Neq>");
            queryString.Append("<Eq><FieldRef Name=\"GUID\" /><Value Type=\"Guid\">" + id + "</Value></Eq>");
            queryString.Append("</And>");
            queryString.Append("</Where>");
            queryString.Append("<OrderBy>");
            queryString.Append("<FieldRef Name=\"Created\" Ascending=\"False\" />");
            queryString.Append("</OrderBy>");
            string ViewFieldsString = "<FieldRef Name=\"GUID\" /><FieldRef Name=\"Created\" /><FieldRef Name=\"Title\" />"
                    + "<FieldRef Name=\"Modified\"/><FieldRef Name=\"LinkFilename\" /><FieldRef Name=\"NewsType\"/>"
                    + "<FieldRef Name=\"ArticleStartDate\"/><FieldRef Name=\"PublishingPageContent\"/>"
                    + "<FieldRef Name=\"_ModerationStatus\"/><FieldRef Name=\"Company1\" Nullable=\"TRUE\" />"
                    + "<FieldRef Name=\"Author\" Nullable=\"TRUE\" />"
                    + "<FieldRef Name=\"VideoAddress\" Nullable=\"TRUE\" />"
                    + "<FieldRef Name=\"NewsCategory\" Nullable=\"TRUE\" />"
                    + "<FieldRef Name=\"ArticleByLine\" Nullable=\"TRUE\" />";//作者
            string queryWebsString = "<Webs Scope='SiteCollection' />";


            NewsServer.MossListOperationSoapClient mos = new NewsServer.MossListOperationSoapClient("MossListOperationSoap1");
            string s = string.Empty;
            try
            {
                s = mos.getNewsStringBySPSiteDataQuery(strSitePath, strWebPath, queryListsString.ToString(),
                ViewFieldsString,
                queryString.ToString(), queryWebsString, 1, "sinooceanland\\" + userName);
            }
            catch (Exception ex)
            {
            }

            var list = GetNewsCollectionForTable(XMLtoDataTable(s));
            var model = new NewsModel();
            if (list.Count > 0)
            {
                model = list.FirstOrDefault();
            }
            return model;
        }

        public DataTable GetNewsModelWithId(String id, String userName)
        {
            XmlDocument xml = new XmlDocument();
            xml = XmlManager(HttpContext.Current.Server.MapPath("../XmlConfig/News.xml"));
            string strSitePath = string.Empty;
            string strWebPath = string.Empty;
            //得到SITE的集合
            XmlNodeList xmlNodeList = xml.GetElementsByTagName("Site");
            ////读取从每个库中取得数据的条数

            XmlNodeList xmlWebs;
            XmlNodeList xmlWeb;
            XmlNodeList xmlLists;
            XmlNodeList xmlList;
            StringBuilder queryListsString = new StringBuilder();
            bool queryLists = false;
            queryListsString.Append("<Lists>");
            //以单位为准
            for (int i = 0; i < xmlNodeList.Count; i++)
            {
                strSitePath = xmlNodeList[i].Attributes["Path"].Value.ToString();
                xmlWebs = xmlNodeList[i].ChildNodes;
                for (int j = 0; j < xmlWebs.Count; j++)
                {
                    xmlWeb = xmlWebs[j].ChildNodes;
                    for (int m = 0; m < xmlWeb.Count; m++)
                    {
                        xmlLists = xmlWeb[m].ChildNodes;
                        for (int n = 0; n < xmlLists.Count; n++)
                        {
                            xmlList = xmlLists[n].ChildNodes;
                            for (int h = 0; h < xmlList.Count; h++)
                            {
                                XmlElement xmlEle = (XmlElement)xmlList[h];
                                queryListsString.Append("<List ID='" + xmlEle.Attributes["ID"].Value.ToString() + "'></List>");
                                queryLists = true;
                            }
                        }
                    }

                }
            }

            queryListsString.Append("</Lists>");

            StringBuilder queryString = new StringBuilder();
            queryString.Append("<Where><And>");
            queryString.Append("<Neq><FieldRef Name=\"NewsPosition\" /><Value Type=\"Choice\">外网发布</Value></Neq>");
            queryString.Append("<Eq><FieldRef Name=\"GUID\" /><Value Type=\"Guid\">" + id + "</Value></Eq>");
            queryString.Append("</And>");
            queryString.Append("</Where>");
            queryString.Append("<OrderBy>");
            queryString.Append("<FieldRef Name=\"Created\" Ascending=\"False\" />");
            queryString.Append("</OrderBy>");
            string ViewFieldsString = "<FieldRef Name=\"Created\" /><FieldRef Name=\"Title\" />"
                    + "<FieldRef Name=\"Modified\"/><FieldRef Name=\"LinkFilename\" /><FieldRef Name=\"NewsType\"/>"
                    + "<FieldRef Name=\"ArticleStartDate\"/><FieldRef Name=\"PublishingPageContent\"/>"
                    + "<FieldRef Name=\"_ModerationStatus\"/><FieldRef Name=\"Company1\" Nullable=\"TRUE\" />"
                    + "<FieldRef Name=\"Author\" Nullable=\"TRUE\" />"
                    + "<FieldRef Name=\"VideoAddress\" Nullable=\"TRUE\" />"
                    + "<FieldRef Name=\"NewsCategory\" Nullable=\"TRUE\" />"
                    + "<FieldRef Name=\"ArticleByLine\" Nullable=\"TRUE\" />";//作者
            string queryWebsString = "<Webs Scope='SiteCollection' />";


            NewsServer.MossListOperationSoapClient mos = new NewsServer.MossListOperationSoapClient("MossListOperationSoap1");
            string s = string.Empty;
            try
            {
                s = mos.getNewsStringBySPSiteDataQuery(strSitePath, strWebPath, queryListsString.ToString(),
                ViewFieldsString,
                queryString.ToString(), queryWebsString, 1, "sinooceanland\\" + userName);
            }
            catch (Exception ex)
            {
            }
            return XMLtoDataTable(s);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="isModel">true加载指定类型</param>
        /// <param name="key"></param>
        /// <returns></returns>
        private ReadXmlModel ReadXml(bool isModel, String key)
        {
            StringBuilder queryListsString = new StringBuilder();
            String strSitePath = String.Empty;
            int count = 0;
            bool queryLists = false;
            XmlDocument xmlDocument = new XmlDocument();
            xmlDocument = XmlManager(HttpContext.Current.Server.MapPath("../XmlConfig/News.xml"));
            //得到SITE的集合
            XmlNodeList xmlNodeList = xmlDocument.GetElementsByTagName("Site");
            ////读取从每个库中取得数据的条数
            if (Convert.ToInt32(xmlDocument.DocumentElement.Attributes["ReadCount"].Value.ToString()) > 0)
            {
                count = Convert.ToInt32(xmlDocument.DocumentElement.Attributes["ReadCount"].Value.ToString());
            }
            XmlNodeList xmlWebs;
            XmlNodeList xmlWeb;
            XmlNodeList xmlLists;
            XmlNodeList xmlList;
            for (int i = 0; i < xmlNodeList.Count; i++)
            {
                strSitePath = xmlNodeList[i].Attributes["Path"].Value.ToString();
                xmlWebs = xmlNodeList[i].ChildNodes;
                for (int j = 0; j < xmlWebs.Count; j++)
                {
                    xmlWeb = xmlWebs[j].ChildNodes;
                    for (int m = 0; m < xmlWeb.Count; m++)
                    {
                        var flag = xmlWeb[m].OuterXml.Contains(key);
                        if (isModel)
                        {
                            if (flag)
                            {
                                xmlLists = xmlWeb[m].ChildNodes;
                                for (int n = 0; n < xmlLists.Count; n++)
                                {
                                    xmlList = xmlLists[n].ChildNodes;
                                    for (int h = 0; h < xmlList.Count; h++)
                                    {
                                        XmlElement xmlEle = (XmlElement)xmlList[h];
                                        queryListsString.Append("<List ID='" + xmlEle.Attributes["ID"].Value + "'></List>");
                                        queryLists = true;
                                    }
                                }
                            }
                        }
                        else
                        {
                            xmlLists = xmlWeb[m].ChildNodes;
                            for (int n = 0; n < xmlLists.Count; n++)
                            {
                                xmlList = xmlLists[n].ChildNodes;
                                for (int h = 0; h < xmlList.Count; h++)
                                {
                                    XmlElement xmlEle = (XmlElement)xmlList[h];
                                    queryListsString.Append("<List ID='" + xmlEle.Attributes["ID"].Value + "'></List>");
                                    queryLists = true;
                                }
                            }
                        }
                    }
                }
            }
            return new ReadXmlModel() { Count = count, QueryLists = queryLists, QueryListsString = queryListsString.ToString(), StrSitePath = strSitePath };
        }
        private ReadXmlModel ReadXml()
        {
            StringBuilder queryListsString = new StringBuilder();
            String strSitePath = String.Empty;
            int count = 0;
            bool queryLists = false;
            XmlDocument xmlDocument = new XmlDocument();
            xmlDocument = XmlManager(HttpContext.Current.Server.MapPath("../XmlConfig/News.xml"));
            //得到SITE的集合
            XmlNodeList xmlNodeList = xmlDocument.GetElementsByTagName("Site");
            ////读取从每个库中取得数据的条数
            if (Convert.ToInt32(xmlDocument.DocumentElement.Attributes["ReadCount"].Value.ToString()) > 0)
            {
                count = Convert.ToInt32(xmlDocument.DocumentElement.Attributes["ReadCount"].Value.ToString());
            }
            XmlNodeList xmlWebs;
            XmlNodeList xmlWeb;
            XmlNodeList xmlLists;
            XmlNodeList xmlList;
            for (int i = 0; i < xmlNodeList.Count; i++)
            {
                strSitePath = xmlNodeList[i].Attributes["Path"].Value.ToString();
                xmlWebs = xmlNodeList[i].ChildNodes;
                for (int j = 0; j < xmlWebs.Count; j++)
                {
                    xmlWeb = xmlWebs[j].ChildNodes;
                    for (int m = 0; m < xmlWeb.Count; m++)
                    {
                        xmlLists = xmlWeb[m].ChildNodes;
                        for (int n = 0; n < xmlLists.Count; n++)
                        {
                            xmlList = xmlLists[n].ChildNodes;
                            for (int h = 0; h < xmlList.Count; h++)
                            {
                                XmlElement xmlEle = (XmlElement)xmlList[h];
                                queryListsString.Append("<List ID='" + xmlEle.Attributes["ID"].Value + "'></List>");
                                queryLists = true;
                            }
                        }
                    }
                }
            }
            return new ReadXmlModel() { Count = count, QueryLists = queryLists, QueryListsString = queryListsString.ToString(), StrSitePath = strSitePath };
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="type">新闻列类型</param>
        /// <param name="userName"></param>
        /// <returns></returns>
        public NewsModelCollection GetModelCollectionByType(String type, String newsCategory, String userName, int count, String CreateTime)
        {
            string strWebPath = string.Empty;
            var category = "";
            StringBuilder queryListsString = new StringBuilder();
            var xml = type == "all" ? ReadXml() : ReadXml(true, type);
            queryListsString.Append("<Lists>");
            queryListsString.Append(xml.QueryListsString);
            queryListsString.Append("</Lists>");
            StringBuilder queryString = new StringBuilder();
            queryString.Append("<Where>");
            if (!string.IsNullOrEmpty(CreateTime))
            {
                //queryString.Append("<And><Lt><FieldRef IncludeTimeValue='true' Name=\"Created\" /><Value Type=\"DateTime\">" + CreateTime + "</Value></Lt><Neq><FieldRef Name=\"NewsPosition\" /><Value Type=\"Choice\">外网发布</Value></Neq></And>");
                queryString.Append("<Lt><FieldRef IncludeTimeValue='true' Name=\"Created\" /><Value Type=\"DateTime\">" + CreateTime + "</Value></Lt>");
            }
            if (!String.IsNullOrEmpty(newsCategory))
            {

                switch (newsCategory)
                {
                    case "managementReport": category = "管理报告"; break;
                }
                if (!String.IsNullOrEmpty(category))
                {
                    queryString.Append("<Eq><FieldRef Name=\"NewsCategory\" /><Value Type=\"Choice\">" + category + "</Value></Eq>");
                }
            }
            queryString.Append("<Neq><FieldRef Name=\"NewsPosition\" /><Value Type=\"Choice\">外网发布</Value></Neq>");
            queryString.Append("</Where>");
            queryString.Append("<OrderBy>");
            queryString.Append("<FieldRef Name=\"Created\" Ascending=\"False\" />");// <FieldRef Name="Created" Ascending="False" />
            queryString.Append("</OrderBy>");
            string ViewFieldsString = "<FieldRef Name=\"NewsCategory\" /><FieldRef Name=\"PublishingPageContent\" /><FieldRef Name=\"Title\" /><FieldRef Name=\"Modified\"/><FieldRef Name=\"LinkFilename\" /><FieldRef Name=\"NewsType\"/><FieldRef Name=\"ArticleStartDate\"/><FieldRef Name=\"Created\"/><FieldRef Name=\"_ModerationStatus\"/><FieldRef Name=\"Company1\" Nullable=\"TRUE\" />";
            string queryWebsString = "<Webs Scope='SiteCollection' />";
            if (!xml.QueryLists)
            {
                queryListsString = new StringBuilder();
            }
            MossListOperationSoapClient mos = new MossListOperationSoapClient();
            string s = string.Empty;
            try
            {
                s = mos.getNewsStringBySPSiteDataQuery(xml.StrSitePath, strWebPath, queryListsString.ToString(),
                ViewFieldsString,
                queryString.ToString(), queryWebsString, count, "sinooceanland\\" + userName);
            }
            catch (Exception)
            {
            }
            var temp = GetNewsCollectionForTable(XMLtoDataTable(s));
            var list = new NewsModelCollection();
            if (!String.IsNullOrEmpty(newsCategory) && !String.IsNullOrEmpty(category))
            {
                foreach (var item in temp)
                {
                    if (item.NewsCategory == category)
                    {
                        list.Add(item);
                    }
                }
            }
            else
            {
                list = temp;
            }
            return list;
        }


        public String NewsXmls()
        {
            StringBuilder queryListsString = new StringBuilder();
            String strSitePath = String.Empty;
            string strWebPath = string.Empty;
            int count = 0;
            bool queryLists = false;
            var str = "";
            XmlDocument xmlDocument = new XmlDocument();
            xmlDocument = XmlManager(HttpContext.Current.Server.MapPath("../XmlConfig/News.xml"));
            //得到SITE的集合
            XmlNodeList xmlNodeList = xmlDocument.GetElementsByTagName("Site");
            ////读取从每个库中取得数据的条数
            if (Convert.ToInt32(xmlDocument.DocumentElement.Attributes["ReadCount"].Value.ToString()) > 0)
            {
                count = Convert.ToInt32(xmlDocument.DocumentElement.Attributes["ReadCount"].Value.ToString());
            }
            XmlNodeList xmlWebs;
            XmlNodeList xmlWeb;
            XmlNodeList xmlLists;
            XmlNodeList xmlList;
            for (int i = 0; i < xmlNodeList.Count; i++)
            {
                strSitePath = xmlNodeList[i].Attributes["Path"].Value.ToString();
                xmlWebs = xmlNodeList[i].ChildNodes;
                for (int j = 0; j < xmlWebs.Count; j++)
                {
                    xmlWeb = xmlWebs[j].ChildNodes;
                    for (int m = 0; m < xmlWeb.Count; m++)
                    {
                        xmlLists = xmlWeb[m].ChildNodes;
                        strWebPath = xmlWeb[m].Attributes["Path"].Value.ToString();
                        string comName = xmlWeb[m].Attributes["Name"].Value.ToString();
                        str += "{" + strWebPath + ":" + comName + "},";
                        for (int n = 0; n < xmlLists.Count; n++)
                        {
                            xmlList = xmlLists[n].ChildNodes;
                            for (int h = 0; h < xmlList.Count; h++)
                            {
                                XmlElement xmlEle = (XmlElement)xmlList[i];
                                queryListsString.Append("<List ID='" + xmlEle.Attributes["ID"].Value.ToString() + "'></List>");
                            }
                        }
                    }
                }
            }
            return str.TrimEnd(',');
        }





    }
}