using MCS.Library.Core;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.Serialization.Json;
using System.Text;
using System.Web;

namespace Seagull2.YuanXin.AppApi.Models
{
    /// <summary>
    /// 所有Model类基类用于固定字段的封装
    /// </summary>
    public class AbstractBean
    {
        /// <summary>
        /// ID，唯一标示GUID
        /// </summary>
        public string Code { get; set; }
        /// <summary>
        /// 创建人ID，关联用户信息表UserInfo
        /// </summary>
        public string Creator { get; set; }
        /// <summary>
        /// 创建时间
        /// </summary>
        public DateTime CreateTime { get; set; }
        /// <summary>
        /// 修改人
        /// </summary>
        public string Modifier { get; set; }
        /// <summary>
        /// 修改时间
        /// </summary>
        public DateTime ModifyTime { get; set; }
        /// <summary>
        /// 有效性
        /// </summary>
        public bool ValidStatus { get; set; }
    }

    /// <summary>
    /// 构造枚举列表[下拉列表数据源的格式]
    /// </summary>
    public class DrapDownListItem
    {
        /// <summary>
        /// 选择值
        /// </summary>
        public int Value { get; set; }
        /// <summary>
        /// 选择的文本
        /// </summary>
        public string Text { get; set; }
    }

    /// <summary>
    /// 用于实体类属性的各种扩展
    /// </summary>
    public static class ModelExtention
    {
        #region 枚举操作
        /// <summary>
        /// 获取枚举值所对应的描述
        /// </summary>
        public static string GetEnumDescriptionByValue<T>(int value)
        {
            EnumItemDescriptionList m_result = EnumItemDescriptionAttribute.GetDescriptionList(typeof(T));
            return m_result.ToList().Find(c => c.EnumValue == value)?.Description;
        }
        /// <summary>
        /// 获取枚举所有的值和描述,以列表形式返回
        /// </summary>
        public static List<DrapDownListItem> GetEnumTYpeDesList<T>()
        {
            EnumItemDescriptionList m_result = EnumItemDescriptionAttribute.GetDescriptionList(typeof(T));
            List<DrapDownListItem> m_ReturnResult = new List<DrapDownListItem>();
            foreach (var item in m_result)
            {
                DrapDownListItem m_Model = new DrapDownListItem();
                m_Model.Value = item.EnumValue;
                m_Model.Text = item.Description;
                m_ReturnResult.Add(m_Model);
            }
            return m_ReturnResult;
        }
        #endregion

        #region Json
        /// <summary>
        /// JSON反序列化
        /// </summary>
        public static T JsonDeserialize<T>(string jsonString) where T : new()
        {
            try
            {
                DataContractJsonSerializer ser = new DataContractJsonSerializer(typeof(T));
                MemoryStream ms = new MemoryStream(Encoding.UTF8.GetBytes(jsonString));
                T obj = (T)ser.ReadObject(ms);
                return obj;
            }
            catch
            {
                return new T();
            }
        }
        #endregion
        
        #region 图片处理
        /// <summary>
        /// 获取图片类型
        /// </summary>
        /// <param name="imgSrc"></param>
        /// <returns></returns>
        public static string GetImageType(string imgSrc)
        {
            try
            {
                string m_ImageType = string.Empty;
                if (!string.IsNullOrWhiteSpace(imgSrc))
                {
                    m_ImageType = imgSrc.Split('/', ';')?[1];
                }
                return m_ImageType;
            }
            catch
            {
                return "";
            }
        }
        /// <summary>
        /// 获取图片Base64字符串
        /// </summary>
        /// <param name="imgSrc"></param>
        /// <returns></returns>
        public static string GetBase64Str(string imgSrc)
        {
            try
            {
                string m_Base64Str = string.Empty;
                if (!string.IsNullOrWhiteSpace(imgSrc))
                {
                    m_Base64Str = imgSrc.Split(',')[1];
                }
                return m_Base64Str;
            }
            catch
            {
                return "";
            }
        }
        #endregion
    }
}