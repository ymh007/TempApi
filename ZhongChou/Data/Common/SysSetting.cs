using MCS.Library.Core;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Xml;

namespace Seagull2.YuanXin.AppApi.ZhongChouData.Common
{
    public class SysSetting
    {
        private string key = string.Empty;//配置键
        private string value = string.Empty;//配置值
        private string desc = string.Empty;//配置描述
        private string text=string.Empty;//配置显示文字
        private string type = string.Empty;//配置类型

        public SysSetting(XmlNode node)
        {
            this.key = XmlHelper.GetAttributeText(node, "key");
            this.value = XmlHelper.GetAttributeText(node, "value");
            this.desc = XmlHelper.GetAttributeText(node, "desc");
            this.text = XmlHelper.GetAttributeText(node, "text");
            this.type = XmlHelper.GetAttributeText(node, "type");
        }

        #region 属性

        public string Key
        {
            get
            {
                return this.key;
            }
            set
            {
                this.key = value;
            }
        }

        public string Value
        {
            get
            {
                return this.value;
            }
            set
            {
                this.value = value;
            }
        }

        public string Desc
        {
            get
            {
                return this.desc;
            }
            set
            {
                this.desc = value;
            }
        }
        public string Text
        {
            get
            {
                return this.text;
            }
            set
            {
                this.text = value;
            }
        }

        public string Type
        {
            get
            {
                return this.type;
            }
            set
            {
                this.type = value;
            }
        }
        #endregion
    }


    [Serializable]
    public class SysSettingCollection : KeyedCollection<string, SysSetting>
    {
        internal SysSettingCollection()
        {
        }

        protected override string GetKeyForItem(SysSetting item)
        {
            return item.Key;
        }
    }
}
