using MCS.Library.Configuration;
using MCS.Library.SOA.DataObjects;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Seagull2.YuanXin.AppApi.Configuration
{
    public class UrlMappingConfigurationSection : ConfigurationSection
    {
        public static UrlMappingConfigurationSection GetConfig()
        {
            UrlMappingConfigurationSection settings =
                (UrlMappingConfigurationSection)ConfigurationBroker.GetSection("UrlMappingSettings") ??
                new UrlMappingConfigurationSection();
            return settings;
        }

        [ConfigurationProperty("urlMapping", IsRequired = false)]
        public ExtraElementCollection UrlMapping
        {
            get
            {
                return (ExtraElementCollection)base["urlMapping"];
            }
        }
    }
}