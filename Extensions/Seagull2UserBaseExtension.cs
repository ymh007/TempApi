using System;
using System.Web;
using System.Linq;
using Seagull2.Core.Models;
using Seagull2.Core.Extensions;
using System.Collections.Generic;
using Seagull2.YuanXin.AppApi.Models;
//using Seagull2.Owin.Organization.Models;

namespace Seagull2.YuanXin.AppApi.Extensions
{
    public static class Seagull2UserBaseExtension
    {
        public static Dictionary<string, string> ToSipWrapperCollection(this IEnumerable<Seagull2UserBase> source)
        {
            return source.Distinct(user => user.Id, StringComparer.CurrentCultureIgnoreCase).ToDictionary(user => user.Id, user => (string)user.Property.GetValueOrDefault("Sip", null));
        }

        public static Dictionary<string, UserYuanxinInfoWrapper> ToUserContactInfoWrapperCollection(this IEnumerable<Seagull2UserBase> source)
        {
            return source.Distinct(user => user.Id, StringComparer.CurrentCultureIgnoreCase).ToDictionary(
                user => user.Id,
                user => new UserYuanxinInfoWrapper()
                {
                    DisplayName = user.DisplayName,
                    Email = (string)user.Property.GetValueOrDefault("E_MAIL", string.Empty),
                    Id = user.Id,
                    CompanyName = (string)user.Property.GetValueOrDefault("CompanyName", string.Empty),
                    DepartmentName = (string)user.Property.GetValueOrDefault("DepartmentName", string.Empty),
                    Phone = (string)user.Property.GetValueOrDefault("MP", string.Empty)
                });
        }
    }
}
