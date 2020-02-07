using MCS.Library.SOA.DataObjects;
using Seagull2.YuanXin.AppApi.Models;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Web;
using System.Xml;

namespace Seagull2.YuanXin.AppApi.Adapter.Sign
{
    /// <summary>
    /// 考勤统计
    /// </summary>
    public class ComparePunchAdapater : UpdatableAndLoadableAdapterBase<EmployeeServicesModel, EmployeeServicesCollection>
    {
        /// <summary>
        /// 实例化
        /// </summary>
        public static readonly ComparePunchAdapater Instance = new ComparePunchAdapater();

        /// <summary>
        /// 数据库连接字符串
        /// </summary>
        protected override string GetConnectionName()
        {
            return ConnectionNameDefine.EmployeeAttendance;
        }

        #region 查询原始打卡记录
        /// <summary>
        ///查询原始打卡记录
        /// </summary>
        /// <param name="page"></param>
        /// <param name="Creator"></param>
        /// <param name="startTime"></param>
        /// <param name="endTime"></param>
        /// <returns></returns>
        public List<EmployeeServicesModel> LoadData(int page, string Creator, string startTime, string endTime)
        {
            try
            {

                string sql = string.Format(@"SELECT TOP 5 *
FROM    ( SELECT    ROW_NUMBER() OVER ( ORDER BY CreateTime ) AS rownumber ,
                    CONVERT (VARCHAR(20), CreateTime, 120) AS CreateTime ,
                    CnName,StandardPunchCode,CONVERT (VARCHAR(6), REPLACE([MapUrl], '中国', ''), 120) AS MapUrl 
          FROM      EmployeeServices
          WHERE    Creator='{0}'
                    AND CreateTime BETWEEN '{1}' AND  '{2}'
        ) EmployeeServices
 WHERE EmployeeServices.rownumber <= {3} ORDER BY CreateTime DESC", Creator, startTime, endTime, page);

                List<EmployeeServicesModel> list = new List<EmployeeServicesModel>();
                foreach (var item in this.QueryData(sql))
                {
                    list.Add(item);
                }
                return list;
            }
            catch (Exception ex)
            {
                Log.WriteLog(ex.Message);
                Log.WriteLog(ex.StackTrace);
                throw;
            }
        }
        #endregion

        #region 查询统计过的打卡记录
        /// <summary>
        /// 查询统计过的打卡记录
        /// </summary>
        /// <param name="Sort">排序</param>
        /// <param name="Creator">userID</param>
        /// <param name="startTime">开始时间</param>
        /// <param name="endTime">结束时间</param>
        public List<EmployeeServicesModel> LoadCompareList(string Sort, string Creator, string startTime, string endTime)
        {

            string sql = string.Format(@"SELECT  CnName ,
        CreateTime ,StandardPunchCode,
        rownumber
FROM    ( SELECT    TOP 1 CnName ,
                    CreateTime ,StandardPunchCode,
                    ROW_NUMBER() OVER ( ORDER BY CreateTime {0} ) AS rownumber
          FROM      ( SELECT    RANK() OVER ( PARTITION BY CONVERT (VARCHAR(10), CreateTime, 120),
                                              CnName ORDER BY CreateTime {0} ) AS rowno ,
                                t.*
                      FROM      EmployeeServices t
                      WHERE     Creator = '{1}' and  CreateTime BETWEEN '{2}' AND '{3}'
                    ) aa
        ) bb ORDER BY bb.CreateTime DESC", Sort, Creator, startTime, endTime);





            //string sql = string.Format(@"SELECT CnName ,CreateTime from
            //                    (
            //                    SELECT Rank() over(PARTITION BY  Convert ( VARCHAR(10),  CreateTime,  120),CnName ORDER BY CreateTime {0} ) as rowno,  t.* FROM EmployeeServices t
            //                    WHERE  Creator='{1}' and  CreateTime BETWEEN '{2}' AND '{3}'
            //                    ) aa
            //                    WHERE rowno=1 ORDER BY CreateTime DESC  OFFSET {4} ROW FETCH NEXT {5} ROWS ONLY", Sort, Creator, startTime, endTime, taskPageSize, pageSize);

            List<EmployeeServicesModel> list = new List<EmployeeServicesModel>();
            foreach (var item in this.QueryData(sql))
            {
                list.Add(item);
            }
            return list;

        }
        #endregion

        #region 按照人员ID，时间 查询原始打卡记录总数
        /// <summary>
        /// 按照人员ID，时间 查询原始打卡记录总数
        /// </summary>
        /// <param name="Creator"></param>
        /// <param name="startTime"></param>
        /// <param name="endTime"></param>
        /// <returns></returns>
        public List<EmployeeServicesModel> QueryDataCount(string Creator, string startTime, string endTime)
        {
            try
            {

                string sql = string.Format(@"SELECT * FROM dbo.EmployeeServices WHERE  Creator = '{0}' and  CreateTime BETWEEN '{1}' AND '{2}'", Creator, startTime, endTime);
                List<EmployeeServicesModel> list = new List<EmployeeServicesModel>();
                foreach (var item in this.QueryData(sql))
                {
                    list.Add(item);
                }
                return list;
            }
            catch (Exception ex)
            {
                Log.WriteLog(ex.Message);
                Log.WriteLog(ex.StackTrace);
                throw;
            }

        }
        #endregion

        #region 读取XML中的信息
        /// <summary>
        /// 读取XML中的信息
        /// </summary>
        /// <returns></returns>
        public List<WorkMessages> QueryWorkMessages()
        {

            List<WorkMessages> list = new List<WorkMessages>();

            XmlDocument xml = new XmlDocument();
            xml = XmlManager(HttpContext.Current.Server.MapPath("../XmlConfig/Punch.xml"));
            //xml.Load(@"E:\C#\S2C#\DLCL\打印电脑\MyComputer\XulieHua\XML.xml");  //你的xml地址
            string id = "";
            string ontime = "";
            string offtime = "";
            double lat = 0.00;
            double lng = 0.00;
            WorkMessages info = null;
            //////////*******下面开始循环读取xml文件信息********/

            foreach (XmlNode node in xml.ChildNodes)
            {
                if (node.Name == "workMessage")
                {
                    foreach (XmlNode node1 in node.ChildNodes)
                    {
                        if (node1.Name == "item")
                        {
                            foreach (XmlNode node2 in node1.ChildNodes)
                            {
                                switch (node2.Name)
                                {
                                    case "id":
                                        id = node2.InnerText;
                                        break;
                                    case "ontime":
                                        ontime = node2.InnerText;
                                        break;
                                    case "offtime":
                                        offtime = node2.InnerText;
                                        break;
                                    case "lat":
                                        lat = Convert.ToDouble(node2.InnerText);
                                        break;
                                    default:
                                        lng = Convert.ToDouble(node2.InnerText);
                                        break;
                                }
                            }
                            info = new WorkMessages(id, ontime, offtime, lat, lng);
                            //将信息保存至集合
                            list.Add(info);
                        }
                    }
                }
            }

            return list;
        }
        #endregion

        #region 根据路径加载XML信息
        /// <summary>
        /// 根据路径加载XML信息
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
        #endregion
    }
}