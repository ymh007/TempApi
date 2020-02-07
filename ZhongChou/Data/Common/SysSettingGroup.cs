using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml;
using System.Collections.ObjectModel;
using MCS.Library.Core;

namespace Seagull2.YuanXin.AppApi.ZhongChouData.Common
{
    public class SysSettingGroup
    {
        private string groupID = string.Empty;//模块ID
        private string groupDesc = string.Empty;//模块描述
        private SysSettingCollection settings = null;//模块中的模板集合

        internal SysSettingGroup(XmlNode node)
        {
            this.groupID = XmlHelper.GetAttributeText(node, "groupID");
            this.groupDesc = XmlHelper.GetAttributeText(node, "groupDesc");
            this.settings = new SysSettingCollection();

            foreach (XmlElement elem in node.SelectNodes("Setting"))
                settings.Add(new SysSetting(elem));
        }


        public string GroupID
        {
            get
            {
                return this.groupID;
            }
            set
            {
                this.groupID = value;
            }
        }

        public string GroupDesc
        {
            get
            {
                return this.groupDesc;
            }
            set
            {
                this.groupDesc = value;
            }
        }


        public SysSetting this[string key]
        {
            get
            {
                return this.GetSysSetting(key);
            }
        }

        private SysSetting GetSysSetting(string key)
        {
            return this.settings.Where(it => it.Key == key).FirstOrDefault();
        }

        public SysSettingCollection SettingCollection
        {
            get
            {
                return settings;
            }
        }
    }

    [Serializable]
    public class SysSettingGroupCollection : KeyedCollection<string, SysSettingGroup>
    {
        internal SysSettingGroupCollection()
        {
        }

        protected override string GetKeyForItem(SysSettingGroup item)
        {
            return item.GroupID;
        }
    }
}
