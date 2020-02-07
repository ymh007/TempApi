using MCS.Library.OGUPermission;
using MCS.Library.SOA.DataObjects;
using Seagull2.YuanXin.AppApi.Models;
using Seagull2.YuanXin.AppApi.NewsServer;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.IO;
using System.Linq;
using System.Text;
using System.Web;
using System.Xml;

namespace Seagull2.YuanXin.AppApi.Adapter
{
    /// <summary>
    /// 推荐图片新闻适配器
    /// </summary>
    public class RecommendedImageNewsModelAdapter : UpdatableAndLoadableAdapterBase<RecommendedImageNewsModel, RecommendedImageNewsModelCollection>
    {
        /// <summary>
        /// 实例化
        /// </summary>
        public static readonly RecommendedImageNewsModelAdapter Instance = new RecommendedImageNewsModelAdapter();

        /// <summary>
        /// 图片新闻
        /// </summary>
        public RecommendedImageNewsModelCollection GetRecommendedImageNewsModelCollectionByLogion(string userName, string CreateTime = "")
        {
            DataTable newsTable = GetRecommendedImageNewsModelCollectionByLogion(10, userName, CreateTime);

            return GetNewsCollectionForTable(newsTable);
        }
        /// <summary>
        /// 构造函数
        /// </summary>
        /// <param name="strUrlPath">xml文件的路径</param>
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
            catch (Exception ex)
            {
                Log.WriteLog(ex.Message);
                throw new Exception("加载XML文档时发生错误");
            }
            return xmlDocument;
        }
        /// <summary>
        /// 图片新闻
        /// </summary>
        /// <returns>图片推荐新闻集合</returns>
        public DataTable GetRecommendedImageNewsModelCollectionByLogion(int count, string userName, string CreateTime)
        {
            XmlDocument xmlDocument = new XmlDocument();
            xmlDocument = XmlManager(HttpContext.Current.Server.MapPath("../XmlConfig/RecommendedNews.xml"));

            string strSitePath = string.Empty;
            string strWebPath = string.Empty;
            //得到SITE的集合
            XmlNodeList xmlNodeList = xmlDocument.GetElementsByTagName("Site");
            //读取从每个库中取得数据的条数
            if (Convert.ToInt32(xmlDocument.DocumentElement.Attributes["ReadCount"].Value.ToString()) > 0)
            {
                count = Convert.ToInt32(xmlDocument.DocumentElement.Attributes["ReadCount"].Value.ToString());
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

            queryString.Append("<Where>");
            queryString.Append("<Contains><FieldRef Name=\"PicOrder\" /><Value Type=\"Text\">图片</Value></Contains>");
            queryString.Append("</Where>");
            queryString.Append("<OrderBy><FieldRef Name=\"RecOrder\" Ascending=\"TRUE\" /></OrderBy>");
            string ViewFieldsString = "<FieldRef Name=\"Title\" /><FieldRef Name=\"EIPImage\" /><FieldRef Name=\"Created\"/><FieldRef Name=\"PicOrder\"/><FieldRef Name=\"RecOrder\"/><FieldRef Name=\"NewsAddress\"/><FieldRef Name=\"TopPicAddress\"/>";
            string queryWebsString = "<Webs Scope='SiteCollection' />";
            if (!queryLists)
            {
                queryListsString = new StringBuilder();
            }
            MossListOperationSoapClient mos = new MossListOperationSoapClient();
            string s = string.Empty;
            try
            {
                s = mos.getNewsStringBySPSiteDataQuery(strSitePath, strWebPath, queryListsString.ToString(), ViewFieldsString,
                 queryString.ToString(), queryWebsString, count, "sinooceanland\\" + userName);
                DataTable dt = XMLtoDataTable(s);
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
        public RecommendedImageNewsModelCollection GetNewsCollectionForTable(DataTable dt)
        {
            var result = new RecommendedImageNewsModelCollection();
            if (dt != null)
            {
                for (int i = 0; i < dt.Rows.Count; i++)
                {
                    RecommendedImageNewsModel news = new RecommendedImageNewsModel();

                    news.Title = dt.Rows[i]["Title"].ToString();
                    DateTime CreateTime = DateTime.MinValue;
                    if (DateTime.TryParse(dt.Rows[i]["Created"].ToString(), out CreateTime))
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
                    if (dt.Rows[i]["NewsAddress"].ToString().Contains(','))
                    {
                        news.Link = dt.Rows[i]["NewsAddress"].ToString().Split(',')[0];
                    }
                    else
                    {
                        news.Link = dt.Rows[i]["NewsAddress"].ToString();
                    }

                    if (dt.Rows[i]["PicOrder"].ToString().Contains('#'))
                    {
                        news.PicOrder = int.Parse(dt.Rows[i]["PicOrder"].ToString().Split('#')[1]);
                    }
                    if (dt.Rows[i]["EIPImage"].ToString().Contains(','))
                    {
                        news.TopPicAddress = dt.Rows[i]["EIPImage"].ToString().Split(',')[0];
                    }
                    else
                    {
                        news.TopPicAddress = dt.Rows[i]["EIPImage"].ToString();
                    }

                    if (dt.Rows[i]["Title"].ToString().Length > 16 && news.IsNew == true)
                    {
                        news.ShortTitle = dt.Rows[i]["Title"].ToString().Substring(0, 16);
                    }
                    else if (dt.Rows[i]["Title"].ToString().Length > 18 && news.IsNew == false)
                    {
                        news.ShortTitle = dt.Rows[i]["Title"].ToString().Substring(0, 18);
                    }
                    else
                    {
                        news.ShortTitle = dt.Rows[i]["Title"].ToString();
                    }
                    if (dt.Columns.Contains("ID"))
                    {
                        news.Id = dt.Rows[i]["ID"].ToString();
                    }
                    if (dt.Columns.Contains("ListId"))
                    {
                        news.ListId = dt.Rows[i]["ListId"].ToString();
                    }
                    if (dt.Columns.Contains("WebId"))
                    {
                        news.WebId = dt.Rows[i]["WebId"].ToString();
                    }
                    result.Add(news);
                }
            }
            return result;
        }



    }
}