using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Reflection;
using System.Text;
using System.Web;
using log4net;
using Newtonsoft.Json;

namespace Seagull2.YuanXin.AppApi
{
    /// <summary>
    /// 文件服务
    /// </summary>
    public class FileService
    {
        static ILog log = LogManager.GetLogger(MethodBase.GetCurrentMethod().DeclaringType);

        /// <summary>
        /// 文件上传
        /// </summary>
        public static string UploadFile(string base64String)
        {
            string fileUrl;
            try
            {
                string apiUrl = ConfigurationManager.AppSettings["FileUpload"];
                string root = ConfigurationManager.AppSettings["FileFolder"];

                using (HttpClient client = new HttpClient())
                {
                    //设定要响应的数据格式
                    client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
                    //表明是通过 multipart/form-data 的方式上传数据
                    using (var content = new MultipartFormDataContent())
                    {
                        //数据内容
                        content.Add(CreateHttpContent("bucketName", root));
                        content.Add(CreateHttpContent("storageType", "0"));
                        content.Add(CreateHttpContent("c", Guid.NewGuid().ToString("N")));

                        //文件内容
                        string fileType = base64String.Split('/', ';')?[1];
                        string fileBase = base64String.Split(',')[1];
                        byte[] fileBytes = Convert.FromBase64String(fileBase);
                        var fileContent = new ByteArrayContent(fileBytes);
                        fileContent.Headers.ContentDisposition = new ContentDispositionHeaderValue("attachment")
                        {
                            Name = "yuanxin",
                            FileName = Guid.NewGuid().ToString("N") + "." + fileType
                        };
                        content.Add(fileContent);

                        var result = client.PostAsync(apiUrl, content).Result;
                        var json = result.Content.ReadAsStringAsync().Result;

                        log.Info("上传文件返回值：" + json);

                        try
                        {
                            var list = JsonConvert.DeserializeObject<List<string>>(json);
                            if (list.Count > 0)
                            {
                                fileUrl = list[0];
                            }
                            else
                            {
                                fileUrl = string.Empty;
                            }
                        }
                        catch (Exception e)
                        {
                            log.Error("上传文件异常（json转换失败）" + JsonConvert.SerializeObject(e));
                            fileUrl = string.Empty;
                        }
                    }
                }
            }
            catch (Exception e)
            {
                log.Error("上传文件异常：" + JsonConvert.SerializeObject(e));
                fileUrl = string.Empty;
            }
            return fileUrl;
        }


        /// <summary>
        /// 文件上传
        /// </summary>
        public static string UploadFile(System.IO.Stream stream, string fileType)
        {
            string fileUrl;
            try
            {
                string apiUrl = ConfigurationManager.AppSettings["FileUpload"];
                string root = ConfigurationManager.AppSettings["FileFolder"];

                using (HttpClient client = new HttpClient())
                {
                    //设定要响应的数据格式
                    client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
                    //表明是通过 multipart/form-data 的方式上传数据
                    using (var content = new MultipartFormDataContent())
                    {
                        //数据内容
                        content.Add(CreateHttpContent("bucketName", root));
                        content.Add(CreateHttpContent("storageType", "0"));
                        content.Add(CreateHttpContent("resourceID", Guid.NewGuid().ToString("N")));
                          //文件内容
                        //var fileContent = new ByteArrayContent(fileBytes);
                        var fileContent = new StreamContent(stream);
                        fileContent.Headers.ContentDisposition = new ContentDispositionHeaderValue("attachment")
                        {
                            Name = "yuanxin",
                            FileName = Guid.NewGuid().ToString("N")  + fileType
                        };
                        content.Add(fileContent);

                        var result = client.PostAsync(apiUrl, content).Result;
                        var json = result.Content.ReadAsStringAsync().Result;

                        log.Info("上传文件返回值：" + json);

                        try
                        {
                            var list = JsonConvert.DeserializeObject<List<string>>(json);
                            if (list.Count > 0)
                            {
                                fileUrl = list[0];
                            }
                            else
                            {
                                fileUrl = string.Empty;
                            }
                        }
                        catch (Exception e)
                        {
                            log.Error("上传文件异常（json转换失败）" + JsonConvert.SerializeObject(e));
                            fileUrl = string.Empty;
                        }
                    }
                }
            }
            catch (Exception e)
            {
                log.Error("上传文件异常：" + JsonConvert.SerializeObject(e));
                fileUrl = string.Empty;
            }
            return fileUrl;
        }
        /// <summary>
        /// 创建一个 HttpContent ，私有方法
        /// </summary>
        static HttpContent CreateHttpContent(string key, string value)
        {
            var dataContent = new ByteArrayContent(Encoding.UTF8.GetBytes(value));
            dataContent.Headers.ContentDisposition = new ContentDispositionHeaderValue("attachment")
            {
                Name = key
            };
            return dataContent;
        }

        /// <summary>
        /// 文件下载
        /// </summary>
        public static string DownloadFile(string url)
        {
            try
            {
                string apiUrl = ConfigurationManager.AppSettings["FileDownload"];
                return string.Format(apiUrl, url);
            }
            catch (Exception e)
            {
                log.Error("下载文件异常：" + JsonConvert.SerializeObject(e));
                return string.Empty;
            }
        }
    }
}