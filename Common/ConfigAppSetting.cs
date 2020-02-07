using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Web;

namespace Seagull2.YuanXin.AppApi
{
    /// <summary>
    /// AppSetting配置项
    /// </summary>
    public class ConfigAppSetting
    {
        /// <summary>
        /// Moss服务器域名
        /// </summary>
        public static string AppFileService = ConfigurationManager.AppSettings["AppFileService"];
        /// <summary>
        /// 默认用户名
        /// </summary>
        public static string User = ConfigurationManager.AppSettings["User"];
        /// <summary>
        /// 默认密码
        /// </summary>
        public static string Password = ConfigurationManager.AppSettings["password"];
        /// <summary>
        /// 域
        /// </summary>
        public static string Domain = ConfigurationManager.AppSettings["Domain"];
        /// <summary>
        /// Moss图片处理Api地址
        /// </summary>
        public static string MossImageApiUrl = ConfigurationManager.AppSettings["MossImageApiUrl"];
        /// <summary>
        /// 数字远洋面板ID
        /// </summary>
        public static string DigitalPanelId = ConfigurationManager.AppSettings["DigitalPanelId"];
        /// <summary>
        /// 数字远洋今日新增数据要显示横线的项目名称
        /// </summary>
        public static string OceanDataShowHr = ConfigurationManager.AppSettings["OceanDataShowHr"];
        /// <summary>
        /// 海鸥二用户头像显示地址
        /// </summary>
        public static string SignImgPath = ConfigurationManager.AppSettings["signImgPath"];
        /// <summary>
        /// EIP访问角色
        /// </summary>
        public static string EIPACCESSER = ConfigurationManager.AppSettings["EIPACCESSER"];
        /// <summary>
        /// 远洋邦邦访问角色
        /// </summary>
        public static string YBACCESSER = ConfigurationManager.AppSettings["YBACCESSER"];
        /// <summary>
        /// EIP模块，用于控制工作通入口
        /// </summary>
        public static string EIPModuleForWorkRegion = ConfigurationManager.AppSettings["EIPModuleForWorkRegion"];

        /// <summary>
        /// app 各个模块是否可见
        /// </summary>
        public static string AppModuleVisible = ConfigurationManager.AppSettings["AppModuleVisible"];

        /// <summary>
        /// Office路径
        /// </summary>
        public static string OfficePath = ConfigurationManager.AppSettings["OfficePath"];
        /// <summary>
        /// Api路径
        /// </summary>
        public static string ApiPath = ConfigurationManager.AppSettings["ApiPath"];
        /// <summary>
        /// 移动办公AppId
        /// </summary>
        public static string AppId = ConfigurationManager.AppSettings["AppId"];
        /// <summary>
        /// 签到时间线
        /// </summary>
        public static string SignTimeLine = ConfigurationManager.AppSettings["SignTimeLine"];

        /// <summary>
        /// 定时提醒服务的 redis 配置名称
        /// </summary>
        public static string RedisConfName = "TimedReminder";


        /// <summary>
        /// 后台管理端上传日志接口
        /// </summary>
        public static string SaveLog = ConfigurationManager.AppSettings["SaveLog"];
        /// <summary>
        /// 后台管理端获取日志接口
        /// </summary>
        public static string GetLogs = ConfigurationManager.AppSettings["GetLogs"];

    }
}