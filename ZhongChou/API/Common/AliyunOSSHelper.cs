using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Configuration;
using Aliyun.OSS;
using Aliyun.OSS.Common;
using System.IO;
using System.Text;

namespace Seagull2.YuanXin.AppApi.ZhongChouApi.Common
{
    public static class AliyunOSSHelper
    {
        public static string OSS_AccessKeyId = ConfigurationManager.AppSettings["oss-accessKeyId"];
        public static string OSS_AccessKeySecret = ConfigurationManager.AppSettings["oss-accessKeySecret"];
        public static string OSS_Endpoint = ConfigurationManager.AppSettings["oss-endpoint"];
        public static string OSS_Bucket = ConfigurationManager.AppSettings["oss-bucket"];
        public static string OSS_Dir = ConfigurationManager.AppSettings["oss-dir"];
        public static string OSS_Host = ConfigurationManager.AppSettings["oss-host"];


        public static ClientConfiguration GetClientConfiguration()
        {
            return new ClientConfiguration
            {
                ConnectionTimeout = 300, //设置连接超时时间
                MaxErrorRetry = 3 //设置请求发生错误时最大的重试次数

            };
        }

        public static OssClient CreateOssClient()
        {
            return new OssClient(
                AliyunOSSHelper.OSS_Endpoint,
                AliyunOSSHelper.OSS_AccessKeyId,
                AliyunOSSHelper.OSS_AccessKeySecret);
        }

        public static IEnumerable<Bucket> ListBuckets()
        {
            var client = AliyunOSSHelper.CreateOssClient();
            return client.ListBuckets();
        }

        public static string PutObjectFromBase64String(string str, string key)
        {
            var client = AliyunOSSHelper.CreateOssClient();
            byte[] binaryData = Convert.FromBase64String(str);
            MemoryStream requestContent = new MemoryStream(binaryData);
            //string fileName = AliyunOSSHelper.GetFileName(key);
            return client.PutObject(AliyunOSSHelper.OSS_Bucket, key, requestContent).ETag;
        }

        public static void DeleteObject(string key)
        {
            var client = AliyunOSSHelper.CreateOssClient();

            client.DeleteObject(AliyunOSSHelper.OSS_Bucket,
                AliyunOSSHelper.GetFileName(key));

        }

        public static Stream  GetFile(string key)
        {
            var client = AliyunOSSHelper.CreateOssClient();
            var object1 = client.GetObject("yuanxinapp", key);

            return object1.Content;
        }


        public static string GetFileName(string key)
        {
            string fileName = key;
            if (!string.IsNullOrEmpty(AliyunOSSHelper.OSS_Dir))
            {
                fileName = AliyunOSSHelper.OSS_Dir + "/" + fileName;
            }

            return fileName;
        }
    }
}