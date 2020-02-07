using MCS.Library.Core;
using MCS.Library.SOA.DataObjects;
using Seagull2.YuanXin.AppApi.Configuration;
using Seagull2.YuanXin.AppApi.Extension;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Seagull2.YuanXin.AppApi.Services
{
    public static class UrlConvertHelper
    {
        public static List<UserTaskModel> Convert(List<UserTaskModel> tasks)
        {
            var excutors = UrlMappingConfigurationSection.GetConfig().UrlMapping;

            foreach (UrlMappingElement i in excutors)
            {
                var task = tasks.Where(t => t.Url.IsWildcharMatched(i.PcUrl));

                if (task != null)
                {
                    var excutor = i.CreateInstance<IUrlConverter>();

                    task.ForEach(t =>
                    {
                        if (t.Url.ToLower().Contains("thrwebapp") == false)
                        {
                            if (t.ResourceID.IsNotEmpty())
                            {
                                t.Url = excutor.Convert(t, i);
                                if (t.Url.IsNotEmpty())
                                {
                                    t.Enabled = true;
                                }
                            }
                        }
                    });
                }
            }

            return tasks;
        }

        public static string ResolveUri(string uriString)
        {
            Uri url = new Uri(uriString, UriKind.RelativeOrAbsolute);

            if (url.IsAbsoluteUri == false && string.IsNullOrEmpty(uriString) == false)
            {
                if (EnvironmentHelper.Mode == InstanceMode.Web)
                {
                    HttpRequest request = HttpContext.Current.Request;
                    string appPathAndQuery = string.Empty;

                    if (uriString[0] == '~')
                        appPathAndQuery = request.ApplicationPath + uriString.Substring(1);
                    else
                        if (uriString[0] != '/')
                        appPathAndQuery = request.ApplicationPath + "/" + uriString;
                    else
                        appPathAndQuery = uriString;

                    appPathAndQuery = appPathAndQuery.Replace("//", "/");

                    uriString = request.Url.GetComponents(UriComponents.SchemeAndServer, UriFormat.SafeUnescaped) +
                                appPathAndQuery;
                }
            }

            return uriString;
        }

        public static void SetTaskReadFlag(string taskID)
        {
            taskID.NullCheck("taskID");

            UserTaskAdapter.Instance.SetTaskReadFlag(taskID);
        }
    }
}