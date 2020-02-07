using Seagull2.YuanXin.AppApi.ViewsModel;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Seagull2.YuanXin.AppApi.Controllers
{
    /// <summary>
    /// 控制器帮助类
    /// </summary>
    public class ControllerHelp
    {
        /// <summary>
        /// 查
        /// </summary>
        /// <param name="action"></param>
        public static void SelectAction(Action action)
        {
            ViewModelBase result = new ViewModelBase();
            try
            {
                action();
                result.State = true;
            }
            catch (Exception e)
            {
                Log.WriteLog(e.Message);
                Log.WriteLog(e.StackTrace);
                throw e;
            }
        }

        /// <summary>
        /// 增删改
        /// </summary>
        /// <param name="action"></param>
        public static ViewModelBase RunAction(Action action)
        {
            ViewModelBase result = new ViewModelBase();
            try
            {
                action();
                result.State = true;
            }
            catch (Exception e)
            {
                result.State = false;
                result.Message = e.Message;
                Log.WriteLog(e.Message);
                Log.WriteLog(e.StackTrace);
            }
            return result;
        }
        /// <summary>
        /// 增删改(添加校验函数)
        /// </summary>
        public static ViewModelBase RunAction(Func<ViewModelBase> checkAction, Action action)
        {
            ViewModelBase result = new ViewModelBase();
            try
            {
                result = checkAction();
                if (result.State == true)
                {
                    action();
                    result.State = true;
                }
                else
                {
                    return result;
                }
            }
            catch (Exception e)
            {
                result.State = false;
                result.Message = e.Message;
                Log.WriteLog(e.Message);
                Log.WriteLog(e.StackTrace);
            }
            return result;
        }
    }
}