using Seagull2.Core.Models;
using Seagull2.Permission.Organization;
using Seagull2.YuanXin.AppApi.Services;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Web;
using System.Web.Http;

namespace Seagull2.YuanXin.AppApi.Controllers
{
    [AllowAnonymous]
    public class AttentionOperateController : ApiController
    {
        private readonly IAttentionService _AttentionService;

        public AttentionOperateController(IAttentionService AttentionService)
        {
            _AttentionService = AttentionService;
        }

        /// <summary>
        /// 查询关注的事业部或项目
        /// </summary>
        [HttpGet, HttpPost]
        public async Task<IHttpActionResult> LoadAttention()
        {
            return Ok(await _AttentionService.LoadAttention(((Seagull2Identity)User.Identity).Id));
        }

        /// <summary>
        /// 增加我的关注
        /// </summary>
        [HttpGet]
        public async Task<IHttpActionResult> AddAttention()
        {
            //id
            string businessProjectCode = HttpContext.Current.Request["BusinessProjectCode"];
            //name
            string businessProjectName = HttpContext.Current.Request["BusinessProjectName"];
            //事业部
            string businessCode = HttpContext.Current.Request["businessCode"];
            //关注类型
            int attentionType = int.Parse(HttpContext.Current.Request["attentionType"]);

            return Ok(await _AttentionService.AddAttention(((Seagull2Identity)User.Identity).Id, businessProjectCode, businessProjectName, businessCode, attentionType));
        }

        /// <summary>
        /// 删除清空我的关注
        /// </summary>
        [HttpGet]
        public async Task<IHttpActionResult> EmptyAndAddOrAttention()
        {
            //删除的id
            string listbusinessProjectCode = HttpContext.Current.Request["listbusinessProjectCode"];
            //是否清空
            bool IsEmpty = Convert.ToBoolean(HttpContext.Current.Request["IsEmpty"]);

            return Ok(await _AttentionService.EmptyAndAddOrAttention(((Seagull2Identity)User.Identity).Id, listbusinessProjectCode, IsEmpty));

        }

        /// <summary>
        /// 我的关注总数统计
        /// </summary>
        [HttpGet]
        public async Task<IHttpActionResult> StatisticsAttention()
        {
            return Ok(await _AttentionService.StatisticsAttention(((Seagull2Identity)User.Identity).Id));
        }
    }
}