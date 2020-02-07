using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Threading;
using System.Threading.Tasks;
using System.Web;
using log4net;

namespace Seagull2.YuanXin.AppApi
{
    /// <summary>
    /// 数据缓存服务
    /// </summary>
    public class CacheService
    {
        static ILog log = LogManager.GetLogger(MethodBase.GetCurrentMethod().DeclaringType);
        static Dictionary<string, KeyValuePair<DateTime, object>> dictionary = new Dictionary<string, KeyValuePair<DateTime, object>>();

        /// <summary>
        /// 静态构造
        /// </summary>
        static CacheService()
        {
            // 启动一个任务，定时移除过期数据
            Task.Run(() =>
            {
                try
                {
                    log.Info("开始执行定时清理缓存任务。");
                    while (true)
                    {
                        var keys = new List<string>();
                        foreach (var key in dictionary.Keys)
                        {
                            if (dictionary[key].Key < DateTime.Now)
                            {
                                keys.Add(key);
                            }
                        }
                        keys.ForEach(key => Remove(key));
                        Thread.Sleep(5000);
                    }
                }
                catch (Exception e)
                {
                    log.ErrorFormat("执行定时移除过期缓存数据异常：{0}", e.Message);
                }
            });
        }

        /// <summary>
        /// 判断是否存在该数据
        /// </summary>
        /// <param name="key">主键</param>
        /// <returns>Boolean</returns>
        public static bool IsExist(string key)
        {
            return dictionary.ContainsKey(key);
        }

        /// <summary>
        /// 从缓存中获取数据
        /// </summary>
        /// <param name="key">主键</param>
        /// <param name="func">获取数据的委托</param>
        /// <returns>数据内容</returns>
        public static T Get<T>(string key, Func<T> func)
        {
            if (IsExist(key))
            {
                if (dictionary[key].Key > DateTime.Now)
                {
                    return (T)dictionary[key].Value;
                }
                else
                {
                    return SetData(key, func);
                }
            }
            else
            {
                return SetData(key, func);
            }
        }
        static T SetData<T>(string key, Func<T> func)
        {
            T t = func.Invoke();
            dictionary[key] = new KeyValuePair<DateTime, object>(DateTime.Now.AddHours(3), t);
            return t;
        }

        /// <summary>
        /// 根据key移除数据
        /// </summary>
        /// <param name="key"></param>
        public static void Remove(string key)
        {
            if (dictionary.ContainsKey(key))
            {
                dictionary.Remove(key);
            }
        }

        /// <summary>
        /// 移除所有数据
        /// </summary>
        public static void RemoveAll()
        {
            dictionary.Clear();
        }

        /// <summary>
        /// 根据条件移除数据
        /// </summary>
        /// <param name="func">条件表达式</param>
        public static void RemoveByCondition(Func<string, bool> func)
        {
            var keys = new List<string>();
            foreach (var key in dictionary.Keys)
            {
                if (func.Invoke(key))
                {
                    keys.Add(key);
                }
            }
            keys.ForEach(key => Remove(key));
        }
    }
}