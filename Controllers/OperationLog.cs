using Seagull2.YuanXin.AppApi.Adapter.Sys_Menu;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;
using System.Web;
using System.Web.Http;

namespace Seagull2.YuanXin.AppApi.Controllers
{
    /// <summary>
    /// 后台操作日志
    /// </summary>
    public class OperationLogController : ApiController
    {

        /// <summary>
        /// 获取操作人
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        public IHttpActionResult GetOperation()
        {
            var result = ControllerService.Run(() =>
            {
                return Sys_UserAdapter.Instance.Load(p => p.AppendItem("1", 1));
            });
            return this.Ok(result);
        }



        /// <summary>
        /// 获取操纵日志
        /// </summary>
        /// <returns></returns>
        [HttpPost]
        public async Task<HttpResult> GetLogs(QuerPar model)
        {
            HttpResult result = new HttpResult();
            try
            {
                dynamic args = new
                {
                    pageIndex = model.index,
                    pageSize = model.size,
                    data = new
                    {
                        dateStart = model.start,
                        dateEnd = model.end
                    }
                };
                if (!string.IsNullOrEmpty(model.userid))
                {
                    args = new
                    {
                        pageIndex = model.index,
                        pageSize = model.size,
                        data = new
                        {
                            userId = model.userid,
                            dateStart = model.start,
                            dateEnd = model.end
                        }
                    };
                }
                using (var http = new HttpClient())
                {
                    HttpResponseMessage responseMessage = await http.PostAsJsonAsync(ConfigAppSetting.GetLogs, args as object);
                    string response = await responseMessage.Content.ReadAsStringAsync();
                    if (responseMessage.StatusCode == HttpStatusCode.OK)
                    {
                        result = Newtonsoft.Json.JsonConvert.DeserializeObject<HttpResult>(response);
                        result.isSuccess = true;
                    }
                    else
                    {
                        result.message = response;
                    }
                }
            }
            catch (Exception ex)
            {
                result.message = ex.ToString(); 
            }
            if (result.Data == null) {
                result.Data = new List<HttpData>();
            }
            return result;

        }
        public class QuerPar
        {
            public DateTime start { get; set; }
            public DateTime end { get; set; }
            public string userid { get; set; }
            public int index { get; set; }
            public int size { get; set; }

        }

        /// <summary>
        /// 为了sb写这种sb代码
        /// </summary>
        public class HttpResult
        {
            public int pageSize { get; set; }
            public int pageIndex { get; set; }
            public int totalCount { get; set; }
            public int pageCount { get; set; }
            public bool isSuccess { get; set; }
            public string message { get; set; }
            public List<HttpData> Data { get; set; }

        }
        public class HttpData
        {
            public string id { get; set; }
            public string userId { get; set; }
            public string loginName { get; set; }
            public string roleName { get; set; }
            public string operateDesc { get; set; }
            public string operateTime { get; set; }
            public string upLoadTime { get; set; }
            public string operateTimeStr { get; set; }
            public string upLoadTimeStr { get; set; }
            public string userName { get; set; }
        }
    }
}