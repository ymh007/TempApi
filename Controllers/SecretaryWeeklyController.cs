using SinoOcean.Seagull2.TransactionData.SecretaryService;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Http;
using MVC = System.Web.Mvc;

namespace Seagull2.YuanXin.AppApi.Controllers
{
    [AllowAnonymous]
    public class SecretaryWeeklyController : ApiController
    {
        /// <summary>
        /// 高管周报编辑初始化数据
        /// </summary>
        /// <param name="SuperiorCode">高管编码</param>
        /// <returns></returns>
        [HttpGet]
        public IHttpActionResult Index(string SuperiorCode)
        {
            //高管组织暂定：开发事业二部，高管：谢英亮
            //编码：92729744-e8b9-4f69-96f2-424d8b79a062
            string parentCode = "92729744-e8b9-4f69-96f2-424d8b79a062";
            //职责
            WorkDutyCollection zzColl = WorkDutyAdapter.Instence.LoadNodesByParetCode(parentCode);
            //职责明细
            WorkDutyCollection zzmxColl = new WorkDutyCollection();
            if (zzColl.Count > 0)
            {
                zzmxColl = WorkDutyAdapter.Instence.LoadNodesByParetCode(zzColl[0].Code);
            }
            //时间段(开始时间/结束时间)
            WorkOfTimeCollection timeColl = WorkOfTimeAdapter.Instence.GetAll();
            //形式
            WorkStyleCollection xsColl = WorkStyleAdapter.Instence.GetAll();
            //紧急程度
            WorkImportanceCollection jjcdColl = WorkImportanceAdapter.Instence.GetAll();
            //计划
            WorkTypeCollection jhColl = WorkTypeAdapter.Instence.GetAll();

            HelpClass hc = new HelpClass
            {
                obj1 = zzColl,
                obj2 = zzmxColl,
                obj3 = timeColl,
                obj4 = xsColl,
                obj5 = jjcdColl,
                obj6 = jhColl
            };
            return Ok(hc);
        }

        /// <summary>
        /// 高管周报详情初始化数据
        /// </summary>
        /// <param name="WorkSuperiorScheduleCode">周报编码</param>
        /// <returns></returns>
        [HttpGet]
        public IHttpActionResult Detail(string WorkSuperiorScheduleCode)
        {
            return Ok();
        }
    }
    public class HelpClass
    {
        public object obj1 { get; set; }
        public object obj2 { get; set; }
        public object obj3 { get; set; }
        public object obj4 { get; set; }
        public object obj5 { get; set; }
        public object obj6 { get; set; }
    }
}