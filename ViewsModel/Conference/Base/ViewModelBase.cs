using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Seagull2.YuanXin.AppApi.ViewsModel
{
    /// <summary>
    /// API接口通用类，返回两个Object对象
    /// </summary>
    public class ViewModelBase
    {
        /// <summary>
        /// 状态
        /// </summary>
        public bool State { get; set; }
        /// <summary>
        /// 返回消息
        /// </summary>
        public string Message { get; set; }
        /// <summary>
        /// 附带对象1
        /// </summary>
        public object Data1 { get; set; }
        /// <summary>
        /// 附带对象2
        /// </summary>
        public object Data2 { get; set; }
    }
    /// <summary>
    /// API接口通用类，返回 T 对象
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public class ViewModelBase<T> where T : new()
    {
        /// <summary>
        /// 状态
        /// </summary>
        public bool State { get; set; }
        /// <summary>
        /// 返回消息
        /// </summary>
        public string Message { get; set; }
        /// <summary>
        /// 附带对象1
        /// </summary>
        public T Data { get; set; }
    }

    /// <summary>
    /// API接口通用类，返回一个Object对象
    /// </summary>
    public class ViewModelBaseList
    {
        /// <summary>
        /// 状态
        /// </summary>
        public bool State { get; set; }
        /// <summary>
        /// 返回消息
        /// </summary>
        public string Message { get; set; }
        /// <summary>
        /// 附带对象
        /// </summary>
        public object Data { get; set; }
    }

    /// <summary>
    /// API接口通用类，不返回对象
    /// </summary>
    public class ViewModelBaseNull
    {
        /// <summary>
        /// 状态
        /// </summary>
        public bool State { get; set; }
        /// <summary>
        /// 返回消息
        /// </summary>
        public string Message { get; set; }
    }
}