using MCS.Library.Caching;
using MCS.Library.Core;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Text;
using System.Web;
using System.Xml;

namespace Seagull2.YuanXin.AppApi.ZhongChouData.Common
{
    public static class SysSettingHelper
    {
        public static string GetVaule(string virtualPath, string key)
        {
            var collection = GetSysSettingCollection(virtualPath);

            return collection.Where(it => it.Key == key).FirstOrDefault().Value;
        }

        public static SysSettingGroup GetGroup(string virtualPath, string groupID)
        {
            var collection = GetGroups(virtualPath);

            return collection.Where(it => it.GroupID == groupID).FirstOrDefault();
        }

        private static SysSettingCollection GetSysSettingCollection(string virtualPath)
        {
            SysSettingCollection result = null;

            string filePath = HttpContext.Current.Server.MapPath(virtualPath).ToLower();

            if (SysSettingCache.Instance.TryGetValue(filePath, out result) == false)
            {
                XmlDocument xmlDoc = XmlHelper.LoadDocument(filePath);

                result = LoadSetting(xmlDoc);

                FileCacheDependency dependency = new FileCacheDependency(filePath);
                SysSettingCache.Instance.Add(filePath, result, dependency);
            }

            return result;
        }

        private static SysSettingCollection LoadSetting(XmlDocument xmlDoc)
        {
            SysSettingCollection modules = new SysSettingCollection();

            foreach (XmlElement elem in xmlDoc.DocumentElement.SelectNodes("Setting"))
                modules.Add(new SysSetting(elem));

            return modules;
        }

        private static SysSettingGroupCollection GetGroups(string virtualPath)
        {

                               
               //if (virtualPath.StartsWith("\\"))                 
               //{                                    
               //    virtualPath = virtualPath.TrimStart('\\');                
               //}                 
               // return System.IO.Path.Combine(AppDomain.CurrentDomain.BaseDirectory, virtualPath);             }  

            SysSettingGroupCollection result = null;

            string filePath = "";

            if (HttpContext.Current != null)
            {
                filePath = HttpContext.Current.Server.MapPath(virtualPath).ToLower();
            }
            else
            {
                virtualPath = virtualPath.Replace("/", "\\"); 
                if (virtualPath.StartsWith("~"))
                {
                     virtualPath = virtualPath.TrimStart('~');
                }
               
                //filePath = System.IO.Path.Combine(AppDomain.CurrentDomain.BaseDirectory, virtualPath);
                filePath = System.AppDomain.CurrentDomain.BaseDirectory + virtualPath;
               // filePath = ConfigurationManager.AppSettings["HostUrlApi"].ToString() + virtualPath;
            }

            //string filePath = HttpContext.Current.Server.MapPath(virtualPath).ToLower();

            if (SysSettingGroupCache.Instance.TryGetValue(filePath, out result) == false)
            {
                XmlDocument xmlDoc = XmlHelper.LoadDocument(filePath);

                result = LoadGroup(xmlDoc);

                FileCacheDependency dependency = new FileCacheDependency(filePath);
                SysSettingGroupCache.Instance.Add(filePath, result, dependency);
            }

            return result;
        }

        private static SysSettingGroupCollection LoadGroup(XmlDocument xmlDoc)
        {
            SysSettingGroupCollection modules = new SysSettingGroupCollection();

            foreach (XmlElement elem in xmlDoc.DocumentElement.SelectNodes("Group"))
                modules.Add(new SysSettingGroup(elem));

            return modules;
        }
    }

    internal sealed class SysSettingCache : CacheQueue<string, SysSettingCollection>
    {
        public static readonly SysSettingCache Instance = CacheManager.GetInstance<SysSettingCache>();
    }

    internal sealed class SysSettingGroupCache : CacheQueue<string, SysSettingGroupCollection>
    {
        public static readonly SysSettingGroupCache Instance = CacheManager.GetInstance<SysSettingGroupCache>();
    }
}
