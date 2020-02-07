using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Seagull2.YuanXin.AppApi.ZhongChouApi.Common
{
    public class JsonHelper
    {
        public static JsonSerializerSettings GetDefaultJsonSettings()
        {
            var jsonSettiongs = new JsonSerializerSettings
            {
                //默认时间格式
                DateFormatString = "yyyy-MM-dd HH:mm:ss"
            };

            return jsonSettiongs;
        }
    }
}