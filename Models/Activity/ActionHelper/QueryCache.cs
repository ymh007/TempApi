using System;
using System.Data;
using System.Reflection;
using System.Web;
using System.Web.Caching;

namespace Seagull2.YuanXin.AppApi.Models
{
    public sealed class QueryCache
    {
        private static QueryCache _instance = null;

        private static readonly object lockHelper = new object();

        private static DateTime absoluteExpiration = DateTime.Now.AddMinutes(30);

        private System.Web.Caching.Cache resultMap = null;

        private Random sessionIdRandom = null;

        private LogHelper log = LogFactory.GetLogger(MethodBase.GetCurrentMethod().DeclaringType);

        /**
         * 获得本地缓存的实例，该实例为单例模式
         */
        public static QueryCache getInstance() {
                   
            if(_instance == null)
            {
                lock(lockHelper)
                {
                    if(_instance == null)
                         _instance = new QueryCache();
                }
            }
            return _instance;
        }
            
       
        /**
         * 构造函数
         */
        private QueryCache() {
            HttpRuntime httpRT = new HttpRuntime();
		    sessionIdRandom = new Random();
		    resultMap =  HttpRuntime.Cache;
          
	    }

        /**
         * 该方法用于生成sessionID
         */
        private string generateSessioId() {

            int nSeed = (int)(DateTime.Now.Ticks >> 32);
            string sessionId = sessionIdRandom.Next(nSeed).ToString();
    
            return sessionId;
	    }

        /**
         * 根据sessionID从本地缓存获取查询结果
         */
        public DataTable pullQueryDataTable(string sessionId) {
		    
            if (sessionId == null)
			    return null;

            return (DataTable)resultMap.Get(sessionId);
	    }

        /**
         * 该方法用于将查询结果存储到本地缓存，返回sessionID，缓存失效时间为10min
         */
        public string putQueryDataTable(DataTable dt)
        {
            string sessionId = generateSessioId();
            
            try
            {
                this.resultMap.Add(sessionId, dt, null, DateTime.MaxValue, TimeSpan.FromMinutes(10), System.Web.Caching.CacheItemPriority.Normal, null);
            }
            catch (Exception ex) {
                log.Error("put Query DataTable failed! -- ", ex);
            }  
           
		    return sessionId;
	    }

    }
}