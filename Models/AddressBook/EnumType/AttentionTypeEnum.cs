using MCS.Library.Core;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Seagull2.YuanXin.AppApi.Models.AddressBook.EnumType
{
    public enum AttentionTypeEnum
    {
        [EnumItemDescription(Description = "事业部")]
        BusinessName = 1,

        [EnumItemDescription(Description = "城市")]
        CityName = 2,

        [EnumItemDescription(Description = "项目")]
        ProjectName = 3,

        [EnumItemDescription(Description = "全集团")]
        BusinessAddName = 4
    }
}