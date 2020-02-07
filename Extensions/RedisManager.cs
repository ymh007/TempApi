using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
using System.Threading.Tasks;
using StackExchange.Redis;
using System.Configuration;
using ProtoBuf;

namespace Seagull2.YuanXin.AppApi.Extensions
{
    public class RedisManager
    {
        #region
        /// init
        private IDatabase db;

        private RedisConnectionStringElement config;
        private IServer server;
        private void Init(string configName)
        {
            //@"Host"
            ConnectionMultiplexer redis = ConnectionMultiplexer.Connect(GetConnectionString(configName));
            db = redis.GetDatabase(config.DB);
            server = redis.GetServer(config.ConnectionString, config.Port);
        }

        private string GetConnectionString(string configName)
        {
            var a = ((YuanXinRedisConfigSettings)ConfigurationManager.GetSection("yuanxinRedisSettings")).ConnectionOptions[configName];
            config = YuanXinRedisConfigSettings.GetConfig().ConnectionOptions[configName];
            StringBuilder conectionString = new StringBuilder();
            conectionString.AppendFormat(@"{0}:{1}", config.ConnectionString, config.Port);
            if (!string.IsNullOrEmpty(config.PassWord))
            {
                conectionString.AppendFormat(@",password={0},ConnectTimeout=10000,abortConnect=false", config.PassWord);
            }

            return conectionString.ToString();
        }

        /// <summary>
        /// 将配置文件中的连接字符串传入
        /// </summary>
        /// <param name="configName"></param>
        public RedisManager(string configName)
        {
            Init(configName);
        }
        #endregion

        /// <summary>
        /// 取数据
        /// </summary>
        /// <param name="key"></param>
        /// <returns></returns>
        public Task<RedisValue> StringGetAsync(RedisKey key)
        {
            return db.StringGetAsync(key);
        }

        /// <summary>
		/// 删除指定key
		/// </summary>
		/// <param name="key"></param>
		/// <returns></returns>
		public bool DeleteKey(RedisKey key)
        {
            return db.KeyDelete(key);
        }
        /// <summary>
        /// 为了订阅任务而保存值
        /// </summary>
        /// <param name="key"></param>
        /// <param name="value"></param>
        /// <returns></returns>
        public bool StringSet(string key, string value, TimeSpan time)
        {
            return db.StringSet(key, value, time);
        }

        private void log(string msg)
        {
            StreamWriter sw1 = File.AppendText(@"D:\WebApp\YuanXinIM\redis.txt");
            string w1 = msg + "\r\n";
            sw1.Write(w1);
            sw1.Close();
        }

        //todo：此处应该将序列化抽到外部处理
        public RedisValue Serialize<T>(T data)
        {
            using (MemoryStream ms = new MemoryStream())
            {
                Serializer.Serialize(ms, data);
                return ms.ToArray();
            }
        }
        public T DeSerialize<T>(RedisValue value)
        {
            using (MemoryStream ms = new MemoryStream(value))
            {
                return Serializer.Deserialize<T>(ms);
            }
        }


        /// <summary>
        /// Save数据
        /// </summary>
        /// <param name="keys"></param>
        /// <param name="values"></param>
        /// <param name="expiredTime"></param>
        /// <returns></returns>
        public Task<bool> SaveAsync<T>(string key, T value, TimeSpan expiredTime)
        {

            RedisValue rValue = this.Serialize(value);
            var tran = db.CreateTransaction(true);
            tran.StringSetAsync(key, rValue, expiredTime);
            return tran.ExecuteAsync();
        }

        /// <summary>
        /// 获取数据
        /// </summary>
        /// <param name="keys"></param>
        /// <param name="values"></param>
        /// <param name="expiredTime"></param>
        /// <returns></returns>
        public Task<T> GetAsync<T>(string key)
        {
            var redisValue = db.StringGetAsync(key);
            if (!redisValue.Result.IsNullOrEmpty)
                return Task.FromResult(this.DeSerialize<T>(redisValue.Result));
            else
                return null;
        }

        /// <summary>
        /// 获取数据
        /// </summary>
        /// <param name="key"></param>
        /// <returns></returns>
        public string GetAsync(string key)
        {
            var redisValue = db.StringGetAsync(key);
            if (!redisValue.Result.IsNullOrEmpty)
                return redisValue.Result;
            else
                return "";
        }

        /// <summary>
        /// 无序排列存储
        /// </summary>
        /// <param name="key">RedisKey</param>
        /// <param name="values">RedisValue</param>
        /// <returns></returns>
        public bool SetAdd(RedisKey key, RedisValue values)
        {
            return db.SetAdd(key, values);
        }

        /// <summary>
        /// 无序列表获取数据方法法
        /// </summary>
        /// <param name="setOperation">并集，交集，差集</param>
        /// <param name="first">firstKey</param>
        /// <param name="second">secondKey</param>
        /// <returns></returns>
        public RedisValue[] SetCombine(SetOperation setOperation, RedisKey first, RedisKey second)
        {
            return db.SetCombine(setOperation, first, second);
        }

        /// <summary>
        /// 删除指定value
        /// </summary>
        /// <returns></returns>
        public bool SetRemove(RedisKey key, RedisValue value)
        {
            return db.SetRemove(key, value);
        }
    }

    public class YuanXinRedisConfigSettings : ConfigurationSection
    {
        public static YuanXinRedisConfigSettings GetConfig()
        {
            YuanXinRedisConfigSettings result = (YuanXinRedisConfigSettings)ConfigurationManager.GetSection("yuanxinRedisSettings");

            if (result == null)
                result = new YuanXinRedisConfigSettings();

            return result;
        }

        [ConfigurationProperty("connectionOptions")]
        public RedisConnectionStringElementCollection ConnectionOptions
        {
            get
            {
                return (RedisConnectionStringElementCollection)base["connectionOptions"];
            }
        }
    }

    public class RedisConnectionStringElementCollection : ConfigurationElementCollection
    {
        protected override ConfigurationElement CreateNewElement()
        {
            return new RedisConnectionStringElement();
        }

        protected override object GetElementKey(ConfigurationElement element)
        {
            return ((RedisConnectionStringElement)element).Name;
        }

        public RedisConnectionStringElement this[string name]
        {
            get
            {
                return BaseGet(name) as RedisConnectionStringElement;
            }
        }
    }

    public class RedisConnectionStringElement : ConfigurationElement
    {
        [ConfigurationProperty("name", IsRequired = false)]
        public string Name
        {
            get
            {
                return (string)this["name"];
            }
        }

        [ConfigurationProperty("connectionString", IsRequired = false)]
        public string ConnectionString
        {
            get
            {
                return (string)this["connectionString"];
            }
        }

        [ConfigurationProperty("port", IsRequired = false)]
        public int Port
        {
            get
            {
                return (int)this["port"];
            }
        }

        [ConfigurationProperty("passWord", IsRequired = false)]
        public string PassWord
        {
            get
            {
                return (string)this["passWord"];
            }
        }

        [ConfigurationProperty("db", IsRequired = false)]
        public int DB
        {
            get
            {
                return (int)this["db"];
            }
        }
    }

}