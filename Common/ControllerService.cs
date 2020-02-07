using System;
using System.Net.Http;
using System.Reflection;
using System.Security.Principal;
using System.Threading.Tasks;
using System.Web;
using log4net;
using Seagull2.Core.Models;
using Seagull2.YuanXin.AppApi.ViewsModel;

namespace Seagull2.YuanXin.AppApi
{
    /// <summary>
    /// 控制器服务类
    /// </summary>
    public class ControllerService
    {
        static ILog log = LogManager.GetLogger(MethodBase.GetCurrentMethod().DeclaringType);

        /// <summary>
        /// 控制器统一异常处理
        /// </summary>
        public static BaseView Run(Action action)
        {
            BaseView baseView = new BaseView();
            try
            {
                action();
                baseView.State = true;
                baseView.Message = "success.";
            }
            catch (Exception e)
            {
                baseView.State = false;
                baseView.Message = e.Message;
                log.Error(e);
            }
            return baseView;
        }

        /// <summary>
        /// 控制器统一异常处理
        /// </summary>
        public static BaseView Run<T>(Func<T> func)
        {
            BaseView baseView = new BaseView();
            try
            {
                var data = func();
                baseView.State = true;
                baseView.Message = "success.";
                baseView.Data = data;
            }
            catch (Exception e)
            {
                baseView.State = false;
                baseView.Message = e.Message;
                log.Error(e);
            }
            return baseView;
        }

        /// <summary>
        /// 上传日志
        /// </summary>
        public static async Task<HttpResponseMessage> UploadLog(string currUserID, string desc)
        {
            try
            {
                string rule = HttpContext.Current.Request.Headers.Get("RoleType");
                object args = new
                {
                    id = Guid.NewGuid().ToString(),  userId = currUserID,
                    operateTime = DateTime.Now,   operateDesc = desc,
                    roleName = rule == "1" ? "总管理员" : rule == "2" ? "超级管理员" : "分级管理员"
                };
                using (var http = new HttpClient())
                {
                    return await http.PostAsJsonAsync(ConfigAppSetting.SaveLog, args);
                }
            }
            catch (Exception ex)
            {
                Log.WriteLog("日志上传错误:" + ex.Message);
                return null;
            }
        }

    }
}