using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;
using System.Web.Http;

namespace Seagull2.YuanXin.AppApi.Controllers
{
    /// <summary>
    /// 共享文件 Controller
    /// </summary>
    public class ShareFileController : ApiController
    {
        /// <summary>
        /// 文件上传并保存记录
        /// </summary>
        [AllowAnonymous]
        [HttpPost]
        public HttpResponseMessage UploadByKindeditor()
        {
            var response = new HttpResponseMessage(HttpStatusCode.OK)
            {
                Content = new StringContent(string.Format(@"<script type='text/javascript'>document.domain = '{0}'</script>{1}", System.Web.HttpContext.Current.Request.QueryString["domainhost"], "{\"error\":0,\"url\":\"/ ke4/ attached/ W020091124524510014093.jpg\"}"))
            };
            response.Content.Headers.ContentType = new System.Net.Http.Headers.MediaTypeHeaderValue("text/html");

            return response;
        }

        /// <summary>
        /// 文件上传中转服务
        /// </summary>
        [AllowAnonymous]
        [HttpPost]
        public async Task<HttpResponseMessage> PostFormData()
        {
            // must be mime multipart content
            if (!Request.Content.IsMimeMultipartContent())
            {
                throw new HttpResponseException(HttpStatusCode.UnsupportedMediaType);
            }

            // init value
            var bucketName = "Office";
            var storageType = "0";
            var resourceID = Guid.NewGuid().ToString();
            var responseType = "application/json";
            var bytes = new byte[0];
            var fileName = "";

            // get formdata
            var multipartMemoryStreamProvider = await Request.Content.ReadAsMultipartAsync();
            foreach (var content in multipartMemoryStreamProvider.Contents)
            {
                if (!string.IsNullOrEmpty(content.Headers.ContentDisposition.FileName))
                {
                    fileName = content.Headers.ContentDisposition.FileName.Trim('"');
                    using (var stream = await content.ReadAsStreamAsync())
                    {
                        bytes = new byte[stream.Length];
                        stream.Read(bytes, 0, bytes.Length);
                    }
                }
                else
                {
                    string val = await content.ReadAsStringAsync();
                    switch (content.Headers.ContentDisposition.Name.Trim('"'))
                    {
                        case "bucketName":
                            bucketName = val; break;
                        case "storageType":
                            storageType = val; break;
                        case "resourceID":
                            resourceID = val; break;
                        case "responseType":
                            responseType = val; break;
                    }
                }
            }
            if (bytes.Length < 1)
            {
                return Request.CreateErrorResponse(HttpStatusCode.InternalServerError, "please select a file.");
            }

            // post data
            var apiUrl = "http://10.23.74.247/FileUploadService/api/upload";
            using (var client = new HttpClient())
            using (var content = new MultipartFormDataContent())
            {
                content.Add(new StringContent(bucketName), "bucketName");
                content.Add(new StringContent(storageType), "storageType");
                content.Add(new StringContent(resourceID), "resourceID");

                var fileContent = new ByteArrayContent(bytes);
                fileContent.Headers.ContentDisposition = new System.Net.Http.Headers.ContentDispositionHeaderValue("attachment")
                {
                    Name = "yuanxin",
                    FileName = System.IO.Path.GetFileName(fileName)
                };
                content.Add(fileContent);

                var result = client.PostAsync(apiUrl, content).Result;
                var json = result.Content.ReadAsStringAsync().Result;
                return new HttpResponseMessage(HttpStatusCode.OK)
                {
                    Content = new StringContent(json, System.Text.Encoding.UTF8, responseType)
                };
            }
        }
    }
}