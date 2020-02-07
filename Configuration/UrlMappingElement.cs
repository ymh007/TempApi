using MCS.Library.Configuration;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Seagull2.YuanXin.AppApi.Configuration
{
    public class UrlMappingElement : TypeConfigurationElement
    {
        [ConfigurationProperty("pcUrl")]
        public string PcUrl
        {
            get
            {
                return (string)this["pcUrl"];
            }
        }

        [ConfigurationProperty("mobileUrl")]
        public string MobileUrl
        {
            get
            {
                return (string)this["mobileUrl"];
            }
          
        }


        [ConfigurationProperty("exceptScence")]
        public string ExceptScence
        {
            get
            {
                return (string)this["exceptScence"];
            }
        }

        [ConfigurationProperty("architecture")]
        public string Architecture
        {
            get
            {
                return (string)this["architecture"];
            }
        }
    }

    [ConfigurationCollection(typeof(UrlMappingElement), CollectionType = ConfigurationElementCollectionType.AddRemoveClearMap)]
    public class ExtraElementCollection : NamedConfigurationElementCollection<UrlMappingElement>
    {

    }
}