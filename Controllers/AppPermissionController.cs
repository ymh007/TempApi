using Seagull2.YuanXin.AppApi.Adapter.Common;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Http;

namespace Seagull2.YuanXin.AppApi.Controllers
{
    /// <summary>
    /// 权限操作
    /// </summary>
    public class AppPermissionController : ApiController
    {

        /// <summary>
        /// 加载人员管理单元
        /// </summary>
        /// <param name="Rcode"></param>
        /// <returns></returns>
        [HttpGet]
        public IHttpActionResult LoadPersionUnit(string Rcode)
        {
            var result = ControllerService.Run(() =>
            {
                return PersonUnitAdapter.Instance.Load(p => p.AppendItem("RelationCode", Rcode));
            });
            return Ok(result);
        }

    }
}