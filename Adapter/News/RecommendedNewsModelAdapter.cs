using MCS.Library.SOA.DataObjects;
using Seagull2.YuanXin.AppApi.Models;
using Seagull2.YuanXin.AppApi.NewsServer;
using System;
using System.Configuration;
using System.Data;
using System.IO;
using System.Linq;
using System.Text;
using System.Web;
using System.Xml;
using System.Text.RegularExpressions;

namespace Seagull2.YuanXin.AppApi.Adapter
{
    public class RecommendedNewsModelAdapter : UpdatableAndLoadableAdapterBase<RecommendedNewsModel, RecommendedNewsModelCollection>
    {
        public static readonly RecommendedNewsModelAdapter Instance = new RecommendedNewsModelAdapter();

        private RecommendedNewsModelAdapter()
        {

        }

        /// <summary>
        /// 推荐新闻
        /// </summary>
        /// <param name="userName">用户m</param>
        public RecommendedNewsModelCollection GetRecommendedNewsModelCollectionByLogion(string userName, string CreateTime = "")
        {
            DataTable newsTable = GetRecommendedNewsModelCollectionByLogion(10, userName, CreateTime);

            return GetNewsCollectionForTable(newsTable);
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
        /// 推荐新闻
        /// </summary>
        /// <returns></returns>
        public DataTable GetRecommendedNewsModelCollectionByLogion(int count, string userName, string CreateTime)
        {
            StringBuilder queryListsString = new StringBuilder();
            string strWebPath = string.Empty;
            var model = ReadXml();
            queryListsString.Append("<Lists>");
            queryListsString.Append(model.QueryListsString);
            queryListsString.Append("</Lists>");
            StringBuilder queryString = new StringBuilder();
            queryString.Append("<Where>");
            queryString.Append("<Contains><FieldRef Name=\"RecOrder\" /><Value Type=\"Text\">推荐</Value></Contains>");
            queryString.Append("</Where>");
            queryString.Append("<OrderBy><FieldRef Name=\"RecOrder\" Ascending=\"TRUE\" /></OrderBy>");
            string ViewFieldsString = "<FieldRef Name=\"Title\" /><FieldRef Name=\"Modified\"/><FieldRef Name=\"Created\"/><FieldRef Name=\"RecOrder\"/><FieldRef Name=\"NewsAddress\" />";
            string queryWebsString = "<Webs Scope='SiteCollection' />";
            if (!model.QueryLists)
            {
                queryListsString = new StringBuilder();
            }
            MossListOperationSoapClient mos = new MossListOperationSoapClient();
            string s = string.Empty;
            try
            {
                s = mos.getNewsStringBySPSiteDataQuery(model.StrSitePath, strWebPath, queryListsString.ToString(), ViewFieldsString,
                queryString.ToString(), queryWebsString, count, "sinooceanland\\" + userName);
            }
            catch (Exception)
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
        public RecommendedNewsModelCollection GetNewsCollectionForTable(DataTable RecommendedNewsTable)
        {
            var result = new RecommendedNewsModelCollection();
            if (RecommendedNewsTable != null)
            {
                for (int i = 0; i < RecommendedNewsTable.Rows.Count; i++)
                {
                    RecommendedNewsModel news = new RecommendedNewsModel();

                    news.Title = RecommendedNewsTable.Rows[i]["Title"].ToString();

                    DateTime CreateTime = DateTime.MinValue;
                    if (DateTime.TryParse(RecommendedNewsTable.Rows[i]["Created"].ToString(), out CreateTime))
                    {
                        news.CreateTime = CreateTime;
                    }
                    int day = Convert.ToInt32(ConfigurationManager.AppSettings["otherDays"]);
                    DateTime now = DateTime.Now;
                    DateTime past = now.AddDays(-day);
                    if (news.CreateTime >= past && news.CreateTime <= now)
                    {
                        news.IsNew = true;
                    }
                    // news.ImageLink = RecommendedNewsTable.Rows[i]["ImageLink"].ToString();
                    if (RecommendedNewsTable.Rows[i]["NewsAddress"].ToString().Contains(','))
                    {
                        news.Link = RecommendedNewsTable.Rows[i]["NewsAddress"].ToString().Split(',')[0];
                    }
                    else
                    {
                        news.Link = RecommendedNewsTable.Rows[i]["NewsAddress"].ToString();
                    }
                    if (RecommendedNewsTable.Rows[i]["RecOrder"].ToString().Contains('#'))
                    {
                        news.RecOrder = int.Parse(RecommendedNewsTable.Rows[i]["RecOrder"].ToString().Split('#')[1]);
                    }

                    if (RecommendedNewsTable.Columns.Contains("WebId"))
                    {
                        news.WebId = RecommendedNewsTable.Rows[i]["WebId"].ToString();
                    }
                    if (RecommendedNewsTable.Columns.Contains("Id"))
                    {
                        news.Id = RecommendedNewsTable.Rows[i]["Id"].ToString();
                    }
                    if (RecommendedNewsTable.Columns.Contains("ListId"))
                    {
                        news.ListId = RecommendedNewsTable.Rows[i]["ListId"].ToString();
                    }
                    news.CreateTimeString = news.CreateTime.ToString("yyyy-MM-dd HH:mm:ss");
                    result.Add(news);
                }
            }
            return result;
        }

        private ReadXmlModel ReadXml()
        {
            StringBuilder queryListsString = new StringBuilder();
            String strSitePath = String.Empty;
            int count = 0;
            bool queryLists = false;
            XmlDocument xmlDocument = new XmlDocument();
            xmlDocument = XmlManager(HttpContext.Current.Server.MapPath("../XmlConfig/RecommendedNews.xml"));
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
                                queryListsString.Append("<List ID='" + xmlEle.Attributes["ID"].Value.ToString() + "'></List>");
                                queryLists = true;
                            }
                        }
                    }
                }
            }
            return new ReadXmlModel() { Count = count, QueryLists = queryLists, QueryListsString = queryListsString.ToString(), StrSitePath = strSitePath };
        }
    }
}