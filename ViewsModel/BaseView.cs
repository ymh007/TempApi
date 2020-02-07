using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Newtonsoft.Json;

namespace Seagull2.YuanXin.AppApi.ViewsModel
{
    /// <summary>
    /// API统一返回格式（2017-07-13 By：v-sunzhh）
    /// </summary>
    public class BaseView
    {
        /// <summary>
        /// 状态
        /// </summary>
        public bool State { set; get; }
        /// <summary>
        /// 消息提示
        /// </summary>
        public string Message { set; get; }
        /// <summary>
        /// 数据
        /// </summary>
        [JsonProperty(NullValueHandling = NullValueHandling.Ignore)]
        public object Data { set; get; }
    }

    /// <summary>
    /// 分页数据
    /// </summary>
    public class BaseViewPage
    {
        /// <summary>
        /// 总记录数
        /// </summary>
        public int DataCount { set; get; }
        /// <summary>
        /// 总页数
        /// </summary>
        public int PageCount { set; get; }
        /// <summary>
        /// 当前页数据
        /// </summary>
        public object PageData { set; get; }
    }

    /// <summary>
    /// 分页数据
    /// </summary>
    public class BaseViewPage<T>
    {
        /// <summary>
        /// 总记录数
        /// </summary>
        public int DataCount { set; get; }
        /// <summary>
        /// 总页数
        /// </summary>
        public int PageCount { set; get; }
        /// <summary>
        /// 当前页数据
        /// </summary>
        public List<T> PageData { set; get; }
    }

    /// <summary>
    /// 是否存在数据值 如果存在值：序列化Value字段，否则不序列号Value字段
    /// </summary>
    public class BaseViewIsValue<T>
    {
        /// <summary>
        /// 判断Value是否为空
        /// </summary>
        public bool IsSet
        {
            get
            {
                if (Value == null)
                    return false;
                return true;
            }
        }
        /// <summary>
        /// 数据值
        /// </summary>
        [JsonProperty(NullValueHandling = NullValueHandling.Ignore)]
        public T Value;
    }

    /// <summary>
    /// 适用于名称 - 数据
    /// </summary>
    public class BaseViewNameData
    {
        /// <summary>
        /// 名称
        /// </summary>
        public string Name;
        /// <summary>
        /// 数据
        /// </summary>
        public object Data;
    }

    /// <summary>
    /// 适用于Code - Name
    /// </summary>
    public class BaseViewCodeName
    {
        /// <summary>
        /// 编码
        /// </summary>
        public string Code { set; get; }
        /// <summary>
        /// 名称
        /// </summary>
        public string Name { set; get; }
    }
    /// <summary>
    /// 适用于id - displayName 
    /// </summary>
    public class Person
    {

        /// <summary>
        /// 编码
        /// </summary>
        public string Id { set; get; }
        /// <summary>
        /// 名称
        /// </summary>
        public string DisplayName { set; get; }
    }
}