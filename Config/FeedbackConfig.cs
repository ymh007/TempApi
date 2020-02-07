using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Web;

namespace Seagull2.YuanXin.AppApi.Config
{
    public class FeedbackConfig : ConfigurationSection
    {
        public static FeedbackConfig GetConfig()
        {
            FeedbackConfig result = (FeedbackConfig)ConfigurationManager.GetSection("feedbackConfig");

            if (result == null)
                result = new FeedbackConfig();

            return result;
        }

        [ConfigurationProperty("connectionOptions")]
        public RedisConnectionStringElementCollection ConnectionOptions
        {
            get
            {
                return (RedisConnectionStringElementCollection)base["connectionOptions"];
            }
        }
    }

    public class RedisConnectionStringElementCollection : ConfigurationElementCollection
    {
        protected override ConfigurationElement CreateNewElement()
        {
            return new RedisConnectionStringElement();
        }

        protected override object GetElementKey(ConfigurationElement element)
        {
            return ((RedisConnectionStringElement)element).Name;
        }

        public RedisConnectionStringElement this[string name] => BaseGet(name) as RedisConnectionStringElement;
    }

    /// <summary>
    /// 人员信息配置项
    /// </summary>
    public class RedisConnectionStringElement : ConfigurationElement
    {
        /// <summary>
        /// 人员编码
        /// </summary>
        [ConfigurationProperty("code", IsRequired = false)]
        public string Code
        {
            get
            {
                return (string)this["code"];
            }
        }

        /// <summary>
        /// 姓名
        /// </summary>
        [ConfigurationProperty("name", IsRequired = false)]
        public string Name
        {
            get
            {
                return (string)this["name"];
            }
        }

        /// <summary>
        /// 邮箱
        /// </summary>
        [ConfigurationProperty("email", IsRequired = false)]
        public string Email
        {
            get
            {
                return (string)this["email"];
            }
        }
    }
}