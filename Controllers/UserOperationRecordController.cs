using Seagull2.Core.Models;
using Seagull2.YuanXin.AppApi.Adapter.UserOperationRecord;
using Seagull2.YuanXin.AppApi.Models.UserOperationRecord;
using Seagull2.YuanXin.AppApi.ViewsModel.UserOperationRecord;
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
    /// 用户操作记录 Controller
    /// </summary>
    public class UserOperationRecordController : ApiController
    {
        /// <summary>
        /// 添加用户操作记录
        /// </summary>
        [HttpPost]
        public IHttpActionResult Add(AddUserOperationRecordViewModel post)
        {
            return Ok(ControllerService.Run(() =>
            {
                var user = (Seagull2Identity)User.Identity;

                var isExist = UserOperationRecordAdapter.Instance.IsExist(post.Module.ToString(), user.Id);
                if (!isExist)
                {
                    UserOperationRecordAdapter.Instance.Update(new UserOperationRecordModel
                    {
                        Code = Guid.NewGuid().ToString(),
                        Module = post.Module.ToString(),
                        Creator = user.Id,
                        CreateTime = DateTime.Now,
                        ValidStatus = true,
                    });
                }
            }));
        }
    }
}