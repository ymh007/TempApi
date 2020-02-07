using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Seagull2.YuanXin.AppApi.Models
{
    public static class StringExtend
    {
        public static bool IsEmptyOrNull(this string str)
        {
            return string.IsNullOrEmpty(str);
        }
    }
}