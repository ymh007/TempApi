using MCS.Library.Data;
using Newtonsoft.Json.Linq;
using Seagull2.YuanXin.AppApi.Adapter.Conference;
using Seagull2.YuanXin.AppApi.Models;
using Seagull2.YuanXin.AppApi.Models.Conference;
using Seagull2.YuanXin.AppApi.ViewsModel;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Runtime.Serialization.Formatters.Binary;
using System.Threading.Tasks;
using System.Transactions;
using System.Web;
using System.Web.Http;

namespace Seagull2.YuanXin.AppApi.Controllers
{
    /// <summary>
    /// 会议附件控制器
    /// </summary>
    public partial class ConferenceController : ControllerBase
    {
        /// <summary>
        ///  获取会议材料  
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        [Route("GetMeetingMaterialList")]
        public IHttpActionResult GetMeetingMaterialList(string conferenceID)
        {
            var result = ControllerService.Run(() =>
            {
                MeetingMaterialCollection list = new MeetingMaterialCollection();
                if (!string.IsNullOrEmpty(conferenceID))
                {
                    list = MeetingMaterialAdapter.Instance.Load(p => { p.AppendItem("ConfereenceId", conferenceID); });
                }
                return list;
            });
            return Ok(result);
        }

        /// <summary>
        ///  删除会议材料 同时删除材料对应的查看人员
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        [Route("DelMeetingMaterial")]
        public IHttpActionResult DelMeetingMaterial(string id)
        {

            var result = ControllerService.Run(() =>
            {
                if (!string.IsNullOrEmpty(id))
                {
                    MeetingMaterialAdapter.Instance.DeleteMaterial(id);
                    MeetingMaterialAuthorityAdapter.Instance.DeletePersons(id);
                }
            });


            return Ok(result);
        }

        /// <summary>
        /// 保存会议附件
        /// </summary>
        [Route("ImportMeetingMaterial")]
        [HttpPost]
        public IHttpActionResult ImportMeetingMaterial()
        {
            var result = ControllerService.Run(() =>
            {
                string conferenceID = baseRequest.Form["ConferenceID"].ToString();
                if (string.IsNullOrEmpty(conferenceID)) throw new Exception("会议id不能为空");
                if (HttpContext.Current.Request.Files.Count == 0) throw new Exception("请选择附件");
                HttpPostedFile postFile = HttpContext.Current.Request.Files[0];
                string fileName = postFile.FileName;
                string fileType = fileName.Substring(fileName.LastIndexOf("."), fileName.Length - fileName.LastIndexOf("."));
                string filePath = FileService.UploadFile(postFile.InputStream, fileType);
                MeetingMaterial mm = new MeetingMaterial();
                if (string.IsNullOrEmpty(mm.Code))
                {
                    mm.Code = Guid.NewGuid().ToString();
                    mm.IsShowAll = false;
                    mm.MeetingScenes = 1;// 会议
                    mm.ViewUrl = filePath;
                    mm.ConfereenceId = conferenceID;
                    mm.Name = fileName;
                    mm.CreateTime = DateTime.Now;
                    mm.Creator = CurrentUser.CodeSeagull;
                }
                MeetingMaterialAdapter.Instance.Update(mm);
                return mm;
            });
            return Ok(result);
        }

        /// <summary>
        /// 设置会议资料查看人 [FromBody]JObject offlineActivityJson
        /// </summary>
        /// <param name="id">附件id</param>
        /// <returns></returns>
        [Route("SetMaterialViewPerson")]
        [HttpPost]
        public IHttpActionResult SetMaterialViewPerson([FromBody]JObject model)
        {
            var result = ControllerService.Run(() =>
            {
                //判断数据是否合法
                if (model == null || model.Count == 0)
                {
                    throw new Exception("请选择参会人");
                };
                MeetingMaterialAuthorityCollection coll = new MeetingMaterialAuthorityCollection();
                string mid = (string)model.SelectToken("mid");
                JArray broadcastJArray = (JArray)model.SelectToken("list");
                foreach (JObject broadcastJson in broadcastJArray)
                {
                    coll.Add(new MeetingMaterialAuthority()
                    {
                        Code = Guid.NewGuid().ToString(),
                        MaterialID = mid,
                        PeopleName = (string)broadcastJson.SelectToken("name"),
                        PeopleId = (string)broadcastJson.SelectToken("attendeeID"),
                        Creator = "",
                        CreateTime = DateTime.Now,
                        Modifier = "",
                        ModifyTime = DateTime.Now,
                        ValidStatus=true

                    });
                }
                if (coll.Count > 0)
                {
                    var mm = MeetingMaterialAdapter.Instance.Load(p => p.AppendItem("Code", mid)).FirstOrDefault();
                    using (TransactionScope ts = TransactionScopeFactory.Create())
                    {
                        mm.IsShowAll = true;
                        MeetingMaterialAdapter.Instance.Update(mm);
                        MeetingMaterialAuthorityAdapter.Instance.DeletePersons(mid);
                        MeetingMaterialAuthorityAdapter.Instance.UpdateMaterialPersonColl(coll);
                        ts.Complete();
                    }
                }
            });
            return Ok(result);
        }

    }
}