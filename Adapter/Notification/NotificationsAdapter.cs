using System;
using System.Collections.Generic;
using System.Data;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;
using System.Web;
using System.Xml;
using MCS.Library.OGUPermission;
using MCS.Library.Principal;
using MCS.Library.SOA.DataObjects;
using Seagull2.YuanXin.AppApi.Models;
using Seagull2.YuanXin.AppApi.Models.Notification;
using Seagull2.YuanXin.AppApi.NotificationsServer;
using System.Configuration;
using log4net;
using System.Reflection;

namespace Seagull2.YuanXin.AppApi.Adapter
{
    /// <summary>
    /// 通知纪要
    /// </summary>
    public class NotificationsAdapter
    {
        /// <summary>
        /// 实例化
        /// </summary>
        public static readonly NotificationsAdapter Instance = new NotificationsAdapter();

        /// <summary>
        /// 日志
        /// </summary>
        ILog log = LogManager.GetLogger(MethodBase.GetCurrentMethod().DeclaringType);

        /// <summary>
        /// 根据用户ID查询具有权限的会议纪要
        /// </summary>
        public NotificationsCollection GetNotificationsCollectionByLogion(string logionUser, string noticeType, string PublishTime = "")
        {
            DataTable notificationsTable = GetAllNotificationsData(10, logionUser, PublishTime, "", noticeType);

            return GetNotificationsCollectionForTable(notificationsTable);
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
            catch
            {
                throw new Exception("加载XML文档时发生错误");
            }
            return xmlDocument;
        }

        /// <summary>
        /// 根据公告类型获取该公司公告下的所有该类型的数据
        /// </summary>
        public DataTable GetAllNotificationsData(int count, string userName, string publishTime, string type, string noticeType)
        {
            var xml = string.IsNullOrEmpty(type) ? ReadXml() : ReadXml(true, type);

            StringBuilder queryListsString = new StringBuilder();
            bool queryLists = false;
            queryListsString.Append("<Lists>");
            queryListsString.Append(xml.QueryListsString);
            queryListsString.Append("</Lists>");
            StringBuilder queryString = new StringBuilder();
            queryString.Append("<Where>");
            if (!string.IsNullOrEmpty(publishTime))
            {
                queryString.Append("<Lt><FieldRef Name=\"Created\" /><Value Type=\"DateTime\">");
                queryString.Append(publishTime);
                queryString.Append("</Value></Lt>");
            }
            if (!string.IsNullOrEmpty(noticeType))
            {
                queryString.Append("<Eq><FieldRef Name=\"_x7a0b__x5e8f__x540d__x79f0_\" /><Value Type=\"Text\">");
                queryString.Append(noticeType);
                queryString.Append("</Value></Eq>");
            }
            queryString.Append("</Where>");
            queryString.Append("<OrderBy><FieldRef Name=\"Created\" Ascending=\"FALSE\" /></OrderBy>");
            string ViewFieldsString =
                "<FieldRef Name='_x7a0b__x5e8f__x540d__x79f0_' /><FieldRef Name='Title' /><FieldRef Name='Created' /><FieldRef Name='FieldRef' Nullable='TRUE'/><FieldRef Name='_x6587__x53f7_GUID' /><FieldRef Name='_x4e3b__x9001__x5355__x4f4d_' />";
            string queryWebsString = "<Webs Scope='SiteCollection' />";
            if (queryLists)
            {
                queryListsString = new StringBuilder();
            }
            MossListOperationSoapClient mos = new MossListOperationSoapClient();
            string s = null;
            try
            {
                s = mos.getNotificationsStringBySPSiteDataQuery(xml.StrSitePath, xml.StrWebPath, queryListsString.ToString(),
                         ViewFieldsString,
                         queryString.ToString(), queryWebsString, count, "sinooceanland\\" + userName);
            }
            catch (Exception ex)
            {
                Debug.WriteLine(ex.Message);
            }
            return XMLtoDataTable(s);
        }

        /// <summary>
        /// 根据公告类型获取该公司公告下的所有该类型的数据
        /// </summary>
        public DataTable GetAllNotificationsDataByType(string noticeType, int count, string userName)
        {
            var xml = ReadXml();
            StringBuilder queryListsString = new StringBuilder();
            queryListsString.Append("<Lists>");
            queryListsString.Append(xml.QueryListsString);
            queryListsString.Append("</Lists>");
            StringBuilder queryString = new StringBuilder();
            queryString.Append("<Where>");
            queryString.Append("<Eq><FieldRef Name=\"_x7a0b__x5e8f__x540d__x79f0_\" /><Value Type=\"Text\">");
            queryString.Append(noticeType);
            queryString.Append("</Value></Eq>");
            queryString.Append("</Where>");
            queryString.Append("<OrderBy><FieldRef Name=\"Created\" Ascending=\"FALSE\" /></OrderBy>");
            string ViewFieldsString =
                "<FieldRef Name='Title' /><FieldRef Name='Created' /><FieldRef Name='FieldRef' Nullable='TRUE'/>";
            string queryWebsString = "<Webs Scope='SiteCollection' />";

            MossListOperationSoapClient mos = new MossListOperationSoapClient();
            string s = mos.getNotificationsStringBySPSiteDataQuery(xml.StrSitePath, xml.StrWebPath, queryListsString.ToString(),
                ViewFieldsString,
                queryString.ToString(), queryWebsString, count, "sinooceanland\\" + userName);
            return XMLtoDataTable(s);
        }

        /// <summary>
        /// 将xml字符串转化成table表
        /// </summary>
        /// <param name="XMlstr">XML字符串</param>
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

        #region 将 table 转化成 NotificationsCollection
        /// <summary>
        /// 将 table 转化成 NotificationsCollection
        /// </summary>
        public NotificationsCollection GetNotificationsCollectionForTable(DataTable dt)
        {
            var result = new NotificationsCollection();
            if (dt != null)
            {
                for (int i = 0; i < dt.Rows.Count; i++)
                {
                    var notifications = new NotificationsModel();

                    notifications.Title = dt.Rows[i]["Title"].ToString();
                    DateTime CreateTime = DateTime.MinValue;
                    if (DateTime.TryParse(dt.Rows[i]["Created"].ToString(), out CreateTime))
                    {
                        notifications.PublishTime = CreateTime;
                    }
                    if (dt.Columns.Contains("WebId"))
                    {
                        notifications.WebId = Guid.Parse(dt.Rows[i]["WebId"].ToString()).ToString();
                    }
                    if (dt.Columns.Contains("Id"))
                    {
                        notifications.Id = dt.Rows[i]["Id"].ToString();
                    }
                    if (dt.Columns.Contains("ListId"))
                    {
                        notifications.ListId = Guid.Parse(dt.Rows[i]["ListId"].ToString()).ToString();
                    }
                    if (dt.Columns.Contains("_x6587__x53f7_GUID"))
                    {
                        notifications.ResourceId = dt.Rows[i]["_x6587__x53f7_GUID"].ToString();
                    }
                    if (dt.Columns.Contains("_x7a0b__x5e8f__x540d__x79f0_"))
                    {
                        notifications.MossNoticeType = dt.Rows[i]["_x7a0b__x5e8f__x540d__x79f0_"].ToString();
                    }
                    if (dt.Columns.Contains("_x4e3b__x9001__x5355__x4f4d_"))
                    {
                        notifications.Source = dt.Rows[i]["_x4e3b__x9001__x5355__x4f4d_"].ToString();
                    }
                    notifications.CreateTimeString = notifications.PublishTime.ToString("yyyy-MM-dd HH:mm:ss");

                    result.Add(notifications);
                }
            }
            return result;
        }
        #endregion

        /// <summary>
        /// 获取详情
        /// </summary>
        public NotificationsModel ListItemByListItemId(Guid webId, Guid listId, int listItemId, string resourceId, string userId)
        {
            //获取详情
            var model = new NotificationsModel();
            var xml = ReadXml();
            var mos = new MossListOperationSoapClient();
            var str = mos.getListItemByListItemID(xml.StrSitePath, webId, listId, new string[] { "Body1", "GUID", "Title", "_x7a0b__x5e8f__x540d__x79f0_", "Author", "_x6267__x884c__x4eba__x5355__x4f", "Created", "Attachments" }, listItemId, "sinooceanland\\" + userId);
            var modeldt = XMLtoDataTable(str);
            for (int i = 0; i < modeldt.Rows.Count; i++)
            {
                if (modeldt.Rows[i][0].Equals("Title"))
                {
                    model.Title = modeldt.Rows[i][3].ToString();
                }
                if (modeldt.Rows[i][0].Equals("Author"))
                {
                    model.Author = modeldt.Rows[i][3].ToString().Split('#')[1];
                }
                if (modeldt.Rows[i][0].Equals("_x6267__x884c__x4eba__x5355__x4f"))
                {
                    model.WebName = modeldt.Rows[i][3].ToString();
                }
                if (modeldt.Rows[i][0].Equals("Created"))
                {
                    model.PublishTime = Convert.ToDateTime(Convert.ToDateTime(modeldt.Rows[i][3].ToString()).ToString("yyyy-MM-dd HH:mm:ss"));
                }
                if (modeldt.Rows[i][0].Equals("Body1"))
                {
                    model.Body1 = modeldt.Rows[i][3].ToString();
                }
            }

            //获取附件列表，有重复
            var materialList = new MaterialList();
            try
            {
                materialList = MaterialAdapter.Instance.LoadMaterialsByResourceID(resourceId);
            }
            catch (Exception e)
            {
                log.Error("获取通知纪要详情异常：通过 ResourceId 获取 MaterialList 失败！" + e.Message);
            }

            //附件去重
            var attachmentList = new MaterialList();
            var group = materialList.GroupBy(p => p.OriginalName);
            foreach (var g in group)
            {
                attachmentList.Add(g.ToList()[0]);
            }

            /*if (attachmentList.Count < 1)
            {
                attachmentList.Add(new Material() { ID = "1", Title = "1", RelativeFilePath = "1", OriginalName = "远洋集团关于20170922总裁办公会的会议纪要.docx" });
                attachmentList.Add(new Material() { ID = "2", Title = "2", RelativeFilePath = "1", OriginalName = "1.全力冲刺100天 确保业绩达成.pdf" });
                attachmentList.Add(new Material() { ID = "3", Title = "3", RelativeFilePath = "1", OriginalName = "2.2017年回款工作安排及措施.pdf" });
                attachmentList.Add(new Material() { ID = "4", Title = "4", RelativeFilePath = "1", OriginalName = "3. 2017年下半年住宅客户满意度提升举措.pdf" });
            }*/

            //附件Url
            var xmlPath = ReadXmlById(webId.ToString(), listId.ToString());
            var pdfUrl = xmlPath.SitePath + xmlPath.WebPath + "/" + xmlPath.ListPath + "/Attachments/" + listItemId + "/";
            var preview = ConfigurationManager.AppSettings["preview"];
            foreach (Material t in attachmentList)
            {
                var url = "";
                if (t.OriginalName.ToLower().EndsWith(".pdf"))
                {
                    //如：http://bimosssrv01.sinooceanland.com:88/Company11/Lists/List/Attachments/1271/2017年下半年住宅客户满意度提升举措.pdf
                    url = pdfUrl + HttpUtility.UrlEncode(t.OriginalName);
                }
                else
                {
                    url = preview + "?materialId=" + t.ID + "&fileName=" + HttpUtility.UrlEncode(t.OriginalName);
                }
                model.Attachments.Add(string.Format("<a href='javascript:void(0);' jumpurl='{0}'>{1}</br>", url, t.OriginalName));
                model.SrcAddress.Add(url);
            }

            return model;
        }

        public ReadXmlByIdModel ReadXmlById(string webId, string listId)
        {
            var model = new ReadXmlByIdModel();
            var xml = XmlManager(HttpContext.Current.Server.MapPath("../XmlConfig/Notifications.xml"));
            XmlNode sites = xml.DocumentElement;
            foreach (XmlNode site in sites)
            {
                model.SitePath = site.Attributes["Path"].Value;
                foreach (XmlNode webs in site)
                {
                    foreach (XmlNode web in webs)
                    {
                        if (web.Attributes["ID"].Value.ToUpper() == webId.ToUpper())
                        {
                            model.WebPath = web.Attributes["Path"].Value;
                            model.WebCompanyName = web.Attributes["ComPanyName"].Value;
                            foreach (XmlNode lists in web)
                            {
                                foreach (XmlNode list in lists)
                                {
                                    if (list.Attributes["ID"].Value.ToUpper() == listId.ToUpper())
                                    {
                                        model.ListName = list.Attributes["Name"].Value;
                                        model.ListPath = list.Attributes["Path"].Value;
                                    }
                                }
                            }
                        }
                    }
                }
            }
            return model;
        }
        public class ReadXmlByIdModel
        {
            public string SitePath { set; get; }
            public string WebPath { set; get; }
            public string WebCompanyName { set; get; }
            public string ListName { set; get; }
            public string ListPath { set; get; }
        }

        private ReadXmlModel ReadXml()
        {
            StringBuilder queryListsString = new StringBuilder();
            String strSitePath = String.Empty;
            string strWebPath = string.Empty;
            int count = 0;
            bool queryLists = false;
            XmlDocument xmlDocument = new XmlDocument();
            xmlDocument = XmlManager(HttpContext.Current.Server.MapPath("../XmlConfig/Notifications.xml"));
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
                        string comName = xmlWeb[m].Attributes["ComPanyName"].Value.ToString();
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
            return new ReadXmlModel()
            {
                Count = count,
                QueryLists = queryLists,
                QueryListsString = queryListsString.ToString(),
                StrSitePath = strSitePath,
                StrWebPath = strWebPath
            };
        }
        private ReadXmlModel ReadXml(bool isModel, String key)
        {
            StringBuilder queryListsString = new StringBuilder();
            String strSitePath = String.Empty;
            int count = 0;
            bool queryLists = false;
            XmlDocument xmlDocument = new XmlDocument();
            xmlDocument = XmlManager(HttpContext.Current.Server.MapPath("../XmlConfig/Notifications.xml"));
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
                    }
                }
            }
            return new ReadXmlModel() { Count = count, QueryLists = queryLists, QueryListsString = queryListsString.ToString(), StrSitePath = strSitePath };
        }
        public string NotificationsXml()
        {
            StringBuilder queryListsString = new StringBuilder();
            String strSitePath = String.Empty;
            string strWebPath = string.Empty;
            int count = 0;
            bool queryLists = false;
            var str = "";
            XmlDocument xmlDocument = new XmlDocument();
            xmlDocument = XmlManager(HttpContext.Current.Server.MapPath("../XmlConfig/Notifications.xml"));
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
                        string comName = xmlWeb[m].Attributes["ComPanyShortName"].Value.ToString();
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