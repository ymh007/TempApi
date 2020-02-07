using System.Configuration;
using System.Web.Http;
using Seagull2.YuanXin.AppApi.Adapter.UserTaks;
using Seagull2.YuanXin.AppApi.Models.UserTask;

namespace Seagull2.YuanXin.AppApi.Controllers
{
    /// <summary>
    /// 
    /// </summary>
    public class UserTaksOperationController: ControllerBase
    {
        /// <summary>
        /// 功能：获取推送的数据
        /// 作者:v-dengwh
        /// 邮箱：1030736409@sinooceanland.com
        /// 时间：2107-03-16 15：58
        /// </summary>
        /// <param name="Id"></param>
        /// <returns></returns>
        [HttpGet]
        public IHttpActionResult LoadUserTaskOperationById(string Id)
        {
            UserTaskOperation userTaskOperation = UserTaskOperationAdapter.Instance.LoadUserTaskOperationById(Id);
            if (userTaskOperation != null)
            {
                YXUserTaskAdapter yxUserTask = new YXUserTaskAdapter();
                yxUserTask.SetUserTaskReadFlag(userTaskOperation.ResourceID);

                if (userTaskOperation.TaskUrl.ToLower().Contains("thrwebapp"))
                {
                    //int i = userTaskOperation.TaskUrl.IndexOf("?");
                    //string url = userTaskOperation.TaskUrl.Substring(0, i);
                    userTaskOperation.TaskUrl = ConfigurationManager.AppSettings["UrlMappingNG"] + userTaskOperation.TaskUrl;
                }
            }
            return Ok(userTaskOperation);
        }
    }
}