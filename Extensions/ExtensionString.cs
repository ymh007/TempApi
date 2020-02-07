using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using System.Web;
using MCS.Library.Core;

namespace Seagull2.YuanXin.AppApi.Extension
{
    /// <summary>
    /// 字符串扩展
    /// </summary>
    public static class ExtensionString
    {
        /// <summary>
        /// 判断字符串是否为Null或WhiteSpace
        /// </summary>
        public static bool IsNullOrWhiteSpace(this string str)
        {
            return string.IsNullOrWhiteSpace(str);
        }

        public static bool IsWildcharMatched(this string data, string pattern)
        {
            //结尾加*为了保持兼容性
            pattern = pattern.ToLower() + "*";

            return MatchWithAsterisk(data.ToLower(), pattern);
        }

        private static bool MatchWithAsterisk(string data, string pattern)
        {
            if (data.IsNullOrEmpty() || pattern.IsNullOrEmpty())
                return false;

            string[] ps = pattern.Split('*');

            if (ps.Length == 1) // 没有*的模型
                return MatchWithInterrogation(data, ps[0]);

            var si = data.IndexOf(ps[0], 0);	// 从string头查找第一个串

            if (si != 0)
                return false; // 第一个串没找到或者不在string的头部

            si += ps[0].Length; // 找到了串后,按就近原则,移到未查询过的最左边

            int plast = ps.Length - 1; // 最后一串应单独处理,为了提高效率,将它从循环中取出
            int pi = 0; // 跳过之前处理过的第一串

            while (++pi < plast)
            {
                if (ps[pi] == "")
                    continue; //连续的*号,可以忽略

                si = data.IndexOf(ps[pi], si);	// 继续下一串的查找

                if (-1 == si)
                    return false; // 没有找到

                si += ps[pi].Length; // 就近原则
            }

            if (ps[plast] == "") // 模型尾部为*,说明所有有效字符串部分已全部匹配,string后面可以是任意字符
                return true;

            // 从尾部查询最后一串是否存在
            int last_index = data.LastIndexOf(ps[plast]);

            // 如果串存在,一定要在string的尾部, 并且不能越过已查询过部分
            return (last_index == data.Length - ps[plast].Length) && (last_index >= si);
        }

        private static bool MatchWithInterrogation(string data, string pattern)
        {
            bool result = false;

            if (data.Length == pattern.Length)
                result = data.IndexOf(pattern) > -1;

            return result;
        }

        public static string AddressSplit(this string resource, int startIndex, char joinStr)
        {
            var strsp = resource.Split('/');
            var name = "";
            for (int j = startIndex; j < strsp.Length; j++)
            {
                name += strsp[j] + joinStr;
            }
            return name.TrimEnd(joinStr);
        }
        public static string StringManipulation(this string resources, string regex)
        {
            return Regex.Replace(resources, regex, string.Empty, RegexOptions.IgnoreCase).Replace("\r\n", string.Empty);
        }
    }
}